// Copyright © 2025 Curt Gilman
// SPDX-License-Identifier: AGPL-3.0-only
// Viae: A geo-centric, journey-focused, federated blog platform

using System.Net;
using System.Text.Json;
using FluentAssertions;
using Microsoft.Extensions.Logging;
using Moq;
using Moq.Protected;

namespace Viae.Services.Tests;

/// <summary>
/// Tests for the ActivityPubDiscoveryService that discovers ActivityPub actor information.
/// </summary>
[TestClass]
public class ActivityPubDiscoveryServiceTests
{
    private Mock<IHttpClientFactory> _httpClientFactoryMock = null!;
    private Mock<HttpMessageHandler> _httpMessageHandlerMock = null!;
    private Mock<ILogger<ActivityPubDiscoveryService>> _loggerMock = null!;
    private ActivityPubDiscoveryService _service = null!;

    [TestInitialize]
    public void TestInitialize()
    {
        _httpClientFactoryMock = new Mock<IHttpClientFactory>();
        _httpMessageHandlerMock = new Mock<HttpMessageHandler>();
        _loggerMock = new Mock<ILogger<ActivityPubDiscoveryService>>();

        var httpClient = new HttpClient(_httpMessageHandlerMock.Object);
        _httpClientFactoryMock.Setup(f => f.CreateClient(It.IsAny<string>())).Returns(httpClient);

        _service = new ActivityPubDiscoveryService(
            _httpClientFactoryMock.Object,
            _loggerMock.Object
        );
    }

    #region DiscoverInboxAsync Tests

    [TestMethod]
    public async Task DiscoverInboxAsync_ShouldReturnInboxUri_WhenActorHasValidInbox()
    {
        // Arrange
        var actorUri = new Uri("https://example.com/users/alice");
        var expectedInbox = new Uri("https://example.com/users/alice/inbox");

        var actorObject = new
        {
            id = actorUri.ToString(),
            type = "Person",
            inbox = expectedInbox.ToString(),
        };

        var jsonResponse = JsonSerializer.Serialize(actorObject);

        _httpMessageHandlerMock
            .Protected()
            .Setup<Task<HttpResponseMessage>>(
                "SendAsync",
                ItExpr.IsAny<HttpRequestMessage>(),
                ItExpr.IsAny<CancellationToken>()
            )
            .ReturnsAsync(
                new HttpResponseMessage
                {
                    StatusCode = HttpStatusCode.OK,
                    Content = new StringContent(
                        jsonResponse,
                        System.Text.Encoding.UTF8,
                        "application/activity+json"
                    ),
                }
            );

        // Act
        var result = await _service.DiscoverInboxAsync(actorUri);

        // Assert
        result.Should().NotBeNull();
        result.Should().Be(expectedInbox);
    }

    [TestMethod]
    public async Task DiscoverInboxAsync_ShouldReturnNull_WhenHttpRequestFails()
    {
        // Arrange
        var actorUri = new Uri("https://example.com/users/alice");

        _httpMessageHandlerMock
            .Protected()
            .Setup<Task<HttpResponseMessage>>(
                "SendAsync",
                ItExpr.IsAny<HttpRequestMessage>(),
                ItExpr.IsAny<CancellationToken>()
            )
            .ReturnsAsync(
                new HttpResponseMessage
                {
                    StatusCode = HttpStatusCode.NotFound,
                    Content = new StringContent("Not found"),
                }
            );

        // Act
        var result = await _service.DiscoverInboxAsync(actorUri);

        // Assert
        result.Should().BeNull();
    }

    [TestMethod]
    public async Task DiscoverInboxAsync_ShouldReturnNull_WhenActorObjectMissingInbox()
    {
        // Arrange
        var actorUri = new Uri("https://example.com/users/alice");

        var actorObject = new
        {
            id = actorUri.ToString(),
            type = "Person",
            // No inbox property
        };

        var jsonResponse = JsonSerializer.Serialize(actorObject);

        _httpMessageHandlerMock
            .Protected()
            .Setup<Task<HttpResponseMessage>>(
                "SendAsync",
                ItExpr.IsAny<HttpRequestMessage>(),
                ItExpr.IsAny<CancellationToken>()
            )
            .ReturnsAsync(
                new HttpResponseMessage
                {
                    StatusCode = HttpStatusCode.OK,
                    Content = new StringContent(
                        jsonResponse,
                        System.Text.Encoding.UTF8,
                        "application/activity+json"
                    ),
                }
            );

        // Act
        var result = await _service.DiscoverInboxAsync(actorUri);

        // Assert
        result.Should().BeNull();
    }

    [TestMethod]
    public async Task DiscoverInboxAsync_ShouldReturnNull_WhenInboxIsNotValidUri()
    {
        // Arrange
        var actorUri = new Uri("https://example.com/users/alice");

        var actorObject = new
        {
            id = actorUri.ToString(),
            type = "Person",
            inbox = "not-a-valid-uri",
        };

        var jsonResponse = JsonSerializer.Serialize(actorObject);

        _httpMessageHandlerMock
            .Protected()
            .Setup<Task<HttpResponseMessage>>(
                "SendAsync",
                ItExpr.IsAny<HttpRequestMessage>(),
                ItExpr.IsAny<CancellationToken>()
            )
            .ReturnsAsync(
                new HttpResponseMessage
                {
                    StatusCode = HttpStatusCode.OK,
                    Content = new StringContent(
                        jsonResponse,
                        System.Text.Encoding.UTF8,
                        "application/activity+json"
                    ),
                }
            );

        // Act
        var result = await _service.DiscoverInboxAsync(actorUri);

        // Assert
        result.Should().BeNull();
    }

    [TestMethod]
    public async Task DiscoverInboxAsync_ShouldReturnNull_WhenInboxIsEmptyString()
    {
        // Arrange
        var actorUri = new Uri("https://example.com/users/alice");

        var actorObject = new
        {
            id = actorUri.ToString(),
            type = "Person",
            inbox = "",
        };

        var jsonResponse = JsonSerializer.Serialize(actorObject);

        _httpMessageHandlerMock
            .Protected()
            .Setup<Task<HttpResponseMessage>>(
                "SendAsync",
                ItExpr.IsAny<HttpRequestMessage>(),
                ItExpr.IsAny<CancellationToken>()
            )
            .ReturnsAsync(
                new HttpResponseMessage
                {
                    StatusCode = HttpStatusCode.OK,
                    Content = new StringContent(
                        jsonResponse,
                        System.Text.Encoding.UTF8,
                        "application/activity+json"
                    ),
                }
            );

        // Act
        var result = await _service.DiscoverInboxAsync(actorUri);

        // Assert
        result.Should().BeNull();
    }

    [TestMethod]
    public async Task DiscoverInboxAsync_ShouldReturnNull_WhenResponseIsNotValidJson()
    {
        // Arrange
        var actorUri = new Uri("https://example.com/users/alice");

        _httpMessageHandlerMock
            .Protected()
            .Setup<Task<HttpResponseMessage>>(
                "SendAsync",
                ItExpr.IsAny<HttpRequestMessage>(),
                ItExpr.IsAny<CancellationToken>()
            )
            .ReturnsAsync(
                new HttpResponseMessage
                {
                    StatusCode = HttpStatusCode.OK,
                    Content = new StringContent(
                        "not valid json",
                        System.Text.Encoding.UTF8,
                        "application/activity+json"
                    ),
                }
            );

        // Act
        var result = await _service.DiscoverInboxAsync(actorUri);

        // Assert
        result.Should().BeNull();
    }

    [TestMethod]
    public async Task DiscoverInboxAsync_ShouldReturnNull_WhenExceptionThrown()
    {
        // Arrange
        var actorUri = new Uri("https://example.com/users/alice");

        _httpMessageHandlerMock
            .Protected()
            .Setup<Task<HttpResponseMessage>>(
                "SendAsync",
                ItExpr.IsAny<HttpRequestMessage>(),
                ItExpr.IsAny<CancellationToken>()
            )
            .ThrowsAsync(new HttpRequestException("Network error"));

        // Act
        var result = await _service.DiscoverInboxAsync(actorUri);

        // Assert
        result.Should().BeNull();
    }

    [TestMethod]
    public async Task DiscoverInboxAsync_ShouldSendRequestWithActivityJsonAcceptHeader()
    {
        // Arrange
        var actorUri = new Uri("https://example.com/users/alice");
        var expectedInbox = new Uri("https://example.com/users/alice/inbox");

        var actorObject = new
        {
            id = actorUri.ToString(),
            type = "Person",
            inbox = expectedInbox.ToString(),
        };

        var jsonResponse = JsonSerializer.Serialize(actorObject);
        HttpRequestMessage? capturedRequest = null;

        _httpMessageHandlerMock
            .Protected()
            .Setup<Task<HttpResponseMessage>>(
                "SendAsync",
                ItExpr.IsAny<HttpRequestMessage>(),
                ItExpr.IsAny<CancellationToken>()
            )
            .Callback<HttpRequestMessage, CancellationToken>((req, _) => capturedRequest = req)
            .ReturnsAsync(
                new HttpResponseMessage
                {
                    StatusCode = HttpStatusCode.OK,
                    Content = new StringContent(
                        jsonResponse,
                        System.Text.Encoding.UTF8,
                        "application/activity+json"
                    ),
                }
            );

        // Act
        await _service.DiscoverInboxAsync(actorUri);

        // Assert
        capturedRequest.Should().NotBeNull();
        capturedRequest!.Method.Should().Be(HttpMethod.Get);
        capturedRequest.RequestUri.Should().Be(actorUri);
        capturedRequest
            .Headers.Accept.Should()
            .Contain(accept => accept.MediaType == "application/activity+json");
    }

    [TestMethod]
    public async Task DiscoverInboxAsync_ShouldReturnNull_ForRelativeInboxUri()
    {
        // Arrange
        var actorUri = new Uri("https://example.com/users/alice");
        var relativeInbox = "/inbox";

        var actorObject = new
        {
            id = actorUri.ToString(),
            type = "Person",
            inbox = relativeInbox,
        };

        var jsonResponse = JsonSerializer.Serialize(actorObject);

        _httpMessageHandlerMock
            .Protected()
            .Setup<Task<HttpResponseMessage>>(
                "SendAsync",
                ItExpr.IsAny<HttpRequestMessage>(),
                ItExpr.IsAny<CancellationToken>()
            )
            .ReturnsAsync(
                new HttpResponseMessage
                {
                    StatusCode = HttpStatusCode.OK,
                    Content = new StringContent(
                        jsonResponse,
                        System.Text.Encoding.UTF8,
                        "application/activity+json"
                    ),
                }
            );

        // Act
        var result = await _service.DiscoverInboxAsync(actorUri);

        // Assert - relative URI creates a file:// URI which is not HTTP/HTTPS
        // The service validates the scheme and rejects non-HTTP/HTTPS URIs
        result.Should().BeNull();
    }

    [TestMethod]
    public async Task DiscoverInboxAsync_ShouldAcceptInboxWithDifferentDomain()
    {
        // Arrange
        var actorUri = new Uri("https://example.com/users/alice");
        var expectedInbox = new Uri("https://different-server.com/inbox");

        var actorObject = new
        {
            id = actorUri.ToString(),
            type = "Person",
            inbox = expectedInbox.ToString(),
        };

        var jsonResponse = JsonSerializer.Serialize(actorObject);

        _httpMessageHandlerMock
            .Protected()
            .Setup<Task<HttpResponseMessage>>(
                "SendAsync",
                ItExpr.IsAny<HttpRequestMessage>(),
                ItExpr.IsAny<CancellationToken>()
            )
            .ReturnsAsync(
                new HttpResponseMessage
                {
                    StatusCode = HttpStatusCode.OK,
                    Content = new StringContent(
                        jsonResponse,
                        System.Text.Encoding.UTF8,
                        "application/activity+json"
                    ),
                }
            );

        // Act
        var result = await _service.DiscoverInboxAsync(actorUri);

        // Assert
        result.Should().NotBeNull();
        result.Should().Be(expectedInbox);
    }

    #endregion
}
