using PetApp.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

public interface IPetPrintingService
{
    Task PrintPetsByStatusAsync(List<PetStatus> status);
}
