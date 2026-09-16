# Storage & Embedded Templates

## Serialization model
`MacroStorage` contains JSON conversion/load/save methods for both individual macro data and project/profile data. The serialized object model carries action semantics, timing, target binding and condition metadata.

## Embedded templates
Three managed JSON resources ship inside the EXE:
- `Template.All Action.json`
- `Template.Piano Tile.json`
- `Template.Piano Tile opt 2.json`

These are unusually valuable evidence because they demonstrate the application's own expected schema.

### All Action
Contains representative examples covering the ActionType range, making it a practical schema fixture.

### Piano Tile templates
Demonstrate:
- target process `chrome`;
- a browser window title;
- `RelativeToWindow=true`;
- color-condition driven actions;
- fixed client-relative coordinates.

One template uses multiple point conditions; the alternate demonstrates an area-color action.

## Future use
Treat these resources as regression fixtures when rebuilding a compatible macro parser. A reimplementation should be able to deserialize them without inventing fields or coercing action types.

## Version warning
Do not assume future builds preserve exact JSON schema. Fingerprint the binary and compare embedded resources first.