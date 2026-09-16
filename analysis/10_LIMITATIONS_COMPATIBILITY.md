# Limitations & Compatibility

## Background click compatibility
The sample's Free Mouse route posts Win32 mouse messages. That works only when the target's interaction path consumes the corresponding window messages.

Potential non-compatible input models include:
- Raw Input;
- DirectInput/device polling;
- game engines reading centralized hardware state rather than per-window messages;
- custom render surfaces that ignore parent/child window mouse messages;
- software that explicitly detects/rejects synthetic messages.

## Covered-window issue
The generic `SendBackgroundClick(screenPoint)` begins with `WindowFromPoint`, so a different topmost window over the coordinate can receive the hit-test result. When target metadata is known, direct-bound HWND + client coordinates is more deterministic.

## Minimized windows
The target resolver validates visibility and rejects `IsIconic`; therefore this sample should not be documented as having proven minimized-window click support.

## Coordinate/DPI risks
The app declares DPI awareness and uses client/screen conversion APIs, but future implementations must still test:
- mixed-DPI multi-monitor setups;
- negative virtual-screen coordinates;
- window borders/client origins;
- child controls that move independently.

## Action capability mismatch
- mouse single/double click: background transport exists;
- keyboard/type text: `SendInput`;
- drag/drop: physical cursor + `SendInput`;
- pixel observer: desktop rendered pixels.

## Runtime proof boundary
Static IL proves what the tool attempts. It does not prove a named third-party application accepts that input. Compatibility claims belong in a separate runtime-test matrix with target version, window state, focus state and result.