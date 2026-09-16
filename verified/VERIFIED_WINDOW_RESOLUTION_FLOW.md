# VERIFIED_STATIC — Window resolution flow

Frozen sample: see `SAMPLE_MANIFEST.md`.

`NativeMethods.FindWindowByTarget` (`0x060002B3`, RVA `0x1ACC0`) normalizes process/title input, uses a cache keyed by `process|title`, and revalidates cached handles before reuse. A cached nonzero handle is accepted only when `IsWindow`, `IsWindowVisible`, and `!IsIconic` remain true.

When enumeration is required, the method gathers PIDs for the requested process, enumerates top-level windows, rejects invisible/minimized candidates, checks process identity, and prefers an exact title match over a weaker title/process candidate. The result (including a zero result) is cached with a timestamp; the recovered cache freshness threshold is approximately 1000 ms.

Consequences:
- a minimized target is intentionally excluded by the normal resolver;
- closing/reopening can be recovered after re-resolution instead of trusting a stale HWND;
- process + title is stronger than either alone when several windows exist.

Runtime behavior for a particular application remains target-specific.
