# Executive Overview

## Architecture map

```text
Program/MainForm
   |
   +-- ClickConfig -----------------------------+
   |                                           |
   +-- ClickEngine                             |
   |    +-- FreeMouse=false -> physical input  |
   |    +-- FreeMouse=true  -> background msg  |
   |                                           |
   +-- Advanced Macro                          |
   |    +-- MacroRunner                        |
   |    +-- ActionExecutor                     |
   |    +-- MacroStorage                       |
   |                                           |
   +-- Window targeting                        |
   |    +-- CoordinatePicker                   |
   |    +-- FindWindowByTarget                 |
   |    +-- WindowTracker                      |
   |                                           |
   +-- Observation                             |
   |    +-- pixel/color conditions             |
   |                                           |
   +-- UI assist                               |
        +-- OverlayForm
        +-- UnfocusClickFilter
```

## Most reusable technical idea

The sample separates **what action to perform** from **how a mouse click reaches the target**. A click can use:

1. physical global input (`SetCursorPos` + `SendInput`); or
2. target-window message injection (`PostMessage`) while the real cursor stays untouched.

The second route is the source of its Free Mouse behavior.

## Important design strengths
- Target identity can be process + title instead of raw HWND only.
- Stored point can be client-relative and converted at runtime.
- Direct-window background route retargets child HWND.
- Cached HWND is revalidated.
- Macro schema stores action semantics separately from target metadata.
- Pixel checks provide state-dependent flow rather than pure fixed-delay playback.

## Important architectural boundaries
- Background mouse click does **not** imply background keyboard or drag.
- Overlay click-through/no-activate is a separate subsystem.
- Window-message injection does not emulate hardware state for software using Raw Input/DirectInput.

## Recommendation for future tools
Reuse the abstractions — target resolver, coordinate transformer, click transport, macro action, observer, verification — rather than copying one giant click function. Keep physical and background transports interchangeable behind one interface.