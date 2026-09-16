# AI Bootstrap — read this first

Repo này là canonical KB cho cơ chế Windows background click / “Free Mouse” của sample Auto Clicker by Max v1.0.

## Workflow bắt buộc

1. Đọc `SAMPLE_MANIFEST.md` để xác định đúng binary.
2. Đọc `AI_INDEX.md` và `AI_ROUTER.md`.
3. Ưu tiên `verified/` trước `analysis/`.
4. Dùng `database/FACTS.jsonl` cho fact atomic; không đọc tuần tự mọi raw file.
5. Chỉ xuống `raw/` khi cần chứng minh call-chain hoặc hằng số/message cụ thể.
6. Nếu một build EXE khác có SHA-256 khác, tạo snapshot/manifest mới; không trộn fact giữa hai build.

## Evidence levels

- `VERIFIED`: có bằng chứng trực tiếp trong PE/CLR metadata/IL/resource.
- `PROBABLE`: inference mạnh nhưng chưa có runtime proof.
- `HYPOTHESIS`: hướng test/nghiên cứu.

## Canonical high-value fact

`FreeMouseMode=true` chuyển `ClickEngine` sang `SendBackgroundClick` hoặc `PerformClickDirectToWindow`; hai đường này sử dụng `PostMessage` thay vì `SetCursorPos + SendInput` cho click.