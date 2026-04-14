# Research: Document Upload and Management

## Decision: Use local filesystem storage with a storage service abstraction

- Local filesystem storage is the correct choice for this training-focused app because the repository constraints explicitly require offline capability and no external cloud dependencies.
- The feature will store document metadata in SQLite via Entity Framework Core and store file bytes on disk outside `wwwroot`.
- A small `IFileStorageService` interface will be introduced so business logic does not depend on file path details and can be migrated later to Azure Blob Storage or another provider.

## Rationale

- The existing app already uses ASP.NET Core server-side Blazor and SQLite, so adding local file storage is low-risk and consistent with the repository's architecture.
- Storing files outside `wwwroot` protects documents from direct URL access and enables controlled download/preview endpoints.
- The abstraction keeps the implementation simple now while preserving future cloud migration paths.

## Alternatives considered

- Store document bytes directly in the SQLite database.
  - Rejected: storing large files in SQLite adds overhead, complicates migration, and is less clear for learners.
- Store uploaded files under `wwwroot`.
  - Rejected: this would create public access control issues and violate secure-by-design guidance.
- Use a full cloud storage provider now.
  - Rejected: the feature must work offline without cloud services and should remain a local training scenario.

## Decision: Share access should grant explicit permissions

- Documents shared with a specific user or team will grant that recipient explicit access even if they are not already a member of the associated project.
- This simplifies the sharing model and supports the user story of direct document collaboration across project boundaries.

## Rationale

- Existing role-based project membership is already used for access control, but sharing must override that boundary for intended collaboration.
- Explicit shares will be recorded in a dedicated entity so access checks can query both project membership and share grants.

## Decision: Use a mixed browse/search UI in the existing Blazor pages

- The feature will add a "My Documents" view, a project-specific documents section, and a shared documents area.
- Search will be implemented on the server side using EF Core query filters against title, description, tags, uploader, and project.
- Common preview types (PDF, JPEG, PNG) will be rendered via a secured download/preview endpoint.

## Rationale

- The current app is server-side Blazor and already supports page navigation and query-based filtering in the UI.
- Implementing browse/search within the same app avoids introducing a separate SPA framework.

## Decision: Add task attachment support using a document-task association

- Documents can optionally be attached to tasks and still be associated with the task's project.
- A dedicated `DocumentTaskAttachment` relationship (or equivalent) provides flexibility without coupling document ownership to a single task.

## Rationale

- This design keeps document metadata independent from task attachment semantics and preserves reusability for documents attached to multiple tasks in the future.

## Summary of research findings

- Use ASP.NET Core Blazor Server + EF Core SQLite for metadata.
- Store document files locally outside `wwwroot` with a `IFileStorageService` abstraction.
- Share access grants explicit permission, independent of project membership.
- Implement document discovery in existing pages with search and filtering.
- Keep the feature simple, secure, and consistent with the training-focused app.
