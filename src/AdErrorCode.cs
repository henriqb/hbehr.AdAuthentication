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
using System.DirectoryServices;

namespace AdAuthentication
{
    public static class AdErrorCode
    {
        public static void TreatErrorMessage(DirectoryServicesCOMException e)
        {
            /** http://www-01.ibm.com/support/docview.wss?uid=swg21290631
             * 525 - user not found
             * 52e - invalid credentials
             * 530 - not permitted to logon at this time
             * 531 - not permitted to logon at this workstation
             * 532 - password expired
             * 533 - account disabled
             * 534 - The user has not been granted the requested logon type at this machine
             * 701 - account expired
             * 773 - user must reset password
             * 775 - user account locked */

            string msg = e.ExtendedErrorMessage ?? "";
            if (msg.Contains("525"))
            {
                throw new AdException(AdError.UserNotFound, "User Not found (525)", e);
            }
            if (msg.Contains("52e"))
            {
                throw new AdException(AdError.IncorrectPassword, "Invalid Credentials (52e)", e);
            }
            if (msg.Contains("530"))
            {
                throw new AdException(AdError.NotPermittedToLogonAtThisTime, "Not permitted to logon at this time (530)", e);
            }
            if (msg.Contains("531"))
            {
                throw new AdException(AdError.NotPermittedToLogonAtThisWorkstation, "Not permitted to logon at this workstation (531)", e);
            }
            if (msg.Contains("532"))
            {
                throw new AdException(AdError.ExpiredPassword, "Expired Password (532)", e);
            }
            if (msg.Contains("533"))
            {
                throw new AdException(AdError.AccountDisabled, "Account Disabled (533)", e);
            }
            if (msg.Contains("534"))
            {
                throw new AdException(AdError.UserNotGrantedRequestedLogonType, "User has not been granted the requested logon type at this machine (534)", e);
            }
            if (msg.Contains("701"))
            {
                throw new AdException(AdError.AccountExpired, "Account Expired (701)", e);
            }
            if (msg.Contains("773"))
            {
                throw new AdException(AdError.UserMustResetPassword, "User must Reset Password (733)", e);
            }
            if (msg.Contains("775"))
            {
                throw new AdException(AdError.AccountLocked, "User Account Locked (775)", e);
            }
            throw new AdException("Unkown Error", e);
        }
    }
}