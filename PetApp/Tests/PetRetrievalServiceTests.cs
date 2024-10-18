using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using PetApp.Models;
using Refit;
using System.Collections.Generic;
using System.Threading.Tasks;

[TestClass]
public class PetServiceTests
{
    private Mock<IPetStoreApi> _mockPetStoreApi;
    private PetRetrievalService _petService;

    [TestInitialize]
    public void Setup()
    {
        // Create the mock IPetStoreApi
        _mockPetStoreApi = new Mock<IPetStoreApi>();

        // Inject the mocked API into the PetService
        _petService = new PetRetrievalService(_mockPetStoreApi.Object);
    }

    [TestMethod]
    public async Task FindByStatusAsync_ShouldReturnPets_WhenStatusIsAvailable()
    {
        // Arrange
        var expectedPets = new List<Pet>
        {
            new Pet { Id = 1, Name = "Doggie", Status = "available" }
        };

        _mockPetStoreApi
            .Setup(api => api.FindByStatusAsync("available"))
            .ReturnsAsync(expectedPets);

        // Act
        var result = await _petService.FindByStatusAsync(new List<PetStatus>() { PetStatus.Available });

        // Assert
        Assert.IsNotNull(result);
        Assert.AreEqual(1, result.Count);
        Assert.AreEqual("Doggie", result[0].Name);
    }

}