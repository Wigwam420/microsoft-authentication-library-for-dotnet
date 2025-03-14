﻿// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using Microsoft.Identity.Client.Http;
using Microsoft.Identity.Client.Instance;
using Microsoft.Identity.Client.Instance.Discovery;
using Microsoft.Identity.Client.Internal;
using Microsoft.Identity.Client.PlatformsCommon.Interfaces;
using Microsoft.Identity.Client.Utils;
using Microsoft.Identity.Json;
using Microsoft.IdentityModel.Abstractions;

namespace Microsoft.Identity.Client
{
    /// <summary>
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public abstract class AbstractApplicationBuilder<T>
        where T : AbstractApplicationBuilder<T>
    {
        internal AbstractApplicationBuilder(ApplicationConfiguration configuration)
        {
            Config = configuration;
        }

        internal ApplicationConfiguration Config { get; }

        /// <summary>
        /// Uses a specific <see cref="IMsalHttpClientFactory"/> to communicate
        /// with the IdP. This enables advanced scenarios such as setting a proxy,
        /// or setting the Agent.
        /// </summary>
        /// <param name="httpClientFactory">HTTP client factory</param>
        /// <remarks>MSAL does not guarantee that it will not modify the HttpClient, for example by adding new headers.
        /// Prior to the changes needed in order to make MSAL's httpClients thread safe (https://github.com/AzureAD/microsoft-authentication-library-for-dotnet/pull/2046/files),
        /// the httpClient had the possibility of throwing an exception stating "Properties can only be modified before sending the first request".
        /// MSAL's httpClient will no longer throw this exception after 4.19.0 (https://github.com/AzureAD/microsoft-authentication-library-for-dotnet/releases/tag/4.19.0)
        /// see (https://aka.ms/msal-httpclient-info) for more information.
        /// </remarks>
        /// <returns>The builder to chain the .With methods</returns>
        public T WithHttpClientFactory(IMsalHttpClientFactory httpClientFactory)
        {
            Config.HttpClientFactory = httpClientFactory;
            return (T)this;
        }

        /// <summary>
        /// Allows developers to configure their own valid authorities. A json string similar to https://aka.ms/aad-instance-discovery should be provided.
        /// MSAL uses this information to: 
        /// <list type="bullet">
        /// <item><description>Call REST APIs on the environment specified in the preferred_network</description></item>
        /// <item><description>Identify an environment under which to save tokens and accounts in the cache</description></item>
        /// <item><description>Use the environment aliases to match tokens issued to other authorities</description></item>
        /// </list>
        /// For more details see https://aka.ms/msal-net-custom-instance-metadata
        /// </summary>
        /// <remarks>
        /// Developers take responsibility for authority validation if they use this method. Should not be used when the authority is not know in advance. 
        /// Has no effect on ADFS or B2C authorities, only for AAD authorities</remarks>
        /// <param name="instanceDiscoveryJson"></param>
        /// <returns></returns>
        [Obsolete("This method name has a typo, please use WithInstanceDiscoveryMetadata instead", false)]
        [EditorBrowsable(EditorBrowsableState.Never)]
        public T WithInstanceDicoveryMetadata(string instanceDiscoveryJson)
        {
            if (string.IsNullOrEmpty(instanceDiscoveryJson))
            {
                throw new ArgumentNullException(instanceDiscoveryJson);
            }

            try
            {
                InstanceDiscoveryResponse instanceDiscovery = JsonHelper.DeserializeFromJson<InstanceDiscoveryResponse>(instanceDiscoveryJson);
                Config.CustomInstanceDiscoveryMetadata = instanceDiscovery;
                return (T)this;
            }
            catch (JsonException ex)
            {
                throw new MsalClientException(
                    MsalError.InvalidUserInstanceMetadata,
                    MsalErrorMessage.InvalidUserInstanceMetadata,
                    ex);
            }
        }

        /// <summary>
        /// Allows developers to configure their own valid authorities. A json string similar to https://aka.ms/aad-instance-discovery should be provided.
        /// MSAL uses this information to: 
        /// <list type="bullet">
        /// <item><description>Call REST APIs on the environment specified in the preferred_network</description></item>
        /// <item><description>Identify an environment under which to save tokens and accounts in the cache</description></item>
        /// <item><description>Use the environment aliases to match tokens issued to other authorities</description></item>
        /// </list>
        /// For more details see https://aka.ms/msal-net-custom-instance-metadata
        /// </summary>
        /// <remarks>
        /// Developers take responsibility for authority validation if they use this method. Should not be used when the authority is not known in advance. 
        /// Has no effect on ADFS or B2C authorities, only for AAD authorities</remarks>
        /// <param name="instanceDiscoveryJson"></param>
        /// <returns></returns>
        public T WithInstanceDiscoveryMetadata(string instanceDiscoveryJson)
        {
            if (string.IsNullOrEmpty(instanceDiscoveryJson))
            {
                throw new ArgumentNullException(instanceDiscoveryJson);
            }

            try
            {
                InstanceDiscoveryResponse instanceDiscovery = JsonHelper.DeserializeFromJson<InstanceDiscoveryResponse>(instanceDiscoveryJson);
                Config.CustomInstanceDiscoveryMetadata = instanceDiscovery;
                return (T)this;
            }
            catch (JsonException ex)
            {
                throw new MsalClientException(
                    MsalError.InvalidUserInstanceMetadata,
                    MsalErrorMessage.InvalidUserInstanceMetadata,
                    ex);
            }
        }

        /// <summary>
        /// Lets an organization setup their own service to handle instance discovery, which enables better caching for microservice/service environments.
        /// A Uri that returns a response similar to https://aka.ms/aad-instance-discovery should be provided. MSAL uses this information to: 
        /// <list type="bullet">
        /// <item><description>Call REST APIs on the environment specified in the preferred_network</description></item>
        /// <item><description>Identify an environment under which to save tokens and accounts in the cache</description></item>
        /// <item><description>Use the environment aliases to match tokens issued to other authorities</description></item>
        /// </list>
        /// For more details see https://aka.ms/msal-net-custom-instance-metadata
        /// </summary>
        /// <remarks>
        /// Developers take responsibility for authority validation if they use this method. Should not be used when the authority is not know in advance. 
        /// Has no effect on ADFS or B2C authorities, only for AAD authorities</remarks>
        /// <param name="instanceDiscoveryUri"></param>
        /// <returns></returns>
        [Obsolete("This method name has a typo, please use WithInstanceDiscoveryMetadata instead", false)]
        [EditorBrowsable(EditorBrowsableState.Never)]
        public T WithInstanceDicoveryMetadata(Uri instanceDiscoveryUri)
        {
            Config.CustomInstanceDiscoveryMetadataUri = instanceDiscoveryUri ??
                throw new ArgumentNullException(nameof(instanceDiscoveryUri));

            return (T)this;
        }

        /// <summary>
        /// Lets an organization setup their own service to handle instance discovery, which enables better caching for microservice/service environments.
        /// A Uri that returns a response similar to https://aka.ms/aad-instance-discovery should be provided. MSAL uses this information to: 
        /// <list type="bullet">
        /// <item><description>Call REST APIs on the environment specified in the preferred_network</description></item>
        /// <item><description>Identify an environment under which to save tokens and accounts in the cache</description></item>
        /// <item><description>Use the environment aliases to match tokens issued to other authorities</description></item>
        /// </list>
        /// For more details see https://aka.ms/msal-net-custom-instance-metadata
        /// </summary>
        /// <remarks>
        /// Developers take responsibility for authority validation if they use this method. Should not be used when the authority is not known in advance. 
        /// Has no effect on ADFS or B2C authorities, only for AAD authorities</remarks>
        /// <param name="instanceDiscoveryUri"></param>
        /// <returns></returns>
        public T WithInstanceDiscoveryMetadata(Uri instanceDiscoveryUri)
        {
            Config.CustomInstanceDiscoveryMetadataUri = instanceDiscoveryUri ??
                throw new ArgumentNullException(nameof(instanceDiscoveryUri));

            return (T)this;
        }

        internal T WithHttpManager(IHttpManager httpManager)
        {
            Config.HttpManager = httpManager;
            return (T)this;
        }

        /// <summary>
        /// Options for MSAL token caches. 
        /// 
        /// MSAL maintains a token cache internally in memory. By default, this cache object is part of each instance of <see cref="PublicClientApplication"/> or <see cref="ConfidentialClientApplication"/>.
        /// This method allows customization of the in-memory token cache of MSAL. 
        /// 
        /// MSAL's memory cache is different than token cache serialization. Cache serialization pulls the tokens from a cache (e.g. Redis, Cosmos, or a file on disk), 
        /// where they are stored in JSON format, into MSAL's internal memory cache. Memory cache operations do not involve JSON operations. 
        /// 
        /// External cache serialization remains the recommended way to handle desktop apps, web site and web APIs, as it provides persistence. These options
        /// do not currently control external cache serialization.
        /// 
        /// Detailed guidance for each application type and platform:
        /// https://aka.ms/msal-net-token-cache-serialization
        /// </summary>
        /// <param name="options">Options for the internal MSAL token caches. </param>
#if !SUPPORTS_CUSTOM_CACHE || WINDOWS_APP
    [EditorBrowsable(EditorBrowsableState.Never)]
#endif
        public T WithCacheOptions(CacheOptions options)
        {
#if !SUPPORTS_CUSTOM_CACHE || WINDOWS_APP
            throw new PlatformNotSupportedException("WithCacheOptions is supported only on platforms where MSAL stores tokens in memory and not on mobile platforms or UWP.");
#else

            Config.AccessorOptions = options;
            return (T)this;
#endif
        }

        internal T WithPlatformProxy(IPlatformProxy platformProxy)
        {
            Config.PlatformProxy = platformProxy;
            return (T)this;
        }

        internal T WithUserTokenCacheInternalForTest(ITokenCacheInternal tokenCacheInternal)
        {
            Config.UserTokenCacheInternalForTest = tokenCacheInternal;
            return (T)this;
        }

        /// <summary>
        /// Enables legacy ADAL cache serialization and deserialization.
        /// </summary>
        /// <param name="enableLegacyCacheCompatibility">Enable legacy ADAL cache compatibility.</param>
        /// <returns>The builder to chain the .With methods.</returns>
        /// <remarks>
        /// ADAL is a previous legacy generation of MSAL.NET authentication library. 
        /// If you don't use <c>.WithLegacyCacheCompatibility(false)</c>, then by default, the ADAL cache is used
        /// (along with MSAL cache). <c>true</c> flag is only needed for specific migration scenarios 
        /// from ADAL.NET to MSAL.NET when both library versions are running side-by-side.
        /// To improve performance add <c>.WithLegacyCacheCompatibility(false)</c> unless you care about migration scenarios.
        /// </remarks>
        public T WithLegacyCacheCompatibility(bool enableLegacyCacheCompatibility = true)
        {
            Config.LegacyCacheCompatibilityEnabled = enableLegacyCacheCompatibility;
            return (T)this;
        }

        /// <summary>
        /// Sets the logging callback. For details see https://aka.ms/msal-net-logging
        /// </summary>
        /// <param name="loggingCallback"></param>
        /// <param name="logLevel">Desired level of logging.  The default is LogLevel.Info</param>
        /// <param name="enablePiiLogging">Boolean used to enable/disable logging of
        /// Personally Identifiable Information (PII).
        /// PII logs are never written to default outputs like Console, Logcat or NSLog
        /// Default is set to <c>false</c>, which ensures that your application is compliant with GDPR.
        /// You can set it to <c>true</c> for advanced debugging requiring PII
        /// If both WithLogging apis are set, the other one will overide the this one
        /// </param>
        /// <param name="enableDefaultPlatformLogging">Flag to enable/disable logging to platform defaults.
        /// In Desktop/UWP, Event Tracing is used. In iOS, NSLog is used.
        /// In android, Logcat is used. The default value is <c>false</c>
        /// </param>
        /// <returns>The builder to chain the .With methods</returns>
        /// <exception cref="InvalidOperationException"/> is thrown if the loggingCallback
        /// was already set on the application builder
        public T WithLogging(
            LogCallback loggingCallback,
            LogLevel? logLevel = null,
            bool? enablePiiLogging = null,
            bool? enableDefaultPlatformLogging = null)
        {
            if (Config.LoggingCallback != null)
            {
                throw new InvalidOperationException(MsalErrorMessage.LoggingCallbackAlreadySet);
            }

            Config.LoggingCallback = loggingCallback;
            Config.LogLevel = logLevel ?? Config.LogLevel;
            Config.EnablePiiLogging = enablePiiLogging ?? Config.EnablePiiLogging;
            Config.IsDefaultPlatformLoggingEnabled = enableDefaultPlatformLogging ?? Config.IsDefaultPlatformLoggingEnabled;
            return (T)this;
        }

#if !XAMARINMAC2_0
        /// <summary>
        /// Sets the Identity Logger. For details see https://aka.ms/msal-net-logging
        /// </summary>
        /// <param name="identityLogger">IdentityLogger</param>
        /// <param name="enablePiiLogging">Boolean used to enable/disable logging of
        /// Personally Identifiable Information (PII).
        /// PII logs are never written to default outputs like Console, Logcat or NSLog
        /// Default is set to <c>false</c>, which ensures that your application is compliant with GDPR.
        /// You can set it to <c>true</c> for advanced debugging requiring PII
        /// If both WithLogging apis are set, this one will override the other
        /// </param>
        /// <returns>The builder to chain the .With methods</returns>
        /// <remarks>This is an experimental API. The method signature may change in the future without involving a major version upgrade.</remarks>
        public T WithLogging(
            IIdentityLogger identityLogger,
            bool enablePiiLogging = false)
        {
            ValidateUseOfExperimentalFeature("IIdentityLogger");

            Config.IdentityLogger = identityLogger;
            Config.EnablePiiLogging = enablePiiLogging;
            return (T)this;
        }
#endif

        /// <summary>
        /// Sets the Debug logging callback to a default debug method which displays
        /// the level of the message and the message itself. For details see https://aka.ms/msal-net-logging
        /// </summary>
        /// <param name="logLevel">Desired level of logging.  The default is LogLevel.Info</param>
        /// <param name="enablePiiLogging">Boolean used to enable/disable logging of
        /// Personally Identifiable Information (PII).
        /// PII logs are never written to default outputs like Console, Logcat or NSLog
        /// Default is set to <c>false</c>, which ensures that your application is compliant with GDPR.
        /// You can set it to <c>true</c> for advanced debugging requiring PII
        /// </param>
        /// <param name="withDefaultPlatformLoggingEnabled">Flag to enable/disable logging to platform defaults.
        /// In Desktop/UWP, Event Tracing is used. In iOS, NSLog is used.
        /// In android, logcat is used. The default value is <c>false</c>
        /// </param>
        /// <returns>The builder to chain the .With methods</returns>
        /// <exception cref="InvalidOperationException"/> is thrown if the loggingCallback
        /// was already set on the application builder by calling <see cref="WithLogging(LogCallback, LogLevel?, bool?, bool?)"/>
        /// <seealso cref="WithLogging(LogCallback, LogLevel?, bool?, bool?)"/>
        public T WithDebugLoggingCallback(
            LogLevel logLevel = LogLevel.Info,
            bool enablePiiLogging = false,
            bool withDefaultPlatformLoggingEnabled = false)
        {
            WithLogging(
                (level, message, pii) => { Debug.WriteLine($"{level}: {message}"); },
                logLevel,
                enablePiiLogging,
                withDefaultPlatformLoggingEnabled);
            return (T)this;
        }

        /// <summary>
        /// Sets the telemetry callback. For details see https://aka.ms/msal-net-telemetry
        /// </summary>
        /// <param name="telemetryCallback">Delegate to the callback sending the telemetry
        /// elaborated by the library to the telemetry endpoint of choice</param>
        /// <returns>The builder to chain the .With methods</returns>
        /// <exception cref="InvalidOperationException"/> is thrown if the method was already
        /// called on the application builder.
        [Obsolete("Telemetry is sent automatically by MSAL.NET. See https://aka.ms/msal-net-telemetry.", false)]
        [EditorBrowsable(EditorBrowsableState.Never)]
        internal T WithTelemetry(TelemetryCallback telemetryCallback)
        {
            return (T)this;
        }

        /// <summary>
        /// Sets the Client ID of the application
        /// </summary>
        /// <param name="clientId">Client ID (also known as <i>Application ID</i>) of the application as registered in the
        ///  application registration portal (https://aka.ms/msal-net-register-app)</param>
        /// <returns>The builder to chain the .With methods</returns>
        public T WithClientId(string clientId)
        {
            Config.ClientId = clientId;
            return (T)this;
        }

        /// <summary>
        /// Sets the redirect URI of the application. See https://aka.ms/msal-net-application-configuration
        /// </summary>
        /// <param name="redirectUri">URL where the STS will call back the application with the security token.
        /// This parameter is not required for desktop or UWP applications (as a default is used).
        /// It's not required for mobile applications that don't use a broker
        /// It is required for web apps</param>
        /// <returns>The builder to chain the .With methods</returns>
        public T WithRedirectUri(string redirectUri)
        {
            Config.RedirectUri = GetValueIfNotEmpty(Config.RedirectUri, redirectUri);
            return (T)this;
        }

        /// <summary>
        /// Sets the Tenant Id of the organization from which the application will let
        /// users sign-in. This is classically a GUID or a domain name. See https://aka.ms/msal-net-application-configuration.
        /// Although it is also possible to set <paramref name="tenantId"/> to <c>common</c>,
        /// <c>organizations</c>, and <c>consumers</c>, it's recommended to use one of the
        /// overrides of <see cref="WithAuthority(AzureCloudInstance, AadAuthorityAudience, bool)"/>
        /// </summary>
        /// <param name="tenantId">tenant ID of the Azure AD tenant
        /// or a domain associated with this Azure AD tenant, in order to sign-in a user of a specific organization only</param>
        /// <returns>The builder to chain the .With methods</returns>
        public T WithTenantId(string tenantId)
        {
            Config.TenantId = GetValueIfNotEmpty(Config.TenantId, tenantId);
            return (T)this;
        }

        /// <summary>
        /// Sets the name of the calling application for telemetry purposes.
        /// </summary>
        /// <param name="clientName">The name of the application for telemetry purposes.</param>
        /// <returns></returns>
        public T WithClientName(string clientName)
        {
            Config.ClientName = GetValueIfNotEmpty(Config.ClientName, clientName);
            return (T)this;
        }

        /// <summary>
        /// Sets the version of the calling application for telemetry purposes.
        /// </summary>
        /// <param name="clientVersion">The version of the calling application for telemetry purposes.</param>
        /// <returns></returns>
        public T WithClientVersion(string clientVersion)
        {
            Config.ClientVersion = GetValueIfNotEmpty(Config.ClientVersion, clientVersion);
            return (T)this;
        }

        /// <summary>
        /// Sets application options, which can, for instance have been read from configuration files.
        /// See https://aka.ms/msal-net-application-configuration.
        /// </summary>
        /// <param name="applicationOptions">Application options</param>
        /// <returns>The builder to chain the .With methods</returns>
        protected T WithOptions(ApplicationOptions applicationOptions)
        {
            WithClientId(applicationOptions.ClientId);
            WithRedirectUri(applicationOptions.RedirectUri);
            WithTenantId(applicationOptions.TenantId);
            WithClientName(applicationOptions.ClientName);
            WithClientVersion(applicationOptions.ClientVersion);
            WithClientCapabilities(applicationOptions.ClientCapabilities);
            WithLegacyCacheCompatibility(applicationOptions.LegacyCacheCompatibilityEnabled);

            WithLogging(
                null,
                applicationOptions.LogLevel,
                applicationOptions.EnablePiiLogging,
                applicationOptions.IsDefaultPlatformLoggingEnabled);

            Config.Instance = applicationOptions.Instance;
            Config.AadAuthorityAudience = applicationOptions.AadAuthorityAudience;
            Config.AzureCloudInstance = applicationOptions.AzureCloudInstance;

            return (T)this;
        }

        /// <summary>
        /// Sets Extra Query Parameters for the query string in the HTTP authentication request
        /// </summary>
        /// <param name="extraQueryParameters">This parameter will be appended as is to the query string in the HTTP authentication request to the authority
        /// as a string of segments of the form <c>key=value</c> separated by an ampersand character.
        /// The parameter can be null.</param>
        /// <returns>The builder to chain the .With methods</returns>
        public T WithExtraQueryParameters(IDictionary<string, string> extraQueryParameters)
        {
            Config.ExtraQueryParameters = extraQueryParameters ?? new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);
            return (T)this;
        }

        /// <summary>
        /// Sets Extra Query Parameters for the query string in the HTTP authentication request
        /// </summary>
        /// <param name="extraQueryParameters">This parameter will be appended as is to the query string in the HTTP authentication request to the authority.
        /// The string needs to be properly URL-encoded and ready to send as a string of segments of the form <c>key=value</c> separated by an ampersand character.
        /// </param>
        /// <returns></returns>
        public T WithExtraQueryParameters(string extraQueryParameters)
        {
            if (!string.IsNullOrWhiteSpace(extraQueryParameters))
            {
                return WithExtraQueryParameters(CoreHelpers.ParseKeyValueList(extraQueryParameters, '&', true, null));
            }
            return (T)this;
        }

        /// <summary>
        /// Allows usage of experimental features and APIs. If this flag is not set, experimental features 
        /// will throw an exception. For details see https://aka.ms/msal-net-experimental-features
        /// </summary>
        /// <remarks>
        /// Changes in the public API of experimental features will not result in an increment of the major version of this library.
        /// For these reason we advise against using these features in production.
        /// </remarks>
        public T WithExperimentalFeatures(bool enableExperimentalFeatures = true)
        {
            Config.ExperimentalFeaturesEnabled = enableExperimentalFeatures;
            return (T)this;
        }

        /// <summary>
        /// Microsoft Identity specific OIDC extension that allows resource challenges to be resolved without interaction. 
        /// Allows configuration of one or more client capabilities, e.g. "llt"
        /// </summary>
        /// <remarks>
        /// MSAL will transform these into special claims request. See https://openid.net/specs/openid-connect-core-1_0-final.html#ClaimsParameter for
        /// details on claim requests.
        /// For more details see https://aka.ms/msal-net-claims-request
        /// </remarks>
        public T WithClientCapabilities(IEnumerable<string> clientCapabilities)
        {
            if (clientCapabilities != null && clientCapabilities.Any())
            {
                Config.ClientCapabilities = clientCapabilities;
            }

            return (T)this;
        }

        /// <summary>
        /// Generate telemetry aggregation events.
        /// </summary>
        /// <param name="telemetryConfig"></param>
        /// <returns></returns>
        [Obsolete("Telemetry is sent automatically by MSAL.NET. See https://aka.ms/msal-net-telemetry.", false)]
        [EditorBrowsable(EditorBrowsableState.Never)]
        public T WithTelemetry(ITelemetryConfig telemetryConfig)
        {
            return (T)this;
        }

        internal virtual void Validate()
        {
            if (string.IsNullOrWhiteSpace(Config.ClientId))
            {
                throw new MsalClientException(MsalError.NoClientId, MsalErrorMessage.NoClientIdWasSpecified);
            }

            if (Config.CustomInstanceDiscoveryMetadata != null && Config.CustomInstanceDiscoveryMetadataUri != null)
            {
                throw new MsalClientException(
                    MsalError.CustomMetadataInstanceOrUri,
                    MsalErrorMessage.CustomMetadataInstanceOrUri);
            }

            if (Config.Authority.AuthorityInfo.ValidateAuthority &&
                (Config.CustomInstanceDiscoveryMetadata != null || Config.CustomInstanceDiscoveryMetadataUri != null))
            {
                throw new MsalClientException(MsalError.ValidateAuthorityOrCustomMetadata, MsalErrorMessage.ValidateAuthorityOrCustomMetadata);
            }
        }

        internal ApplicationConfiguration BuildConfiguration()
        {
            ResolveAuthority();
            Validate();
            return Config;
        }

        internal void ValidateUseOfExperimentalFeature([System.Runtime.CompilerServices.CallerMemberName] string memberName = "")
        {
            if (!Config.ExperimentalFeaturesEnabled)
            {
                throw new MsalClientException(
                    MsalError.ExperimentalFeature,
                    MsalErrorMessage.ExperimentalFeature(memberName));
            }
        }


        #region Authority
        private void ResolveAuthority()
        {
            if (Config.Authority?.AuthorityInfo != null)
            {
                var isB2C = Config.Authority is B2CAuthority;
                
                AadAuthority aadAuthority = Config.Authority as AadAuthority;
                if (!string.IsNullOrEmpty(Config.TenantId) 
                    && !isB2C
                    && aadAuthority != null )
                {
                    if (!aadAuthority.IsCommonOrganizationsOrConsumersTenant() &&
                        !string.Equals(aadAuthority.TenantId, Config.TenantId))
                    {
                        throw new MsalClientException(
                            MsalError.AuthorityTenantSpecifiedTwice,
                            "You specified a different tenant - once in WithAuthority() and once using WithTenant().");
                    }

                    Config.Authority = Authority.CreateAuthorityWithTenant(Config.Authority.AuthorityInfo, Config.TenantId);
                }
            }
            else
            {
                string authorityInstance = GetAuthorityInstance();
                string authorityAudience = GetAuthorityAudience();

                var authorityInfo = new AuthorityInfo(
                        AuthorityType.Aad,
                        new Uri($"{authorityInstance}/{authorityAudience}").ToString(),
                        Config.ValidateAuthority);

                Config.Authority = new AadAuthority(authorityInfo);
            }
        }

        private string GetAuthorityAudience()
        {
            if (!string.IsNullOrWhiteSpace(Config.TenantId) &&
                Config.AadAuthorityAudience != AadAuthorityAudience.None &&
                Config.AadAuthorityAudience != AadAuthorityAudience.AzureAdMyOrg)
            {
                // Conflict, user has specified a string tenantId and the enum audience value for AAD, which is also the tenant.
                throw new InvalidOperationException(MsalErrorMessage.TenantIdAndAadAuthorityInstanceAreMutuallyExclusive);
            }

            if (Config.AadAuthorityAudience != AadAuthorityAudience.None)
            {
                return AuthorityInfo.GetAadAuthorityAudienceValue(Config.AadAuthorityAudience, Config.TenantId);
            }

            if (!string.IsNullOrWhiteSpace(Config.TenantId))
            {
                return Config.TenantId;
            }

            return AuthorityInfo.GetAadAuthorityAudienceValue(AadAuthorityAudience.AzureAdAndPersonalMicrosoftAccount, string.Empty);
        }

        private string GetAuthorityInstance()
        {
            // Check if there's enough information in the existing config to build up a default authority.
            if (!string.IsNullOrWhiteSpace(Config.Instance) && Config.AzureCloudInstance != AzureCloudInstance.None)
            {
                // Conflict, user has specified a string instance and the enum instance value.
                throw new InvalidOperationException(MsalErrorMessage.InstanceAndAzureCloudInstanceAreMutuallyExclusive);
            }

            if (!string.IsNullOrWhiteSpace(Config.Instance))
            {
                Config.Instance = Config.Instance.TrimEnd(' ', '/');
                return Config.Instance;
            }

            if (Config.AzureCloudInstance != AzureCloudInstance.None)
            {
                return AuthorityInfo.GetCloudUrl(Config.AzureCloudInstance);
            }

            return AuthorityInfo.GetCloudUrl(AzureCloudInstance.AzurePublic);
        }

        /// <summary>
        /// Adds a known authority to the application from its Uri. See https://aka.ms/msal-net-application-configuration.
        /// This constructor is mainly used for scenarios where the authority is not a standard Azure AD authority,
        /// nor an ADFS authority, nor an Azure AD B2C authority. For Azure AD, even in national and sovereign clouds, prefer
        /// using other overrides such as <see cref="WithAuthority(AzureCloudInstance, AadAuthorityAudience, bool)"/>
        /// </summary>
        /// <param name="authorityUri">Uri of the authority</param>
        /// <param name="validateAuthority">Whether the authority should be validated against the server metadata.</param>
        /// <returns>The builder to chain the .With methods</returns>
        public T WithAuthority(Uri authorityUri, bool validateAuthority = true)
        {
            if (authorityUri == null)
            {
                throw new ArgumentNullException(nameof(authorityUri));
            }

            return WithAuthority(authorityUri.ToString(), validateAuthority);
        }

        /// <summary>
        /// Adds a known Azure AD authority to the application to sign-in users specifying
        /// the full authority Uri. See https://aka.ms/msal-net-application-configuration.
        /// </summary>
        /// <param name="authorityUri">URL of the security token service (STS) from which MSAL.NET will acquire the tokens.
        ///  Usual authorities endpoints for the Azure public Cloud are:
        ///  <list type="bullet">
        ///  <item><description><c>https://login.microsoftonline.com/tenant/</c> where <c>tenant</c> is the tenant ID of the Azure AD tenant
        ///  or a domain associated with this Azure AD tenant, in order to sign-in users of a specific organization only</description></item>
        ///  <item><description><c>https://login.microsoftonline.com/common/</c> to sign-in users with any work and school accounts or Microsoft personal account</description></item>
        ///  <item><description><c>https://login.microsoftonline.com/organizations/</c> to sign-in users with any work and school accounts</description></item>
        ///  <item><description><c>https://login.microsoftonline.com/consumers/</c> to sign-in users with only personal Microsoft accounts (live)</description></item>
        ///  </list>
        ///  Note that this setting needs to be consistent with what is declared in the application registration portal</param>
        /// <param name="validateAuthority">Whether the authority should be validated against the server metadata.</param>
        /// <returns>The builder to chain the .With methods</returns>
        public T WithAuthority(string authorityUri, bool validateAuthority = true)
        {
            if (string.IsNullOrWhiteSpace(authorityUri))
            {
                throw new ArgumentNullException(authorityUri);
            }

            Config.Authority = Authority.CreateAuthority(authorityUri, validateAuthority);

            return (T)this;
        }

        /// <summary>
        /// Adds a known Azure AD authority to the application to sign-in users from a single
        /// organization (single tenant application) specified by its tenant ID. See https://aka.ms/msal-net-application-configuration.
        /// </summary>
        /// <param name="cloudInstanceUri">Azure Cloud instance.</param>
        /// <param name="tenantId">GUID of the tenant from which to sign-in users.</param>
        /// <param name="validateAuthority">Whether the authority should be validated against the server metadata.</param>
        /// <returns>The builder to chain the .With methods.</returns>
        public T WithAuthority(
            string cloudInstanceUri,
            Guid tenantId,
            bool validateAuthority = true)
        {
            WithAuthority(cloudInstanceUri, tenantId.ToString("D", CultureInfo.InvariantCulture), validateAuthority);
            return (T)this;
        }

        /// <summary>
        /// Adds a known Azure AD authority to the application to sign-in users from a single
        /// organization (single tenant application) described by its domain name. See https://aka.ms/msal-net-application-configuration.
        /// </summary>
        /// <param name="cloudInstanceUri">Uri to the Azure Cloud instance (for instance
        /// <c>https://login.microsoftonline.com)</c></param>
        /// <param name="tenant">domain name associated with the tenant from which to sign-in users</param>
        /// <param name="validateAuthority">Whether the authority should be validated against the server metadata.</param>
        /// <remarks>
        /// <paramref name="tenant"/> can also contain the string representation of a GUID (tenantId),
        /// or even <c>common</c>, <c>organizations</c> or <c>consumers</c> but in this case
        /// it's recommended to use another override (<see cref="WithAuthority(AzureCloudInstance, Guid, bool)"/>
        /// and <see cref="WithAuthority(AzureCloudInstance, AadAuthorityAudience, bool)"/>
        /// </remarks>
        /// <returns>The builder to chain the .With methods</returns>
        public T WithAuthority(
            string cloudInstanceUri,
            string tenant,
            bool validateAuthority = true)
        {
            if (string.IsNullOrWhiteSpace(cloudInstanceUri))
            {
                throw new ArgumentNullException(nameof(cloudInstanceUri));
            }
            if (string.IsNullOrWhiteSpace(tenant))
            {
                throw new ArgumentNullException(nameof(tenant));
            }

            var authorityInfo = AuthorityInfo.FromAadAuthority(
                new Uri(cloudInstanceUri),
                tenant,
                validateAuthority);
            Config.Authority = new AadAuthority(authorityInfo);

            return (T)this;
        }

        /// <summary>
        /// Adds a known Azure AD authority to the application to sign-in users from a single
        /// organization (single tenant application) described by its cloud instance and its tenant ID.
        /// See https://aka.ms/msal-net-application-configuration.
        /// </summary>
        /// <param name="azureCloudInstance">Instance of Azure Cloud (for instance Azure
        /// worldwide cloud, Azure German Cloud, US government ...)</param>
        /// <param name="tenantId">Tenant Id of the tenant from which to sign-in users</param>
        /// <param name="validateAuthority">Whether the authority should be validated against the server metadata.</param>
        /// <returns>The builder to chain the .With methods</returns>
        public T WithAuthority(
            AzureCloudInstance azureCloudInstance,
            Guid tenantId,
            bool validateAuthority = true)
        {
            WithAuthority(azureCloudInstance, tenantId.ToString("D", CultureInfo.InvariantCulture), validateAuthority);
            return (T)this;
        }

        /// <summary>
        /// Adds a known Azure AD authority to the application to sign-in users from a single
        /// organization (single tenant application) described by its cloud instance and its domain
        /// name or tenant ID. See https://aka.ms/msal-net-application-configuration.
        /// </summary>
        /// <param name="azureCloudInstance">Instance of Azure Cloud (for instance Azure
        /// worldwide cloud, Azure German Cloud, US government ...).</param>
        /// <param name="tenant">Domain name associated with the Azure AD tenant from which
        /// to sign-in users. This can also be a GUID.</param>
        /// <param name="validateAuthority">Whether the authority should be validated against the server metadata.</param>
        /// <returns>The builder to chain the .With methods.</returns>
        public T WithAuthority(
            AzureCloudInstance azureCloudInstance,
            string tenant,
            bool validateAuthority = true)
        {
            if (string.IsNullOrWhiteSpace(tenant))
            {
                throw new ArgumentNullException(nameof(tenant));
            }

            Config.AzureCloudInstance = azureCloudInstance;
            Config.TenantId = tenant;
            Config.ValidateAuthority = validateAuthority;

            return (T)this;
        }

        /// <summary>
        /// Adds a known Azure AD authority to the application to sign-in users specifying
        /// the cloud instance and the sign-in audience. See https://aka.ms/msal-net-application-configuration.
        /// </summary>
        /// <param name="azureCloudInstance">Instance of Azure Cloud (for instance Azure
        /// worldwide cloud, Azure German Cloud, US government ...)</param>
        /// <param name="authorityAudience">Sign-in audience (one AAD organization,
        /// any work and school accounts, or any work and school accounts and Microsoft personal
        /// accounts</param>
        /// <param name="validateAuthority">Whether the authority should be validated against the server metadata.</param>
        /// <returns>The builder to chain the .With methods</returns>
        public T WithAuthority(AzureCloudInstance azureCloudInstance, AadAuthorityAudience authorityAudience, bool validateAuthority = true)
        {
            Config.AzureCloudInstance = azureCloudInstance;
            Config.AadAuthorityAudience = authorityAudience;
            Config.ValidateAuthority = validateAuthority;

            return (T)this;
        }

        /// <summary>
        /// Adds a known Azure AD authority to the application to sign-in users specifying
        /// the sign-in audience (the cloud being the Azure public cloud). See https://aka.ms/msal-net-application-configuration.
        /// </summary>
        /// <param name="authorityAudience">Sign-in audience (one AAD organization,
        /// any work and school accounts, or any work and school accounts and Microsoft personal
        /// accounts</param>
        /// <param name="validateAuthority">Whether the authority should be validated against the server metadata.</param>
        /// <returns>The builder to chain the .With methods</returns>
        public T WithAuthority(AadAuthorityAudience authorityAudience, bool validateAuthority = true)
        {
            Config.AadAuthorityAudience = authorityAudience;
            Config.ValidateAuthority = validateAuthority;
            return (T)this;
        }

        /// <summary>
        /// Adds a known Authority corresponding to an ADFS server. See https://aka.ms/msal-net-adfs
        /// </summary>
        /// <param name="authorityUri">Authority URL for an ADFS server</param>
        /// <param name="validateAuthority">Whether the authority should be validated against the server metadata.</param>
        /// <remarks>MSAL.NET will only support ADFS 2019 or later.</remarks>
        /// <returns>The builder to chain the .With methods</returns>
        public T WithAdfsAuthority(string authorityUri, bool validateAuthority = true)
        {

            var authorityInfo = AuthorityInfo.FromAdfsAuthority(authorityUri, validateAuthority);
            Config.Authority = AdfsAuthority.CreateAuthority(authorityInfo);
            return (T)this;
        }

        /// <summary>
        /// Adds a known authority corresponding to an Azure AD B2C policy.
        /// See https://aka.ms/msal-net-b2c-specificities
        /// </summary>
        /// <param name="authorityUri">Azure AD B2C authority, including the B2C policy (for instance
        /// <c>"https://fabrikamb2c.b2clogin.com/tfp/{Tenant}/{policy}</c></param>)
        /// <returns>The builder to chain the .With methods</returns>
        public T WithB2CAuthority(string authorityUri)
        {
            var authorityInfo = AuthorityInfo.FromB2CAuthority(authorityUri);
            Config.Authority = B2CAuthority.CreateAuthority(authorityInfo);

            return (T)this;
        }

#endregion

        private static string GetValueIfNotEmpty(string original, string value)
        {
            return string.IsNullOrWhiteSpace(value) ? original : value;
        }
    }
}
