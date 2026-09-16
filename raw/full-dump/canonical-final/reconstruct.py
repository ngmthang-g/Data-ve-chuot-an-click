#!/usr/bin/env python3
import base64, csv, hashlib
from pathlib import Path

ROOT = Path(__file__).resolve().parent
OUT = ROOT / "MouseHiddenClick-FullStaticDump.tar.xz"
EXPECTED_SIZE = 328664
EXPECTED_SHA256 = "be16b7ac428512faef9e9457efaef844373b7334a1e7b0fdbc4ef677f4e29131"

with (ROOT / "MANIFEST.tsv").open(newline="") as f:
    rows = list(csv.DictReader(f, delimiter="\t"))
payload = b"".join((ROOT / row["path"]).read_bytes() for row in rows)
data = base64.b64decode(payload, validate=True)
OUT.write_bytes(data)
sha = hashlib.sha256(data).hexdigest()
assert len(data) == EXPECTED_SIZE, (len(data), EXPECTED_SIZE)
assert sha == EXPECTED_SHA256, (sha, EXPECTED_SHA256)
print(f"PASS {OUT.name}: {len(data)} bytes sha256={sha}")
