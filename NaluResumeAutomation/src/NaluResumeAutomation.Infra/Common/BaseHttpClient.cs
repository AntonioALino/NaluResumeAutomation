using System;
using System.Collections.Generic;
using System.Net.Http.Json;
using System.Text;

namespace NaluResumeAutomation.Infra.Common
{
    public abstract class BaseHttpClient
    {
        protected readonly HttpClient HttpClient;

        protected BaseHttpClient(HttpClient httpClient)
        {
            HttpClient = httpClient;
        }

        protected async Task<TResponse?> PostFileAsync<TResponse>(
            string endpoint,
            Stream fileStream,
            string fileName,
            string fileParameterName,
            CancellationToken cancellationToken)
        {
            using var content = new MultipartFormDataContent();

            var streamContent = new StreamContent(fileStream);

            content.Add(streamContent, fileParameterName, fileName);

            var response = await HttpClient.PostAsync(endpoint, content, cancellationToken);

            if (!response.IsSuccessStatusCode)
            {
                var errorDetails = await response.Content.ReadAsStringAsync(cancellationToken);
                throw new HttpRequestException($"Erro na API Externa ({response.StatusCode}): {errorDetails}");
            }

            return await response.Content.ReadFromJsonAsync<TResponse>(cancellationToken: cancellationToken);

        }
    }
}
