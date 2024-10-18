using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using Newtonsoft.Json;
using PetApp.Models;


public class PetRetreivalService : IPetRetreivalService
{
    private readonly HttpClient _httpClient;
    const string API_ROOT = "https://petstore.swagger.io/v2/";
    const string PET_RESOURCE_STRING = "pet";
    const string FIND_BY_STATUS_STRING = "findByStatus";
    const string STATUS_STRING = "status";


    public PetRetreivalService(HttpClient httpClient = null)
    {
        _httpClient = httpClient ?? new HttpClient
        {
            BaseAddress = new Uri(API_ROOT),
        };
        _httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
    }

    public async Task<List<Pet>> FindByStatusAsync(PetStatus status)
    {
        try
        {
            // Map the enum to lowercase strings for the API call
            string statusString = status.ToString().ToLower();

            // Make the request to the API
            HttpResponseMessage response = await _httpClient.GetAsync($"{PET_RESOURCE_STRING}/{FIND_BY_STATUS_STRING}?{STATUS_STRING}={statusString}");

            if (!response.IsSuccessStatusCode)
            {
                throw new Exception($"Error retrieving pets by status: {response.ReasonPhrase}");
            }

            // Read and deserialize the response body
            string responseBody = await response.Content.ReadAsStringAsync();
            return JsonConvert.DeserializeObject<List<Pet>>(responseBody);
        }
        catch (Exception ex)
        {
            //In an enterprise solution I would log the error here
            return new List<Pet>();
        }

    }
}