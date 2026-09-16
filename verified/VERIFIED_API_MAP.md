# VERIFIED Win32 API map

## user32.dll — input
| API | Role in sample |
|---|---|
| `SetCursorPos` | physical point/jitter/drag |
| `SendInput` | physical mouse + keyboard/type text |
| `GetCursorPos` | cursor-mode source point |
| `PostMessage` | background mouse MOVE/DOWN/UP |
| `SendMessage` | miscellaneous window messaging/icon/UI helpers; not canonical Free Mouse click path |

## user32.dll — window targeting / coordinates
| API | Role |
|---|---|
| `WindowFromPoint` | screen hit-test |
| `RealChildWindowFromPoint` | child HWND resolution |
| `ScreenToClient` | screen -> client coordinates |
| `ClientToScreen` | parent client -> screen for child retarget |
| `GetAncestor` | normalize picker hit to top-level window |
| `EnumWindows` | target discovery |
| `GetWindowThreadProcessId` | PID association |
| `GetWindowText`, `GetWindowTextLength` | title matching |
| `GetClassName` | window enumeration/filtering |
| `IsWindow`, `IsWindowVisible`, `IsIconic` | cached-target validation |
| `GetClientRect`, `GetWindowRect` | geometry |
| `FindWindow` | single-instance/main-window helper |
| `GetForegroundWindow`, `SetForegroundWindow`, `ShowWindow` | focus/show helpers |

## user32.dll — overlay/window style
`GetWindowLong/Ptr`, `SetWindowLong/Ptr`, `SetWindowPos`, `GetWindow`, `GetClassLong/Ptr`, `GetWindowDC`, `ReleaseDC`, `RegisterHotKey`, `UnregisterHotKey`, `ReleaseCapture`, `MapVirtualKey`, `keybd_event`.

## gdi32.dll
- `GetPixel` — screen pixel color acquisition through a DC.

## dwmapi.dll
- `DwmGetWindowAttribute` — window enumeration/cloaking checks.

## shcore.dll
- `SetProcessDpiAwareness` — per-monitor DPI setup; fallback `SetProcessDPIAware` from user32.

## Key mouse message constants
- `WM_MOUSEMOVE = 512`
- `WM_LBUTTONDOWN = 513`; `WM_LBUTTONUP = 514`
- `WM_RBUTTONDOWN = 516`; `WM_RBUTTONUP = 517`
- `WM_MBUTTONDOWN = 519`; `WM_MBUTTONUP = 520`

## Key physical input flags
- left down/up: `2 / 4`
- right down/up: `8 / 16`
- middle down/up: `32 / 64`
- wheel: `2048`

This file maps observed imports to proven roles; it does not claim every imported API is used in every execution path.