# Runtime verification

This directory exists to convert static hypotheses into target-specific runtime evidence without mixing the two evidence levels.

## Probe
`windows/BackgroundClickProbe.ps1` reproduces the static donor flow:

The probe can reproduce four recovered transports via `-TransportVariant`:

- `NativeDirect`: child retarget, `MOVE/DOWN/UP` (ClickEngine bound path);
- `NativeScreen`: screen hit-test + child retarget, `MOVE/DOWN/UP`;
- `AdvancedDirect`: no child retarget, `MOVE/DOWN/UP`;
- `AdvancedScreen`: screen hit-test, no child retarget, `DOWN/UP` only.

This distinction is direct IL evidence; do not collapse the four variants into one generic "PostMessage click".

The probe records cursor position before and after. A background click should leave the global cursor unchanged; whether the target application reacts is a separate observation.

## Example
```powershell
powershell -ExecutionPolicy Bypass -File .\runtime\windows\BackgroundClickProbe.ps1 `
  -ProcessName notepad -ClientX 120 -ClientY 80 -Button Left `
  -TestId BG-FOREGROUND-LEFT -StateTag foreground `
  -ObservedResult success -Output .\runtime\results\notepad.jsonl
```

Use `-WindowTitleContains` when multiple windows share one process name. `-TargetPid` can pin a specific instance.

## Evidence promotion
A row in `RUNTIME_TEST_MATRIX.md` may move from `PENDING` to `VERIFIED_RUNTIME` only when:
1. a matching JSONL result is committed;
2. `test_id`, target identity and state tag match the row;
3. the log includes before/after cursor coordinates and resolved HWNDs;
4. `observed_result` is supplied by the tester rather than inferred by the script.

Do not use the probe to bypass privilege, integrity-level, anti-cheat, or protected-input boundaries.

## Resolver fidelity note
The sample's `FindWindowByTarget` rejects minimized (`IsIconic`) windows. The probe therefore does the same by default. `-AllowIconic` exists only to isolate transport behavior and must not be reported as normal sample behavior. Use `-TargetPid` rather than `-Pid` because `$PID` is a PowerShell automatic variable.
