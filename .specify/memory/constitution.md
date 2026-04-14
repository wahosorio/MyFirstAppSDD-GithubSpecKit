<!--
Sync Impact Report
- Version change: none → 1.0.0
- Modified principles: Placeholder template → Training Transparency; Secure-by-Design Training; Spec-Driven Development; Minimal Complexity; Testable Delivery
- Added sections: Project Constraints; Development Workflow
- Removed sections: placeholder tokens only
- Templates requiring updates: .specify/templates/plan-template.md ✅ unchanged; .specify/templates/spec-template.md ✅ unchanged; .specify/templates/tasks-template.md ✅ unchanged
- Follow-up TODOs: none
-->

# MyFirstAppSDD-GithubSpecKit Constitution

## Core Principles

### I. Training Transparency
All project documentation, README content, and feature notes MUST explicitly state that this repository is a training-focused application, not a production system. This principle protects learners from assuming the implementation is production-grade and preserves the educational intent of the codebase.

### II. Secure-by-Design Training
The repository MUST use secure practices appropriate for learning: simplified or mock implementations are permitted only when isolated, labeled clearly, and accompanied by a production-safe migration note. This keeps the training code honest while still teaching sound security concepts.

### III. Spec-Driven Development
Every feature MUST begin with a written specification containing user scenarios, functional requirements, and measurable success criteria before implementation begins. This ensures work is grounded in user value and makes review, testing, and scope decisions explicit.

### IV. Minimal Complexity
Architecture and implementation MUST favor simplicity and separation of concerns. Avoid additional layers, services, or abstractions unless they directly support the current training objectives, because unnecessary complexity harms maintainability and learner comprehension.

### V. Testable Delivery
Work MUST be organized so each user story can be independently tested, with foundational infrastructure completed before story-specific implementation. This enforces reliable iteration and reduces hidden dependencies across feature work.

## Project Constraints
This repository is designed for local training and demonstration use only. Constraints:
- The application MUST remain offline-first and avoid external production services unless a migration path is explicitly documented.
- Mock authentication, demo data, and simplified services MUST be clearly labeled as training-only implementations.
- Security and privacy considerations MUST be documented even when using simplified solutions.
- Architecture choices MUST prioritize learner transparency over production optimization.

## Development Workflow
- Every feature MUST use `spec.md`, `plan.md`, and `tasks.md` to define scope, design, and execution before code changes begin.
- PR reviews MUST verify compliance with this constitution and the feature's measurable success criteria.
- Architecture decisions and tradeoffs MUST be documented in feature plans to support review and future handoff.
- Tasks MUST be grouped by independent user stories to enable separate implementation and testing.

## Governance
This constitution is the authoritative source for repository workflow, documentation, and feature discipline. Amendments follow these rules:
- Changes MUST be documented in the constitution file and require a version update.
- PR reviewers MUST confirm that proposed amendments improve governance or clarify existing rules.
- Compliance review is required during planning, before implementation, and before merge.
- If an amendment is made, the `Last Amended` date MUST be updated to the current date.

**Version**: 1.0.0 | **Ratified**: 2026-04-14 | **Last Amended**: 2026-04-14
