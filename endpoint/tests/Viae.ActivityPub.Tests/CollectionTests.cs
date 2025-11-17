// Copyright © 2025 Curt Gilman
// SPDX-License-Identifier: AGPL-3.0-only
// Viae: A geo-centric, journey-focused, federated blog platform

using System.Text.Json;
using FluentAssertions;

namespace Viae.ActivityPub.Tests;

[TestClass]
public class CollectionTests
{
    [TestMethod]
    public void Collection_ShouldDeserialize_FromSimpleJson()
    {
        // Arrange
        var json = """
            {
                "@context": "https://www.w3.org/ns/activitystreams",
                "type": "Collection",
                "id": "https://example.com/collection/1",
                "totalItems": 5,
                "first": "https://example.com/collection/1?page=1"
            }
            """;

        // Act
        var obj = JsonSerializer.Deserialize<ActivityObject>(json);

        // Assert
        obj.Should().NotBeNull();
        obj.Should().BeOfType<Collection>();

        var collection = (Collection)obj!;
        collection.Id.Should().Be(new Uri("https://example.com/collection/1"));
        collection.TotalItems.Should().Be(5);
        collection.First.Should().NotBeNull();
        collection.First!.Value.IsLink.Should().BeTrue();
        collection
            .First.Value.Link!.Value.Href.Should()
            .Be(new Uri("https://example.com/collection/1?page=1"));
    }

    [TestMethod]
    public void Collection_WithItems_ShouldDeserialize()
    {
        // Arrange
        var json = """
            {
                "@context": "https://www.w3.org/ns/activitystreams",
                "type": "Collection",
                "id": "https://example.com/collection/2",
                "totalItems": 2,
                "items": [
                    "https://example.com/notes/1",
                    "https://example.com/notes/2"
                ]
            }
            """;

        // Act
        var obj = JsonSerializer.Deserialize<ActivityObject>(json);
        var collection = (Collection)obj!;

        // Assert
        collection.Items.Should().NotBeNull();
        collection.Items.Should().HaveCount(2);
        collection.Items![0].IsLink.Should().BeTrue();
        collection.Items[0].Link!.Value.Href.Should().Be(new Uri("https://example.com/notes/1"));
        collection.Items[1].Link!.Value.Href.Should().Be(new Uri("https://example.com/notes/2"));
    }

    [TestMethod]
    public void Collection_ShouldSerialize_WithRequiredProperties()
    {
        // Arrange
        var collection = new Collection
        {
            Id = new Uri("https://example.com/collection/test"),
            TotalItems = 10,
            First = new LinkOr<CollectionPage>(
                new Uri("https://example.com/collection/test?page=1")
            ),
            Last = new LinkOr<CollectionPage>(
                new Uri("https://example.com/collection/test?page=5")
            ),
        };

        // Act
        var json = JsonSerializer.Serialize<ActivityObject>(collection);
        var deserialized = JsonSerializer.Deserialize<ActivityObject>(json);

        // Assert
        deserialized.Should().NotBeNull();
        deserialized.Should().BeOfType<Collection>();

        var deserializedCollection = (Collection)deserialized!;
        deserializedCollection.Id.Should().Be(collection.Id);
        deserializedCollection.TotalItems.Should().Be(10);
        deserializedCollection.First.Should().NotBeNull();
        deserializedCollection.Last.Should().NotBeNull();
    }

    [TestMethod]
    public void OrderedCollection_ShouldDeserialize_FromSimpleJson()
    {
        // Arrange
        var json = """
            {
                "@context": "https://www.w3.org/ns/activitystreams",
                "type": "OrderedCollection",
                "id": "https://example.com/outbox",
                "totalItems": 3,
                "first": "https://example.com/outbox?page=1"
            }
            """;

        // Act
        var obj = JsonSerializer.Deserialize<ActivityObject>(json);

        // Assert
        obj.Should().NotBeNull();
        obj.Should().BeOfType<OrderedCollection>();

        var collection = (OrderedCollection)obj!;
        collection.Id.Should().Be(new Uri("https://example.com/outbox"));
        collection.TotalItems.Should().Be(3);
    }

    [TestMethod]
    public void OrderedCollection_WithOrderedItems_ShouldDeserialize()
    {
        // Arrange
        var json = """
            {
                "@context": "https://www.w3.org/ns/activitystreams",
                "type": "OrderedCollection",
                "id": "https://example.com/ordered/1",
                "totalItems": 3,
                "orderedItems": [
                    "https://example.com/notes/3",
                    "https://example.com/notes/2",
                    "https://example.com/notes/1"
                ]
            }
            """;

        // Act
        var obj = JsonSerializer.Deserialize<ActivityObject>(json);
        var collection = (OrderedCollection)obj!;

        // Assert
        collection.OrderedItems.Should().NotBeNull();
        collection.OrderedItems.Should().HaveCount(3);
        collection.OrderedItems![0].IsLink.Should().BeTrue();
        collection
            .OrderedItems[0]
            .Link!.Value.Href.Should()
            .Be(new Uri("https://example.com/notes/3"));
        collection
            .OrderedItems[1]
            .Link!.Value.Href.Should()
            .Be(new Uri("https://example.com/notes/2"));
        collection
            .OrderedItems[2]
            .Link!.Value.Href.Should()
            .Be(new Uri("https://example.com/notes/1"));
    }

    [TestMethod]
    public void OrderedCollection_ShouldSerialize_WithOrderedItems()
    {
        // Arrange
        var collection = new OrderedCollection
        {
            Id = new Uri("https://example.com/ordered/test"),
            TotalItems = 2,
            OrderedItems = new OneOrMany<LinkOr<ActivityObject>>([
                new LinkOr<ActivityObject>(new Uri("https://example.com/item/1")),
                new LinkOr<ActivityObject>(new Uri("https://example.com/item/2")),
            ]),
        };

        // Act
        var json = JsonSerializer.Serialize<ActivityObject>(collection);
        var deserialized = JsonSerializer.Deserialize<ActivityObject>(json);

        // Assert
        deserialized.Should().NotBeNull();
        deserialized.Should().BeOfType<OrderedCollection>();

        var deserializedCollection = (OrderedCollection)deserialized!;
        deserializedCollection.OrderedItems.Should().HaveCount(2);
    }

    [TestMethod]
    public void CollectionPage_ShouldDeserialize_FromSimpleJson()
    {
        // Arrange
        var json = """
            {
                "@context": "https://www.w3.org/ns/activitystreams",
                "type": "CollectionPage",
                "id": "https://example.com/collection/1?page=2",
                "partOf": "https://example.com/collection/1",
                "next": "https://example.com/collection/1?page=3",
                "prev": "https://example.com/collection/1?page=1",
                "items": [
                    "https://example.com/item/5",
                    "https://example.com/item/6"
                ]
            }
            """;

        // Act
        var obj = JsonSerializer.Deserialize<ActivityObject>(json);

        // Assert
        obj.Should().NotBeNull();
        obj.Should().BeOfType<CollectionPage>();

        var page = (CollectionPage)obj!;
        page.Id.Should().Be(new Uri("https://example.com/collection/1?page=2"));
        page.PartOf.Should().NotBeNull();
        page.PartOf!.Value.IsLink.Should().BeTrue();
        page.PartOf.Value.Link!.Value.Href.Should().Be(new Uri("https://example.com/collection/1"));
        page.Next.Should().NotBeNull();
        page.Next!.Value.IsLink.Should().BeTrue();
        page.Prev.Should().NotBeNull();
        page.Prev!.Value.IsLink.Should().BeTrue();
        page.Items.Should().HaveCount(2);
    }

    [TestMethod]
    public void CollectionPage_ShouldSerialize_WithNavigationProperties()
    {
        // Arrange
        var page = new CollectionPage
        {
            Id = new Uri("https://example.com/page/test"),
            PartOf = new LinkOr<Collection>(new Uri("https://example.com/collection/main")),
            Next = new LinkOr<CollectionPage>(
                new Uri("https://example.com/collection/main?page=2")
            ),
            Items = new OneOrMany<LinkOr<ActivityObject>>(
                new LinkOr<ActivityObject>(new Uri("https://example.com/item/1"))
            ),
        };

        // Act
        var json = JsonSerializer.Serialize<ActivityObject>(page);
        var deserialized = JsonSerializer.Deserialize<ActivityObject>(json);

        // Assert
        deserialized.Should().NotBeNull();
        deserialized.Should().BeOfType<CollectionPage>();

        var deserializedPage = (CollectionPage)deserialized!;
        deserializedPage.PartOf.Should().NotBeNull();
        deserializedPage.Next.Should().NotBeNull();
        deserializedPage.Items.Should().HaveCount(1);
    }

    [TestMethod]
    public void OrderedCollectionPage_ShouldDeserialize_FromSimpleJson()
    {
        // Arrange
        var json = """
            {
                "@context": "https://www.w3.org/ns/activitystreams",
                "type": "OrderedCollectionPage",
                "id": "https://example.com/outbox?page=1",
                "partOf": "https://example.com/outbox",
                "next": "https://example.com/outbox?page=2",
                "startIndex": 0,
                "orderedItems": [
                    {
                        "type": "Note",
                        "id": "https://example.com/note/1",
                        "content": "First note"
                    },
                    {
                        "type": "Note",
                        "id": "https://example.com/note/2",
                        "content": "Second note"
                    }
                ]
            }
            """;

        // Act
        var obj = JsonSerializer.Deserialize<ActivityObject>(json);

        // Assert
        obj.Should().NotBeNull();
        obj.Should().BeOfType<OrderedCollectionPage>();

        var page = (OrderedCollectionPage)obj!;
        page.Id.Should().Be(new Uri("https://example.com/outbox?page=1"));
        page.PartOf.Should().NotBeNull();
        page.Next.Should().NotBeNull();
        page.StartIndex.Should().Be(0);
        page.OrderedItems.Should().NotBeNull();
        page.OrderedItems.Should().HaveCount(2);
    }

    [TestMethod]
    public void OrderedCollectionPage_WithInlineObjects_ShouldDeserialize()
    {
        // Arrange
        var json = """
            {
                "@context": "https://www.w3.org/ns/activitystreams",
                "type": "OrderedCollectionPage",
                "id": "https://example.com/timeline?page=1",
                "partOf": "https://example.com/timeline",
                "startIndex": 10,
                "orderedItems": [
                    {
                        "type": "Note",
                        "id": "https://example.com/note/100",
                        "content": "<p>Inline note content</p>",
                        "published": "2025-10-22T12:00:00Z"
                    }
                ]
            }
            """;

        // Act
        var obj = JsonSerializer.Deserialize<ActivityObject>(json);
        var page = (OrderedCollectionPage)obj!;

        // Assert
        page.OrderedItems.Should().NotBeNull();
        page.OrderedItems.Should().HaveCount(1);
        page.OrderedItems![0].IsValue.Should().BeTrue();
        page.OrderedItems[0].Value.Should().BeOfType<Note>();

        var note = (Note)page.OrderedItems[0].Value!;
        note.Id.Should().Be(new Uri("https://example.com/note/100"));
        note.Content.Should().Be("<p>Inline note content</p>");
    }

    [TestMethod]
    public void OrderedCollectionPage_ShouldSerialize_WithAllProperties()
    {
        // Arrange
        var page = new OrderedCollectionPage
        {
            Id = new Uri("https://example.com/ordered-page/test"),
            PartOf = new LinkOr<Collection>(new Uri("https://example.com/ordered-collection/main")),
            Next = new LinkOr<CollectionPage>(
                new Uri("https://example.com/ordered-collection/main?page=2")
            ),
            Prev = new LinkOr<CollectionPage>(
                new Uri("https://example.com/ordered-collection/main?page=0")
            ),
            StartIndex = 20,
            OrderedItems = new OneOrMany<LinkOr<ActivityObject>>([
                new LinkOr<ActivityObject>(new Uri("https://example.com/activity/20")),
                new LinkOr<ActivityObject>(new Uri("https://example.com/activity/21")),
            ]),
        };

        // Act
        var json = JsonSerializer.Serialize<ActivityObject>(page);
        var deserialized = JsonSerializer.Deserialize<ActivityObject>(json);

        // Assert
        deserialized.Should().NotBeNull();
        deserialized.Should().BeOfType<OrderedCollectionPage>();

        var deserializedPage = (OrderedCollectionPage)deserialized!;
        deserializedPage.StartIndex.Should().Be(20);
        deserializedPage.OrderedItems.Should().HaveCount(2);
        deserializedPage.Next.Should().NotBeNull();
        deserializedPage.Prev.Should().NotBeNull();
    }

    [TestMethod]
    public void Collection_WithInlineCollectionPage_ShouldDeserialize()
    {
        // Arrange
        var json = """
            {
                "@context": "https://www.w3.org/ns/activitystreams",
                "type": "Collection",
                "id": "https://example.com/collection/complex",
                "totalItems": 100,
                "first": {
                    "type": "CollectionPage",
                    "id": "https://example.com/collection/complex?page=1",
                    "items": [
                        "https://example.com/item/1"
                    ]
                }
            }
            """;

        // Act
        var obj = JsonSerializer.Deserialize<ActivityObject>(json);
        var collection = (Collection)obj!;

        // Assert
        collection.First.Should().NotBeNull();
        collection.First!.Value.IsValue.Should().BeTrue();
        collection.First.Value.Value.Should().BeOfType<CollectionPage>();

        var firstPage = collection.First.Value.Value!;
        firstPage.Id.Should().Be(new Uri("https://example.com/collection/complex?page=1"));
    }

    [TestMethod]
    public void RoundTrip_Collection_ShouldPreserveAllData()
    {
        // Arrange
        var original = new Collection
        {
            Context = "https://www.w3.org/ns/activitystreams",
            Id = new Uri("https://example.com/roundtrip/collection"),
            TotalItems = 42,
            Current = new LinkOr<CollectionPage>(
                new Uri("https://example.com/roundtrip/collection?page=2")
            ),
            First = new LinkOr<CollectionPage>(
                new Uri("https://example.com/roundtrip/collection?page=1")
            ),
            Last = new LinkOr<CollectionPage>(
                new Uri("https://example.com/roundtrip/collection?page=5")
            ),
            Items = new OneOrMany<LinkOr<ActivityObject>>([
                new LinkOr<ActivityObject>(new Uri("https://example.com/item/a")),
                new LinkOr<ActivityObject>(new Uri("https://example.com/item/b")),
                new LinkOr<ActivityObject>(new Uri("https://example.com/item/c")),
            ]),
        };

        // Act
        var json = JsonSerializer.Serialize<ActivityObject>(original);
        var deserialized = JsonSerializer.Deserialize<ActivityObject>(json) as Collection;

        // Assert
        deserialized.Should().NotBeNull();
        deserialized!.Id.Should().Be(original.Id);
        deserialized.TotalItems.Should().Be(42);
        deserialized.Current.Should().NotBeNull();
        deserialized.First.Should().NotBeNull();
        deserialized.Last.Should().NotBeNull();
        deserialized.Items.Should().HaveCount(3);
    }

    [TestMethod]
    public void RoundTrip_OrderedCollection_ShouldPreserveAllData()
    {
        // Arrange
        var original = new OrderedCollection
        {
            Context = "https://www.w3.org/ns/activitystreams",
            Id = new Uri("https://example.com/roundtrip/ordered"),
            TotalItems = 15,
            First = new LinkOr<CollectionPage>(
                new Uri("https://example.com/roundtrip/ordered?page=1")
            ),
            OrderedItems = new OneOrMany<LinkOr<ActivityObject>>([
                new LinkOr<ActivityObject>(new Uri("https://example.com/activity/1")),
                new LinkOr<ActivityObject>(new Uri("https://example.com/activity/2")),
            ]),
        };

        // Act
        var json = JsonSerializer.Serialize<ActivityObject>(original);
        var deserialized = JsonSerializer.Deserialize<ActivityObject>(json) as OrderedCollection;

        // Assert
        deserialized.Should().NotBeNull();
        deserialized!.Id.Should().Be(original.Id);
        deserialized.TotalItems.Should().Be(15);
        deserialized.OrderedItems.Should().HaveCount(2);
    }

    [TestMethod]
    public void RoundTrip_CollectionPage_ShouldPreserveAllData()
    {
        // Arrange
        var original = new CollectionPage
        {
            Context = "https://www.w3.org/ns/activitystreams",
            Id = new Uri("https://example.com/roundtrip/page"),
            PartOf = new LinkOr<Collection>(new Uri("https://example.com/roundtrip/main")),
            Next = new LinkOr<CollectionPage>(new Uri("https://example.com/roundtrip/main?page=3")),
            Prev = new LinkOr<CollectionPage>(new Uri("https://example.com/roundtrip/main?page=1")),
            Items = new OneOrMany<LinkOr<ActivityObject>>(
                new LinkOr<ActivityObject>(new Uri("https://example.com/item/page2"))
            ),
        };

        // Act
        var json = JsonSerializer.Serialize<ActivityObject>(original);
        var deserialized = JsonSerializer.Deserialize<ActivityObject>(json) as CollectionPage;

        // Assert
        deserialized.Should().NotBeNull();
        deserialized!.Id.Should().Be(original.Id);
        deserialized.PartOf.Should().NotBeNull();
        deserialized.Next.Should().NotBeNull();
        deserialized.Prev.Should().NotBeNull();
        deserialized.Items.Should().HaveCount(1);
    }

    [TestMethod]
    public void RoundTrip_OrderedCollectionPage_ShouldPreserveAllData()
    {
        // Arrange
        var original = new OrderedCollectionPage
        {
            Context = "https://www.w3.org/ns/activitystreams",
            Id = new Uri("https://example.com/roundtrip/ordered-page"),
            PartOf = new LinkOr<Collection>(new Uri("https://example.com/roundtrip/ordered-main")),
            Next = new LinkOr<CollectionPage>(
                new Uri("https://example.com/roundtrip/ordered-main?page=2")
            ),
            StartIndex = 50,
            OrderedItems = new OneOrMany<LinkOr<ActivityObject>>([
                new LinkOr<ActivityObject>(new Uri("https://example.com/activity/50")),
                new LinkOr<ActivityObject>(new Uri("https://example.com/activity/51")),
                new LinkOr<ActivityObject>(new Uri("https://example.com/activity/52")),
            ]),
        };

        // Act
        var json = JsonSerializer.Serialize<ActivityObject>(original);
        var deserialized =
            JsonSerializer.Deserialize<ActivityObject>(json) as OrderedCollectionPage;

        // Assert
        deserialized.Should().NotBeNull();
        deserialized!.Id.Should().Be(original.Id);
        deserialized.PartOf.Should().NotBeNull();
        deserialized.Next.Should().NotBeNull();
        deserialized.StartIndex.Should().Be(50);
        deserialized.OrderedItems.Should().HaveCount(3);
    }

    [TestMethod]
    public void TypeDiscriminator_Collection_ShouldBePresent()
    {
        // Arrange
        var collection = new Collection { Id = new Uri("https://example.com/test") };

        // Act
        var json = JsonSerializer.Serialize<ActivityObject>(collection);
        var jsonElement = JsonSerializer.Deserialize<JsonElement>(json);

        // Assert
        jsonElement.TryGetProperty("type", out var typeProperty).Should().BeTrue();
        typeProperty.GetString().Should().Be("Collection");
    }

    [TestMethod]
    public void TypeDiscriminator_OrderedCollection_ShouldBePresent()
    {
        // Arrange
        var collection = new OrderedCollection { Id = new Uri("https://example.com/test") };

        // Act
        var json = JsonSerializer.Serialize<ActivityObject>(collection);
        var jsonElement = JsonSerializer.Deserialize<JsonElement>(json);

        // Assert
        jsonElement.TryGetProperty("type", out var typeProperty).Should().BeTrue();
        typeProperty.GetString().Should().Be("OrderedCollection");
    }

    [TestMethod]
    public void TypeDiscriminator_CollectionPage_ShouldBePresent()
    {
        // Arrange
        var page = new CollectionPage { Id = new Uri("https://example.com/test") };

        // Act
        var json = JsonSerializer.Serialize<ActivityObject>(page);
        var jsonElement = JsonSerializer.Deserialize<JsonElement>(json);

        // Assert
        jsonElement.TryGetProperty("type", out var typeProperty).Should().BeTrue();
        typeProperty.GetString().Should().Be("CollectionPage");
    }

    [TestMethod]
    public void TypeDiscriminator_OrderedCollectionPage_ShouldBePresent()
    {
        // Arrange
        var page = new OrderedCollectionPage { Id = new Uri("https://example.com/test") };

        // Act
        var json = JsonSerializer.Serialize<ActivityObject>(page);
        var jsonElement = JsonSerializer.Deserialize<JsonElement>(json);

        // Assert
        jsonElement.TryGetProperty("type", out var typeProperty).Should().BeTrue();
        typeProperty.GetString().Should().Be("OrderedCollectionPage");
    }

    [TestMethod]
    public void MultipleCollectionTypes_ShouldDeserialize_ToCorrectTypes()
    {
        // Arrange
        var collections = new List<ActivityObject>
        {
            new Collection { Id = new Uri("https://example.com/collection/1"), TotalItems = 10 },
            new OrderedCollection
            {
                Id = new Uri("https://example.com/ordered/1"),
                TotalItems = 20,
            },
            new CollectionPage
            {
                Id = new Uri("https://example.com/page/1"),
                PartOf = new LinkOr<Collection>(new Uri("https://example.com/collection/1")),
            },
            new OrderedCollectionPage
            {
                Id = new Uri("https://example.com/ordered-page/1"),
                StartIndex = 0,
            },
        };

        // Act
        var json = JsonSerializer.Serialize(collections);
        var deserialized = JsonSerializer.Deserialize<List<ActivityObject>>(json);

        // Assert
        deserialized.Should().NotBeNull();
        deserialized.Should().HaveCount(4);
        deserialized![0].Should().BeOfType<Collection>();
        deserialized[1].Should().BeOfType<OrderedCollection>();
        deserialized[2].Should().BeOfType<CollectionPage>();
        deserialized[3].Should().BeOfType<OrderedCollectionPage>();
    }

    [TestMethod]
    public void Collection_ShouldDeserialize_FromTestDataFile()
    {
        // Arrange
        var json = File.ReadAllText("TestData/collection-simple.json");

        // Act
        var obj = JsonSerializer.Deserialize<ActivityObject>(json);

        // Assert
        obj.Should().NotBeNull();
        obj.Should().BeOfType<Collection>();

        var collection = (Collection)obj!;
        collection.Id.Should().Be(new Uri("https://mastodon.social/users/alice/followers"));
        collection.TotalItems.Should().Be(250);
        collection.First.Should().NotBeNull();
        collection.First!.Value.IsLink.Should().BeTrue();
        collection.Last.Should().NotBeNull();
        collection.Last!.Value.IsLink.Should().BeTrue();
    }

    [TestMethod]
    public void OrderedCollection_ShouldDeserialize_FromTestDataFile()
    {
        // Arrange
        var json = File.ReadAllText("TestData/ordered-collection-outbox.json");

        // Act
        var obj = JsonSerializer.Deserialize<ActivityObject>(json);

        // Assert
        obj.Should().NotBeNull();
        obj.Should().BeOfType<OrderedCollection>();

        var collection = (OrderedCollection)obj!;
        collection.Id.Should().Be(new Uri("https://mastodon.social/users/alice/outbox"));
        collection.TotalItems.Should().Be(42);
        collection.First.Should().NotBeNull();
        collection.Last.Should().NotBeNull();
    }

    [TestMethod]
    public void CollectionPage_ShouldDeserialize_FromTestDataFile()
    {
        // Arrange
        var json = File.ReadAllText("TestData/collection-page.json");

        // Act
        var obj = JsonSerializer.Deserialize<ActivityObject>(json);

        // Assert
        obj.Should().NotBeNull();
        obj.Should().BeOfType<CollectionPage>();

        var page = (CollectionPage)obj!;
        page.Id.Should().Be(new Uri("https://mastodon.social/users/alice/followers?page=2"));
        page.PartOf.Should().NotBeNull();
        page.PartOf!.Value.IsLink.Should().BeTrue();
        page.PartOf.Value.Link!.Value.Href.Should()
            .Be(new Uri("https://mastodon.social/users/alice/followers"));
        page.Next.Should().NotBeNull();
        page.Prev.Should().NotBeNull();
        page.TotalItems.Should().Be(250);
        page.Items.Should().HaveCount(3);
    }

    [TestMethod]
    public void OrderedCollectionPage_ShouldDeserialize_FromTestDataFile()
    {
        // Arrange
        var json = File.ReadAllText("TestData/ordered-collection-page.json");

        // Act
        var obj = JsonSerializer.Deserialize<ActivityObject>(json);

        // Assert
        obj.Should().NotBeNull();
        obj.Should().BeOfType<OrderedCollectionPage>();

        var page = (OrderedCollectionPage)obj!;
        page.Id.Should().Be(new Uri("https://mastodon.social/users/alice/outbox?page=true"));
        page.PartOf.Should().NotBeNull();
        page.Next.Should().NotBeNull();
        page.StartIndex.Should().Be(0);
        page.OrderedItems.Should().NotBeNull();
        page.OrderedItems.Should().HaveCount(2);
    }

    [TestMethod]
    public void OrderedCollectionPage_WithInlineCreateActivities_ShouldDeserialize()
    {
        // Arrange
        var json = File.ReadAllText("TestData/ordered-collection-page.json");

        // Act
        var obj = JsonSerializer.Deserialize<ActivityObject>(json);
        var page = (OrderedCollectionPage)obj!;

        // Assert - Verify first item is an inline Create activity with inline Note
        page.OrderedItems![0].IsValue.Should().BeTrue();
        page.OrderedItems[0].Value.Should().BeOfType<CreateActivity>();

        var createActivity = (CreateActivity)page.OrderedItems[0].Value!;
        createActivity
            .Id.Should()
            .Be(new Uri("https://mastodon.social/users/alice/statuses/67890/activity"));
        createActivity.Objekt.Should().NotBeNull();
        createActivity.Objekt!.Value.IsValue.Should().BeTrue();
        createActivity.Objekt.Value.Value.Should().BeOfType<Note>();

        var note = (Note)createActivity.Objekt.Value.Value!;
        note.Content.Should().Contain("recent post from my outbox");

        // Assert - Verify second item is a Create activity with a link to the object
        page.OrderedItems[1].IsValue.Should().BeTrue();
        page.OrderedItems[1].Value.Should().BeOfType<CreateActivity>();

        var secondActivity = (CreateActivity)page.OrderedItems[1].Value!;
        secondActivity.Objekt.Should().NotBeNull();
        secondActivity.Objekt!.Value.IsLink.Should().BeTrue();
        secondActivity
            .Objekt.Value.Link!.Value.Href.Should()
            .Be(new Uri("https://mastodon.social/users/alice/statuses/67889"));
    }

    [TestMethod]
    public void Collection_WithNestedObjects_ShouldNotSerializeContextOnNestedObjects()
    {
        // Arrange
        var collection = new Collection
        {
            Id = new Uri("http://localhost:5689/collection/test"),
            Items = new OneOrMany<LinkOr<ActivityObject>>([
                new LinkOr<ActivityObject>(
                    new Place
                    {
                        Name = "Aéroport de Montpellier–Méditerranée",
                        Latitude = 43.57884216246277,
                        Longitude = 3.9595467508554423,
                        Id = new Uri("http://localhost:5689/loca/MTji3oZQpWs"),
                        Url = new LinkOr<Uri>(
                            new Uri(
                                "http://localhost:5689/loca/MTji3oZQpWs/aeroport-de-montpelliermediterranee"
                            )
                        ),
                    }
                ),
            ]),
        };

        // Act
        var json = JsonSerializer.Serialize<ActivityObject>(collection);
        var jsonElement = JsonSerializer.Deserialize<JsonElement>(json);

        // Assert - Root object should have @context
        jsonElement.TryGetProperty("@context", out var rootContext).Should().BeTrue();
        rootContext.GetString().Should().Be("https://www.w3.org/ns/activitystreams");

        // Assert - Nested Place object should NOT have @context
        jsonElement.TryGetProperty("items", out var items).Should().BeTrue();
        var firstItem = items.EnumerateArray().First();
        firstItem.TryGetProperty("@context", out _).Should().BeFalse();

        // Assert - Nested object should have all other properties
        firstItem.TryGetProperty("type", out var type).Should().BeTrue();
        type.GetString().Should().Be("Place");
        firstItem.TryGetProperty("name", out var name).Should().BeTrue();
        name.GetString().Should().Be("Aéroport de Montpellier–Méditerranée");
        firstItem.TryGetProperty("id", out var id).Should().BeTrue();
        id.GetString().Should().Be("http://localhost:5689/loca/MTji3oZQpWs");
    }
}
