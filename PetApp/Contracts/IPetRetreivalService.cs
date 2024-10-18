using PetApp.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

public interface IPetRetreivalService
{
    Task<List<Pet>> FindByStatusAsync(List<PetStatus> status);
}