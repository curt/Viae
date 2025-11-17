# Viae

*Latin for "ways" or "paths"*

## Vision

For over 20 years, the dream has been to build a web application that highlights the relationships between **people**, **places**, **images**, and **words** - and allows its owner and users to share those connections with others. Viae is the project that finally realizes this vision.

Viae is a geo-centric, federated platform that puts maps front and center, enabling users to explore and share the intricate web of relationships between content, locations, and communities. It's not just about isolated posts or photos - it's about the meaningful connections between them.

## What Makes Viae Different

### Geographic at Its Core
Maps aren't an afterthought - they're the foundation. Every piece of content exists in spatial context, allowing you to discover how places, journeys, and experiences interconnect.

### Federated & Open
Built on ActivityPub, Viae participates in the federated social web. Share your journeys, places, and images with the wider fediverse while maintaining ownership of your content.

### Community-Driven
Unlike traditional single-author blogs, Viae empowers users with meaningful privileges:
- Comment on and react to content
- Create and share their own ActivityPub objects
- Build connections within communities (collegia)
- Leave testimonies (reviews) about places

### Journey-Focused
Whether you're documenting a walk across a city or a multi-day trek, Viae treats journeys as first-class citizens:
- Import and visualize GPS traces
- Create check-ins (vestigia) along your path
- Connect places (loca) to routes (itineraria)
- Share photo galleries (tabulae) with geographic context

## The Story Behind Viae

Viae is a rewrite and extension of **Klaxon**, a Phoenix Framework blog completed about two years ago. While building with Elixir was enjoyable, the desire to work in a familiar object-oriented paradigm (C# with 20+ years of experience) combined with the need for significant new functionality made a fresh start the right choice.

Klaxon was server-side only with limited blog-owner privileges. Viae transforms this into a modern, interactive platform with:
- Comprehensive user authentication (AWS Cognito)
- Full ActivityPub integration with HTTP signature verification
- Rich media support with cloud storage
- Spatial queries and geographic discovery

## Technical Overview

### Architecture

**Back-End**: .NET 10 with Clean Architecture
- Domain-driven design with database-agnostic models
- Three distinct presentation layers:
  - **Viae.Presentation.Mvc**: Public-facing site with Fluid templates
  - **Viae.Presentation.Api**: Authenticated API
  - **Viae.Presentation.ActivityPub**: Federated inbox and collections
- Entity Framework Core with Fluent API (code-first migrations)
- Comprehensive unit testing

**Data**:
- PostgreSQL with PostGIS extensions
- Spatial queries for distance-based searches
- Optimized for geographic operations

**Deployment**: Docker Compose stack for simplified orchestration

### Integration Plans
- **AWS S3**: Photo storage with automated resizing
- **Overpass API**: Discover points of interest near places
- **ActivityPub**: Two-way federation with signature verification

## A Note on Naming

Throughout Viae, you'll encounter Latin terminology for routes and concepts. This isn't just aesthetic - it creates a cohesive metaphor around journeys and connections while distinguishing the project from typical blog software:

- **folios** (leaves/pages) - Posts and articles
- **loca** (places) - Geographic locations and landmarks
- **itineraria** (routes) - GPS traces and journeys
- **vestigia** (footprints) - Check-ins marking presence
- **tabulae** (tablets) - Image galleries
- **testimonia** (testimonies) - Place reviews
- **personae** (persons) - User identities

The code, comments, and APIs remain in English for collaboration and maintainability.

## Current Status

Viae is in active proof-of-concept development (approximately 2 weeks in). Core authentication, architecture, and foundational features are in place. Immediate priorities include:

1. Standing up a public development server for ActivityPub testing
2. Implementing check-in functionality
3. Photo import and resize pipeline
4. GPS trace import and visualization

## The Road Ahead

This is the beginning of a journey to create something unique: a platform where geography, narrative, community, and federation converge. Where your walks become stories, your photos become memories anchored in place, and your experiences become part of a larger shared tapestry.

Viae isn't just about building software - it's about finally realizing a two-decade vision of showing how everything connects.

---

*More documentation and code will be published as development progresses.*
