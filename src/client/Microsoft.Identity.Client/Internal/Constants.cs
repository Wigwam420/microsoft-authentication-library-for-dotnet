﻿// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

using System;
using System.Globalization;

namespace Microsoft.Identity.Client.Internal
{
    internal static class Constants
    {
        public const string MsAppScheme = "ms-app";
        public const int ExpirationMarginInMinutes = 5;
        public const int CodeVerifierLength = 128;
        public const int CodeVerifierByteSize = 96;

        public const string UapWEBRedirectUri = "https://sso"; // for WEB
        public const string DefaultRedirectUri = "urn:ietf:wg:oauth:2.0:oob";
        public const string NativeClientRedirectUri = "https://login.microsoftonline.com/common/oauth2/nativeclient";
        public const string LocalHostRedirectUri = "http://localhost";
        public const string DefaultConfidentialClientRedirectUri = "https://replyUrlNotSet";

        public const string DefaultRealm = "http://schemas.microsoft.com/rel/trusted-realm";

        public const string MsaTenantId = "9188040d-6c67-4c5b-b112-36a304b66dad";
        public const string ConsumerTenant = "consumers";
        public const string OrganizationsTenant = "organizations";
        public const string CommonTenant = "common";

        public const string UserRealmMsaDomainName = "live.com";

        public const string CcsRoutingHintHeader = "x-anchormailbox";
        public const string AadThrottledErrorCode = "AADSTS50196";
        //Represents 5 minutes in Unit time stamp
        public const int DefaultJitterRangeInSeconds = 300;
        public static readonly TimeSpan AccessTokenExpirationBuffer = TimeSpan.FromMinutes(5);
        public const string EnableSpaAuthCode = "1";
        public const string PoPTokenType = "pop";
        public const string PoPAuthHeaderPrefix = "PoP";
        public const string RequestConfirmation = "req_cnf";

        public static string FormatEnterpriseRegistrationOnPremiseUri(string domain)
        {
            return string.Format(
                CultureInfo.InvariantCulture,
                "https://enterpriseregistration.{0}/enrollmentserver/contract",
                domain);
        }

        public static string FormatEnterpriseRegistrationInternetUri(string domain)
        {
            return string.Format(
                CultureInfo.InvariantCulture,
                "https://enterpriseregistration.windows.net/{0}/enrollmentserver/contract",
                domain);
        }

        public const string WellKnownOpenIdConfigurationPath = ".well-known/openid-configuration";
        public const string OpenIdConfigurationEndpoint = "v2.0/" + WellKnownOpenIdConfigurationPath;
        public const string Tenant = "{tenant}";
        public const string TenantId = "{tenantid}";
        public static string FormatAdfsWebFingerUrl(string host, string resource)
        {
            return string.Format(
                CultureInfo.InvariantCulture,
                "https://{0}/.well-known/webfinger?rel={1}&resource={2}",
                host,
                DefaultRealm,
                resource);
        }
    }
}
