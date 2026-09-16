# Physical Input Engine

## Mouse click
`NativeMethods.SendPhysicalClick` maps logical button to `MOUSEEVENTF_*DOWN/*UP`, creates `INPUT/MOUSEINPUT`, calls `SendInput` for DOWN, sleeps for hold duration, then calls `SendInput` for UP.

## Positioning
When Free Mouse is disabled, `ClickEngine` can call `SetCursorPos` before physical click. Optional jitter changes the real pointer coordinate. Smooth point-to-point movement is implemented by repeated cursor movement through `MouseMovementSimulator.MoveSmoothly`.

## Drag/drop
The advanced drag path is physical:
```text
SetCursorPos(start)
 -> SendInput(LEFTDOWN)
 -> smooth cursor movement toward end
 -> SendInput(LEFTUP)
```
So the sample does not implement a message-only background drag equivalent.

## Why this is separate from Free Mouse
`SendInput` changes system input state and is consumed through the normal global input pipeline. Even if the cursor were restored afterward, this is still fundamentally different from posting a mouse message to one HWND.

## Reuse guidance
Future tools should model physical and background input as separate transports with explicit capability flags:
- click: both transports;
- drag: physical in this sample;
- keyboard: system SendInput in this sample;
- foreground requirement: action/target-specific.

Do not silently fall back from background to physical input because that would unexpectedly steal the user's cursor.