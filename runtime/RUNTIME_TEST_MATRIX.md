# Runtime Test Matrix

All rows are intentionally `PENDING` in this static-analysis environment. A result becomes `VERIFIED_RUNTIME` only with a committed probe log.

| Test ID | Window state / condition | Button | Coordinate mode | Expected transport property | Status |
|---|---|---:|---|---|---|
| BG-FOREGROUND-LEFT | target foreground | Left | bound client | cursor unchanged; target receives MOVE/DOWN/UP | PENDING |
| BG-BACKGROUND-LEFT | another app foreground | Left | bound client | cursor unchanged; no activation required by transport | PENDING |
| BG-COVERED-LEFT | target covered by another window | Left | bound client/direct HWND | direct-HWND path should not depend on screen hit-test | PENDING |
| RESOLVER-MINIMIZED | target minimized | Left | process/title resolver | sample resolver rejects `IsIconic`; no action should be dispatched | PENDING |
| TRANSPORT-MINIMIZED-DIRECT | known HWND, transport-only probe with `-AllowIconic` | Left | direct client | isolates whether raw PostMessage can be queued; **not the normal sample resolver path** | PENDING |
| BG-RESIZED | target resized after point capture | Left | client-relative | client-relative coordinate remains tied to client origin | PENDING |
| BG-MOVED | target moved after point capture | Left | client-relative | no dependence on old screen coordinates | PENDING |
| BG-HWND-RECREATE | target closed/reopened | Left | process/title target | resolver must find live replacement HWND | PENDING |
| BG-CHILD-LEFT | child control under point | Left | client-relative | child HWND selected and coordinates converted | PENDING |
| VARIANT-NATIVE-SCREEN | visible target | Left | screen | `SendBackgroundClick`: child-retarget + MOVE/DOWN/UP | PENDING |
| VARIANT-ADV-DIRECT | visible target | Left | bound client | `Advanced.ActionExecutor` direct: no child retarget + MOVE/DOWN/UP | PENDING |
| VARIANT-ADV-SCREEN | visible target | Left | screen | `Advanced.ActionExecutor` fallback: no child retarget + DOWN/UP only | PENDING |
| BG-RIGHT | ordinary visible target | Right | bound client | right MOVE/DOWN/UP mapping | PENDING |
| BG-MIDDLE | ordinary visible target | Middle | bound client | middle MOVE/DOWN/UP mapping | PENDING |
| BG-HOLD-250 | ordinary visible target | Left | bound client | DOWN-to-UP interval >= configured hold approximately | PENDING |
| DPI-100 | 100% scaling | Left | client-relative | coordinate/log baseline | PENDING |
| DPI-125 | 125% scaling | Left | client-relative | verify logical/physical behavior on target | PENDING |
| DPI-150 | 150% scaling | Left | client-relative | verify logical/physical behavior on target | PENDING |
| MULTIMON-SECONDARY | target on non-primary monitor | Left | client-relative | client->screen->child transform remains coherent | PENDING |
| TARGET-WINFORMS | WinForms control | Left | bound client | establish known-positive Windows-message baseline | PENDING |
| TARGET-WPF | WPF target | Left | bound client | target-specific result | PENDING |
| TARGET-UNITY | Unity/DirectX game target | Left | bound client | target-specific; may ignore window messages | PENDING |
| PHYS-CURSOR | physical branch comparison | Left | screen | cursor moves; SendInput path | PENDING |

## Required recording for each row
- exact target application and version;
- target process/PID/title;
- top-level and child HWND;
- DPI and window rectangles;
- before/after cursor position;
- posted message sequence and Win32 return values;
- observed application result (`success`, `no_effect`, `partial`, `unknown`);
- notes about focus, covering window, monitor and privilege level.
