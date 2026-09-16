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

`FreeMouse=true -> HWND resolution -> coordinate transform -> PostMessage(...)`

**Lưu ý:** có 4 transport background khác nhau; không phải nhánh nào cũng có `WM_MOUSEMOVE` hoặc child-retarget. Xem `verified/VERIFIED_BACKGROUND_TRANSPORT_VARIANTS.md`.

`FreeMouse=false -> SetCursorPos -> SendInput(mouse down/up)`

Xem `verified/VERIFIED_BACKGROUND_CLICK_FLOW.md` trước.

## Bắt đầu đọc

1. `AI_BOOTSTRAP.md`
2. `AI_INDEX.md`
3. `verified/VERIFIED_FACTS.md`
4. `analysis/02_BACKGROUND_CLICK_ENGINE.md`
5. `analysis/03_WINDOW_TARGETING_COORDINATES.md`
6. `verified/VERIFIED_BACKGROUND_TRANSPORT_VARIANTS.md`
7. Khi cần bằng chứng IL đầy đủ: `raw/full-dump/base64-parts/README.md` để tái tạo exact full-static archive; evidence lõi vẫn đọc trực tiếp trong `raw/`/`reconstructed/evidence/`
8. Khi cần kiểm chứng Windows thật: `runtime/RUNTIME_TEST_MATRIX.md`
9. Khi cần viết lại: `reconstructed/README.md`

## Giới hạn

Đây là **static reverse engineering** của đúng SHA-256 ghi trong `SAMPLE_MANIFEST.md`; không có claim runtime nào được nâng lên `VERIFIED_RUNTIME` nếu chưa có probe log thật. EXE gốc không được nhân bản vào repo; hash cố định sample. Bulk derived evidence được đóng gói kèm per-file SHA trong `raw/full-dump/`; GitHub lưu archive dưới dạng Base64 parts có hash để tái tạo byte-for-byte.
