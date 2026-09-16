# Deep Research Runtime Reconstruction Implementation Plan

> **For agentic workers:** REQUIRED SUB-SKILL: Use superpowers:subagent-driven-development (recommended) or superpowers:executing-plans to implement this plan task-by-task. Steps use checkbox (`- [ ]`) syntax for tracking.

**Goal:** Convert the current static KB into a reproducible donor-data repository with complete raw evidence preservation, a Windows runtime probe/matrix, and readable C# reconstructions of the core classes.

**Architecture:** Keep immutable evidence separate from interpretation. Runtime evidence is produced by a standalone PowerShell probe and committed as JSONL; reconstructed C# lives under `reconstructed/` with token/RVA provenance. A validator ties the frozen EXE fingerprint, metadata counts, call graph, schemas and required files together.

**Tech Stack:** Python 3, PowerShell 5+/Windows user32.dll P/Invoke, C# reference source, GitHub text repository.

**Spec:** `docs/superpowers/specs/2026-09-16-deep-research-runtime-reconstruction-design.md`

## Global Constraints
- Frozen sample SHA-256: `1aa4e0e2ea46e7460641fdb05c2a5f409c63505312179549ef9fc7a14633d35e`.
- Runtime results remain `PENDING` until a matching Windows probe log exists.
- Reconstructed C# is labeled `RECONSTRUCTED`, never represented as original source.
- Preserve full static data without replacing it by summaries.

---

### Task 1: Complete static evidence package
**Files:** preserve `raw/il/*`, `raw/strings_*`, `database/METADATA.json`, `database/METHOD_INDEX.tsv`, `database/call_graph/*`; create `raw/FULL_DUMP_MANIFEST.tsv`.

- [x] Inventory every bulk file with path, size and SHA-256.
- [x] Verify method/type/field counts and 12,257 call edges.
- [x] Verify all four full IL parts are present and non-empty.
- [x] Record the manifest.

### Task 2: Runtime probe and matrix
**Files:** create `runtime/README.md`, `runtime/RUNTIME_TEST_MATRIX.md`, `runtime/RESULT_SCHEMA.json`, `runtime/windows/BackgroundClickProbe.ps1`, `runtime/windows/Run-Matrix.ps1`.

- [x] Implement target resolution, HWND validation, child-HWND retargeting, coordinate transforms and PostMessage sequence.
- [x] Log before/after cursor/window/DPI state to JSONL.
- [x] Define the test matrix for foreground/background/covered/minimized/resize/DPI/multi-monitor/recreated-HWND/button variants.
- [x] Keep all runtime outcomes PENDING unless a real Windows result is supplied.

### Task 3: Reconstructed reference source
**Files:** create `reconstructed/*.Reconstructed.cs`, `reconstructed/README.md`, and per-class evidence extracts.

- [x] Extract IL sections for all methods in the 11 high-value classes.
- [x] Reconstruct the behavioral core in readable C# with method token/RVA annotations.
- [x] Mark all source as reconstructed and identify static evidence vs interpretation.

### Task 4: Engineering lessons and schemas
**Files:** create `analysis/12_THREADING_TIMERS.md`, `analysis/13_CONFIG_AND_ACTION_SCHEMA.md`, `database/ACTION_SCHEMA.json`, `database/CONFIG_SCHEMA.json`, `DESIGN_LESSONS.md`, `applications/THAN_LONG_MAPPING.md`.

- [x] Document worker-thread model, fixed sleeps and cancellation semantics.
- [x] Formalize reusable action/config fields from embedded templates and IL.
- [x] Separate reusable HWND transport from game-semantic action design.

### Task 5: Validation
**Files:** create `tools/validate_kb.py`.

- [x] Validate frozen EXE hash when present.
- [x] Validate metadata and call-edge counts.
- [x] Validate bulk-file hashes, JSON schemas and required reconstruction/runtime artifacts.
- [x] Run the validator and require PASS before integration.
