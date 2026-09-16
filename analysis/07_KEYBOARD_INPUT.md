# Keyboard Input

## Key press
`KeyboardSimulator.ExecuteKeyPress` constructs keyboard `INPUT` records and sends them through `SendInput`. Extended-key handling is present for keys that require the extended flag.

## Type text
Text typing uses Unicode keyboard injection:
- key down: `KEYEVENTF_UNICODE = 4`
- key up: `KEYEVENTF_UNICODE | KEYEVENTF_KEYUP = 6`

## Key parsing
The simulator includes virtual-key parsing and `MapVirtualKey` support.

## Critical distinction
Unlike Free Mouse click, this sample does **not** implement a parallel keyboard `PostMessage(WM_KEYDOWN/WM_KEYUP)` transport. Keyboard automation remains system input.

Therefore a future “fully background” automation engine must treat keyboard as an unsolved/separate capability rather than assuming Free Mouse covers it.