// <copyright file="TestCaseModel.cs" company="Microsoft Corporation">
// Copyright (c) Microsoft Corporation. All rights reserved.
// </copyright>

namespace AutomationFramework
{
    using Newtonsoft.Json;
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.Security.Cryptography.X509Certificates;

    /// <summary>
    /// TestCaseModel class containing tenants to put common configurations.
    /// </summary>
    public class TestCaseModel
    {
        /// <summary>
        /// Gets or sets the api url.
        /// </summary>
        [JsonProperty("url", Order = 1)]
        public string Url { get; set; }

        /// <summary>
        /// Gets or sets the api headers.
        /// </summary>
        [JsonProperty("headers", Order = 1)]
        public Dictionary<string, string> Headers { get; set; }

        /// <summary>
        /// Gets or sets the query params.
        /// </summary>
        [JsonProperty("queryParams", Order = 1)]
        public Dictionary<string, string> QueryParams { get; set; }

        /// <summary>
        /// Gets or sets the test json payload.
        /// </summary>
        [JsonProperty("jsonPayload", Order = 1)]
        public string JsonPayload { get; set; }

        /// <summary>
        /// Gets or sets the api base address.
        /// </summary>
        [JsonProperty("baseAddress", Order = 1)]
        public string BaseAddress { get; set; }

        /// <summary>
        /// Gets or sets the api certificate.
        /// </summary>
        [JsonProperty("certificate", Order = 1)]
        public X509Certificate2 Certificate { get; set; }

        /// <summary>
        /// Gets or sets the api contentType.
        /// </summary>
        [JsonProperty("contentType", Order = 1)]
        public string ContentType { get; set; }
    }
}
