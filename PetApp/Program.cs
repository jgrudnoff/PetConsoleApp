using Microsoft.Extensions.DependencyInjection;
using PetApp.Models;
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
                IPetRetreivalService petService = new PetRetrievalService();
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
