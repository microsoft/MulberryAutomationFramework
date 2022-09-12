// <copyright file="CosmosDB.cs" company="Microsoft Corporation">
// Copyright (c) Microsoft Corporation. All rights reserved.
// </copyright>

namespace AutomationFramework
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using Microsoft.Azure.Cosmos;

    /// <summary>
    /// Cosmos db connection class.
    /// </summary>
    public class CosmosDB
    {
        private CosmosClient client;

        public void MakeConnection(string endpointUrl, string primaryKey)
        {
            client = new CosmosClient(endpointUrl, primaryKey);
        }

        /// <summary>
        /// Get all cosmos documents for a single partition key executing the provided sql query.
        /// </summary>
        /// <param name="databaseName">Database name.</param>
        /// <param name="containerName">Container name.</param>
        /// <param name="sqlQuery">Cosmos sql query.</param>
        /// <returns>list of T objects.</returns>
        public dynamic ExecuteSqlQuery(string endpointUrl, string primaryKey, string databaseName, string containerName, string sqlQuery)
        {
            List<dynamic> items = new List<dynamic>();
            var queryRequestOptions = new QueryRequestOptions
            {
                MaxItemCount = int.MaxValue
            };

            CosmosClient cosmosClient = new CosmosClient(endpointUrl, primaryKey);

            Container container = cosmosClient.GetContainer(databaseName, containerName);

            QueryDefinition query = new QueryDefinition(sqlQuery);

            FeedIterator<dynamic> queryResultSetIterator = container.GetItemQueryIterator<dynamic>(query, default, queryRequestOptions);
            while (queryResultSetIterator.HasMoreResults)
            {
                var results = queryResultSetIterator.ReadNextAsync().Result;
                if (results.StatusCode == System.Net.HttpStatusCode.OK)
                {
                    items.AddRange(results.Resource);
                }
            }

            return items;
        }
    }
}
