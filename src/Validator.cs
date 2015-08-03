/*
 * The MIT License (MIT)
 * Copyright (c) 2014 - 2015 Henrique Borba Behr
 * 
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
using System.Runtime.InteropServices;

namespace AdAuthentication
{
    internal class Validator
    {
        private readonly AdAuthenticator _adAuthenticator;
        private const int WrongUserOrPassword = -2147023570;
        private const int InvalidLdapPath1 = -2147016661;
        private const int InvalidLdapPath2 = -2147467259;
        private const int InvalidLdapPathComError = -2147467259;

        public Validator(AdAuthenticator adAuthenticator)
        {
            _adAuthenticator = adAuthenticator;
        }

        internal Validator ValidateConfiguration()
        {
            if (string.IsNullOrWhiteSpace(_adAuthenticator.LdapDomain))
            {
                throw new AdException("LDAP Domain not configured");
            }
            if (string.IsNullOrWhiteSpace(_adAuthenticator.LdapPath))
            {
                throw new AdException("LDAP Path not configured");
            }
            return this;
        }

        internal Validator ValidateParameters(string login, string senha)
        {
            if (string.IsNullOrWhiteSpace(login))
            {
                throw new ArgumentNullException("login");
            }
            if (string.IsNullOrWhiteSpace(senha))
            {
                throw new ArgumentNullException("senha");
            }
            return this;
        }

        internal Validator ValidateUserPasswordAtAd(string login, string senha)
        {
            try
            {
                var entry = new DirectoryEntry(_adAuthenticator.LdapPath, login, senha);
                Object objnative = entry.NativeObject;
                if (objnative == null) 
                {
                    throw new Exception();
                }
                return this;
            }
            catch (DirectoryServicesCOMException e)
            {
                if (e.ErrorCode == InvalidLdapPath1 || e.ErrorCode == InvalidLdapPath2)
                {
                    throw new AdException(AdError.InvalidLdapPath, "Invalid Ldap Path", e);
                }
                if (e.ErrorCode == WrongUserOrPassword)
                {
                    _adAuthenticator.GetUserFromAdBy(login); // throws AdError.UserNotFound if user dosen't exists !
                    throw new AdException(AdError.IncorrectPassword, "Invalid Password", e); // Or the password is wrong !
                }
                throw;
            }
            catch (COMException e)
            {
                if (e.ErrorCode == InvalidLdapPathComError)
                {
                    throw new AdException(AdError.InvalidLdapPath, "Invalid Ldap Path", e);
                }
                throw;
            }
            catch (Exception e)
            {
                throw new AdException("Unkown Error", e);
            }
        }
    }
}