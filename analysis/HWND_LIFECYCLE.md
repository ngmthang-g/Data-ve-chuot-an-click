# HWND lifecycle and stale-handle handling

A Windows HWND is a runtime identity, not a durable application identity. This donor mitigates stale handles by pairing logical identity (`ProcessName`, `WindowTitle`) with repeated resolution and a short cache.

## Static facts
- `FindWindowByTarget` caches target results for about one second.
- Cached nonzero handles are revalidated by `IsWindow`, `IsWindowVisible`, and `!IsIconic` before reuse.
- WindowTracker periodically computes signatures from the live HWND plus current client-origin screen position.
- CoordinatePicker stores target identity and client-relative points rather than only absolute desktop coordinates.

## Failure modes a new tool should handle
- process remains but top-level HWND is recreated;
- title changes after login/state transition;
- multiple same-process windows exist;
- a target becomes hidden/minimized;
- child control HWND is recreated while top-level HWND remains;
- DPI/monitor move changes screen geometry while client-relative coordinates remain stable.

## Recommended policy
Cache only as an optimization. Before a mutable action, resolve or validate the live target and derive child/coordinate state from that live window. Never persist an HWND across process restarts as though it were a permanent identifier.
