# Pixel / Color Condition Engine

## Pixel acquisition
`NativeMethods.GetPixelColor` obtains a desktop DC, calls `gdi32!GetPixel`, converts the COLORREF result, then releases the DC.

## Comparison
`ActionExecutor.MatchesColor` compares observed RGB against a target using a tolerance. The tolerance makes macro conditions resilient to small rendering/color differences.

## Area search
`FindMatchingPixelInArea` scans a configured rectangle and returns a matching pixel coordinate when one satisfies the color predicate.

## Control-flow actions
- `WaitColor`: poll until target color condition succeeds.
- `WaitChange`: snapshot an initial pixel and wait for meaningful change.
- `IfColor`: branch by one point.
- `IfColorArea`: branch by an area search.

Polling contains short sleeps (about tens of milliseconds) rather than a tight busy loop.

## Architectural value
This turns the program from a blind timer macro into a primitive observer/action state machine. For future automation, the important pattern is:
`observe -> predicate -> action -> verify`, not simply `sleep -> click`.

## Limitation
GetPixel observes rendered desktop pixels, not semantic UI state. Occlusion, animation, color management, scaling and theme changes can affect it. Prefer semantic state APIs when available; use pixel conditions as a visual fallback.