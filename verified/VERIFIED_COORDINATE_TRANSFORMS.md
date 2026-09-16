# VERIFIED_STATIC — Coordinate transforms

## Capturing a bound point
`CoordinatePicker.ConfirmPoint` (`0x060002D8`, RVA `0x1B7B0`) starts from a screen point, obtains the window under it, promotes that to a top-level/root window, rejects the tool's own process as a target, records process/title, and converts the screen point into top-level client coordinates using `ScreenToClient`.

## Executing a bound point
A client-relative point is resolved against the current live target HWND. The advanced executor and ClickEngine can convert the target-client origin/point back to screen using `ClientToScreen` when a screen coordinate is needed.

## Direct background click to a known target
`PerformClickDirectToWindow` starts with `(top HWND, client X/Y)`. It probes `RealChildWindowFromPoint`. If a distinct child is selected, the point is converted:

`parent-client -> ClientToScreen(parent) -> ScreenToClient(child) -> child-client`.

The final child-client X/Y values are packed into the mouse message LPARAM.

## Screen-based background click
`SendBackgroundClick` starts from a screen coordinate, uses `WindowFromPoint`, converts to client, probes a real child, and if retargeting occurs converts the original screen point into that child's client coordinates.

This distinction explains why direct-HWND + client-relative targeting can remain meaningful even when another window visually covers the target point, whereas screen hit-testing depends on the current desktop stacking at that coordinate.
