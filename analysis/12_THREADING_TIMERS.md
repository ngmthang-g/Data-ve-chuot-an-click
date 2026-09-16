# Threading, timers, sleeps, and cancellation

**Evidence level:** VERIFIED_STATIC unless explicitly marked otherwise. Frozen sample SHA-256 is in `SAMPLE_MANIFEST.md`.

## ClickEngine worker
`ClickEngine.Start` (`0x0600021A`, RVA `0x16FCC`) creates a dedicated background `System.Threading.Thread`. The compiler-generated worker closure is token `0x06000507` (RVA `0x16A40`).

The worker normalizes the configured interval to at least 1 ms. It derives a short click-hold portion and remaining cycle delay, then loops while `_isRunning` and until the configured loop limit is reached. The branch decision is made inside the worker on `FreeMouseMode`:

- Free Mouse + bound relative point: resolve live HWND and call `PerformClickDirectToWindow`.
- Free Mouse + screen/cursor path: call `SendBackgroundClick`.
- Physical mode: resolve/move cursor, optionally smooth-move between points, then `SendPhysicalClick`.

`Interlocked` is used when incrementing `_totalClicks`. Progress callbacks are throttled rather than emitted for every low-level message.

## MacroRunner worker
`Advanced.MacroRunner.Start` (`0x0600001A`, RVA `0x3830`) clones the step list and constructs a case-insensitive subprofile map for RunScript actions. It then starts a background thread named `MacroRunnerThread`. The worker closure (`0x060004DE`, RVA `0x3690`) sleeps 60 ms before beginning the loop, skips disabled steps, executes them serially, increments completed cycles, and invokes progress events.

There is no evidence that ordinary mutable macro actions execute concurrently with each other inside one runner; they are sequenced on this worker thread. Nested RunScript steps execute synchronously from the caller's flow.

## Fixed sleeps observed
The executable intentionally contains fixed waits in several places. High-value examples include:

- click hold: `Thread.Sleep(holdMs)` between DOWN and UP;
- click-engine timing: remaining cycle delay after the action;
- MacroRunner startup: 60 ms;
- WaitColor / WaitChange polling: 20 ms in the recovered control-flow;
- physical click setup in the advanced executor: short pre-click wait;
- smooth movement: approximately 10 ms samples while interpolation is active;
- stop/join paths: bounded join attempts rather than indefinite waits.

These sleeps are **timing policy**, not proof that a target application changed state.

## Cancellation model
Both ClickEngine and MacroRunner use a volatile `_isRunning` flag. Stop paths clear the flag and make a bounded `Join` attempt. Long operations therefore cooperate through loop/predicate checks; there is no recovered use of `Thread.Abort` as the normal cancellation mechanism.

## Reuse guidance
For a new tool, preserve the separation between an action transport and scheduling, but prefer state/event confirmation over fixed sleeps where the target exposes a semantic state. Do not interpret a completed sleep as action success.
