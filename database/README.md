# Database usage

- `FACTS.jsonl`: atomic high-value facts with evidence path.
- `TYPE_INDEX.tsv`: all managed TypeDefs.
- `METHOD_INDEX.tsv`: all 1,333 managed MethodDefs + tokens/RVAs.
- `PINVOKE_MAP.tsv`: Win32 surface.
- `CONSTANTS.tsv`: action enum + message/input constants.
- `METADATA.json`: full parsed metadata snapshot produced by `tools/parse_dotnet.py`; the exact large snapshot is preserved inside `raw/full-dump/base64-parts/`.

For a specific method, locate its token in METHOD_INDEX. High-value class IL is browsable under `reconstructed/evidence/`; the complete split IL is preserved in the reconstructable full-dump archive under `raw/full-dump/base64-parts/`.
