using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using Newtonsoft.Json;
using PetApp.Models;


public class PetRetrievalService : IPetRetreivalService
{
    private readonly HttpClient _httpClient;
    const string API_ROOT = "https://petstore.swagger.io/v2/";
    const string PET_RESOURCE_STRING = "pet";
    const string FIND_BY_STATUS_STRING = "findByStatus";
    const string STATUS_STRING = "status";


    public PetRetrievalService(HttpClient httpClient = null)
    {
        _httpClient = httpClient ?? new HttpClient
        {
            BaseAddress = new Uri(API_ROOT),
        };
        _httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
    }

    public async Task<List<Pet>> FindByStatusAsync(List<PetStatus> statusList)
    {
        try
        {
            // Map the enum to lowercase strings for the API call
            if (statusList == null || statusList.Count() == 0)
            {
                throw new ArgumentException("At least one status must be provided");
            }

            string statusString = string.Join(",", statusList.Select(s => s.ToString().ToLower()));

            // Make the query parameters
            var requestString = $"{FIND_BY_STATUS_STRING}?{STATUS_STRING}={statusString}";
            return await RetreivePetsAsync(requestString);
        }
        catch (Exception ex)
        {
            //In an enterprise solution I would log the error here and return an empty list 
            return new List<Pet>();
        }

    }

    public async Task<List<Pet>> RetreivePetsAsync(string requestParametersString)
    {
        HttpResponseMessage response = await _httpClient.GetAsync($"{PET_RESOURCE_STRING}/{requestParametersString}");

        if (!response.IsSuccessStatusCode)
        {
            throw new Exception($"Error retrieving pets by status: {response.ReasonPhrase}");
        }

        // Read and deserialize the response body
        string responseBody = await response.Content.ReadAsStringAsync();
        return JsonConvert.DeserializeObject<List<Pet>>(responseBody);
    }
}