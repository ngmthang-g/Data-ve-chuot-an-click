# Knowledge-base Method

## Goal
Preserve enough evidence that future tool development can reuse proven behavior without repeating broad reverse engineering.

## Evidence levels
- **VERIFIED** — directly supported by PE/CLR metadata, IL, manifest or embedded resource.
- **PROBABLE** — strong inference, not direct runtime proof.
- **HYPOTHESIS** — test/research direction only.

## Rules
1. Every binary snapshot is identified by SHA-256.
2. Method tokens/RVAs are evidence only for the matching binary snapshot.
3. `verified/` is the canonical conclusion layer.
4. `analysis/` explains architecture/trade-offs but must not silently upgrade uncertainty.
5. `database/` stores machine-friendly facts/lookups.
6. `raw/` preserves evidence sufficient to reproduce key claims.
7. Runtime compatibility with a third-party program is separate from static proof of what this EXE sends.
8. New versions should be diffed against the frozen sample before facts are reused.

## Static-analysis boundary
The sample was analyzed without execution in this study. Therefore claims such as “PostMessage is called” can be VERIFIED, while “application X always accepts the click” cannot be VERIFIED without a controlled runtime test.