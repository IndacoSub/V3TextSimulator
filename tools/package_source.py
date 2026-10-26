#!/usr/bin/env python3
"""Package only the project's distributable source files, including uncommitted edits."""

import argparse
import hashlib
from pathlib import Path
import zipfile


def source_files(root):
    top_level = (".gitignore", "DGRV3TS.sln", "global.json", "README.md", "LICENSE", "CHANGELOG.txt")
    for name in top_level:
        yield root / name
    for directory in ("DGRV3TS", "DGRV3TS.Core", "DGRV3TS.Core.Tests"):
        for path in (root / directory).rglob("*"):
            relative = path.relative_to(root)
            if any(part in {"bin", "obj", ".vs"} for part in relative.parts):
                continue
            if path.is_file() and path.suffix in {".cs", ".csproj", ".resx"}:
                yield path
    for directory, extensions in (("docs", {".md"}), ("ThirdParty", {".txt"}),
                                  ("tools", {".py"}), (".github/workflows", {".yml", ".yaml"})):
        for path in (root / directory).rglob("*"):
            if path.is_file() and path.suffix in extensions:
                yield path
    # The samples are explicit: never pick up a user's external VARS_BAK or game assets.
    yield root / "examples/VARS_BAK"
    yield root / "examples/dialoghi.po"


def main():
    root = Path(__file__).resolve().parents[1]
    parser = argparse.ArgumentParser(description=__doc__)
    parser.add_argument("--output", type=Path, default=root.parent / "artifacts/V3TextSimulator-v25-sorgenti.zip")
    destination = parser.parse_args().output.resolve()
    destination.parent.mkdir(parents=True, exist_ok=True)
    files = sorted(set(source_files(root)))
    prefix = "V3TextSimulator-v25/"
    checksums = []
    with zipfile.ZipFile(destination, "w", compression=zipfile.ZIP_DEFLATED, compresslevel=9) as archive:
        for path in files:
            if path.is_symlink() or not path.resolve().is_relative_to(root):
                raise ValueError(f"Source file must be a regular project file: {path}")
            content = path.read_bytes()
            name = path.relative_to(root).as_posix()
            info = zipfile.ZipInfo(prefix + name, date_time=(2026, 1, 1, 0, 0, 0))
            info.compress_type = zipfile.ZIP_DEFLATED
            info.external_attr = 0o100644 << 16
            archive.writestr(info, content)
            checksums.append(f"{hashlib.sha256(content).hexdigest()}  {name}")
        archive.writestr(prefix + "SHA256SUMS.txt", "\n".join(checksums) + "\n")
    with zipfile.ZipFile(destination) as archive:
        bad = archive.testzip()
        if bad:
            raise ValueError(f"Archive integrity check failed: {bad}")
    print(f"{destination}\n{len(files)} source files, {destination.stat().st_size:,} bytes")


if __name__ == "__main__":
    main()
