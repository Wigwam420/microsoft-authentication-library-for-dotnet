﻿// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

using System;
using System.Globalization;
using Microsoft.Identity.Client.Core;
using Microsoft.Identity.Client.PlatformsCommon.Interfaces;

namespace Microsoft.Identity.Client.TelemetryCore.Internal.Events
{
    internal class ApiEvent
    {
        public enum ApiIds
        {
            None = 0,
            AcquireTokenByAuthorizationCode = 1000,
            AcquireTokenByRefreshToken = 1001,
            AcquireTokenByIntegratedWindowsAuth = 1002,
            AcquireTokenByUsernamePassword = 1003,
            AcquireTokenForClient = 1004,
            AcquireTokenInteractive = 1005,
            AcquireTokenOnBehalfOf = 1006,
            AcquireTokenSilent = 1007,
            AcquireTokenByDeviceCode = 1008,
            GetAuthorizationRequestUrl = 1009,
            GetAccounts = 1010,
            GetAccountById = 1011,
            GetAccountsByUserFlow = 1012,
            RemoveAccount = 1013
        }

        private readonly ICryptographyManager _cryptographyManager;
        private readonly ICoreLogger _logger;

        public ApiEvent(
            ICoreLogger logger,
            ICryptographyManager cryptographyManager,
            Guid correlationId)
        {
            _logger = logger;
            _cryptographyManager = cryptographyManager;
            CorrelationId = correlationId;
        }

        public Guid CorrelationId { get; set; }

        public ApiIds ApiId { get; set; }

        public string ApiIdString
        {
            get => ((int)ApiId).ToString(CultureInfo.InvariantCulture);
        }

        public string TokenEndpoint { get; set; }

        // Some of these properties like Authority, TenantId, LoginHint, etc.
        // were set only, never used, maybe can be removed?
        public Uri Authority { get; set; }

        public string AuthorityType { get; set; }

        public string Prompt { get; set; }

        public string TenantId { get; set; }

        public string AccountId { get; set; }

        public bool WasSuccessful { get; set; }

        public bool IsConfidentialClient { get; set; }

        public bool IsAccessTokenCacheHit { get; set; }

        public string ApiErrorCode { get; set; }

        public string LoginHint { get; set; }

        #region Region
        public string RegionUsed { get; set; }

        private int? _regionAutodetectionSource;
        public int RegionAutodetectionSource
        {
            get { return _regionAutodetectionSource.HasValue ? _regionAutodetectionSource.Value : 0; }
            set { _regionAutodetectionSource = value; }
        }

        public string RegionAutodetectionSourceString
        {
            get => _regionAutodetectionSource.HasValue ? _regionAutodetectionSource.Value.ToString(CultureInfo.InvariantCulture) : null;
        }

        private int? _regionOutcome;
        public int RegionOutcome
        {
            get { return _regionOutcome.HasValue ? _regionOutcome.Value : 0; }
            set { _regionOutcome = value; }
        }

        public string RegionOutcomeString
        {
            get => _regionOutcome.HasValue ? _regionOutcome.Value.ToString(CultureInfo.InvariantCulture) : null;
        }
        #endregion

        public bool IsTokenCacheSerialized { get; set; }

        public string IsTokenCacheSerializedString
        {
            get => IsTokenCacheSerialized.ToString().ToLowerInvariant();
        }

        public bool IsLegacyCacheEnabled { get; set; }

        public string IsLegacyCacheEnabledString
        {
            get => IsLegacyCacheEnabled.ToString().ToLowerInvariant();
        }

        private int? _cacheInfo;
        public int CacheInfo
        {
            get { return _cacheInfo.HasValue ? _cacheInfo.Value : (int)CacheRefreshReason.NotApplicable; }
            set { _cacheInfo = value; }
        }

        public string CacheInfoString
        {
            get => _cacheInfo.HasValue ? _cacheInfo.Value.ToString(CultureInfo.InvariantCulture) : null;
        }

        public long DurationInHttpInMs { get; set; }

        public long DurationInCacheInMs { get; set; }
    }
}
