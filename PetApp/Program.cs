using Microsoft.Extensions.DependencyInjection;
using PetApp.Models;
using Refit;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;


namespace PetApp
{
    class Program
    {
        static async Task Main(string[] args)
        {
            try
            {
                var petStoreApi = RestService.For<IPetStoreApi>("https://petstore.swagger.io/v2");
                var petService = new PetRetrievalService(petStoreApi);
                PetConsolePrintingService petPrintingService = new PetConsolePrintingService(petService);
                List<PetStatus> statusList = new List<PetStatus>() { PetStatus.Available };
                await petPrintingService.PrintPetsByStatusAsync(statusList);
            } catch (Exception ex) { 
                Console.WriteLine(ex.Message);
            } finally
            {
                Console.ReadLine();
            }
        }
    }
}
