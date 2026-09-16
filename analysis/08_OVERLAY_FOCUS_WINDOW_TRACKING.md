# Overlay, Focus & Window Tracking

## Overlay click-through
`OverlayForm.SetClickThrough` changes extended window style with `GetWindowLongPtr/SetWindowLongPtr`, toggling `WS_EX_TRANSPARENT`.

## WndProc behavior
When the overlay is interactive rather than fully click-through:
- `WM_MOUSEACTIVATE (33)` can return `MA_NOACTIVATE (3)` so interaction does not force normal activation.
- `WM_NCHITTEST (132)` returns `HTCLIENT (1)` when a marker should receive interaction and `HTTRANSPARENT (-1)` elsewhere.

## What this solves
It lets the tool draw markers/selection UI over other windows without making the whole overlay a mouse blocker or focus thief.

## What it does NOT solve
This subsystem is not what sends background clicks to the target. The actual Free Mouse transport is in `ClickEngine`/`NativeMethods` and uses `PostMessage`.

## `UnfocusClickFilter`
A message filter handles selected mouse-down messages in the tool UI, including `WM_LBUTTONDOWN`, `WM_RBUTTONDOWN`, and `WM_NCLBUTTONDOWN`. Its purpose is UI focus behavior, not target input injection.

## `WindowTracker`
Window tracking observes target geometry/identity changes and updates overlay positions. A target signature contains HWND/coordinate-origin information so marker overlays remain visually aligned.

## Reuse lesson
Keep visualization and input delivery decoupled. An overlay can be removed entirely without changing the core background click transport.