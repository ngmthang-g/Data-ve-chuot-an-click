# Background Click Engine — deep analysis

## Core idea
The sample synthesizes ordinary Win32 mouse messages and posts them directly to a target HWND. This avoids touching the global cursor.

## Route 1 — screen point (`SendBackgroundClick`)
Input: screen X/Y + logical button + hold time.

Pipeline:
```text
screen POINT
 -> WindowFromPoint
 -> ScreenToClient(parent)
 -> RealChildWindowFromPoint
 -> if child: screen -> child client
 -> encode LPARAM(x,y)
 -> WM_MOUSEMOVE
 -> button DOWN
 -> hold
 -> button UP
```

### Strength
No cursor displacement and no requirement to foreground the target merely to create the message sequence.

### Weakness
`WindowFromPoint` resolves the window currently occupying that screen point. A covering window can therefore change the recipient.

## Route 2 — bound target (`PerformClickDirectToWindow`)
Input: known HWND + point already relative to that window.

Pipeline:
```text
bound HWND + client point
 -> RealChildWindowFromPoint(parent)
 -> if child: parent-client -> screen -> child-client
 -> encode LPARAM
 -> PostMessage MOVE/DOWN/UP
```

This route avoids the topmost-window ambiguity of `WindowFromPoint`; it is the preferred model when a reliable target binding already exists.

## LPARAM encoding
Coordinates are encoded in the low/high 16 bits:
`(y & 0xffff) << 16 | (x & 0xffff)`.

This is standard mouse-message coordinate packing. A future implementation should use signed extraction semantics when decoding and account for coordinates outside unsigned 16-bit assumptions on multi-monitor layouts.

## DOWN wParam
The sample sets the corresponding mouse-key state on DOWN (left=1, right=2, middle=16) and zero on UP.

## Message order
The explicit preliminary `WM_MOUSEMOVE` matters: some target controls update hover/hit state before consuming the button press. Copying only DOWN/UP would not be behaviorally identical.

## Timing
The hold duration is implemented by sleeping between DOWN and UP. This affects the posting worker, not the target message queue itself.

## Free Mouse selection
The background transport is chosen by `ClickConfig.FreeMouseMode`. It is therefore a first-class mode, not a last-second cursor restore trick.

## Important non-properties
This engine does not inject into another process's memory, does not call a target's internal function, and does not create a hardware mouse event. It relies on the target's normal window-message handling accepting the posted messages.