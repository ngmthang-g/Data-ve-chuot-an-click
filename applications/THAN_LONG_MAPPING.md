# Applying this donor knowledge to Thần Long

This file is an **application mapping**, not a claim that the Thần Long client has the same input architecture as Auto Clicker by Max.

## Preferred decision order
For Thần Long automation, choose the highest-level stable action that is actually verified for the target feature:

`semantic game/Lua/UI action -> live UI handler -> HWND background message fallback -> physical input fallback`

Examples of why the distinction matters:
- A stock game request/UI callback can carry exact item or action identity and can avoid coordinate ambiguity.
- HWND PostMessage can be useful for ordinary Windows-message-driven controls, but a Unity/DirectX input layer may ignore it.
- Physical SendInput is broadly visible to the OS but moves/uses global input state and can interfere with the user.

## What this donor contributes
- robust `process/title -> live HWND` resolution concept;
- client-relative coordinate storage;
- top-level -> child HWND retargeting;
- screen/client coordinate conversion rules;
- clean split between background and physical transports;
- focus-neutral overlay patterns;
- macro/action serialization patterns;
- runtime compatibility test methodology.

## What must stay target-specific
- whether the Thần Long render/input window reacts to WM_* messages;
- which child HWND, if any, owns the interaction;
- whether minimized/background states are accepted;
- DPI/multi-monitor behavior in the real client;
- semantic Lua/client handlers and their state guards.

Never promote a donor behavior to a Thần Long VERIFIED fact without a Thần Long-specific static or runtime proof.
