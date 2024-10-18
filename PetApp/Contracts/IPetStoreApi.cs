using PetApp.Models;
using Refit;
using System.Collections.Generic;
using System.Threading.Tasks;

public interface IPetStoreApi
{
    // Define the endpoint for finding pets by status
    [Get("/pet/findByStatus")]
    Task<List<Pet>> FindByStatusAsync([AliasAs("status")] string status);
}