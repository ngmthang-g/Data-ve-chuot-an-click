# Advanced Macro Engine

## Main objects
- `MacroProfile`: project-level defaults and ordered steps.
- `MacroStep`: one action plus timing/target/condition metadata.
- `MacroRunner`: executes a step list, nested/script flow and loop control.
- `ActionExecutor`: executes one concrete action.
- `MacroStorage`: JSON serialization/deserialization.

## MacroStep data observed
`Id`, `Name`, `ActionType`, `Enabled`, `StartPoint`, `EndPoint`, `HoldMs`, `DelayMs`, `RepeatCount`, `ScrollStep`, `KeyData`, `Note`, `TargetColor`, `ColorHex`, `Tolerance`, `IfTrueStep`, `IfFalseStep`, `WindowTitle`, `ProcessName`, `RelativeToWindow`.

## MacroProfile data observed
`Name`, `IsCombine`, `LoopCount`, `RandomIntervalMs`, `RandomJitterPx`, `DefaultWindowTitle`, `DefaultProcessName`, `DefaultRelativeToWindow`, `Steps`.

## Action types
0 LeftClick
1 RightClick
2 MiddleClick
3 DoubleClick
4 DragDrop
5 KeyPress
6 TypeText
7 Delay
8 WaitColor
9 IfColor
10 IfColorArea
11 WaitChange
12 RunScript

## Execution architecture
The runner handles control flow and repetition; the executor handles primitive action semantics. This separation is important: a future tool can add a new transport or observer without rewriting macro project storage.

## Click behavior
For click actions, `ActionExecutor.Execute` can choose direct-window relative operation when target metadata permits; otherwise it resolves the point and uses the generic click path. Double-click is implemented as two click actions with an inter-click interval derived from system double-click timing.

## Script/nesting
`RunScript` uses the step's key/script identity to find another script and repeat it. This means the macro format can express reusable subroutines instead of duplicating every sequence.

## Timing/randomization
Hold, delay, repeat, random interval and jitter are independent concepts. A future implementation should preserve that separation rather than collapse all timing into one sleep field.