# Design lessons for future automation tools

## Reuse directly as architecture ideas
- Resolve a **live** target HWND from stable identity rather than retaining a stale handle forever.
- Validate cached HWNDs before reuse; the sample checks window existence, visibility and minimized state.
- Store bound coordinates in target-client space and transform at execution time.
- When a specific target HWND is already known, prefer the direct-HWND route over screen hit-testing.
- Resolve a child HWND at the interaction point and transform parent-client -> screen -> child-client.
- Keep physical input and message-based background input as separate transports.
- Keep macro data (what to do) separate from the scheduler/transport (how it is dispatched).
- Freeze binary fingerprints so facts from a repacked/new build are not silently mixed with the current sample.

## Do not copy blindly
- `PostMessage` is not a universal substitute for device input. Raw Input/DirectInput/custom engines can ignore it.
- The donor's drag path and keyboard typing still use `SendInput`; they are not proof of full background input.
- Fixed `Thread.Sleep` values are timing, not target-state confirmation.
- A cached or saved screen coordinate is weaker than semantic object identity or a client-relative coordinate.
- Do not treat successful Win32 API return values as proof that the application accepted the action.

## Better hierarchy for a specialized game tool
1. semantic/internal client action when a stable supported action exists;
2. live UI object/handler invocation when the target exposes it safely;
3. bound HWND background message as a UI fallback;
4. physical `SendInput` only when no better transport exists and cursor takeover is acceptable.

The donor is especially valuable for levels 3–4, not as a reason to replace a stronger semantic game API with synthetic mouse messages.
