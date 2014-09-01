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
using AdAuthentication;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace UnitTest
{
    [TestClass]
    public class UnitTest
    {
        [TestMethod]
        public void WrongPasswordTest()
        {
            AdUser user;
            try
            {
                user = new AdAuthenticator()
                    .ConfigureSetLdapPath("LDAP://DC=radixengrj,DC=matriz")
                    .ConfigureLdapDomain("radixengrj")
                    .SearchUserBy("stefano.bassan", "asddasd");
            }
            catch (Exception e)
            {
                var adException = (AdException) e;
                string erro;
                switch (adException.AdError)
                {
                    case AdError.InvalidLdapDomain:
                    case AdError.InvalidLdapPath:
                        erro = "Servidor não configurado";
                        break;
                    case AdError.UserNotFound:
                        erro = "Usuário não existe no Ad";
                        break;
                    case AdError.IncorrectPassword:
                        erro = "Senha incorreta";
                        break;
                }

                var texto = e.Message;
            }
            //var adUser = new AdAuthenticator()
               //.ConfigureSetLdapPath("LDAP://DC=radixengrj,DC=matriz")
               //.ConfigureLdapDomain("radixengrj")
               //.SearchUserBy("stefano.bassan", "sdf");
        }
    }
}
