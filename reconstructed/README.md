# Reconstructed C# reference source

These files are a **behavioral reconstruction from CLR metadata + IL**, not the author's original source and not a promise of byte-identical decompilation. Their purpose is to make the donor architecture readable while retaining the exact IL beside them in `reconstructed/evidence/*.il.txt`.

Each important method is annotated with its MethodDef token/RVA. When readable C# and IL appear to disagree, the IL is authoritative.

Coverage policy:
- click/window/input paths are reconstructed at statement-level where IL is straightforward;
- large UI painting and JSON string-builder bodies are represented by their recovered public behavior/schema, while their complete IL remains in the evidence file;
- compiler-generated display classes/lambdas are normalized into ordinary C# control flow when safe;
- target-specific runtime effects are not inferred from static code.

High-value classes included: `NativeMethods`, `ClickEngine`, `ActionExecutor`, `MacroRunner`, `MacroStorage`, `KeyboardSimulator`, `MouseMovementSimulator`, `CoordinatePicker`, `OverlayForm`, `WindowTracker`, `UnfocusClickFilter`.
