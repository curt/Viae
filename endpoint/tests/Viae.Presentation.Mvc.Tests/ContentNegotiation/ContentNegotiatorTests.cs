// Copyright © 2025 Curt Gilman
// SPDX-License-Identifier: AGPL-3.0-only
// Viae: A geo-centric, journey-focused, federated blog platform

using System.Text.Json;
using FluentAssertions;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Viae.ActivityPub;
using Viae.Presentation.Mvc.ContentNegotiation;

namespace Viae.Presentation.Mvc.Tests.ContentNegotiation;

[TestClass]
public class ContentNegotiatorTests
{
    private sealed class TestModel
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
    }

    private static readonly JsonSerializerOptions JsonOptions = new(JsonSerializerDefaults.Web)
    {
        Converters = { new ActivityObjectJsonConverter() },
    };

    private static HttpRequest CreateRequest(string acceptHeader)
    {
        var context = new DefaultHttpContext();
        context.Request.Headers.Accept = acceptHeader;
        return context.Request;
    }

    [TestMethod]
    public async Task ExecuteAsync_WithJsonAcceptHeader_ReturnsJsonResult()
    {
        // Arrange
        var model = new TestModel { Id = 1, Name = "Test" };
        var request = CreateRequest("application/json");
        var negotiator = new ContentNegotiator<TestModel>(model, request, JsonOptions);

        // Act
        negotiator.ForJson();
        var result = await negotiator.ExecuteAsync();

        // Assert
        result.Should().BeOfType<JsonResult>();
        var jsonResult = result.As<JsonResult>();
        jsonResult.Value.Should().Be(model);
    }

    [TestMethod]
    public async Task ExecuteAsync_WithJsonTransform_ReturnsTransformedResult()
    {
        // Arrange
        var model = new TestModel { Id = 1, Name = "Test" };
        var request = CreateRequest("application/json");
        var negotiator = new ContentNegotiator<TestModel>(model, request, JsonOptions);

        // Act
        negotiator.ForJson(m => new { id = m.Id, upperName = m.Name.ToUpperInvariant() });
        var result = await negotiator.ExecuteAsync();

        // Assert
        var jsonResult = result.As<JsonResult>();
        dynamic value = jsonResult.Value!;
        ((int)value.id).Should().Be(1);
        ((string)value.upperName).Should().Be("TEST");
    }

    [TestMethod]
    public async Task ExecuteAsync_WithHtmlAcceptHeader_ReturnsViewResult()
    {
        // Arrange
        var model = new TestModel { Id = 1, Name = "Test" };
        var request = CreateRequest("text/html");
        var negotiator = new ContentNegotiator<TestModel>(model, request, JsonOptions);

        // Act
        negotiator.ForHtml("TestView");
        var result = await negotiator.ExecuteAsync();

        // Assert
        result.Should().BeOfType<ViewResult>();
        var viewResult = result.As<ViewResult>();
        viewResult.ViewName.Should().Be("TestView");
        viewResult.ViewData.Model.Should().Be(model);
    }

    [TestMethod]
    public async Task ExecuteAsync_WithActivityPubAcceptHeader_ReturnsActivityPubResult()
    {
        // Arrange
        var model = new TestModel { Id = 1, Name = "Test" };
        var request = CreateRequest("application/activity+json");
        var negotiator = new ContentNegotiator<TestModel>(model, request, JsonOptions);

        // Act
        negotiator.ForActivityPub(m => new Note { Content = m.Name });
        var result = await negotiator.ExecuteAsync();

        // Assert
        var contentResult = result.As<ContentResult>();
        contentResult.ContentType.Should().Be("application/activity+json");
        contentResult.Content.Should().NotBeNullOrWhiteSpace();

        // Verify that the serialized JSON contains the type discriminator and content
        contentResult.Content.Should().Contain("\"type\"");
        contentResult.Content.Should().Contain("\"Note\"");
        contentResult.Content.Should().Contain("\"content\"");
        contentResult.Content.Should().Contain("Test");
    }

    [TestMethod]
    public async Task ExecuteAsync_WithJsonLdAcceptHeader_ReturnsActivityPubResult()
    {
        // Arrange
        var model = new TestModel { Id = 1, Name = "Test" };
        var request = CreateRequest("application/ld+json");
        var negotiator = new ContentNegotiator<TestModel>(model, request, JsonOptions);

        // Act
        negotiator.ForActivityPub(m => new Note { Content = m.Name });
        var result = await negotiator.ExecuteAsync();

        // Assert
        var contentResult = result.As<ContentResult>();
        contentResult
            .ContentType.Should()
            .Be("application/ld+json; profile=\"https://www.w3.org/ns/activitystreams\"");
    }

    [TestMethod]
    public async Task ExecuteAsync_WithCustomMediaType_ReturnsCustomResult()
    {
        // Arrange
        var model = new TestModel { Id = 1, Name = "Test" };
        var request = CreateRequest("application/xml");
        var negotiator = new ContentNegotiator<TestModel>(model, request, JsonOptions);

        // Act
        negotiator.ForMediaType(
            "application/xml",
            m =>
                Task.FromResult<IActionResult>(
                    new ContentResult
                    {
                        Content = $"<test><id>{m.Id}</id><n>{m.Name}</n></test>",
                        ContentType = "application/xml",
                    }
                )
        );
        var result = await negotiator.ExecuteAsync();

        // Assert
        var contentResult = result.As<ContentResult>();
        contentResult.ContentType.Should().Be("application/xml");
        contentResult.Content.Should().Contain("<id>1</id>");
    }

    [TestMethod]
    public async Task ExecuteAsync_WithMultipleAcceptHeaders_ReturnsFirstMatch()
    {
        // Arrange
        var model = new TestModel { Id = 1, Name = "Test" };
        var request = CreateRequest("text/html, application/json");
        var negotiator = new ContentNegotiator<TestModel>(model, request, JsonOptions);

        // Act
        negotiator.ForJson().ForHtml("TestView");
        var result = await negotiator.ExecuteAsync();

        // Assert - HTML appears first in Accept header, but JSON is checked first in handlers
        // This test documents current behavior - order matters
        result.Should().BeOfType<JsonResult>();
    }

    [TestMethod]
    public async Task ExecuteAsync_WithNoMatchingHandler_ReturnsDefaultHandler()
    {
        // Arrange
        var model = new TestModel { Id = 1, Name = "Test" };
        var request = CreateRequest("application/pdf");
        var negotiator = new ContentNegotiator<TestModel>(model, request, JsonOptions);

        // Act
        negotiator
            .ForJson()
            .WithDefault(m =>
                Task.FromResult<IActionResult>(new ContentResult { Content = "Default response" })
            );
        var result = await negotiator.ExecuteAsync();

        // Assert
        var contentResult = result.As<ContentResult>();
        contentResult.Content.Should().Be("Default response");
    }

    [TestMethod]
    public async Task ExecuteAsync_WithNoMatchingHandlerAndNoDefault_ReturnsFirstRegistered()
    {
        // Arrange
        var model = new TestModel { Id = 1, Name = "Test" };
        var request = CreateRequest("application/pdf");
        var negotiator = new ContentNegotiator<TestModel>(model, request, JsonOptions);

        // Act
        negotiator.ForJson().ForHtml("Test");
        var result = await negotiator.ExecuteAsync();

        // Assert - Falls back to first registered handler
        result.Should().BeOfType<JsonResult>();
    }

    [TestMethod]
    public async Task ExecuteAsync_WithNoHandlers_Returns406NotAcceptable()
    {
        // Arrange
        var model = new TestModel { Id = 1, Name = "Test" };
        var request = CreateRequest("application/json");
        var negotiator = new ContentNegotiator<TestModel>(model, request, JsonOptions);

        // Act - No handlers registered
        var result = await negotiator.ExecuteAsync();

        // Assert
        var statusResult = result.As<StatusCodeResult>();
        statusResult.StatusCode.Should().Be(406);
    }

    [TestMethod]
    public async Task ExecuteAsync_WithJsonInActivityPubHeader_SkipsPlainJson()
    {
        // Arrange
        var model = new TestModel { Id = 1, Name = "Test" };
        var request = CreateRequest("application/activity+json");
        var negotiator = new ContentNegotiator<TestModel>(model, request, JsonOptions);

        // Act
        negotiator
            .ForJson(m => new { plain = true })
            .ForActivityPub(m => new Note { Content = "activity" });
        var result = await negotiator.ExecuteAsync();

        // Assert - Should match ActivityPub, not plain JSON
        var contentResult = result.As<ContentResult>();
        contentResult.ContentType.Should().Be("application/activity+json");
        contentResult.Content.Should().Contain("\"content\"");
        contentResult.Content.Should().Contain("activity");
        contentResult.Content.Should().NotContain("plain");
    }
}
