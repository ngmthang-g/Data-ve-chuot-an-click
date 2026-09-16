#!/usr/bin/env python3
"""Validate the frozen MouseHiddenClick knowledge base. This validator never upgrades runtime evidence."""
from __future__ import annotations
import argparse,csv,hashlib,json,sys
from pathlib import Path
EXPECTED_EXE_SHA256="1aa4e0e2ea46e7460641fdb05c2a5f409c63505312179549ef9fc7a14633d35e"
EXPECTED={"types":117,"methods":1333,"fields":775,"call_edges":12257}
RECON_CLASSES=["NativeMethods","ClickEngine","ActionExecutor","MacroRunner","MacroStorage","KeyboardSimulator","MouseMovementSimulator","CoordinatePicker","OverlayForm","WindowTracker","UnfocusClickFilter"]
RUNTIME_REQUIRED=["runtime/README.md","runtime/RUNTIME_TEST_MATRIX.md","runtime/RESULT_SCHEMA.json","runtime/windows/BackgroundClickProbe.ps1","runtime/windows/Run-Matrix.ps1"]
def sha256(path):
    h=hashlib.sha256()
    with path.open("rb") as f:
        for chunk in iter(lambda:f.read(1024*1024),b""):h.update(chunk)
    return h.hexdigest()
def fail(msg,errors):errors.append(msg);print(f"FAIL  {msg}")
def ok(msg):print(f"PASS  {msg}")
def main():
    ap=argparse.ArgumentParser();ap.add_argument("--root",default=str(Path(__file__).resolve().parents[1]));ap.add_argument("--exe",default=None);ns=ap.parse_args();root=Path(ns.root).resolve();errors=[]
    if ns.exe:
        exe=Path(ns.exe)
        if not exe.is_file():fail(f"EXE not found: {exe}",errors)
        elif sha256(exe)!=EXPECTED_EXE_SHA256:fail(f"frozen EXE SHA-256 mismatch: {sha256(exe)}",errors)
        else:ok("frozen EXE SHA-256")
    else:print("SKIP  frozen EXE SHA-256 (--exe not supplied)")
    try:
        meta=json.loads((root/"database/METADATA.json").read_text(encoding="utf-8"));counts={k:len(meta[k]) for k in("types","methods","fields")}
        for k,expected in (("types",117),("methods",1333),("fields",775)):
            if counts[k]!=expected:fail(f"metadata {k}: expected {expected}, got {counts[k]}",errors)
            else:ok(f"metadata {k}={expected}")
    except Exception as e:fail(f"metadata parse: {e}",errors)
    edge_count=0
    try:
        for p in sorted((root/"database/call_graph").glob("CALL_EDGES_PART_*.jsonl")):
            with p.open(encoding="utf-8") as f:
                for line in f:
                    if line.strip():json.loads(line);edge_count+=1
        if edge_count!=EXPECTED["call_edges"]:fail(f"call edges: expected 12257, got {edge_count}",errors)
        else:ok("call edges=12257 and every row parses as JSON")
    except Exception as e:fail(f"call graph parse: {e}",errors)
    for i in range(1,5):
        p=root/f"raw/il/FULL_IL_PART_{i:02d}.txt"
        if not p.is_file() or p.stat().st_size==0:fail(f"missing/empty {p.relative_to(root)}",errors)
        else:ok(f"{p.relative_to(root)} non-empty")
    try:
        with (root/"raw/FULL_DUMP_MANIFEST.tsv").open(encoding="utf-8",newline="") as f:rows=list(csv.DictReader(f,delimiter="\t"))
        for row in rows:
            p=root/row["path"]
            if not p.is_file():fail(f"manifest missing file {row['path']}",errors);continue
            size=p.stat().st_size;digest=sha256(p)
            if size!=int(row["size_bytes"]):fail(f"size mismatch {row['path']}: {size} != {row['size_bytes']}",errors)
            elif digest!=row["sha256"]:fail(f"sha256 mismatch {row['path']}",errors)
            else:ok(f"manifest {row['path']}")
    except Exception as e:fail(f"full dump manifest: {e}",errors)
    for rel in("database/ACTION_SCHEMA.json","database/CONFIG_SCHEMA.json","runtime/RESULT_SCHEMA.json"):
        try:json.loads((root/rel).read_text(encoding="utf-8"));ok(f"JSON parses: {rel}")
        except Exception as e:fail(f"JSON parse {rel}: {e}",errors)
    for c in RECON_CLASSES:
        src=root/f"reconstructed/{c}.Reconstructed.cs";ev=root/f"reconstructed/evidence/{c}.il.txt"
        if not src.is_file():fail(f"missing reconstruction {src.relative_to(root)}",errors)
        elif "RECONSTRUCTED" not in src.read_text(encoding="utf-8",errors="replace")[:500]:fail(f"reconstruction lacks RECONSTRUCTED label: {src.relative_to(root)}",errors)
        else:ok(f"reconstruction labeled: {c}")
        if not ev.is_file() or ev.stat().st_size==0:fail(f"missing evidence {ev.relative_to(root)}",errors)
        else:ok(f"IL evidence: {c}")
    for rel in RUNTIME_REQUIRED:
        p=root/rel
        if not p.is_file() or p.stat().st_size==0:fail(f"missing runtime artifact {rel}",errors)
        else:ok(f"runtime artifact: {rel}")
    matrix=(root/"runtime/RUNTIME_TEST_MATRIX.md").read_text(encoding="utf-8");results=list((root/"runtime/results").glob("*.jsonl")) if (root/"runtime/results").exists() else []
    if any("| VERIFIED_RUNTIME |" in line for line in matrix.splitlines()) and not results:fail("runtime matrix contains VERIFIED_RUNTIME row but no committed JSONL results",errors)
    if not results:ok("runtime outcomes remain unpromoted (no JSONL logs present)")
    else:
        for p in results:
            with p.open(encoding="utf-8-sig") as f:
                for line in f:
                    if line.strip():json.loads(line)
            ok(f"runtime JSONL parses: {p.relative_to(root)}")
    forbidden=["EvaluateColor(MacroStepSnapshot s) => true","GetPointAtReference(Point p) => -1"];recon_text="\n".join(p.read_text(encoding="utf-8",errors="replace") for p in (root/"reconstructed").glob("*.cs"))
    for needle in forbidden:
        if needle in recon_text:fail(f"misleading reconstruction placeholder remains: {needle}",errors)
    if not errors:ok("no known misleading reconstruction placeholders")
    print("\nVALIDATION:","PASS" if not errors else f"FAIL ({len(errors)} errors)");return 0 if not errors else 1
if __name__=="__main__":sys.exit(main())
