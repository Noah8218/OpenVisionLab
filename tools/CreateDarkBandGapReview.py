#!/usr/bin/env python3
"""Create a deterministic visual-review queue from batch-evidence CSV rows."""

from __future__ import annotations

import argparse
import csv
import hashlib
from collections import Counter
from pathlib import Path

from PIL import Image, ImageDraw, ImageFont, ImageOps


RISK_SELECTIONS = (
    ("ScoreMax", False, 8, "matching_score_low"),
    ("ScoreMargin", False, 8, "matching_margin_low"),
    ("FixtureScaleRatio", False, 8, "matching_scale_low"),
    ("FixtureScaleRatio", True, 8, "matching_scale_high"),
    ("FixtureValidPixelRatio", False, 8, "normalization_coverage_low"),
    ("DistancePxAvg", False, 8, "distance_low"),
    ("DistancePxAvg", True, 8, "distance_high"),
    ("DistancePxRange", True, 8, "range_high"),
    ("GapSelectedSupportRatio", False, 8, "support_low"),
    ("GapDarkContrast", False, 8, "contrast_low"),
    ("GapDarkCoverageRatio", False, 8, "coverage_low"),
    ("GapScoreMargin", False, 8, "margin_low"),
    ("GapCandidatePairCount", True, 8, "pair_count_high"),
)


def number(row: dict[str, str], name: str) -> float | None:
    try:
        return float(row.get(name, ""))
    except (TypeError, ValueError):
        return None


def load_font(size: int) -> ImageFont.ImageFont:
    try:
        return ImageFont.truetype("arial.ttf", size)
    except OSError:
        return ImageFont.load_default()


def add_reason(selected: dict[str, set[str]], row: dict[str, str], reason: str) -> None:
    selected.setdefault(row["ImagePath"], set()).add(reason)


def select_rows(rows: list[dict[str, str]]) -> list[dict[str, str]]:
    selected: dict[str, set[str]] = {}
    measured = [row for row in rows if row.get("StepSuccess", "").lower() == "true"]

    for row in rows:
        if row.get("StepSuccess", "").lower() != "true":
            add_reason(selected, row, "all_fail_closed")

    for metric, descending, count, reason in RISK_SELECTIONS:
        available = [row for row in measured if number(row, metric) is not None]
        available.sort(key=lambda row: (number(row, metric), row["SourceSha256"]), reverse=descending)
        for row in available[:count]:
            add_reason(selected, row, reason)

    for expected in sorted({row.get("Expected", "") for row in rows}):
        stratum = [row for row in rows if row.get("Expected", "") == expected]
        stratum.sort(key=lambda row: (row["SourceSha256"], row["ImagePath"]))
        for row in stratum[:15]:
            add_reason(selected, row, f"hash_audit_{expected.lower()}")

    output: list[dict[str, str]] = []
    for row in sorted(rows, key=lambda item: item["ImagePath"]):
        reasons = selected.get(row["ImagePath"])
        if reasons:
            copy = dict(row)
            copy["ReviewReasons"] = ";".join(sorted(reasons))
            output.append(copy)
    return output


def save_queue(rows: list[dict[str, str]], path: Path) -> str:
    fieldnames = list(rows[0])
    with path.open("w", newline="", encoding="utf-8") as stream:
        writer = csv.DictWriter(stream, fieldnames=fieldnames)
        writer.writeheader()
        writer.writerows(rows)
    canonical = "\n".join(f"{row['ImagePath']}|{row['ReviewReasons']}" for row in rows)
    return hashlib.sha256(canonical.encode("utf-8")).hexdigest().upper()


def render_pages(rows: list[dict[str, str]], output: Path, prefix: str) -> int:
    columns, page_rows = 2, 3
    tile_width, tile_height, label_height = 640, 480, 78
    page_capacity = columns * page_rows
    font = load_font(16)
    small_font = load_font(13)
    page_count = 0

    for offset in range(0, len(rows), page_capacity):
        page_count += 1
        subset = rows[offset : offset + page_capacity]
        canvas = Image.new("RGB", (columns * tile_width, page_rows * (tile_height + label_height)), "#111111")
        draw = ImageDraw.Draw(canvas)
        for index, row in enumerate(subset):
            x = (index % columns) * tile_width
            y = (index // columns) * (tile_height + label_height)
            result_path = Path(
                row.get("StepOverlayPath", "")
                if row.get("StepSuccess", "").lower() != "true" and row.get("StepOverlayPath", "")
                else row["ResultImagePath"]
            )
            try:
                with Image.open(result_path) as source:
                    tile = ImageOps.fit(source.convert("RGB"), (tile_width, tile_height), Image.Resampling.LANCZOS)
            except OSError:
                tile = Image.new("RGB", (tile_width, tile_height), "#550000")
            canvas.paste(tile, (x, y))

            name = Path(row["ImagePath"]).stem
            status = "MEASURED" if row.get("StepSuccess", "").lower() == "true" else "FAIL-CLOSED"
            metric = (
                f"avg={row.get('DistancePxAvg') or '-'} range={row.get('DistancePxRange') or '-'} "
                f"support={row.get('GapSelectedSupportRatio') or '-'} margin={row.get('GapScoreMargin') or '-'}"
            )
            reasons = row["ReviewReasons"].replace(";", ", ")
            draw.text((x + 5, y + tile_height + 3), f"{name} | {row.get('Expected')} | {status}", fill="#ffffff", font=font)
            draw.text((x + 5, y + tile_height + 23), metric, fill="#c8f7ff", font=small_font)
            draw.text((x + 5, y + tile_height + 43), reasons[:64], fill="#ffd966", font=small_font)

        canvas.save(output / f"{prefix}_{page_count:02d}.png")
    return page_count


def main() -> int:
    parser = argparse.ArgumentParser()
    parser.add_argument("evidence_csv", type=Path)
    parser.add_argument("output_dir", type=Path)
    args = parser.parse_args()

    with args.evidence_csv.open(newline="", encoding="utf-8-sig") as stream:
        rows = list(csv.DictReader(stream))
    if not rows:
        raise SystemExit("Evidence CSV has no rows.")

    # Batch evidence contains one row per executed Step. Review one terminal row per image.
    grouped: dict[str, list[dict[str, str]]] = {}
    for row in rows:
        grouped.setdefault(row["ImagePath"], []).append(row)
    terminal: list[dict[str, str]] = []
    for image_rows in grouped.values():
        merged = dict(max(image_rows, key=lambda row: int(row.get("StepIndex") or 0)))
        for row in image_rows:
            for name, value in row.items():
                if value and not merged.get(name):
                    merged[name] = value
        terminal.append(merged)
    rows = terminal

    args.output_dir.mkdir(parents=True, exist_ok=True)
    queue = select_rows(rows)
    queue_hash = save_queue(queue, args.output_dir / "review_queue.csv")
    fail_rows = [row for row in queue if row.get("StepSuccess", "").lower() != "true"]
    measured_rows = [row for row in queue if row.get("StepSuccess", "").lower() == "true"]
    fail_pages = render_pages(fail_rows, args.output_dir, "fail_closed")
    measured_pages = render_pages(measured_rows, args.output_dir, "measured")
    reasons = Counter(reason for row in queue for reason in row["ReviewReasons"].split(";"))

    summary = [
        f"Rows={len(rows)}",
        f"Measured={sum(row.get('StepSuccess', '').lower() == 'true' for row in rows)}",
        f"FailClosed={sum(row.get('StepSuccess', '').lower() != 'true' for row in rows)}",
        f"ReviewQueue={len(queue)}",
        f"ReviewQueueMeasured={len(measured_rows)}",
        f"ReviewQueueFailClosed={len(fail_rows)}",
        f"ReviewQueueSha256={queue_hash}",
        f"FailClosedContactPages={fail_pages}",
        f"MeasuredContactPages={measured_pages}",
    ]
    summary.extend(f"Reason.{name}={count}" for name, count in sorted(reasons.items()))
    (args.output_dir / "review_summary.txt").write_text("\n".join(summary) + "\n", encoding="utf-8")
    print("\n".join(summary))
    return 0


if __name__ == "__main__":
    raise SystemExit(main())
