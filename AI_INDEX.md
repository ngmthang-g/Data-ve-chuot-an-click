# AI Index — Auto Clicker by Max v1.0

## Start here
- `AI_BOOTSTRAP.md` — cách đọc KB và luật evidence.
- `SAMPLE_MANIFEST.md` — định danh binary cố định.
- `verified/VERIFIED_FACTS.md` — fact kỹ thuật đã chứng minh.
- `verified/VERIFIED_BACKGROUND_CLICK_FLOW.md` — đường Free Mouse / background click.
- `verified/VERIFIED_API_MAP.md` — Win32 API và vai trò.

## Analysis
- `analysis/00_EXECUTIVE_OVERVIEW.md` — kiến trúc toàn cục.
- `analysis/01_PE_CLR_ARCHITECTURE.md` — PE/CLR/.NET metadata.
- `analysis/02_BACKGROUND_CLICK_ENGINE.md` — engine click nền.
- `analysis/03_WINDOW_TARGETING_COORDINATES.md` — HWND, child HWND, client/screen coordinate.
- `analysis/04_PHYSICAL_INPUT_ENGINE.md` — SetCursorPos + SendInput.
- `analysis/05_ADVANCED_MACRO_ENGINE.md` — ActionExecutor/MacroRunner.
- `analysis/06_PIXEL_CONDITIONS.md` — GetPixel/WaitColor/IfColor.
- `analysis/07_KEYBOARD_INPUT.md` — keyboard/type text.
- `analysis/08_OVERLAY_FOCUS_WINDOW_TRACKING.md` — overlay, no-activate, click-through.
- `analysis/09_STORAGE_TEMPLATES.md` — JSON/project/template.
- `analysis/10_LIMITATIONS_COMPATIBILITY.md` — giới hạn Win32 message injection.
- `analysis/11_REUSE_BLUEPRINT.md` — kiến trúc nên tái sử dụng cho tool mới.

## Database
- `database/FACTS.jsonl` — fact atomic cho AI/query.
- `database/CONSTANTS.tsv` — message/flag/action type.
- `database/PINVOKE_MAP.tsv` — P/Invoke surface.
- `database/TYPE_INDEX.tsv` — type inventory.

## Raw evidence
- `raw/APP_MANIFEST.xml`
- `raw/CORE_EVIDENCE.md`
- `raw/resources/*.json`

## Evidence priority
`VERIFIED -> canonical analysis -> database -> raw evidence -> PROBABLE/HYPOTHESIS`.

Không dùng fact của binary khác nếu SHA-256 không trùng `SAMPLE_MANIFEST.md`.