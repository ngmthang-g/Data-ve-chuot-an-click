# Reuse Blueprint for Future Tools

## Recommended architecture

```text
TargetDescriptor(process,title)
        |
TargetResolver -> HWND validity/cache
        |
CoordinateResolver(client/screen/child)
        |
ActionRequest
        |
Transport selector
  +-----+----------------+
  |                      |
BackgroundMouse       PhysicalInput
PostMessage           SetCursorPos/SendInput
  |                      |
  +----------+-----------+
             |
        Result/verification
```

Keep macro control flow, observation and overlay outside the transport layer.

## Suggested components
1. `TargetResolver`
   - process/title matching;
   - HWND cache with validation;
   - explicit minimized/visibility policy.
2. `CoordinateResolver`
   - stored client-relative point;
   - parent/child transforms;
   - DPI-aware conversions.
3. `BackgroundMouseTransport`
   - MOVE/DOWN/UP message sequence;
   - button/wParam mapping;
   - configurable hold.
4. `PhysicalMouseTransport`
   - real cursor positioning + SendInput.
5. `CapabilityMatrix`
   - state which action supports which transport; never silently steal cursor.
6. `Observer`
   - semantic state where available; pixel fallback otherwise.
7. `MacroRunner`
   - loops, conditions, delays, subroutines.
8. `Overlay`
   - visualization only; no core input dependency.

## Reliability rules learned from this sample
- Persist target semantics, not raw HWND alone.
- Persist relative coordinates when targeting movable windows.
- Resolve live HWND before each action/burst.
- Revalidate cache entries.
- Resolve child HWND at action time.
- Send `WM_MOUSEMOVE` before button message when emulating this implementation.
- Keep DOWN and UP ordering deterministic.
- Record whether fallback to physical input is permitted; default should be no.
- Separate “action posted” from “action succeeded”; verify target state when possible.

## What not to copy blindly
- `WindowFromPoint` as the only target strategy.
- Pixel-only state checks when semantic APIs exist.
- Thread sleeps as proof of success.
- Assumption that PostMessage works for every renderer/game.
- Physical drag/keyboard when the product requirement is truly background-only.

This blueprint extracts the reusable architecture while keeping sample-specific quirks isolated.