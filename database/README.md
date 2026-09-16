# Database layer

Machine-friendly lookup layer for the frozen sample.

- `FACTS.jsonl`: atomic technical facts with evidence pointers.
- `CONSTANTS.tsv`: action IDs, Win32 messages and flags.
- `PINVOKE_MAP.tsv`: imported native API -> observed managed use.
- `TYPE_INDEX.tsv`: important CLR type inventory.

The complete local research bundle additionally contains the 1,333-method index, complete metadata JSON, full static call graph (12,257 edges), full IL dump and strings. Those bulk files are intentionally separated from normal AI routing so ordinary tasks do not preload megabytes of raw evidence.