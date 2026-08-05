# OpenVisionLab Document Control-Plane Cleanup

Date: 2026-08-05 KST

Status: Complete

## Scope

Reduce LLM search ambiguity and default context cost without deleting evidence
or changing product behavior. The approved scope included global and project
agent routing, the live handoff, historical document ownership, the final P284
R3 report, and documentation encoding/redirect validation.

Excluded: application/runtime/UI/XML/sample changes, original-repository sync,
commit, push, and broad deletion of compatibility redirects or evidence.

## Responsibility Change

### Before

- Global `C:\Users\USER\.codex\AGENTS.md` owned an obsolete LLM-first
  OpenVisionLab identity, Pin skill default priority, and a mandatory ten-file
  reading bundle.
- Project `AGENTS.md` owned current invariants plus 96 long P-number evidence
  records totaling about 97 KB.
- `docs/admin/OPENVISIONLAB_CURRENT_HANDOFF.md` owned both live status and 106
  P-history sections in a 448,525-byte file.
- Legacy recovery, WPF, and Next Work documents remained outside the machine
  index with current-sounding paths, priorities, and percentages.
- The root XSD compatibility redirect was CP949, and root `.html` redirects
  were outside `TestDocumentationIndex.ps1` coverage.

### After

- The global file owns only machine-wide rules and routes OpenVisionLab work to
  the nearest project `AGENTS.md`, `docs/README.md`, and task-specific JSON
  route. It owns no product feature priority.
- Project `AGENTS.md` owns current product constraints and verification rules;
  P-number evidence is resolved through the historical documentation route.
- The live handoff owns current identity, maturity, recent completion evidence,
  activation conditions, and restart instructions in a small file.
- The former cumulative handoff remains byte-preserved except for an archive
  banner at
  `docs/admin/archive/OPENVISIONLAB_CURRENT_HANDOFF_HISTORY_THROUGH_P284.md`.
- Legacy documents are explicitly archived:
  - `docs/admin/archive/CODEX_RECOVERY_20260703.md`
  - `docs/admin/archive/CODEX_HANDOFF_WPF_HISTORY_20260621.md`
  - `docs/admin/archive/OpenVisionLab.NextWork_20260618.md`
  - `docs/roadmap/archive/OPENVISIONLAB_WPF_MIGRATION_PLAN_20260621.md`
- Existing root compatibility files point to the archive locations.
- Release routing includes the repository-owned final P284 R3 report; P-number
  lookup includes the archived cumulative handoff only on the historical route.
- Documentation validation reads indexed/root documents as strict UTF-8 and
  validates `.md`, `.json`, `.xsd`, and `.html` root redirects.

## Structural Proof

| Check | Before | After |
| --- | --- | --- |
| Global AGENTS | 29,652 bytes, stale LLM/Pin priority | 24,234 bytes, project/index routing only |
| Project AGENTS | 141,997 bytes, 96 P-record lines | 39,372 bytes, 0 P-record lines |
| Live handoff | 448,525 bytes | compact current source; former 448,828-byte content archived with banner |
| Current default path | global fixed bundle plus cumulative status | project rules -> compact current handoff -> task-specific JSON route |
| Historical path | mixed with current status | explicit historical route and archive folders |
| Root redirect validation | 99 `.md`/`.json`/`.xsd` redirects, platform decoding | 101 redirects including `.html`, strict UTF-8 |

The source-of-truth owner and read path changed; this is not a filename-only
move. Searches confirmed no old canonical legacy paths outside archive content,
no global LLM/Pin priority, and no P-record lines in project `AGENTS.md`.

## Verification

- `powershell -NoProfile -ExecutionPolicy Bypass -File tools\TestDocumentationIndex.ps1`
  - PASS: `IndexedPaths=54 Routes=11 RootRedirects=101` on the final tree.
- PowerShell parser for `tools/TestDocumentationIndex.ps1`: PASS.
- `docs/LLM_DOCUMENT_INDEX.json` parse and every `routes[].read` path: PASS.
- Strict UTF-8 scan of repository text documents and global AGENTS: PASS,
  zero invalid files.
- Markdown relative-link scan: PASS, zero broken targets.
- Stale global-priority, project P-record, old canonical path, R3 commit/hash
  searches: PASS.
- `git diff --check`: PASS; only checkout line-ending notices were emitted.
- `dotnet run --project tools\OpenVisionReadinessCheck\OpenVisionReadinessCheck.csproj -c Debug -- C:\Git\OpenVisionLab_Dev`
  - PASS: all 13 contracts.

## Boundary

This proves document routing, ownership, encoding, redirect integrity, and
repository readiness for the changed documentation control plane. It does not
prove application behavior beyond the unchanged contracts checked by the
readiness tool. The original repository was not touched. The repository changes
are included only in the user-approved Dev documentation commit; the global
AGENTS edit is user-scoped and is not part of that commit. No push is included.
