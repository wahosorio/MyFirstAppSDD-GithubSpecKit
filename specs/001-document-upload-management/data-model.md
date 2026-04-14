# Data Model: Document Upload and Management

## Document

Represents an uploaded file and its metadata.

- **DocumentId**: Integer primary key
- **Title**: Required, max length 255
- **Description**: Optional, max length 2000
- **Category**: Required, text value (e.g. Project Documents, Team Resources, Personal Files, Reports, Presentations, Other)
- **ProjectId**: Optional foreign key to `Project`
- **UploaderId**: Required foreign key to `User`
- **UploadDate**: Required timestamp
- **FileName**: Required, original filename for display purposes, max length 255
- **FilePath**: Required, secure server-side path or storage key, max length 2000
- **FileType**: Required, MIME type, max length 255
- **FileSize**: Required, bytes
- **IsPreviewable**: Computed or derived from `FileType` for preview-capable documents
- **Tags**: Optional text store or normalized tag collection
- **UpdatedDate**: Timestamp when metadata or file content is replaced

### Relationships

- `Document` → `User` (Uploader)
- `Document` → `Project` (Optional project association)
- `Document` → `DocumentShare` (Many share records)
- `Document` → `DocumentActivity` (Many audit events)
- `Document` → `DocumentTaskAttachment` (Optional task associations)

## DocumentShare

Tracks explicit document share grants.

- **DocumentShareId**: Integer primary key
- **DocumentId**: Required foreign key to `Document`
- **RecipientUserId**: Optional foreign key to `User`
- **RecipientTeam**: Optional text field for a named team or role share
- **SharedByUserId**: Required foreign key to `User`
- **SharedDate**: Required timestamp
- **AccessType**: Text or enum indicating `View`, `Download`, `Edit` (initially `View`/`Download`)

### Notes

- Sharing creates explicit access independent of project membership.
- The effective access check will consider project permission plus share grants.

## DocumentActivity

Records document-related audit events.

- **DocumentActivityId**: Integer primary key
- **DocumentId**: Required foreign key to `Document`
- **UserId**: Required foreign key to `User`
- **ActivityType**: Enum or text (`Upload`, `Download`, `Delete`, `Replace`, `Share`, `Preview`, `MetadataUpdate`)
- **ActivityDate**: Timestamp
- **Details**: Optional text for additional context

## DocumentTaskAttachment

Supports task-level attachments without duplicating document metadata.

- **DocumentTaskAttachmentId**: Integer primary key
- **DocumentId**: Required foreign key to `Document`
- **TaskId**: Required foreign key to `TaskItem`
- **AttachedByUserId**: Required foreign key to `User`
- **AttachedDate**: Timestamp

### Notes

- A task attachment also implies project association through the linked task.
- This entity enables documents to be attached to tasks without coupling the document to only one task.

## Validation Rules

- `Title` is required and must be meaningful.
- `Category` is required and must come from the predefined category set.
- `FileSize` must be <= 25 MB.
- `FileType` must be a supported MIME type for upload and preview.
- `FilePath` must be generated server-side as a secure GUID-based path.
- `DocumentShare` records must include either a `RecipientUserId` or `RecipientTeam`.
- Document deletion must remove the file from disk and delete related shares, attachments, and audit events.
