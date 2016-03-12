/*
 * The MIT License (MIT)
 * Copyright (c) 2014 - 2016 Henrique Borba Behr

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

using System.Globalization;
using System.Linq;
using AdAuthentication;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace UnitTest
{
    [TestClass]
    public class AdTest
    {
        private const string LdapPath = "LDAP://DC=radixengrj,DC=matriz";
        private const string LdapDomain = "radixengrj";
        private const string RightUser = "henrique.beh";
        private const string NameRightUser = "Henrique Borba Behr";
        private const string RightPassword = "xxx - sua senha aqui - xxx";

        [TestMethod]
        public void RightPasswordTest()
        {
            AdUser user = new AdAuthenticator()
                .ConfigureSetLdapPath(LdapPath)
                .ConfigureLdapDomain(LdapDomain)
                .SearchUserBy(RightUser, RightPassword);

            Assert.IsNotNull(user);
            Assert.AreEqual(RightUser, user.Login);
        }

        [TestMethod]
        public void RetrievesGroupsFromUserTest()
        {
            AdUser user = new AdAuthenticator()
                .ConfigureSetLdapPath(LdapPath)
                .ConfigureLdapDomain(LdapDomain)
                .SearchUserBy(RightUser, RightPassword);

            Assert.IsNotNull(user.AdGroups);
            Assert.IsTrue(user.AdGroups.Any());
            Assert.IsTrue(user.AdGroups.Any(x => "Usuários do domínio".Equals(x.Code)));
        }

        [TestMethod]
        public void RetrivesGroupsFromAdTest()
        {
            AdGroup[] groups = new AdAuthenticator()
                .ConfigureSetLdapPath(LdapPath)
                .ConfigureLdapDomain(LdapDomain)
                .GetAdGroups().ToArray();

            Assert.IsNotNull(groups);
            Assert.IsTrue(groups.Any());
            Assert.IsTrue(groups.Any(x => "Usuários do domínio".Equals(x.Code)));
        }

        [TestMethod]
        public void TestFindAllUsers()
        {
            var list = new AdAuthenticator()
                .ConfigureSetLdapPath(LdapPath)
                .ConfigureLdapDomain(LdapDomain)
                .GetAllUsers()
                .ToArray();

            Assert.IsNotNull(list);
            Assert.IsTrue(list.Count() > 1);
            Assert.IsTrue(list.Any(u => string.Equals(u.Login, RightUser)));
        }

        [TestMethod]
        public void TestFindUsersByFilter()
        {
            var ad = new AdAuthenticator()
                .ConfigureSetLdapPath(LdapPath)
                .ConfigureLdapDomain(LdapDomain);

            int total;
            var list = ad.GetUsersByFilter(RightUser.First().ToString(CultureInfo.InvariantCulture), 1, out total, 9999);
            Assert.IsNotNull(list);
            var adUsers = list as AdUser[] ?? list.ToArray();
            Assert.IsTrue(total >= adUsers.Count());
            Assert.IsTrue(adUsers.Any(u => string.Equals(u.Login, RightUser)));

            list = ad.GetUsersByFilter(RightUser.Last().ToString(CultureInfo.InvariantCulture), 1, out total, 9999);
            Assert.IsNotNull(list);
            adUsers = list as AdUser[] ?? list.ToArray();
            Assert.IsTrue(total >= adUsers.Count());
            Assert.IsTrue(adUsers.Any(u => string.Equals(u.Login, RightUser)));
        }

        [TestMethod]
        public void TestFindUsersByFilterName()
        {
            var ad = new AdAuthenticator()
                .ConfigureSetLdapPath(LdapPath)
                .ConfigureLdapDomain(LdapDomain);

            int total;
            var list = ad.GetUsersByNameFilter(NameRightUser.First().ToString(CultureInfo.InvariantCulture), 1, out total, 9999);
            Assert.IsNotNull(list);
            var adUsers = list as AdUser[] ?? list.ToArray();
            Assert.IsTrue(total >= adUsers.Count());
            Assert.IsTrue(adUsers.Any(u => string.Equals(u.Login, RightUser)));

            list = ad.GetUsersByNameFilter(RightUser.Last().ToString(CultureInfo.InvariantCulture), 1, out total, 9999);
            Assert.IsNotNull(list);
            adUsers = list as AdUser[] ?? list.ToArray();
            Assert.IsTrue(total >= adUsers.Count());
            Assert.IsTrue(adUsers.Any(u => string.Equals(u.Login, RightUser)));
        }

        [TestMethod]
        public void TestGetLdapSettingFromConfig()
        {
            AdUser user = new AdAuthenticator().SearchUserBy(RightUser, RightPassword);

            Assert.IsNotNull(user);
            Assert.AreEqual(RightUser, user.Login);
        }

        [TestMethod]
        [ExpectedException(typeof(AdException))]
        public void WrongPasswordTest()
        {
            const string wrongPassword = "Lepo-Lepo-Lepo";
            try
            {
                new AdAuthenticator()
                    .ConfigureSetLdapPath(LdapPath)
                    .ConfigureLdapDomain(LdapDomain)
                    .SearchUserBy(RightUser, wrongPassword);
            }
            catch (AdException e)
            {
                Assert.AreEqual(AdError.IncorrectPassword, e.AdError);
                throw;
            }
        }
        
        [TestMethod]
        [ExpectedException(typeof(AdException))]
        public void WrongUserTest()
        {
            const string wrongUser = "Lepo-Lepo-Lepo";
            try
            {
                new AdAuthenticator()
                    .ConfigureSetLdapPath(LdapPath)
                    .ConfigureLdapDomain(LdapDomain)
                    .SearchUserBy(wrongUser, RightPassword);
            }
            catch (AdException e)
            {
                Assert.AreEqual(AdError.UserNotFound, e.AdError);
                throw;
            }
        }

        [TestMethod]
        [ExpectedException(typeof(AdException))]
        public void WrongLdapDomainTest()
        {
            const string wrongLdapDomain = "Lepo-Lepo-Lepo";
            try
            {
                new AdAuthenticator()
                    .ConfigureSetLdapPath(LdapPath)
                    .ConfigureLdapDomain(wrongLdapDomain)
                    .SearchUserBy(RightUser, RightPassword);
            }
            catch (AdException e)
            {
                Assert.AreEqual(AdError.InvalidLdapDomain, e.AdError);
                throw;
            }
        }

        [TestMethod]
        [ExpectedException(typeof(AdException))]
        public void WrongLdapPathTest()
        {
            const string wrongLdapPath = "Lepo-Lepo-Lepo";
            try
            {
                new AdAuthenticator()
                    .ConfigureSetLdapPath(wrongLdapPath)
                    .ConfigureLdapDomain(LdapDomain)
                    .SearchUserBy(RightUser, RightPassword);
            }
            catch (AdException e)
            {
                Assert.AreEqual(AdError.InvalidLdapPath, e.AdError);
                throw;
            }
        }
    }
}
