using Microsoft.Extensions.DependencyInjection;
using PetApp.Models;
using System;
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
                IPetRetreivalService petService = new PetRetreivalService();
                PetConsolePrintingService petPrintingService = new PetConsolePrintingService(petService);
                await petPrintingService.PrintPetsByStatusAsync(PetStatus.Available);
            } catch (Exception ex) { 
                Console.WriteLine(ex.Message);
            } finally
            {
                Console.ReadLine();
            }
        }
    }
}
