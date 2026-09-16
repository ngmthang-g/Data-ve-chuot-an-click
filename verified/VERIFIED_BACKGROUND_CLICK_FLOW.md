# VERIFIED — Background / Free Mouse flow

## A. `ClickEngine.Start` decision

Worker IL reads `ClickConfig.FreeMouseMode`.

```text
FreeMouseMode == true
  |
  +-- point + bound relative target
  |     -> FindWindowByTarget
  |     -> PerformClickDirectToWindow(hwnd, clientX, clientY, button, hold)
  |
  +-- other point path
  |     -> ResolveActualPoint
  |     -> SendBackgroundClick(screenX, screenY, button, hold)
  |
  +-- cursor mode
        -> GetCursorPos
        -> optional coordinate jitter calculation
        -> SendBackgroundClick(...)

FreeMouseMode == false
  -> ResolveActualPoint
  -> SetCursorPos
  -> optional physical jitter / smooth move
  -> SendPhysicalClick
```

## B. `NativeMethods.SendBackgroundClick`

Observed IL behavior:

1. Construct screen POINT.
2. `WindowFromPoint` -> target HWND.
3. Return if HWND == 0.
4. Convert screen point to parent client coordinates with `ScreenToClient`.
5. `RealChildWindowFromPoint`.
6. If a distinct child exists, convert original screen point to child client coordinates and retarget.
7. Pack LPARAM: `(y & 0xffff) << 16 | (x & 0xffff)`.
8. Map logical mouse button to Win32 DOWN/UP message and DOWN wParam.
9. `PostMessage(hwnd, WM_MOUSEMOVE=512, 0, lParam)`.
10. `PostMessage(hwnd, buttonDown, wParam, lParam)`.
11. Optional `Thread.Sleep(holdMs)`.
12. `PostMessage(hwnd, buttonUp, 0, lParam)`.

There is no `SetCursorPos` in this path.

## C. `NativeMethods.PerformClickDirectToWindow`

Input is already `(known HWND, client X/Y)`.

1. Probe child with `RealChildWindowFromPoint(parent, clientPoint)`.
2. If child differs: parent client -> screen (`ClientToScreen`) -> child client (`ScreenToClient`).
3. Retarget child.
4. Pack LPARAM.
5. Post MOVE -> DOWN -> hold -> UP.

This route is stronger than screen hit-testing when the desired target window is partially covered, because it starts from a known bound HWND rather than `WindowFromPoint(screen)`.

## D. Why mouse remains free

Windows cursor state is global. `PostMessage` places mouse-like messages into a target window's queue without moving the global pointer. By contrast, the physical branch explicitly calls `SetCursorPos` and `SendInput`.

Therefore the sample's Free Mouse property follows directly from architecture, not cursor hiding.

## E. What is NOT proven

This does not prove every target application will react. Applications that consume Raw Input, DirectInput, device state, proprietary input layers, or intentionally reject synthetic window messages may ignore these messages. That is a target-specific runtime fact.