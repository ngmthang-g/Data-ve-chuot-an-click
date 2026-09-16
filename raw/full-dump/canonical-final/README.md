# Canonical full static dump

This directory is the **sole canonical GitHub reconstruction source** for `MouseHiddenClick-FullStaticDump.tar.xz`. Other `raw/full-dump/*` upload directories are historical transport experiments and must not be used as authoritative data.

Reconstruction:
1. Read `MANIFEST.tsv` in `order`.
2. Concatenate the listed `.b64` files byte-for-byte with no separator.
3. Base64-decode the result.
4. The output must be exactly **328,664 bytes** and SHA-256 `be16b7ac428512faef9e9457efaef844373b7334a1e7b0fdbc4ef677f4e29131`.
5. The archive contains all **23/23** static artifacts indexed by `raw/FULL_DUMP_MANIFEST.tsv`.

Use `reconstruct.py` for a standard-library-only reconstruction and verification. Runtime compatibility remains `PENDING` until Windows GUI tests produce real logs.
