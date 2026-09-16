# Raw Core Evidence — selected CLR IL facts

Frozen sample SHA-256: `1aa4e0e2ea46e7460641fdb05c2a5f409c63505312179549ef9fc7a14633d35e`.

This file records the minimum token/RVA-level evidence needed to independently verify the major conclusions without loading the multi-megabyte full IL dump.

## Token `0x0600000e` — `ModernAutoClicker.Advanced.ActionExecutor::PerformClickDirectToWindow`, RVA `0x2ff8`
Observed operations:
- packs point as `(Y & 65535) << 16 | (X & 65535)`;
- maps left/right/middle to 513/516/519 DOWN and 514/517/520 UP;
- down wParam left/right/middle = 1/2/16;
- calls `NativeMethods.PostMessage(hwnd, 512, 0, lParam)`;
- calls `PostMessage(hwnd, DOWN, downWParam, lParam)`;
- optional `Thread.Sleep(holdMs)`;
- calls `PostMessage(hwnd, UP, 0, lParam)`.

Representative IL:
```text
ldarg.0
ldc.i4 512
ldsfld System.IntPtr::Zero
ldloc.0
call ModernAutoClicker.NativeMethods::PostMessage
...
ldarg.0
ldloc.1
ldloc.3
ldloc.0
call ModernAutoClicker.NativeMethods::PostMessage
...
call System.Threading.Thread::Sleep
...
call ModernAutoClicker.NativeMethods::PostMessage
```

## Token `0x0600000f` — `ActionExecutor::PerformClick`, RVA `0x30b8`
The method contains two mutually distinct transports.

### Background branch
Observed calls:
```text
WindowFromPoint
ScreenToClient
PostMessage(button DOWN)
Thread.Sleep
PostMessage(button UP)
```

### Physical branch
Observed immediately after background return path:
```text
System.Drawing.Point::get_X
System.Drawing.Point::get_Y
NativeMethods::SetCursorPos
Thread.Sleep
construct INPUT/MOUSEINPUT
NativeMethods::SendInput
```

This one method alone proves the program intentionally separates message-based and physical mouse delivery.

## Token `0x0600029d` — `NativeMethods::SendPhysicalClick`
Observed mapping:
- left `2/4`;
- right `8/16`;
- middle `32/64`.

It constructs `INPUT` records and calls `SendInput` for button down/up around the hold interval.

## Token `0x0600029e` — `NativeMethods::SendBackgroundClick`
Observed sequence:
```text
POINT(screenX, screenY)
WindowFromPoint
ScreenToClient(parent)
RealChildWindowFromPoint
[if distinct child: ScreenToClient(child) using original screen point]
pack LPARAM
PostMessage(WM_MOUSEMOVE=512)
PostMessage(button DOWN)
Sleep(holdMs if > 0)
PostMessage(button UP)
```
No `SetCursorPos` occurs in this method.

## Token `0x0600029f` — `NativeMethods::PerformClickDirectToWindow`
Observed sequence starts from known HWND + client point:
```text
RealChildWindowFromPoint(parent, clientPoint)
if child != parent:
  ClientToScreen(parent)
  ScreenToClient(child)
  target = child
pack LPARAM
PostMessage MOVE/DOWN/UP
```
No global cursor movement occurs.

## Token `0x06000507` — compiler-generated worker used by `ClickEngine.Start`
This is the decisive Free Mouse routing evidence.

Observed high-level branch:
```text
if config.FreeMouseMode:
    if point-list + RelativeToWindow + target:
        hwnd = FindWindowByTarget(...)
        PerformClickDirectToWindow(hwnd, point.X, point.Y, ...)
    else:
        resolved = ResolveActualPoint(...)
        SendBackgroundClick(resolved.X, resolved.Y, ...)
else:
    resolved = ResolveActualPoint(...)
    SetCursorPos(resolved.X, resolved.Y)
    ...
    SendPhysicalClick(...)
```
Cursor-mode under Free Mouse reads `GetCursorPos` as a source coordinate but does not reposition the pointer before `SendBackgroundClick`.

## Token `0x06000219` — `ClickEngine::ResolveActualPoint`
When `RelativeToWindow` and target data are present:
```text
FindWindowByTarget
POINT(storedClientX, storedClientY)
ClientToScreen(hwnd, &point)
return screen point
```
Otherwise it returns the original point.

## Token `0x060002b3` — `NativeMethods::FindWindowByTarget`
Observed behavior:
- process normalization incl. `.exe` removal;
- title normalization;
- cache key process/title;
- ~1000 ms cached target lifetime;
- cache validation with `IsWindow`, `IsWindowVisible`, `!IsIconic`;
- `Process.GetProcessesByName` candidate PID set;
- `EnumWindows` discovery;
- exact match preferred, process match fallback.

## Other high-value evidence
- Drag path token `0x06000011`: `SetCursorPos` + `SendInput(LEFTDOWN)` + smooth movement + `SendInput(LEFTUP)`.
- `UnfocusClickFilter::PreFilterMessage` token `0x060003b6` handles the tool's own UI click/focus messages, not target click injection.
- Overlay WndProc handles `WM_MOUSEACTIVATE`/`WM_NCHITTEST`; separate from click transport.

## Full raw bundle
The companion full research archive contains:
- all 1,333 disassembled managed methods split into 4 IL parts;
- high-value selected IL dump;
- 12,257 extracted call edges split into 4 JSONL files;
- method/type indexes;
- metadata JSON;
- ASCII/UTF-16 strings;
- PE headers;
- embedded resources;
- parser source.