---
name: csharp-governance
description: "Use when refactoring C# code, migrating dynamic Godot patterns to typed C#, enforcing CORE.instructions.md, or running C# code reviews. Triggers: refactor C#, migrate from GDScript, enforce CORE rules, C# naming conventions, type-safe Godot interop, .NET coding conventions."
---

# C# Governance for Wave Tower

## Purpose

Keep project C# code aligned with:
- Workspace rules in .github/instructions/CORE.instructions.md
- Official C#/.NET conventions for readability, maintainability, and type safety

This skill is complementary to CORE instructions:
- CORE = always-on file-level guardrails for .cs files
- This skill = workflow, checklist, batching strategy, and review reporting

## Use When

- You are refactoring C# files and need consistent enforcement.
- You are migrating gameplay logic away from dynamic Godot patterns.
- You are doing a code review focused on convention, typing, and API clarity.
- You need an incremental, build-validated migration plan by micro-batches.

## Do Not Use When

- The task is unrelated to C# source files.
- The task is only art/audio/content authoring with no code changes.
- The task is project scaffolding unrelated to rule enforcement.

## Required Inputs

- Target scope (single file, folder, or whole project)
- Current build command and expected green baseline
- Whether the task is migration, review-only, or refactor+fix
- Preferred micro-batch size (default: 3-10 files)

## How To Activate This Skill

This skill can be activated in two ways:

1. Manual activation
- Select/invoke this skill explicitly in chat.

2. Intent-based activation
- Ask for C# migration/refactor/review using prompts such as:
  - "aplica CORE en estos archivos C#"
  - "migra este modulo de GDScript a C# tipado"
  - "review de convencion C# y .NET"
  - "refactor this folder to typed C# and CORE rules"
  - "enforce C# naming and type-safety conventions"

## Enforcement Baseline

Always apply CORE first, then apply .NET conventions where CORE is silent.

### CORE-Driven Checks

- Prefer typed C# APIs over dynamic Godot patterns.
- Prefer C# collections:
  - Use Dictionary<TKey, TValue>
  - Avoid Godot.Collections.Array in new/refactored C#
  - Prefer List<T> over T[] for mutable/list-like data
- Avoid Variant except strict Godot API boundary interop.
- Avoid regular this. member access unless language-required.
- Naming:
  - Public members in PascalCase
  - Private fields in _camelCase
  - Private methods in PascalCase (do not use leading `_`)
  - Method arguments and locals in camelCase
  - Remove legacy parameter/local patterns p_* and *_p
- Remove using Godot; when file does not use Godot APIs.

### Extended .NET Convention Checks

Apply these when they do not conflict with CORE or engine constraints:
- Prefer explicit, intention-revealing names.
- Use readonly for immutable private fields when possible.
- Keep nullability explicit and avoid silent null fallback logic.
- Keep methods small and single-purpose where practical.
- Remove dead code and stale usings.
- Prefer strongly typed calls over reflective/dynamic dispatch.
- Prefer immutable value objects for contextual metadata models (for example `Source`) and create new instances instead of mutating existing ones.
- For asynchronous methods, use `Task`/`Task<T>` return types.
- Avoid `async void` except where the signature is framework-required.
- Prefer C# `async`/`await` orchestration whenever possible.
- When available, propagate `CancellationToken` through async call chains.

## Micro-Batch Workflow

1. Define batch
- Pick a small batch (3-10 files) by folder or dependency boundary.
- Prioritize core/shared systems first, feature folders later.

2. Audit before edit
- Mark violations by rule category (CORE vs extended .NET).
- Identify unsafe changes that may alter gameplay behavior.

3. Refactor incrementally
- Apply naming and typing fixes first.
- Replace dynamic containers/patterns with typed models.
- Keep behavior stable; avoid unrelated rewrites.

4. Validate immediately
- Build after each micro-batch.
- Recommended command:
  - dotnet build "Wave Tower.sln"

5. Report batch outcome
- List changed files.
- List resolved violations.
- List deferred items and rationale.
- Confirm build status.

## Project-Wide Sweep Checklist

Use this sequence for full-project review. Do not skip to a lower phase until the current phase is validated.

### Phase 0 - Baseline

- Run baseline build and confirm green start.
- Record known warnings/errors before edits.
- Confirm target branch and scope with user.

### Phase 1 - Core Foundations

Target folders (high priority):
- core/
- global/
- run_context/

Checklist:
- Remove dynamic patterns from shared models/utilities first.
- Normalize naming (`PascalCase`, `_camelCase`, `camelCase`).
- Replace `Godot.Collections.Array` and non-boundary `Variant` usage.
- Remove unnecessary `using Godot;` directives.
- Build and report.

### Phase 2 - Runtime Orchestration

Target folders (high priority):
- main/
- levels/
- utils/

Checklist:
- Keep orchestration APIs strongly typed across node boundaries.
- Replace reflective/dynamic dispatch in gameplay paths.
- Enforce explicit nullability and avoid silent fallback behavior.
- Convert async flows to typed `Task`/`Task<T>` APIs when signatures can change safely.
- Build and report.

### Phase 3 - Combat and Entities

Target folders:
- enemies/
- towers/
- consumables/
- relics/

Checklist:
- Prioritize behavior-preserving typing changes.
- Standardize container usage (`Dictionary<TKey, TValue>`, `List<T>`).
- Rename legacy `p_*`/`*_p` locals and parameters.
- Build and report.

### Phase 4 - Content Systems

Target folders:
- events/
- ui/

Checklist:
- Keep data contracts typed and naming consistent.
- Avoid introducing Godot-specific containers into project logic.
- Ensure public API names remain stable where required.
- Build and report.

### Phase 5 - Final Consolidation

- Re-run full solution build.
- Review deferred findings and close critical/high items.
- Produce summary of remaining medium/low debt.
- Confirm final status and next candidate batches.

## Review Output Format

For each finding include:
- Severity: critical | high | medium | low
- File and location
- Rule violated (CORE or extended .NET)
- Suggested fix
- Status: pending | fixed | validated

Batch summary must also include:
- Folder phase and batch ID
- Build command and result
- Deferred items with owner/next step

## Activation Examples

Natural-language prompts that should trigger this skill intent:
- "Refactor this folder to follow CORE C# rules"
- "Migrate these scripts from dynamic Godot patterns to typed C#"
- "Review these .cs files for naming and type-safety conventions"
- "Apply C# conventions and validate per micro-batch"

## Exit Criteria

A batch is complete only when:
- All critical/high violations in scope are fixed or explicitly deferred
- Changes are limited to intended scope
- Build passes for the current batch
- Output report is produced with traceable rule mapping
