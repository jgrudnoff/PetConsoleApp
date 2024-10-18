using PetApp.Models;
using Refit;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

public class PetRetrievalService : IPetRetreivalService
{
    private readonly IPetStoreApi _petStoreApi;

    // Inject the Refit-generated client via the constructor
    public PetRetrievalService(IPetStoreApi petStoreApi) 
    {
        _petStoreApi = petStoreApi;
    }

    // Fetch pets by status
    public async Task<List<Pet>> FindByStatusAsync(List<PetStatus> statusList)
    {
        try
        {
            if (statusList == null || statusList.Count() == 0)
            {
                throw new ArgumentException("At least one status must be provided");
            }

            string statusString = string.Join(",", statusList.Select(s => s.ToString().ToLower()));
            // Call the Refit-generated client to fetch pets by status
            return await _petStoreApi.FindByStatusAsync(statusString);
        }
        catch (Exception ex)
        {
            //if (ex is ApiException)
            //    //Log something useful
            //else if (ex is ArgumentException)
            //    //Log something useful

            return new List<Pet> { };
        }
    }
}
