using PetApp.Models;
using System.Threading.Tasks;

public interface IPetPrintingService
{
    Task PrintPetsByStatusAsync(PetStatus status);
}
