# VERIFIED_STATIC — Four recovered background-click transport variants

The frozen sample does **not** have one generic PostMessage implementation. Static IL shows four materially different routes.

| Variant | Method | Starts from | Child retarget? | Message sequence |
|---|---|---|---|---|
| NativeDirect | `NativeMethods.PerformClickDirectToWindow` `0x0600029F` | known HWND + client point | yes, `RealChildWindowFromPoint` | `WM_MOUSEMOVE -> DOWN -> hold -> UP` |
| NativeScreen | `NativeMethods.SendBackgroundClick` `0x0600029E` | screen point | yes | `WM_MOUSEMOVE -> DOWN -> hold -> UP` |
| AdvancedDirect | `Advanced.ActionExecutor.PerformClickDirectToWindow` `0x0600000E` | known HWND + client point | **no** | `WM_MOUSEMOVE -> DOWN -> hold -> UP` |
| AdvancedScreen | `Advanced.ActionExecutor.PerformClick(..., freeMouse=true)` `0x0600000F` | screen point | **no** explicit child probe | `DOWN -> hold -> UP` (**no `WM_MOUSEMOVE`**) |

## NativeDirect
Used by the simple/basic `ClickEngine` when Free Mouse is enabled, a saved point exists, relative-to-window is active, and the target HWND resolves. The method receives parent-client coordinates, probes a real child, converts parent-client -> screen -> child-client when needed, then posts MOVE/DOWN/UP.

## NativeScreen
Used by basic ClickEngine when it has a screen/cursor point rather than a successful bound-direct route. It starts with `WindowFromPoint(screen)`, converts to client, probes `RealChildWindowFromPoint`, retargets if needed, then posts MOVE/DOWN/UP.

## AdvancedDirect
The macro engine's `ActionExecutor` has its **own** method with the same conceptual name. Its IL packs the supplied client point and posts directly to the supplied HWND. It does not call `RealChildWindowFromPoint` or coordinate-conversion APIs in that method.

## AdvancedScreen
The macro engine's fallback `PerformClick` starts from a screen point and uses `WindowFromPoint` + `ScreenToClient`, but the recovered free-mouse block posts button DOWN, sleeps, and posts UP. There is no `WM_MOUSEMOVE` call and no `RealChildWindowFromPoint` call in that block.

## Why this matters
A future implementation that collapses these routes into a single helper can change target behavior. Runtime testing must therefore record which variant is being exercised. `runtime/windows/BackgroundClickProbe.ps1` exposes the same four labels.
