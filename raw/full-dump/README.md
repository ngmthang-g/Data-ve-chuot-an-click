# Full static dump transport

The exact archive is `MouseHiddenClick-FullStaticDump.zip` in the working bundle, but the GitHub connector used for this research cannot upload a local binary path directly. To preserve **every archive byte inside GitHub anyway**, the archive is stored as Base64 text chunks under `base64-parts/`.

Reconstruction instructions and expected hashes are in `base64-parts/README.md`.

Exact archive properties:

- Size: `546856` bytes
- SHA-256: `b3c20d73a740e43cb94aa2f01940e071a6996fc4df50f7379fa1cf8d28abb5a5`
- Frozen input EXE SHA-256: `1aa4e0e2ea46e7460641fdb05c2a5f409c63505312179549ef9fc7a14633d35e`

The decoded archive contains every immutable bulk file listed in `../FULL_DUMP_MANIFEST.tsv`.
