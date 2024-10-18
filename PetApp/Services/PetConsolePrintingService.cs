using System;
using System.Linq;
using System.Collections.Generic;
using System.Threading.Tasks;
using PetApp.Models;
using System.Reflection;

public class PetConsolePrintingService : IPetPrintingService
{
    private readonly IPetRetreivalService _petService;

    // Constructor accepting IPetService as a dependency
    public PetConsolePrintingService(IPetRetreivalService petService)
    {
        _petService = petService;
    }

    // Method responsible for grouping and sorting pets by category and name
    public Dictionary<string, List<Pet>> GroupAndSortPetsByCategory(List<Pet> pets)
    {
        return pets
            .GroupBy(pet => pet.Category?.Name ?? "No Category") // Group by category name, handling null values
            .OrderBy(group => group.Key) // Order by Category Name
            .ToDictionary(
                group => group.Key,
                group => group.OrderByDescending(pet => pet.Name).ToList() // Sort pets within group by name in reverse alphabetical order
            );
    }

    // Method to print the grouped pets to the console
    public void PrintGroupedPets(Dictionary<string, List<Pet>> groupedPets)
    {
        foreach (var group in groupedPets)
        {
            Console.WriteLine($"Category: {group.Key}");

            var pets = group.Value;
            var lastId = pets.Last().Id;
            foreach (var pet in group.Value)
            {
                Console.WriteLine($"  Pet ID: {pet.Id}");
                Console.WriteLine($"  Name: {pet.Name}");
                Console.WriteLine($"  Status: {pet.Status}");
                Console.WriteLine($"  Photo URLs: {string.Join(", ", pet.PhotoUrls)}");

                if (pet.Tags != null && pet.Tags.Count > 0)
                {
                    Console.WriteLine("  Tags:");
                    foreach (var tag in pet.Tags)
                    {
                        Console.WriteLine($"\tTag ID: {tag.Id}, Tag Name: {tag.Name}");
                    }
                }
                else
                {
                    Console.WriteLine("  Tags: No tags available.");
                }
                if (!pet.Id.Equals(lastId))
                    Console.WriteLine(new string('-', 40)); // Separator for better readability
            }
            Console.WriteLine(new string('=', 80)); // Separator for better readability
        }
    }

    // Main method that fetches pets, groups, sorts, and then prints them
    public async Task<Dictionary<string, List<Pet>>> FetchAndGroupPetsByStatusAsync(List<PetStatus> statusList)
    {
        // Fetch pets based on status
        List<Pet> pets = await _petService.FindByStatusAsync(statusList);

        // If there are no pets, return an empty dictionary
        if (pets.Count == 0)
        {
            return new Dictionary<string, List<Pet>>();
        }

        // Group and sort the pets
        return GroupAndSortPetsByCategory(pets);
    }

    // Implementation of PrintPetsByStatusAsync from IPetPrintingService
    public async Task PrintPetsByStatusAsync(List<PetStatus> statusList)
    {
        // Fetch and group pets
        var groupedPets = await FetchAndGroupPetsByStatusAsync(statusList);

        // If there are no pets, print a message
        if (groupedPets.Count == 0)
        {
            Console.WriteLine("No pets found with the specified status.");
            return;
        }

        // Print the grouped pets
        PrintGroupedPets(groupedPets);
    }
}
