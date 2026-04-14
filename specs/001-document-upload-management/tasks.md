# Tasks: Document Upload and Management

**Input**: Design documents from `/specs/001-document-upload-management/`
**Prerequisites**: plan.md, spec.md, research.md, data-model.md, contracts/

## Phase 1: Setup (Shared Infrastructure)

**Purpose**: Establish the feature structure and core integration points in the existing Blazor app.

- [ ] T001 [P] Create `ContosoDashboard/Models/Document.cs` for document metadata storage
- [ ] T002 [P] Create `ContosoDashboard/Models/DocumentShare.cs` for explicit share grants
- [ ] T003 [P] Create `ContosoDashboard/Models/DocumentActivity.cs` for audit logging
- [ ] T004 [P] Create `ContosoDashboard/Models/DocumentTaskAttachment.cs` for task attachments
- [ ] T005 [P] Add `DbSet<Document>`, `DbSet<DocumentShare>`, `DbSet<DocumentActivity>`, and `DbSet<DocumentTaskAttachment>` to `ContosoDashboard/Data/ApplicationDbContext.cs`
- [ ] T006 [P] Add configuration keys for local upload storage and Azure queue names to `ContosoDashboard/appsettings.json`
- [ ] T007 [P] Create `ContosoDashboard/Services/IFileStorageService.cs` and `ContosoDashboard/Services/LocalFileStorageService.cs`
- [ ] T008 [P] Create `ContosoDashboard/Services/IVirusScanQueueService.cs` and `ContosoDashboard/Services/AzureQueueVirusScanService.cs`
- [ ] T009 [P] Register `IFileStorageService`, `IVirusScanQueueService`, and new document services in `ContosoDashboard/Program.cs`
- [ ] T010 [P] Create `ContosoDashboard/Services/IDocumentService.cs` and `ContosoDashboard/Services/DocumentService.cs`
- [ ] T011 [P] Add DTO classes in `ContosoDashboard/Services/DocumentDtos.cs`: `DocumentDto`, `DocumentUploadRequest`, `DocumentMetadataUpdate`, `DocumentShareRequest`, `DocumentQueryParameters`
- [ ] T012 [P] Create `AzureFunctions/DocumentVirusScanFunction.cs` with an Azure Queue Storage trigger that consumes scan requests and updates document scan status

---

## Phase 2: Foundational (Blocking Prerequisites)

**Purpose**: Build the shared document handling infrastructure that every story depends on.

- [ ] T013 Implement secure file path generation and local filesystem persistence in `ContosoDashboard/Services/LocalFileStorageService.cs`
- [ ] T014 Implement document metadata persistence, file save transaction sequence, and upload queue enqueue in `ContosoDashboard/Services/DocumentService.cs`
- [ ] T015 Implement document access checks in `ContosoDashboard/Services/DocumentService.cs` that consider project membership and explicit share grants
- [ ] T016 Implement file validation logic for supported MIME types and 25 MB file size limit in `ContosoDashboard/Services/DocumentService.cs`
- [ ] T017 Implement audit logging for uploads, downloads, deletions, replacements, and shares in `ContosoDashboard/Services/DocumentService.cs`
- [ ] T018 Add EF Core entity relationships and indexes for document-related tables in `ContosoDashboard/Data/ApplicationDbContext.cs`
- [ ] T019 Create secure file download and preview endpoints in `ContosoDashboard/Controllers/DocumentController.cs`
- [ ] T020 Add shared navigation links for documents in `ContosoDashboard/Shared/NavMenu.razor`

---

## Phase 3: User Story 1 - Upload and organize documents (Priority: P1) 🎯 MVP

**Goal**: Enable users to upload supported documents, enter required metadata, save them securely, and trigger background virus scanning.

**Independent Test**: Upload a supported file with title and category; verify the document record appears in the personal document list and the upload progress/completion workflow behaves correctly.

### Implementation for User Story 1

- [ ] T021 Create the upload page `ContosoDashboard/Pages/DocumentUpload.razor` with file selection, title, category, project association, tags, description, and submit controls
- [ ] T022 Add client-side validation and upload progress UI to `ContosoDashboard/Pages/DocumentUpload.razor`
- [ ] T023 Implement the upload flow in `ContosoDashboard/Services/DocumentService.cs` to save files locally, persist metadata, and enqueue a virus scan message
- [ ] T024 Create the personal document list page `ContosoDashboard/Pages/MyDocuments.razor` showing title, category, upload date, file size, and associated project
- [ ] T025 Add the upload result and validation error display logic to `ContosoDashboard/Pages/MyDocuments.razor`
- [ ] T026 Update `ContosoDashboard/Pages/Index.razor` to include a "Recent Documents" widget showing the last five documents uploaded by the user

**Checkpoint**: User Story 1 should allow an employee to upload and view their documents independently.

---

## Phase 4: User Story 2 - Browse, search, and preview documents (Priority: P2)

**Goal**: Provide document discovery and secure preview/download capabilities for authorized documents.

**Independent Test**: Filter or search documents by category, project, date range, or metadata and preview/download a matching document.

### Implementation for User Story 2

- [ ] T027 Implement document query support in `ContosoDashboard/Services/DocumentService.cs` with search by title, description, tags, uploader name, and project
- [ ] T028 Create the shared documents and search page `ContosoDashboard/Pages/SharedDocuments.razor` or extend `ContosoDashboard/Pages/MyDocuments.razor` with search and filter controls
- [ ] T029 Implement project-specific document listing in `ContosoDashboard/Pages/ProjectDetails.razor`
- [ ] T030 Implement preview support for PDF and image file types in `ContosoDashboard/Controllers/DocumentController.cs`
- [ ] T031 Implement secure download support in `ContosoDashboard/Controllers/DocumentController.cs`
- [ ] T032 Add sorting and filtering controls to `ContosoDashboard/Pages/MyDocuments.razor`

**Checkpoint**: User Story 2 should let users discover and preview/download documents they are authorized to access.

---

## Phase 5: User Story 3 - Manage access, metadata, and project attachments (Priority: P3)

**Goal**: Allow document owners and project managers to edit metadata, replace files, delete documents, share documents, and attach documents to tasks.

**Independent Test**: Edit metadata, replace a file, delete a document, share it with another user, and attach it from a task detail view.

### Implementation for User Story 3

- [ ] T033 Create the edit metadata page `ContosoDashboard/Pages/EditDocument.razor`
- [ ] T034 Implement metadata update logic in `ContosoDashboard/Services/DocumentService.cs`
- [ ] T035 Implement file replacement logic in `ContosoDashboard/Services/DocumentService.cs` with secure file path updates and old file cleanup
- [ ] T036 Implement document deletion in `ContosoDashboard/Services/DocumentService.cs` and clean up related file storage, shares, attachments, and audit records
- [ ] T037 Create the sharing UI in `ContosoDashboard/Pages/SharedDocuments.razor` or `ContosoDashboard/Pages/DocumentUpload.razor` for selecting recipient users or teams
- [ ] T038 Implement share grant processing in `ContosoDashboard/Services/DocumentService.cs`
- [ ] T039 Add a "Shared with Me" section to `ContosoDashboard/Pages/SharedDocuments.razor`
- [ ] T040 Update `ContosoDashboard/Pages/Tasks.razor` to allow attaching existing documents or uploading a new document from the task detail page
- [ ] T041 Implement task attachment persistence in `ContosoDashboard/Services/DocumentService.cs` or a dedicated attachment handler
- [ ] T042 Implement recipient notifications for shared documents in `ContosoDashboard/Services/NotificationService.cs` and `ContosoDashboard/Models/Notification.cs` usage

**Checkpoint**: User Story 3 should support full document management, sharing, and attachment independently.

---

## Phase 6: Polish & Cross-Cutting Concerns

**Purpose**: Final quality, security, documentation, and training validation work.

- [ ] T043 [P] Add or update feature documentation in `specs/001-document-upload-management/quickstart.md`
- [ ] T044 [P] Update `ContosoDashboard/README.md` or repository README with document feature setup notes and local storage guidance
- [ ] T045 [P] Add or update authorization comments in `ContosoDashboard/Controllers/DocumentController.cs` and `ContosoDashboard/Services/DocumentService.cs`
- [ ] T046 [P] Add a local fallback mode for Azure queue scanning to `ContosoDashboard/Services/AzureQueueVirusScanService.cs`
- [ ] T047 [P] Validate that upload files are stored outside `wwwroot` and cannot be served directly without authorization
- [ ] T048 [P] Review and clean up new model and service code in `ContosoDashboard/Models/`, `ContosoDashboard/Services/`, and `ContosoDashboard/Controllers/`
- [ ] T049 [P] Ensure navigation updates in `ContosoDashboard/Shared/NavMenu.razor` include document feature links
- [ ] T050 [P] Run manual scenario validation from `specs/001-document-upload-management/quickstart.md`

---

## Dependencies & Execution Order

### Phase Dependencies

- **Setup (Phase 1)**: No dependencies - can start immediately
- **Foundational (Phase 2)**: Depends on Setup completion - BLOCKS all user stories
- **User Stories (Phase 3+)**: All depend on Foundational phase completion
- **Polish (Phase 6)**: Depends on all desired user stories being complete

### User Story Dependencies

- **User Story 1 (P1)**: Can start after Foundational (Phase 2)
- **User Story 2 (P2)**: Can start after Foundational (Phase 2)
- **User Story 3 (P3)**: Can start after Foundational (Phase 2)

### Within Each User Story

- Models before services
- Services before controllers/pages
- Upload and storage before search/preview
- Core implementation before task integration

### Parallel Opportunities

- All Setup tasks marked [P] can be done in parallel
- All Foundational tasks marked [P] can be done in parallel
- User stories can proceed in parallel after Phase 2 is complete
- Multiple service and model tasks within a story can be parallelized when they touch separate files

---

## Implementation Strategy

### MVP First (User Story 1 Only)

1. Complete Phase 1: Setup
2. Complete Phase 2: Foundational
3. Complete Phase 3: User Story 1
4. Stop and validate User Story 1 independently
5. Continue with User Story 2 and User Story 3 once the upload and storage flow is stable

### Incremental Delivery

- Deliver upload + metadata storage first
- Add browse/search/preview next
- Add edit/share/delete and task attachments last
- Keep each story independently testable before moving on
