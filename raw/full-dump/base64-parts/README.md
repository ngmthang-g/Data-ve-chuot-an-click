# Full static dump — Base64 transport parts

These text files reconstruct the exact binary archive `../MouseHiddenClick-FullStaticDump.zip` even when the GitHub connector cannot upload a local binary directly.

## Reconstruct on Linux/macOS

```bash
cat part-*.b64 | base64 -d > MouseHiddenClick-FullStaticDump.zip
sha256sum MouseHiddenClick-FullStaticDump.zip
```

## Reconstruct on Windows PowerShell

```powershell
$b64 = (Get-ChildItem .\part-*.b64 | Sort-Object Name | ForEach-Object { Get-Content $_ -Raw }) -join ''
[IO.File]::WriteAllBytes('MouseHiddenClick-FullStaticDump.zip', [Convert]::FromBase64String($b64))
Get-FileHash .\MouseHiddenClick-FullStaticDump.zip -Algorithm SHA256
```

Expected archive SHA-256:

`b3c20d73a740e43cb94aa2f01940e071a6996fc4df50f7379fa1cf8d28abb5a5`

Expected archive size: `546856` bytes.

The archive contains every immutable bulk artifact listed in `../../FULL_DUMP_MANIFEST.tsv`.
