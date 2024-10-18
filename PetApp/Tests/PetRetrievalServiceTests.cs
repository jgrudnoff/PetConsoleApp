using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using Moq.Protected;
using Newtonsoft.Json;
using PetApp.Models;
using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

[TestClass]
public class PetRetrievalServiceTests
{
    private Mock<HttpMessageHandler> _mockHttpMessageHandler;
    private HttpClient _httpClient;
    private PetRetrievalService _petService;

    [TestInitialize]
    public void Setup()
    {
        // Set up the mock HttpMessageHandler
        _mockHttpMessageHandler = new Mock<HttpMessageHandler>();

        // Create the HttpClient with the mocked HttpMessageHandler
        _httpClient = new HttpClient(_mockHttpMessageHandler.Object)
        {
            BaseAddress = new Uri("https://petstore.swagger.io/v2/")
        };

        // Inject the mocked HttpClient into the service
        _petService = new PetRetrievalService(_httpClient);
    }

    [TestMethod]
    public async Task FindByStatusAsync_ShouldReturnPets_WhenStatusIsAvailable()
    {
        // Arrange
        var expectedPets = new List<Pet>
        {
            new Pet { Id = 1, Name = "Doggie", Status = "available" }
        };

        var responseContent = JsonConvert.SerializeObject(expectedPets);

        // Set up the mock to return a successful response with the expected content
        _mockHttpMessageHandler
            .Protected()
            .Setup<Task<HttpResponseMessage>>(
                "SendAsync",                             // The method name being mocked
                ItExpr.IsAny<HttpRequestMessage>(),      // Any HttpRequestMessage
                ItExpr.IsAny<CancellationToken>()        // Any CancellationToken
            )
            .ReturnsAsync(new HttpResponseMessage
            {
                StatusCode = HttpStatusCode.OK,
                Content = new StringContent(responseContent)
            });

        // Act
        var result = await _petService.FindByStatusAsync(new List<PetStatus>() { PetStatus.Available });

        // Assert
        Assert.IsNotNull(result);
        Assert.AreEqual(1, result.Count);
        Assert.AreEqual("Doggie", result[0].Name);
    }

    [TestMethod]
    public async Task FindByStatusAsync_ShouldThrowException_WhenResponseIsNotSuccess()
    {
        // Arrange
        _mockHttpMessageHandler
            .Protected()
            .Setup<Task<HttpResponseMessage>>(
                "SendAsync",
                ItExpr.IsAny<HttpRequestMessage>(),
                ItExpr.IsAny<CancellationToken>()
            )
            .ReturnsAsync(new HttpResponseMessage
            {
                StatusCode = HttpStatusCode.BadRequest,
                ReasonPhrase = "Bad Request"
            });

        // Act
        var result = await _petService.FindByStatusAsync(new List<PetStatus>() { PetStatus.Available });

        // Assert
        Assert.IsNotNull(result);
        Assert.AreEqual(0, result.Count);
    }

    [TestMethod]
    public async Task FindByStatusAsync_ShouldHandleEmptyResponse()
    {
        // Arrange
        var responseContent = "[]"; // Empty array

        _mockHttpMessageHandler
            .Protected()
            .Setup<Task<HttpResponseMessage>>(
                "SendAsync",
                ItExpr.IsAny<HttpRequestMessage>(),
                ItExpr.IsAny<CancellationToken>()
            )
            .ReturnsAsync(new HttpResponseMessage
            {
                StatusCode = HttpStatusCode.OK,
                Content = new StringContent(responseContent)
            });

        // Act
        var result = await _petService.FindByStatusAsync(new List<PetStatus>() { PetStatus.Available });

        // Assert
        Assert.IsNotNull(result);
        Assert.AreEqual(0, result.Count);
    }
}
