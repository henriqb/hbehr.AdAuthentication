/*
 * The MIT License (MIT)
 * Copyright (c) 2014 Henrique Borba Behr

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
using System.DirectoryServices;
using System.DirectoryServices.AccountManagement;
using System.Web;

namespace AdAuthentication
{
    public class AdAuthenticator
    {
        private string LdapDomain { get; set; }
        private string LdapPath { get; set; }

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
            ValidarConfiguracao();
            ValidarParametros(login, password);

            ValidarUsuarioSenhaAd(login, password);
            return GetUserFromAdBy(login);
        }

        public AdUser GetUserFromAd()
        {
            ValidarConfiguracao();

            if (!HttpContext.Current.User.Identity.IsAuthenticated)
            {
                throw new AdException(AdError.UserNotFound, "User not authenticated, use Windows Authentication");
            }

            string login = HttpContext.Current.User.Identity.Name;
            return GetUserFromAdBy(login);
        }

        private AdUser GetUserFromAdBy(string login)
        {   
            PrincipalContext principalContext;
            try
            {
                principalContext = new PrincipalContext(ContextType.Domain, LdapDomain);
            }
            catch (PrincipalServerDownException)
            {
                throw new AdException(AdError.InvalidLdapDomain, "Ldap Domain not found");
            }
            catch (Exception e)
            {
                throw new AdException(e);
            }

            Principal principal = Principal.FindByIdentity(principalContext, login);

            if (principal == null)
            {
                throw new AdException(AdError.UserNotFound, "User not found");
            }
            return new AdUser(principal, principal.GetGroups(principalContext));
        }

        private void ValidarUsuarioSenhaAd(string login, string senha)
        {
            try
            {
                DirectoryEntry entry = new DirectoryEntry(LdapPath, login, senha);
            }
            catch (DirectoryServicesCOMException e)
            {
                if (e.ErrorCode == -2147023570)
                {
                    throw new AdException(AdError.IncorrectPassword, "Invalid Password");
                }
                throw new AdException(AdError.UserNotFound, "User not found");
            }
            catch (Exception e)
            {
                throw new AdException(e);
            }
        }

        private static void ValidarParametros(string login, string senha)
        {
            if (string.IsNullOrWhiteSpace(login))
            {
                throw new ArgumentNullException("login");
            }
            if (string.IsNullOrWhiteSpace("senha"))
            {
                throw new ArgumentNullException("senha");
            }
        }

        private void ValidarConfiguracao()
        {
            if (string.IsNullOrWhiteSpace(LdapDomain))
            {
                throw new AdException("LDAP Domain not configured");
            }
            if (string.IsNullOrWhiteSpace(LdapPath))
            {
                throw new AdException("LDAP Path not configured");
            }
        }
    }
}
