// Copyright © 2025 Curt Gilman
// SPDX-License-Identifier: AGPL-3.0-only
// Viae: A geo-centric, journey-focused, federated blog platform

using System.Text.Json;
using FluentAssertions;

namespace Viae.ActivityPub.Tests;

[TestClass]
public class ActivityTests
{
    [TestMethod]
    public void CreateActivity_ShouldDeserialize_WithInlineNote()
    {
        // Arrange
        var json = File.ReadAllText("TestData/create-note.json");

        // Act
        var activity = JsonSerializer.Deserialize<ActivityObject>(json);

        // Assert
        activity.Should().NotBeNull();
        activity.Should().BeOfType<CreateActivity>();

        var create = (CreateActivity)activity!;
        create
            .Id.Should()
            .Be(new Uri("https://mastodon.social/users/alice/statuses/12345/activity"));
        create.Actor.Should().NotBeNull();
        create.Actor!.Value.IsLink.Should().BeTrue();
        create.Actor.Value.Link!.Value.Href.Should().Be(new Uri("https://mastodon.social/@alice"));
        create.To.Should().HaveCount(1);
        create.Cc.Should().HaveCount(1);

        create.Objekt.Should().NotBeNull();
        create.Objekt!.Value.IsValue.Should().BeTrue();
        create.Objekt.Value.Value.Should().BeOfType<Note>();

        var note = (Note)create.Objekt.Value.Value!;
        note.Id.Should().Be(new Uri("https://mastodon.social/users/alice/statuses/12345"));
        note.Content.Should().Contain("Hello, federated world!");
    }

    [TestMethod]
    public void CreateActivity_ShouldDeserialize_WithLinkObject()
    {
        // Arrange
        var json = File.ReadAllText("TestData/create-note-with-link-object.json");

        // Act
        var activity = JsonSerializer.Deserialize<ActivityObject>(json);

        // Assert
        activity.Should().NotBeNull();
        activity.Should().BeOfType<CreateActivity>();

        var create = (CreateActivity)activity!;
        create.Objekt.Should().NotBeNull();
        create.Objekt!.Value.IsLink.Should().BeTrue();
        create.Objekt.Value.Link!.Value.Href.Should().Be(new Uri("https://example.com/notes/789"));
    }

    [TestMethod]
    public void FollowActivity_ShouldDeserialize()
    {
        // Arrange
        var json = File.ReadAllText("TestData/follow.json");

        // Act
        var activity = JsonSerializer.Deserialize<ActivityObject>(json);

        // Assert
        activity.Should().NotBeNull();
        activity.Should().BeOfType<FollowActivity>();

        var follow = (FollowActivity)activity!;
        follow.Id.Should().Be(new Uri("https://pixelfed.social/users/charlie/follows/001"));
        follow.Actor.Should().NotBeNull();
        follow.Actor!.Value.IsLink.Should().BeTrue();
        follow
            .Actor.Value.Link!.Value.Href.Should()
            .Be(new Uri("https://pixelfed.social/users/charlie"));
        follow.Objekt.Should().NotBeNull();
        follow.Objekt!.Value.IsLink.Should().BeTrue();
        follow.Objekt.Value.Link!.Value.Href.Should().Be(new Uri("https://mastodon.social/@alice"));
    }

    [TestMethod]
    public void LikeActivity_ShouldDeserialize()
    {
        // Arrange
        var json = File.ReadAllText("TestData/like.json");

        // Act
        var activity = JsonSerializer.Deserialize<ActivityObject>(json);

        // Assert
        activity.Should().NotBeNull();
        activity.Should().BeOfType<LikeActivity>();

        var like = (LikeActivity)activity!;
        like.Id.Should().Be(new Uri("https://example.com/likes/abc"));
        like.Actor.Should().NotBeNull();
        like.Actor!.Value.IsLink.Should().BeTrue();
        like.Actor.Value.Link!.Value.Href.Should().Be(new Uri("https://example.com/users/dave"));
        like.Objekt.Should().NotBeNull();
        like.Objekt!.Value.IsLink.Should().BeTrue();
        like.Objekt.Value.Link!.Value.Href.Should()
            .Be(new Uri("https://mastodon.social/users/alice/statuses/12345"));
    }

    [TestMethod]
    public void CreateActivity_ShouldSerialize_WithInlineNote()
    {
        // Arrange
        var note = new Note
        {
            Id = new Uri("https://example.com/notes/123"),
            Content = "<p>Test content</p>",
            AttributedTo = new LinkOr<Person>(new Uri("https://example.com/users/alice")),
            Published = DateTime.Parse("2025-10-22T12:00:00Z").ToUniversalTime(),
        };

        var create = new CreateActivity
        {
            Id = new Uri("https://example.com/activities/123"),
            Actor = new LinkOr<ActivityObject>(new Uri("https://example.com/users/alice")),
            Objekt = new LinkOr<ActivityObject>(note),
            Published = DateTime.Parse("2025-10-22T12:00:00Z").ToUniversalTime(),
            To = new OneOrMany<LinkOr<ActivityObject>>(
                new LinkOr<ActivityObject>(new Uri("https://www.w3.org/ns/activitystreams#Public"))
            ),
        };

        // Act
        var json = JsonSerializer.Serialize<ActivityObject>(create);
        var deserialized = JsonSerializer.Deserialize<ActivityObject>(json);

        // Assert
        deserialized.Should().NotBeNull();
        deserialized.Should().BeOfType<CreateActivity>();

        var deserializedCreate = (CreateActivity)deserialized!;
        deserializedCreate.Objekt!.Value.IsValue.Should().BeTrue();
        deserializedCreate.Objekt.Value.Value.Should().BeOfType<Note>();

        var deserializedNote = (Note)deserializedCreate.Objekt.Value.Value!;
        deserializedNote.Content.Should().Be(note.Content);
    }

    [TestMethod]
    public void FollowActivity_ShouldSerialize()
    {
        // Arrange
        var follow = new FollowActivity
        {
            Id = new Uri("https://example.com/follows/abc"),
            Actor = new LinkOr<ActivityObject>(new Uri("https://example.com/users/alice")),
            Objekt = new LinkOr<ActivityObject>(new Uri("https://social.example/users/bob")),
            Published = DateTime.Parse("2025-10-22T13:00:00Z").ToUniversalTime(),
        };

        // Act
        var json = JsonSerializer.Serialize<ActivityObject>(follow);
        var deserialized = JsonSerializer.Deserialize<ActivityObject>(json);

        // Assert
        deserialized.Should().NotBeNull();
        deserialized.Should().BeOfType<FollowActivity>();

        var deserializedFollow = (FollowActivity)deserialized!;
        deserializedFollow.Id.Should().Be(follow.Id);
        deserializedFollow.Actor.Should().NotBeNull();
        deserializedFollow.Actor!.Value.IsLink.Should().BeTrue();
        deserializedFollow
            .Actor.Value.Link!.Value.Href.Should()
            .Be(new Uri("https://example.com/users/alice"));
    }

    [TestMethod]
    public void LikeActivity_ShouldSerialize()
    {
        // Arrange
        var like = new LikeActivity
        {
            Id = new Uri("https://example.com/likes/xyz"),
            Actor = new LinkOr<ActivityObject>(new Uri("https://example.com/users/charlie")),
            Objekt = new LinkOr<ActivityObject>(new Uri("https://example.com/notes/456")),
            Published = DateTime.Parse("2025-10-22T14:00:00Z").ToUniversalTime(),
        };

        // Act
        var json = JsonSerializer.Serialize<ActivityObject>(like);
        var deserialized = JsonSerializer.Deserialize<ActivityObject>(json);

        // Assert
        deserialized.Should().NotBeNull();
        deserialized.Should().BeOfType<LikeActivity>();

        var deserializedLike = (LikeActivity)deserialized!;
        deserializedLike.Id.Should().Be(like.Id);
        deserializedLike.Actor.Should().NotBeNull();
        deserializedLike.Actor!.Value.IsLink.Should().BeTrue();
        deserializedLike
            .Actor.Value.Link!.Value.Href.Should()
            .Be(new Uri("https://example.com/users/charlie"));
    }

    [TestMethod]
    public void Activity_WithMultipleRecipients_ShouldHandleTo()
    {
        // Arrange
        var json = File.ReadAllText("TestData/create-note.json");

        // Act
        var activity = JsonSerializer.Deserialize<ActivityObject>(json);
        var create = (CreateActivity)activity!;

        // Assert
        create.To.Should().NotBeNull();
        create.To.Should().HaveCount(1);
        create.To![0].IsLink.Should().BeTrue();
        create
            .To[0]
            .Link!.Value.Href.Should()
            .Be(new Uri("https://www.w3.org/ns/activitystreams#Public"));
    }

    [TestMethod]
    public void Activity_WithMultipleRecipients_ShouldHandleCc()
    {
        // Arrange
        var json = File.ReadAllText("TestData/create-note.json");

        // Act
        var activity = JsonSerializer.Deserialize<ActivityObject>(json);
        var create = (CreateActivity)activity!;

        // Assert
        create.Cc.Should().NotBeNull();
        create.Cc.Should().HaveCount(1);
        create.Cc![0].IsLink.Should().BeTrue();
        create
            .Cc[0]
            .Link!.Value.Href.Should()
            .Be(new Uri("https://mastodon.social/users/alice/followers"));
    }
}
