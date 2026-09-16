# AI Router

| Câu hỏi | Đọc trước | Bằng chứng sâu |
|---|---|---|
| Tại sao không chiếm chuột? | `verified/VERIFIED_BACKGROUND_CLICK_FLOW.md` | `CORE_CLICK_IL.txt` tokens `0x0600029e`, `0x0600029f`, `0x06000507` |
| Basic và macro PostMessage có giống nhau không? | `verified/VERIFIED_BACKGROUND_TRANSPORT_VARIANTS.md` | ActionExecutor `0x0600000e/0f`; NativeMethods `0x0600029e/9f` |
| Bind cửa sổ ra sao? | `analysis/03_WINDOW_TARGETING_COORDINATES.md` | token `0x060002b3` |
| Tọa độ relative hoạt động thế nào? | `analysis/03_WINDOW_TARGETING_COORDINATES.md` | token `0x06000219` |
| So sánh background vs physical | `analysis/02_*`, `analysis/04_*` | `CORE_CLICK_IL.txt` |
| Click trái/phải/giữa map message nào? | `verified/VERIFIED_API_MAP.md` | `database/CONSTANTS.tsv` |
| Macro có action nào? | `analysis/05_ADVANCED_MACRO_ENGINE.md` | `raw/resources/Template.All Action.json` |
| Scan pixel/màu? | `analysis/06_PIXEL_CONDITIONS.md` | `GetPixel`, `GetPixelColor`, `MacroRunner::ExecuteSingleStep` |
| Overlay có phải click ẩn không? | `analysis/08_*` | Overlay `WndProc`, `SetClickThrough` |
| Tại sao game/app nào đó không nhận click? | `analysis/10_LIMITATIONS_COMPATIBILITY.md` | target input architecture/runtime test |
| Cần test runtime thế nào? | `runtime/RUNTIME_TEST_MATRIX.md` | `runtime/windows/BackgroundClickProbe.ps1` |
| Cần C# dễ đọc để viết lại engine? | `reconstructed/README.md` | `reconstructed/*.Reconstructed.cs` + evidence IL |
| Thread/timing/cancel? | `analysis/12_THREADING_TIMERS.md` | worker tokens `0x06000507`, `0x060004DE` |
| Dùng donor cho Thần Long? | `applications/THAN_LONG_MAPPING.md` | `DESIGN_LESSONS.md` |
