# VERIFIED facts — Auto Clicker by Max v1.0

Mọi fact dưới đây được chứng minh trực tiếp từ PE/CLR metadata, IL hoặc embedded resources của sample trong `SAMPLE_MANIFEST.md`.

## 1. Binary / runtime
- PE32 Windows GUI, i386, managed CLR assembly.
- CLR metadata version `v4.0.30319`.
- Entry runtime qua CLR (`_CorExeMain`).
- Manifest chạy `asInvoker`, `uiAccess=false`; DPI awareness có PerMonitor/PerMonitorV2.

## 2. Hai engine click độc lập
### Physical
`SetCursorPos -> SendInput(DOWN) -> hold -> SendInput(UP)`.

### Background / Free Mouse
`HWND/client point -> PostMessage(WM_MOUSEMOVE) -> PostMessage(WM_*DOWN) -> hold -> PostMessage(WM_*UP)`.
Không có `SetCursorPos` trong `SendBackgroundClick` hoặc direct-window background path.

## 3. FreeMouseMode thực sự đổi engine
Worker của `ClickEngine.Start` đọc `ClickConfig.FreeMouseMode`:
- `true`: dùng `SendBackgroundClick` hoặc `PerformClickDirectToWindow`.
- `false`: resolve screen point, dùng `SetCursorPos`, sau đó `SendPhysicalClick`.

Vì vậy “Free Mouse” là message-based background injection, không chỉ ẩn graphic cursor.

## 4. Button/message mapping
- Left: DOWN `513` / UP `514`, down wParam `1`.
- Right: DOWN `516` / UP `517`, down wParam `2`.
- Middle: DOWN `519` / UP `520`, down wParam `16`.
- Mouse move: `512`.

## 5. Child-window targeting
`SendBackgroundClick` dùng `WindowFromPoint`, `ScreenToClient`, `RealChildWindowFromPoint`; nếu có child khác parent nó chuyển lại screen->child client coordinate và gửi message vào child.

`PerformClickDirectToWindow` bắt đầu từ HWND đã bind và client point; dùng `RealChildWindowFromPoint`, `ClientToScreen(parent)` rồi `ScreenToClient(child)` khi cần.

## 6. Window binding
`FindWindowByTarget` hỗ trợ process name + title, cache ~1 giây, kiểm tra `IsWindow`, `IsWindowVisible`, `IsIconic`; trả exact match nếu có, fallback process match.

## 7. Coordinate picker
Picker xác định HWND bằng `WindowFromPoint`, nâng lên top-level qua `GetAncestor(GA_ROOT)`, bỏ chính process tool, lấy process/title, rồi `ScreenToClient` để lưu tọa độ relative.

## 8. Macro action types
0 LeftClick; 1 RightClick; 2 MiddleClick; 3 DoubleClick; 4 DragDrop; 5 KeyPress; 6 TypeText; 7 Delay; 8 WaitColor; 9 IfColor; 10 IfColorArea; 11 WaitChange; 12 RunScript.

## 9. Keyboard / drag không dùng background message path
- Keyboard simulator dùng `SendInput`; TypeText dùng `KEYEVENTF_UNICODE`.
- Drag dùng `SetCursorPos`, `SendInput(LEFTDOWN)`, smooth cursor movement, rồi `SendInput(LEFTUP)`.

## 10. Pixel condition
`GetPixelColor` dùng `GetDC + GetPixel + ReleaseDC`; macro engine có tolerance, WaitColor, WaitChange, IfColor, IfColorArea.

## 11. Overlay ≠ background click engine
Overlay có `WS_EX_TRANSPARENT`, `WM_MOUSEACTIVATE`, `WM_NCHITTEST`, `MA_NOACTIVATE`, `HTTRANSPARENT`; đây là logic để chính overlay không cướp click/focus, tách biệt với việc gửi click cho target.

## 12. Embedded examples
Binary chứa 3 JSON template thật; `All Action` minh họa action schema, hai Piano Tile template minh họa target Chrome + tọa độ relative + color conditions.