using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using PetApp.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

[TestClass]
public class PetPrintingServiceTests
{
    private Mock<IPetRetreivalService> _mockPetService;
    private PetConsolePrintingService _petPrintingService;

    [TestInitialize]
    public void Setup()
    {
        // Mock the IPetService
        _mockPetService = new Mock<IPetRetreivalService>();

        // Inject the mock into PetPrintingService
        _petPrintingService = new PetConsolePrintingService(_mockPetService.Object);
    }

    [TestMethod]
    public async Task FetchAndGroupPetsByStatusAsync_ShouldGroupAndSortCorrectly()
    {
        // Arrange: Set up the mock IPetService to return a list of pets
        var pets = new List<Pet>
        {
            new Pet { Id = 1, Name = "Bella", Category = new Category { Name = "Dogs" }, Status = "available" },
            new Pet { Id = 2, Name = "Max", Category = new Category { Name = "Dogs" }, Status = "available" },
            new Pet { Id = 3, Name = "Charlie", Category = new Category { Name = "Cats" }, Status = "available" },
            new Pet { Id = 4, Name = "Whiskers", Category = new Category { Name = "Cats" }, Status = "available" },
            new Pet { Id = 5, Name = "Rex", Category = null, Status = "available" } // No Category
        };

        _mockPetService.Setup(s => s.FindByStatusAsync(PetStatus.Available))
            .ReturnsAsync(pets);

        // Act: Call the method that groups and sorts the pets
        var groupedPets = await _petPrintingService.FetchAndGroupPetsByStatusAsync(PetStatus.Available);

        // Assert
        Assert.AreEqual(3, groupedPets.Count);  // 3 groups: "Dogs", "Cats", "No Category"
        Assert.IsTrue(groupedPets.ContainsKey("Dogs"));
        Assert.IsTrue(groupedPets.ContainsKey("Cats"));
        Assert.IsTrue(groupedPets.ContainsKey("No Category"));

        // Check sorting within groups
        var dogs = groupedPets["Dogs"];
        Assert.AreEqual(2, dogs.Count);
        Assert.AreEqual("Max", dogs[0].Name);  // Max should come before Bella in reverse alphabetical order
        Assert.AreEqual("Bella", dogs[1].Name);

        var cats = groupedPets["Cats"];
        Assert.AreEqual(2, cats.Count);
        Assert.AreEqual("Whiskers", cats[0].Name);  // Whiskers should come before Charlie in reverse alphabetical order
        Assert.AreEqual("Charlie", cats[1].Name);

        var noCategory = groupedPets["No Category"];
        Assert.AreEqual(1, noCategory.Count);
        Assert.AreEqual("Rex", noCategory[0].Name);
    }

    [TestMethod]
    public async Task FetchAndGroupPetsByStatusAsync_ShouldHandleEmptyList()
    {
        // Arrange: Set up the mock IPetService to return an empty list of pets
        _mockPetService.Setup(s => s.FindByStatusAsync(PetStatus.Available))
            .ReturnsAsync(new List<Pet>());

        // Act
        var groupedPets = await _petPrintingService.FetchAndGroupPetsByStatusAsync(PetStatus.Available);

        // Assert
        Assert.AreEqual(0, groupedPets.Count); // No groups should be returned
    }

    [TestMethod]
    public async Task FetchAndGroupPetsByStatusAsync_ShouldHandleNullCategory()
    {
        // Arrange: Set up the mock IPetService to return pets with null categories
        var pets = new List<Pet>
        {
            new Pet { Id = 1, Name = "Buddy", Category = null, Status = "available" },
            new Pet { Id = 2, Name = "Luna", Category = null, Status = "available" }
        };

        _mockPetService.Setup(s => s.FindByStatusAsync(PetStatus.Available))
            .ReturnsAsync(pets);

        // Act
        var groupedPets = await _petPrintingService.FetchAndGroupPetsByStatusAsync(PetStatus.Available);

        // Assert
        Assert.AreEqual(1, groupedPets.Count); // Only one group, "No Category"
        Assert.IsTrue(groupedPets.ContainsKey("No Category"));
        var noCategoryPets = groupedPets["No Category"];
        Assert.AreEqual(2, noCategoryPets.Count);
        Assert.AreEqual("Luna", noCategoryPets[0].Name); // Sorted in reverse alphabetical order
        Assert.AreEqual("Buddy", noCategoryPets[1].Name);
    }
}
