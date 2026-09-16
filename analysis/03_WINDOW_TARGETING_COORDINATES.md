# Window Targeting & Coordinates

## Target identity
A macro/click target can carry:
- process name;
- window title;
- a flag that coordinates are relative to the target window.

This is more stable than persisting only a raw HWND because HWND values are session-specific.

## `FindWindowByTarget`
Observed behavior:
1. Normalize process name (including trimming `.exe`).
2. Normalize title/trailing punctuation.
3. Use `{process}|{title}` as a cache key.
4. Reuse cached HWND for roughly one second only after validation.
5. Validation includes `IsWindow`, `IsWindowVisible`, and rejection of `IsIconic`.
6. Resolve process name to candidate PIDs.
7. `EnumWindows` and compare PID/title.
8. Prefer exact process+title match; fallback to process match.

### Consequence
The implementation intentionally avoids blindly using stale HWNDs and does not treat minimized windows as valid normal targets.

## Coordinate picker
The picker converts a user-picked screen point into durable target metadata:
```text
cursor screen point
 -> WindowFromPoint
 -> GetAncestor(GA_ROOT)
 -> reject tool's own process
 -> PID/process/title
 -> ScreenToClient(top-level HWND)
 -> save client-relative point
```

This explains how later clicks can survive the target window moving on screen.

## Runtime resolution
`ClickEngine.ResolveActualPoint` reverses relative storage when a screen point is required:
```text
stored client point + resolved HWND
 -> ClientToScreen
 -> current screen point
```

## Child targeting
For actual background message delivery, child-control coordinates are recalculated rather than assuming the parent client coordinate can be reused unchanged.

## Design lesson
Persist semantic target identity + client-relative coordinates. Resolve HWND and coordinate transforms immediately before the action. Never persist only screen coordinates when the target window is expected to move.