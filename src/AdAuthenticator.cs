/*
 * The MIT License (MIT)
 * Copyright (c) 2014 - 2017 Henrique Borba Behr

 * Permission is hereby granted, free of charge, to any person obtaining a copy
 * of this software and associated documentation files (the "Software"), to deal
 * in the Software without restriction, including without limitation the rights
 * to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
 * copies of the Software, and to permit persons to whom the Software is
 * furnished to do so, subject to the following conditions:

 * The above copyright notice and this permission notice shall be included in all
 * copies or substantial portions of the Software.

 * THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
 * IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
 * FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
 * AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
 * LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
 * OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
 * SOFTWARE.
 */
using System;
using System.Collections.Generic;
using System.Configuration;
using System.DirectoryServices.AccountManagement;
using System.Linq;
using System.Web;

namespace hbehr.AdAuthentication
{
    public class AdAuthenticator
    {
        public AdAuthenticator()
        {
            var appSettings = new AppSettingsReader();
            try
            {
                LdapPath = appSettings.GetValue("LdapPath", typeof(string)).ToString();
            }
            catch { }
            try
            {
                LdapDomain = appSettings.GetValue("LdapDomain", typeof(string)).ToString();
            }
            catch { }
        }

        internal string LdapDomain { get; private set; }
        internal string LdapPath { get; private set; }

        public AdAuthenticator ConfigureLdapDomain(string ldapDomain)
        {
            LdapDomain = ldapDomain;
            return this;
        }

        public AdAuthenticator ConfigureSetLdapPath(string ldapPath)
        {
            LdapPath = ldapPath;
            return this;
        }

        public AdUser SearchUserBy(string login, string password)
        {
            new Validator(this)
                .ValidateConfiguration()
                .ValidateParameters(login, password)
                .ValidateUserPasswordAtAd(login, password);

            return GetUserFromAdBy(login);
        }

        public AdUser GetUserFromAd()
        {
            new Validator(this).ValidateConfiguration();

            if (!HttpContext.Current.User.Identity.IsAuthenticated)
            {
                throw new AdException(AdError.UserNotFound, "User not authenticated, use Windows Authentication");
            }

            string login = HttpContext.Current.User.Identity.Name;
            return GetUserFromAdBy(login);
        }

        public IEnumerable<AdGroup> GetAdGroups()
        {
            new Validator(this).ValidateConfiguration();

            PrincipalContext principalContext = GetPrincipalContext();
            
            var queryForGroups = new GroupPrincipal(principalContext);
            var results = new PrincipalSearcher(queryForGroups);

            return results.FindAll().Select(found => new AdGroup
            {
                Code = found.SamAccountName,
                Name = found.DisplayName
            });
        }

        public AdUser GetUserFromAdBy(string login)
        {
            PrincipalContext principalContext = GetPrincipalContext();

            Principal principal = Principal.FindByIdentity(principalContext, login);

            if (principal == null)
            {
                throw new AdException(AdError.UserNotFound, "User not found");
            }
            return new AdUser(principal, principal.GetGroups(principalContext));
        }
        
        private PrincipalContext GetPrincipalContext()
        {
            try
            {
                return new PrincipalContext(ContextType.Domain, LdapDomain);
            }
            catch (PrincipalServerDownException e)
            {
                throw new AdException(AdError.InvalidLdapDomain, "Ldap Domain not found", e);
            }
            catch (Exception e)
            {
                throw new AdException("Unkown Error", e);
            }
        }

        public IEnumerable<AdUser> GetAllUsers()
        {
            PrincipalContext principalContext = GetPrincipalContext();
            var u = new UserPrincipal(principalContext);
            var search = new PrincipalSearcher(u);
            return search.FindAll().Select(x => new AdUser(x, new List<Principal>()));
        }

        public IEnumerable<AdUser> GetUsersByFilter(string text, int page, out int total, int itemsPerPage = 5)
        {
            PrincipalContext principalContext = GetPrincipalContext();
            var u = new UserPrincipal(principalContext) {SamAccountName = "*" + text + "*"};
            var search = new PrincipalSearcher(u);
            total = search.FindAll().Count();
            return search.FindAll().Skip(page - 1).Take(itemsPerPage).Select(x => new AdUser(x, new List<Principal>()));
        }

        public IEnumerable<AdUser> GetUsersByNameFilter(string text, int page, out int total, int itemsPerPage = 5)
        {
            PrincipalContext principalContext = GetPrincipalContext();
            var u = new UserPrincipal(principalContext) {Name = "*" + text + "*"};
            var search = new PrincipalSearcher(u);
            total = search.FindAll().Count();
            return search.FindAll().Skip(page - 1).Take(itemsPerPage).Select(x => new AdUser(x, new List<Principal>()));
        }
    }
}
