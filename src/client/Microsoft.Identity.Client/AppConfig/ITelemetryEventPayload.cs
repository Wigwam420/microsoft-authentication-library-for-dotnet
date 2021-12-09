﻿// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

using System;
using System.Collections.Generic;
using System.ComponentModel;

namespace Microsoft.Identity.Client
{
    /// <summary>
    /// Data that represents a single snapshot in the series of events that are collected
    /// </summary>
    /// <remarks>This API is experimental and it may change in future versions of the library without an major version increment</remarks>
    [Obsolete("Telemetry is sent automatically by MSAL.NET. See https://aka.ms/msal-net-telemetry.", false)]
    [EditorBrowsable(EditorBrowsableState.Never)]
    public interface ITelemetryEventPayload
    {
        /// <summary>
        /// 
        /// </summary>
        /// <remarks>This API is experimental and it may change in future versions of the library without an major version increment</remarks>
        /// <returns></returns>
        string Name { get; }

        /// <summary>
        /// 
        /// </summary>
        /// <remarks>This API is experimental and it may change in future versions of the library without an major version increment</remarks>
        IReadOnlyDictionary<string, bool> BoolValues { get; }

        /// <summary>
        /// 
        /// </summary>
        /// <remarks>This API is experimental and it may change in future versions of the library without an major version increment</remarks>
        IReadOnlyDictionary<string, long> Int64Values { get; }

        /// <summary>
        /// 
        /// </summary>
        /// <remarks>This API is experimental and it may change in future versions of the library without an major version increment</remarks>
        IReadOnlyDictionary<string, int> IntValues { get; }

        /// <summary>
        /// 
        /// </summary>
        /// <remarks>This API is experimental and it may change in future versions of the library without an major version increment</remarks>
        IReadOnlyDictionary<string, string> StringValues { get; }

        /// <summary>
        /// Used for debugging and testing.
        /// </summary>
        /// <remarks>This API is experimental and it may change in future versions of the library without an major version increment</remarks>
        /// <returns></returns>
        string ToJsonString();
    }
}
