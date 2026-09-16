# Data về chuột ẩn / background click — Auto Clicker by Max v1.0

Kho tri thức tĩnh nhiều tầng cho mẫu `Auto Clicker by Max(1).exe`, bố trí theo triết lý DATA 222: **bootstrap → router → verified facts → analysis → database → raw evidence → reproducible tooling**.

## Mục tiêu

Kho này dùng làm dữ liệu gốc để phát triển tool Windows về sau, đặc biệt cho các bài toán:

- click không chiếm chuột / background click;
- bind theo process + title + HWND;
- tọa độ screen/client/relative-to-window;
- chọn đúng child HWND;
- phân biệt input vật lý (`SendInput`) và window-message injection (`PostMessage`);
- macro nhiều action, pixel condition, keyboard, overlay, window tracking.

## Kết luận cốt lõi

`Free Mouse Mode` **không phải giấu hình con trỏ**. Nó đổi engine từ đường vật lý sang đường message:

`FreeMouse=true -> HWND resolution -> coordinate transform -> PostMessage(WM_MOUSEMOVE/DOWN/UP)`

`FreeMouse=false -> SetCursorPos -> SendInput(mouse down/up)`

Xem `verified/VERIFIED_BACKGROUND_CLICK_FLOW.md` trước.

## Bắt đầu đọc

1. `AI_BOOTSTRAP.md`
2. `AI_INDEX.md`
3. `verified/VERIFIED_FACTS.md`
4. `analysis/02_BACKGROUND_CLICK_ENGINE.md`
5. `analysis/03_WINDOW_TARGETING_COORDINATES.md`
6. Khi cần bằng chứng IL: `raw/il/`

## Giới hạn

Đây là **static reverse engineering** của đúng SHA-256 ghi trong `SAMPLE_MANIFEST.md`; không có claim runtime nào được nâng lên VERIFIED nếu chỉ là suy luận. EXE gốc không được nhân bản vào repo; hash cố định sample.