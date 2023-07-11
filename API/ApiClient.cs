// <copyright file="ApiClient.cs" company="Microsoft Corporation">
// Copyright (c) Microsoft Corporation. All rights reserved.
// </copyright>

namespace AutomationFramework.API
{
    using Microsoft.Identity.Client;
    using System;
    using System.Collections.Generic;
    using System.Globalization;
    using System.Net.Http;
    using System.Net.Http.Headers;
    using System.Security.Cryptography.X509Certificates;
    using System.Text;
    using System.Web;

    /// <summary>
    /// class containing methods to execute an API.
    /// </summary>
    public class ApiClient
    {
        private static HttpClient client = null;

        public static void Authenticate(string clientId, string clientSecret, string aadInstance, string aadTenant, string audience, Dictionary<string, string> headers, X509Certificate2 certificate)
        {
            client = new HttpClient();
            client.DefaultRequestHeaders.Accept.Clear();
            client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json-patch+json"));

            if (certificate != null)
            {
                HttpClientHandler handler = new HttpClientHandler();
                handler.ClientCertificates.Add(certificate);
                client = new HttpClient(handler);

                client.DefaultRequestHeaders.Add("X-ARR-ClientCert", certificate.GetRawCertDataString());
                //client.DefaultRequestHeaders.Add("X-ARR-ClientCert", certificate.GetRawCertData().ToString());

            }

            if (headers != null)
            {
                foreach (KeyValuePair<string, string> keyValuePair in headers)
                {
                    client.DefaultRequestHeaders.Add(keyValuePair.Key, keyValuePair.Value);
                }
            }

            // Add a bearer token in authorization header of the call.
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", GetToken(clientId, clientSecret, aadInstance, aadTenant, audience));
            
            //client.BaseAddress = new Uri(testCaseModel.BaseAddress);

        }

        public static string GetToken(string clientId, string clientSecret, string aadInstance, string aadTenant, string audience)
        {
            // Add your tenant to the authentication context. 
            string authority = string.Format(CultureInfo.InvariantCulture, aadInstance + aadTenant);

            var authContext = ConfidentialClientApplicationBuilder.Create(clientId)
                    .WithClientSecret(clientSecret)
                    .WithAuthority(authority)
                    .Build();

            // Acquire token.
            var result = authContext.AcquireTokenForClient(
                        new[] { $"{audience}/.default" })
                        .ExecuteAsync()
                        .ConfigureAwait(false).GetAwaiter().GetResult();

            // ADAL Version.
            //var authContext = new AuthenticationContext(authority);
            // Add your application Id and App Key credentials.
            //var clientCredentials = new ClientCredential(clientId, clientSecret);
            //AuthenticationResult result = authContext.AcquireTokenAsync(audience, clientCredentials).Result;

            if (result == null)
            {
                throw new InvalidOperationException("Failed to obtain the JWT token.");
            }

            // return token.
            return result.AccessToken;
        }

        public static HttpResponseMessage PutRequest(string requestUrl, string jsonPayload, string clientId, string clientSecret, string aadInstance, string aadTenant, string audience, Dictionary<string, string> headers)
        {
            AutomationFramework.Logger.LOGMessage(AutomationFramework.Logger.MSG.MESSAGE, "\t URL found: " + requestUrl);
            AutomationFramework.Logger.LOGMessage(AutomationFramework.Logger.MSG.MESSAGE, "\t Authenticating...");
            Authenticate(clientId, clientSecret, aadInstance, aadTenant, audience, headers, null);

            AutomationFramework.Logger.LOGMessage(AutomationFramework.Logger.MSG.MESSAGE, "\t Sending request <PUT>...");
            var httpContent = new StringContent(jsonPayload, Encoding.UTF8, "application/json");
            
            var result = client.PutAsync(requestUrl, httpContent).Result;
            AutomationFramework.Logger.LOGMessage(AutomationFramework.Logger.MSG.MESSAGE, "\t Receiving response...");
            AutomationFramework.Logger.LOGMessage(AutomationFramework.Logger.MSG.MESSAGE, "\t \t " + (result.Content.ReadAsStringAsync().Result.Length <= 1000 ? result.Content.ReadAsStringAsync().Result : result.Content.ReadAsStringAsync().Result.Substring(0, 1000)));

            return result;
        }

        public static HttpResponseMessage PostRequest(string requestUrl, string jsonPayload, string clientId, string clientSecret, string aadInstance, string aadTenant, string audience, Dictionary<string, string> headers, Dictionary<string, string> queryParams, string contentType = Constants.CONTENT_TYPE)
        {
            AutomationFramework.Logger.LOGMessage(AutomationFramework.Logger.MSG.MESSAGE, "\t URL found: " + requestUrl);
            AutomationFramework.Logger.LOGMessage(AutomationFramework.Logger.MSG.MESSAGE, "\t Authenticating...");
            Authenticate(clientId, clientSecret, aadInstance, aadTenant, audience, headers, null);

            AutomationFramework.Logger.LOGMessage(AutomationFramework.Logger.MSG.MESSAGE, "\t Sending request <POST>...");
            var httpContent = new StringContent(jsonPayload, Encoding.UTF8, contentType);

            var builder = new UriBuilder(requestUrl);

            var query = HttpUtility.ParseQueryString(builder.Query);
            foreach (KeyValuePair<string, string> keyValuePair in queryParams)
            {
                query[keyValuePair.Key] = keyValuePair.Value;
            }
            builder.Query = query.ToString();

            var result = client.PostAsync(builder.ToString(), httpContent).Result;
            AutomationFramework.Logger.LOGMessage(AutomationFramework.Logger.MSG.MESSAGE, "\t Receiving response...");
            AutomationFramework.Logger.LOGMessage(AutomationFramework.Logger.MSG.MESSAGE, "\t \t " + (result.Content.ReadAsStringAsync().Result.Length <= 1000 ? result.Content.ReadAsStringAsync().Result : result.Content.ReadAsStringAsync().Result.Substring(0, 1000)));

            return result;
        }

        public static HttpResponseMessage PutRequestWithCertificate(string requestUrl, string jsonPayload, string clientId, string clientSecret, string aadInstance, string aadTenant, string audience, Dictionary<string, string> headers, X509Certificate2 certificate)
        {
            AutomationFramework.Logger.LOGMessage(AutomationFramework.Logger.MSG.MESSAGE, "\t URL found: " + requestUrl);
            AutomationFramework.Logger.LOGMessage(AutomationFramework.Logger.MSG.MESSAGE, "\t Authenticating...");
            Authenticate(clientId, clientSecret, aadInstance, aadTenant, audience, headers, certificate);

            AutomationFramework.Logger.LOGMessage(AutomationFramework.Logger.MSG.MESSAGE, "\t Sending request <PUT>...");
            var httpContent = new StringContent(jsonPayload, Encoding.UTF8, "application/json");

            var result = client.PutAsync(requestUrl, httpContent).Result;
            AutomationFramework.Logger.LOGMessage(AutomationFramework.Logger.MSG.MESSAGE, "\t Receiving response..");
            AutomationFramework.Logger.LOGMessage(AutomationFramework.Logger.MSG.MESSAGE, "\t \t " + (result.Content.ReadAsStringAsync().Result.Length <= 1000 ? result.Content.ReadAsStringAsync().Result : result.Content.ReadAsStringAsync().Result.Substring(0, 1000)));

            return result;
        }

        public static HttpResponseMessage GetRequest(string requestUrl, string clientId, string clientSecret, string aadInstance, string aadTenant, string audience, Dictionary<string, string> headers, Dictionary<string, string> queryParams)
        {
            AutomationFramework.Logger.LOGMessage(AutomationFramework.Logger.MSG.MESSAGE, "\t URL found: " + requestUrl);
            AutomationFramework.Logger.LOGMessage(AutomationFramework.Logger.MSG.MESSAGE, "\t Authenticating...");
            Authenticate(clientId, clientSecret, aadInstance, aadTenant, audience, headers, null);

            AutomationFramework.Logger.LOGMessage(AutomationFramework.Logger.MSG.MESSAGE, "\t Sending request <GET>...");

            var builder = new UriBuilder(requestUrl);

            var query = HttpUtility.ParseQueryString(builder.Query);
            foreach(KeyValuePair<string, string> keyValuePair in queryParams)
            {
                query[keyValuePair.Key] = keyValuePair.Value;
            }
            builder.Query = query.ToString();

            return client.GetAsync(builder.ToString()).Result;
        }

        public static HttpResponseMessage DeleteRequest(string requestUrl, string clientId, string clientSecret, string aadInstance, string aadTenant, string audience, Dictionary<string, string> headers, Dictionary<string, string> queryParams)
        {
            AutomationFramework.Logger.LOGMessage(AutomationFramework.Logger.MSG.MESSAGE, "\t URL found: " + requestUrl);
            AutomationFramework.Logger.LOGMessage(AutomationFramework.Logger.MSG.MESSAGE, "\t Authenticating...");
            Authenticate(clientId, clientSecret, aadInstance, aadTenant, audience, headers, null);

            AutomationFramework.Logger.LOGMessage(AutomationFramework.Logger.MSG.MESSAGE, "\t Sending request <POST>...");

            var builder = new UriBuilder(requestUrl);

            var query = HttpUtility.ParseQueryString(builder.Query);
            foreach (KeyValuePair<string, string> keyValuePair in queryParams)
            {
                query[keyValuePair.Key] = keyValuePair.Value;
            }
            builder.Query = query.ToString();

            var result = client.DeleteAsync(builder.ToString()).Result;
            AutomationFramework.Logger.LOGMessage(AutomationFramework.Logger.MSG.MESSAGE, "\t Receiving response...");
            AutomationFramework.Logger.LOGMessage(AutomationFramework.Logger.MSG.MESSAGE, "\t \t " + (result.Content.ReadAsStringAsync().Result.Length <= 1000 ? result.Content.ReadAsStringAsync().Result : result.Content.ReadAsStringAsync().Result.Substring(0, 1000)));

            return result;
        }
    }
}
