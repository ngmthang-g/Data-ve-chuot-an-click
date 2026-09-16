# AI Router

Chọn đúng pack kiến thức thay vì đọc toàn bộ repo.

| Câu hỏi | Đọc trước |
|---|---|
| Vì sao Free Mouse không chiếm chuột? | `verified/VERIFIED_BACKGROUND_CLICK_FLOW.md`, `analysis/02_BACKGROUND_CLICK_ENGINE.md` |
| Click vào đúng cửa sổ/child control | `analysis/03_WINDOW_TARGETING_COORDINATES.md`, `verified/VERIFIED_API_MAP.md` |
| So sánh background và physical click | `analysis/02_BACKGROUND_CLICK_ENGINE.md`, `analysis/04_PHYSICAL_INPUT_ENGINE.md` |
| Macro/action types | `analysis/05_ADVANCED_MACRO_ENGINE.md`, `database/CONSTANTS.tsv` |
| Pixel/color condition | `analysis/06_PIXEL_CONDITIONS.md` |
| Keyboard/type text | `analysis/07_KEYBOARD_INPUT.md` |
| Overlay/focus/click-through | `analysis/08_OVERLAY_FOCUS_WINDOW_TRACKING.md` |
| JSON/template/project | `analysis/09_STORAGE_TEMPLATES.md`, `raw/resources/` |
| Dùng lại cho tool mới | `analysis/10_LIMITATIONS_COMPATIBILITY.md`, `analysis/11_REUSE_BLUEPRINT.md` |
| Cần chứng minh cấp IL | `raw/CORE_EVIDENCE.md` |

## Nguyên tắc

Không suy `PostMessage == hoạt động với mọi game`. Chỉ kết luận binary gửi Win32 mouse messages; khả năng target xử lý chúng là fact runtime riêng cần test.