# Quickstart: Document Upload and Management Feature

## Run the application

1. Open a terminal at the repository root.
2. Run the ContosoDashboard app:
   ```powershell
   dotnet run --project ContosoDashboard\ContosoDashboard.csproj
   ```
3. Open the browser at the app URL shown in the terminal (usually `https://localhost:5001`).

## Verify the feature setup

1. Confirm the current branch is `001-document-upload-management`.
2. Ensure the local SQLite database is initialized by the app on startup.
3. Confirm the upload directory is available and configured outside `wwwroot`.

## Validate the feature manually

### Upload a document

- Log in using a sample training user account.
- Navigate to the new document upload page.
- Select a supported file (PDF, Word, Excel, PowerPoint, text, JPEG, PNG).
- Enter a document title and category, optionally choose a project, add tags, and submit.
- Verify the upload succeeds and the document appears in "My Documents." 

### Browse and search

- Filter by category, project, and date range.
- Search by title, description, tags, uploader name, or project.
- Confirm only authorized documents appear in results.

### Preview and download

- Open a supported previewable document (PDF or image).
- Confirm the document displays in the browser without downloading.
- Download a document and verify the file is returned securely.

### Manage metadata and sharing

- Edit document metadata and confirm changes appear immediately.
- Replace an uploaded file and verify the previous file is replaced.
- Share a document with another user and confirm the recipient receives an in-app notification.
- Confirm shared documents appear in the recipient's "Shared with Me" section.

### Task integration

- Open a task detail page.
- Attach a document from the task page.
- Verify the attached document is associated with that task and its project.

## Notes for reviewers

- The feature is intended as a training example; security and migration notes should remain visible in code comments.
- Use the existing authentication model and role-based authorization policies from `Program.cs`.
- Ensure document access checks validate both project membership and explicit document shares.
