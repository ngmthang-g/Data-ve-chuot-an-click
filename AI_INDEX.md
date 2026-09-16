# AI Index

## Core verified
- `verified/VERIFIED_FACTS.md` — fact chính + mức chứng cứ.
- `verified/VERIFIED_BACKGROUND_CLICK_FLOW.md` — flow click ẩn.
- `verified/VERIFIED_API_MAP.md` — Windows API và vai trò.
- `verified/VERIFIED_BACKGROUND_TRANSPORT_VARIANTS.md` — 4 transport PostMessage khác nhau giữa basic/macro.
- `verified/VERIFIED_WINDOW_RESOLUTION_FLOW.md` — resolver/cache/live HWND.
- `verified/VERIFIED_COORDINATE_TRANSFORMS.md` — screen/client/child transforms.

## Analysis
- `analysis/00_EXECUTIVE_OVERVIEW.md` — bản đồ toàn EXE.
- `analysis/01_PE_CLR_ARCHITECTURE.md` — PE/.NET, assemblies, resources, entrypoint.
- `analysis/02_BACKGROUND_CLICK_ENGINE.md` — SendBackgroundClick/DirectToWindow/FreeMouse worker.
- `analysis/03_WINDOW_TARGETING_COORDINATES.md` — target cache, HWND, screen/client conversion.
- `analysis/04_PHYSICAL_INPUT_ENGINE.md` — SendInput, SetCursorPos, drag/scroll.
- `analysis/05_ADVANCED_MACRO_ENGINE.md` — 13 action types, runner, nested script.
- `analysis/06_PIXEL_CONDITIONS.md` — WaitColor/IfColor/area/change.
- `analysis/07_KEYBOARD_INPUT.md` — key press + Unicode typing.
- `analysis/08_OVERLAY_FOCUS_WINDOW_TRACKING.md` — overlay click-through, focus filter, tracking.
- `analysis/09_STORAGE_TEMPLATES.md` — settings, JSON, templates.
- `analysis/10_LIMITATIONS_COMPATIBILITY.md` — giới hạn kỹ thuật.
- `analysis/11_REUSE_BLUEPRINT.md` — kiến trúc tham khảo khi làm tool mới.
- `analysis/12_THREADING_TIMERS.md` — worker thread, sleeps, cancellation.
- `analysis/13_CONFIG_AND_ACTION_SCHEMA.md` — ClickConfig/MacroProfile/MacroStep.
- `analysis/HWND_LIFECYCLE.md` — stale handle/recreate policy.

## Databases
- `database/FACTS.jsonl`
- `database/TYPE_INDEX.tsv`
- `database/METHOD_INDEX.tsv`
- `database/PINVOKE_MAP.tsv`
- `database/CONSTANTS.tsv`
- `database/METADATA.json` — exact snapshot is preserved inside the full-dump Base64 archive.
- `database/ACTION_SCHEMA.json`, `database/CONFIG_SCHEMA.json`
- `database/THREAD_MAP.tsv`, `database/TARGET_COMPATIBILITY.tsv`

## Raw evidence
- `raw/PE_HEADERS.txt`
- `raw/APP_MANIFEST.xml`
- `raw/strings_ascii.txt`, `raw/strings_utf16le.txt`
- High-value per-class IL is browsable under `reconstructed/evidence/*.il.txt`.
- Full raw IL (`CORE_CLICK_IL`, `SELECTED_HIGH_VALUE_IL`, `FULL_IL_PART_*`) is preserved inside the full-dump Base64 archive.
- `raw/resources/*.json`
- `raw/FULL_DUMP_MANIFEST.tsv`
- `raw/full-dump/base64-parts/` — exact bulk archive encoded as hashed Base64 parts; reconstructs byte-for-byte.

## Runtime verification
- `runtime/RUNTIME_TEST_MATRIX.md`
- `runtime/windows/BackgroundClickProbe.ps1`
- `runtime/RESULT_SCHEMA.json`

## Reconstructed reference source
- `reconstructed/*.Reconstructed.cs`
- `reconstructed/evidence/*.il.txt`

## Reproduction / validation
- `tools/parse_dotnet.py`
- `tools/validate_kb.py`

## Reuse mapping
- `DESIGN_LESSONS.md`
- `applications/THAN_LONG_MAPPING.md`
