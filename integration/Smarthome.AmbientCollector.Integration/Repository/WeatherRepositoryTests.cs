using System.Net;
using System.Text;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Moq;
using Moq.Protected;
using Smarthome.AmbientCollector.Api.Repositories.WeatherReport.Api;
using Location = Smarthome.Core.DomainObjects.Location;

namespace Smarthome.AmbientCollector.Test;

public class TomorrowIoApiClientTests
{
    private readonly Mock<IHttpClientFactory> _httpClientFactoryMock;
    private readonly Mock<ILogger<TomorrowIoApiClient>> _loggerMock;
    private readonly IOptions<WeatherApiConfiguration> _options;
    
    public TomorrowIoApiClientTests()
    {
        _httpClientFactoryMock = new Mock<IHttpClientFactory>();
        _loggerMock = new Mock<ILogger<TomorrowIoApiClient>>();
        _options = Options.Create(new WeatherApiConfiguration
        {
            Endpoint = "https://api.tomorrow.io/v4/weather/realtime",
            ApiKey = "test-api-key"
        });
    }

    private HttpClient CreateMockHttpClient(HttpResponseMessage response)
    {
        var handlerMock = new Mock<HttpMessageHandler>();
        handlerMock.Protected()
            .Setup<Task<HttpResponseMessage>>(
                "SendAsync",
                ItExpr.IsAny<HttpRequestMessage>(),
                ItExpr.IsAny<CancellationToken>())
            .ReturnsAsync(response);

        return new HttpClient(handlerMock.Object);
    }

    private HttpClient CreateMockHttpClientWithException(Exception exception)
    {
        var handlerMock = new Mock<HttpMessageHandler>();
        handlerMock.Protected()
            .Setup<Task<HttpResponseMessage>>(
                "SendAsync",
                ItExpr.IsAny<HttpRequestMessage>(),
                ItExpr.IsAny<CancellationToken>())
            .ThrowsAsync(exception);

        return new HttpClient(handlerMock.Object);
    }

    [Fact]
    public async Task GetWeatherDataAsync_Returns403Forbidden_HandledGracefully()
    {
        // Arrange
        var response = new HttpResponseMessage(HttpStatusCode.Forbidden)
        {
            Content = new StringContent("Access denied")
        };
        var httpClient = CreateMockHttpClient(response);
        _httpClientFactoryMock.Setup(f => f.CreateClient(It.IsAny<string>())).Returns(httpClient);

        var client = new TomorrowIoApiClient(_options, _httpClientFactoryMock.Object, _loggerMock.Object);
        var location = new Location { City = "Berlin" };

        // Act
        var result = await client.GetWeatherDataAsync(location, CancellationToken.None);

        // Assert
        Assert.False(result.IsSuccess);
        Assert.Equal(HttpStatusCode.Forbidden, result.ValueFail.StatusCode);
        Assert.Equal("Access denied", result.ValueFail.Message);
    }

    [Fact]
    public async Task GetWeatherDataAsync_Returns500InternalServerError_HandledGracefully()
    {
        // Arrange
        var response = new HttpResponseMessage(HttpStatusCode.InternalServerError)
        {
            Content = new StringContent("Internal server error")
        };
        var httpClient = CreateMockHttpClient(response);
        _httpClientFactoryMock.Setup(f => f.CreateClient(It.IsAny<string>())).Returns(httpClient);

        var client = new TomorrowIoApiClient(_options, _httpClientFactoryMock.Object, _loggerMock.Object);
        var location = new Location { City = "Berlin" };

        // Act
        var result = await client.GetWeatherDataAsync(location, CancellationToken.None);

        // Assert
        Assert.False(result.IsSuccess);
        Assert.Equal(HttpStatusCode.InternalServerError, result.ValueFail.StatusCode);
        Assert.Equal("Internal server error", result.ValueFail.Message);
    }

    [Fact]
    public async Task GetWeatherDataAsync_HttpClientThrowsException_HandledGracefully()
    {
        // Arrange
        var exception = new HttpRequestException("Network error");
        var httpClient = CreateMockHttpClientWithException(exception);
        _httpClientFactoryMock.Setup(f => f.CreateClient(It.IsAny<string>())).Returns(httpClient);

        var client = new TomorrowIoApiClient(_options, _httpClientFactoryMock.Object, _loggerMock.Object);
        var location = new Location { City = "Berlin" };

        // Act
        var result = await client.GetWeatherDataAsync(location, CancellationToken.None);

        // Assert
        Assert.False(result.IsSuccess);
        Assert.Equal(HttpStatusCode.InternalServerError, result.ValueFail.StatusCode);
        Assert.Equal("Network error", result.ValueFail.Message);
    }

    [Fact]
    public async Task GetWeatherDataAsync_ValidResponse_ReturnsWeatherReport()
    {
        // Arrange
        var jsonResponse = """
        {
            "data": {
                "time": "2024-01-15T12:00:00Z",
                "values": {
                    "temperature": 15.5,
                    "temperatureApparent": 14.2,
                    "humidity": 65.0,
                    "rainIntensity": 0.0,
                    "pressureSurfaceLevel": 1013.25,
                    "windSpeed": 5.5
                }
            },
            "location": {
                "lat": 52.52,
                "lon": 13.405,
                "name": "Berlin",
                "type": "city"
            }
        }
        """;
        
        var response = new HttpResponseMessage(HttpStatusCode.OK)
        {
            Content = new StringContent(jsonResponse, Encoding.UTF8, "application/json")
        };
        var httpClient = CreateMockHttpClient(response);
        _httpClientFactoryMock.Setup(f => f.CreateClient(It.IsAny<string>())).Returns(httpClient);

        var client = new TomorrowIoApiClient(_options, _httpClientFactoryMock.Object, _loggerMock.Object);
        var location = new Location { City = "Berlin" };

        // Act
        var result = await client.GetWeatherDataAsync(location, CancellationToken.None);

        // Assert
        Assert.True(result.IsSuccess);
        Assert.NotNull(result.ValueSuccess);
        Assert.Equal("Berlin", result.ValueSuccess.Location);
        Assert.Equal(52.52, result.ValueSuccess.Latitude);
        Assert.Equal(13.405, result.ValueSuccess.Longitude);
        Assert.Equal(15.5, result.ValueSuccess.Temperature);
        Assert.Equal(14.2, result.ValueSuccess.TemperatureApparent);
        Assert.Equal(65.0, result.ValueSuccess.Humidity);
        Assert.Equal(0.0, result.ValueSuccess.RainIntensity);
        Assert.Equal(1013.25, result.ValueSuccess.PressureSurfaceLevel);
        Assert.Equal(5.5, result.ValueSuccess.WindSpeed);
    }
}