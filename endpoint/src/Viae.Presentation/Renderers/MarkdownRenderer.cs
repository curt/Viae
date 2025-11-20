// Copyright © 2025 Curt Gilman
// SPDX-License-Identifier: AGPL-3.0-only
// Viae: A geo-centric, journey-focused, federated blog platform

using Markdig;

namespace Viae.Presentation.Renderers;

public sealed class MarkdownRenderer
{
    private static readonly Lazy<MarkdownRenderer> _instance = new(() => new MarkdownRenderer());

    private readonly MarkdownPipeline _pipeline;

    private MarkdownRenderer() =>
        _pipeline = new MarkdownPipelineBuilder()
            .UseAdvancedExtensions()
            .UseSmartyPants()
            .DisableHtml()
            .Build();

    public static MarkdownRenderer Instance => _instance.Value;

    public string RenderToHtml(string markdown)
    {
        return string.IsNullOrWhiteSpace(markdown)
            ? string.Empty
            : Markdown.ToHtml(markdown, _pipeline);
    }
}
