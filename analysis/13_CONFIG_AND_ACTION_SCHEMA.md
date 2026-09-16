# Configuration and action data model

**Evidence level:** VERIFIED_STATIC for fields/types/defaults directly recovered from CLR metadata, constructors, embedded templates, and `MacroStorage` IL.

## Simple ClickConfig
CLR metadata exposes exactly these backing properties on `ModernAutoClicker.ClickConfig`:

`IntervalMs`, `MouseButton`, `ClickMode`, `Loops`, `JitterPx`, `FreeMouseMode`, `SmoothMouseMove`, `TargetProcessName`, `TargetWindowTitle`, `RelativeToWindow`, `PointsList`.

The constructor itself assigns no explicit defaults, so CLR defaults apply unless the UI fills values later. `database/CONFIG_SCHEMA.json` therefore describes types/meaning, not UI defaults that were not proven at this layer.

## MacroProfile
The constructor (`0x0600003D`, RVA `0x48BC`) sets:

- `Name = "New Macro Profile"`
- `IsCombine = false`
- `LoopCount = 0`
- `RandomIntervalMs = 0`
- `RandomJitterPx = 0`
- `DefaultWindowTitle = ""`
- `DefaultProcessName = ""`
- `DefaultRelativeToWindow = false`
- `Steps = new List<MacroStep>()`

`LoopCount <= 0` is treated as unbounded by the runner.

## MacroStep
Metadata exposes: `Id`, `Name`, `ActionType`, `Enabled` plus legacy alias `IsChecked`, `StartPoint`, `EndPoint`, `HoldMs`, `DelayMs`, `RepeatCount`, `ScrollStep`, `KeyData`, `Note`, `TargetColor`, `ColorHex`, `Tolerance`, `IfTrueStep`, `IfFalseStep`, `WindowTitle`, `ProcessName`, `RelativeToWindow`.

`MacroStep::.ctor` (`0x06000069`, RVA `0x4BF8`) defaults are also recovered directly: `Name="Step"`, `ActionType=0`, `Enabled=true`, empty points, `HoldMs=10`, `DelayMs=240`, `RepeatCount=1`, `ScrollStep=0`, `KeyData="Space"`, `Note=""`, target color RGB `(0,255,0)`, `ColorHex="#00FF00"`, `Tolerance=10`, `IfTrueStep=-2`, `IfFalseStep=0`, empty process/title, and `RelativeToWindow=false`. `Clone` (`0x0600006A`) generates a **fresh GUID** rather than preserving the source Id.

The embedded `Template.All Action.json` contains all 13 recovered action values:

| Value | Action |
|---:|---|
| 0 | LeftClick |
| 1 | RightClick |
| 2 | MiddleClick / middle-scroll depending on ScrollStep |
| 3 | DoubleClick |
| 4 | DragDrop |
| 5 | KeyPress |
| 6 | TypeText |
| 7 | Delay |
| 8 | WaitColor |
| 9 | IfColor |
| 10 | IfColorArea |
| 11 | WaitChange |
| 12 | RunScript |

The executable stores points as integer X/Y fields in JSON and reconstructs `System.Drawing.Point` values when parsing.

## Window targeting fields
`ProcessName + WindowTitle + RelativeToWindow` are semantic targeting fields, not decoration. When relative mode is active and a process is present, a saved point is interpreted in target-client coordinates and resolved against a live HWND at execution time.

## Schemas
- `database/ACTION_SCHEMA.json`: serialized MacroProfile/MacroStep shape.
- `database/CONFIG_SCHEMA.json`: ClickConfig shape.

Schemas are documentation/validation artifacts derived from the frozen sample; they are not claims that the original executable used JSON Schema internally.
