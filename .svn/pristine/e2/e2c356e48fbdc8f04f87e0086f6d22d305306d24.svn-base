/******************************************************************************
 *                          © 2017 SQS India                            *
 *                          All Rights Reserved.                              *
 *                                                                            *
 ******************************************************************************
 *
 * Modification History:
 * 
 * AKS 18Sep2017 Created the class
 *******************************************************************************/

namespace SQS.nTier.TTM.GenericFramework
{
    using System;

    public static class FrameworkSharedConstants
    {
        public static DateTime SQLMinDateTime = DateTime.Parse("1 Jan 1900");

        public class ClientSession
        {
            public const int DefaultSessionTimeout = 60; //Minutes
            public const string InvalidSessionIDFormat = "The session id is not in the correct format";
            public const string SessionIDNotFound = "The session id was not found, it might have expired";
            public const string SessionExpired = "The session has expired";
        }

        public static readonly string MissingAuthorizationHeader = "The request is missing HMAC authorization headers.";
        public static readonly string DateRequestHeaderDoesNotMatchSystemClock = "The Date request header ({0}) does not match the system clock value ({1})";
        public static readonly string RequestDateIncorrect = "The request date is incorrect - check your system clock.";
        public static readonly string InvalidMD5 = "Invalid Content-MD5 hash.";
        public static readonly string HMACSignatureFailure = "HMAC signature failure.";
        public static readonly string MissmatchedHMACSignature = "Mismatched HMAC signature {0} for request: {1}";
        public static readonly string InvalidAppId = "Invalid Application Id";
        public static readonly string InvalidAuthorizationScheme = "Invalid authorization scheme";
    }


}
