# Deep Research, Runtime Verification, and Reconstruction Design

## Purpose
Extend the frozen `Auto Clicker by Max(1).exe` knowledge base from static reverse-engineering into a reproducible donor-data repository that another engineer or AI can use without repeating the initial binary analysis.

## Frozen sample
All conclusions remain bound to SHA-256 `1aa4e0e2ea46e7460641fdb05c2a5f409c63505312179549ef9fc7a14633d35e`.

## Deliverables
1. Preserve the complete static dump: all IL parts, selected IL, metadata, method/type indexes, full call graph, strings, resources, parser, PE headers and manifest.
2. Add a Windows runtime probe that reproduces the sample's `HWND -> child HWND -> WM_MOUSEMOVE -> DOWN -> hold -> UP` transport and writes structured JSONL evidence while proving whether the global cursor moved.
3. Add a runtime test matrix. Runtime rows start as `PENDING`; they become `VERIFIED` only when a matching probe log is committed.
4. Reconstruct the high-value classes as readable C# reference source. Reconstruction is evidence-oriented, not claimed original source and not required to compile as a standalone application.
5. Add design lessons and a Thần Long mapping that explicitly separates semantic/internal game actions from HWND-message fallback.
6. Add a validator that checks frozen hash/counts, full call-edge count, required reconstruction/runtime files, and JSON schemas.

## Evidence policy
- `VERIFIED_STATIC`: direct PE/CLR metadata, IL, embedded resources, or deterministic derivation from those inputs.
- `VERIFIED_RUNTIME`: only a committed runtime log with test case, target information, before/after cursor state and observed outcome.
- `RECONSTRUCTED`: C# rewritten from IL/metadata for readability; behavior must cite method tokens/RVAs.
- `HYPOTHESIS`: target-specific expectations not yet exercised.

## Runtime architecture
`BackgroundClickProbe.ps1` resolves a top-level window by PID/name/title, validates it, takes client coordinates, probes a child with `RealChildWindowFromPoint`, transforms parent-client -> screen -> child-client when needed, packs `LPARAM`, posts move/down/up messages, and records cursor/window/DPI state before and after. It never labels application behavior successful without an explicit operator observation/result field.

## Reconstruction scope
High-value donor classes: `NativeMethods`, `ClickEngine`, `Advanced.ActionExecutor`, `Advanced.MacroRunner`, `Advanced.MacroStorage`, `KeyboardSimulator`, `MouseMovementSimulator`, `CoordinatePicker`, `OverlayForm`, `WindowTracker`, and `UnfocusClickFilter`.

## Non-goals
- No claim that PostMessage works for every game or DirectX/Unity target.
- No bypass of anti-cheat, privilege boundaries, or protected input paths.
- No claim that reconstructed source is byte-identical/original source.
