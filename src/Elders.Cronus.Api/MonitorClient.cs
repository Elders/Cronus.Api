﻿using Elders.Cronus.Api.Controllers;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.IO;
using System.Net.Http;
using System.Text.Json;
using System.Threading.Tasks;

namespace Elders.Cronus.Api
{
    public class MonitorClient
    {
        private readonly JsonSerializerOptions options;

        private readonly HttpClient _client;

        private static readonly ILogger logger = CronusLogger.CreateLogger(typeof(CronusApi));

        public MonitorClient(HttpClient client)
        {
            _client = client;
            options = new JsonSerializerOptions()
            {
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
                PropertyNameCaseInsensitive = true
            };
        }

        public async Task<List<string>> GetBoundedContextListAsync()
        {
            try
            {
                HttpRequestMessage request = new HttpRequestMessage(HttpMethod.Get, "/Monitor/GetLiveServices");

                var response = await ExecuteRequestAsync<List<string>>(request);

                return response.Data;
            }
            catch (Exception ex)
            {
                logger.LogInformation($"Probably the domain for Cronus Client is not the right one. {ex.Message}");
                return new List<string>();
            }
        }

        public async Task<List<string>> GetTenantListAsync()
        {
            try
            {
                HttpRequestMessage request = new HttpRequestMessage(HttpMethod.Get, "/Monitor/GetTenants");

                var response = await ExecuteRequestAsync<List<string>>(request);

                return response.Data;
            }
            catch (Exception ex)
            {
                logger.LogInformation($"Probably the domain for Cronus Client is not the right one. {ex.Message}");
                return new List<string>();
            }
        }

        protected async Task<(HttpResponseMessage Response, T Data)> ExecuteRequestAsync<T>(HttpRequestMessage request)
        {
            if (request is null) throw new ArgumentNullException(nameof(request));

            using (HttpResponseMessage response = await _client.SendAsync(request).ConfigureAwait(false))
            {
                if (response.IsSuccessStatusCode)
                {
                    using (var contentStream = await response.Content.ReadAsStreamAsync().ConfigureAwait(false))
                    {
                        contentStream.Position = 0;
                        T responseObject = await JsonSerializer.DeserializeAsync<T>(contentStream, options).ConfigureAwait(false);
                        return (response, responseObject);
                    }
                }
                else
                {
                    return (response, default);
                }
            }
        }

        private static async Task<string> StreamToStringAsync(Stream stream)
        {
            string content = null;

            if (stream != null)
                using (var sr = new StreamReader(stream))
                    content = await sr.ReadToEndAsync().ConfigureAwait(false);

            return content;
        }
    }
}
