# OpenVisionLab Dev Repository Path Migration

- Date: 2026-08-30 KST
- Status: **Complete**
- Scope: flatten the existing physical Dev repository from
`C:\Git\2D\Dev\OpenVisionLab_Dev` to the canonical physical root
`C:\Git\2D\Dev` without changing repository or working-tree content.

## Included And Excluded Scope

- Included: exact path validation, same-volume directory rename, automatic
  rollback on failure, and pre/post Git and file-equivalence checks.
- Excluded: `C:\Git\2D\Original`, source edits, staging, commit, push, stale
  prunable auxiliary worktree cleanup, release, and deployment.

## Preflight

- `C:\Git\2D\Dev` was a physical directory, not a junction, containing exactly
  one physical child directory: `OpenVisionLab_Dev`.
- The child was the Git root and `.git` directory owner; local `core.worktree`
  was unset.
- The temporary sibling path
  `C:\Git\2D\Dev_wrapper_flatten_20260830` did not exist.
- HEAD was `5f0bbfb5b5e232ee62214070391541e36908b4cf` on
  `codex/public-sample-ux-docs`, tracking
  `origin/codex/public-sample-ux-docs` at the same commit.
- The exact pre-move worktree state was 11 modified tracked files and five
  untracked files.

## Procedure

The operation used only same-volume directory renames:

1. rename the one-child wrapper `C:\Git\2D\Dev` to the validated temporary
   sibling;
2. rename its `OpenVisionLab_Dev` child to `C:\Git\2D\Dev`;
3. run all equivalence checks against the new root;
4. remove only the verified-empty temporary directory.

The same operation contained a rollback path that would restore the original
wrapper/child shape if the second rename or any equivalence check failed. The
rollback was not needed.

## Equivalence Evidence

| Check | Before | After | Result |
| --- | --- | --- | --- |
| Git root | `C:/Git/2D/Dev/OpenVisionLab_Dev` | `C:/Git/2D/Dev` | PASS |
| Git directory | nested root `.git` | `C:/Git/2D/Dev/.git` | PASS |
| Directory file ID | `0x0000000000000000000300000005224f` | same | PASS |
| HEAD | `5f0bbfb5b5e232ee62214070391541e36908b4cf` | same | PASS |
| Branch/upstream | `codex/public-sample-ux-docs` / matching origin branch | same | PASS |
| Origin | `https://github.com/Noah8218/OpenVisionLab_Dev.git` | same | PASS |
| Exact `git status --short` text | 11 modified + 5 untracked | byte-identical | PASS |
| Managed/untracked file count | 1,995 | 1,995 | PASS |
| Path/length/SHA-256 manifest | `248CA4988607154EC61C7B3B88E1E584C98C2344A7467BB943E72FDA5A7DA8BD` | same | PASS |
| Local `core.worktree` | unset | unset | PASS |
| Old nested path | present | absent | PASS |
| Temporary directory | absent | removed after empty check | PASS |

The manifest was computed in sorted relative-path order over
`git ls-files --cached --others --exclude-standard`; each record included the
relative path, file length, and SHA-256.

## Verification Boundary

This proves an exact path-only relocation of the current Git repository and its
managed/untracked working-tree files. It does not prove correctness of the dirty
source changes, ignored build outputs, auxiliary prunable worktree records,
release readiness, or behavior of `C:\Git\2D\Original`.

## Closure Record

- Status: **Complete**
- Scope: exact same-volume flattening to `C:\Git\2D\Dev`
- Acceptance criteria: physical target root -> PASS; old nested path absent -> PASS; Git identity/state preserved -> PASS; managed/untracked content manifest preserved -> PASS; temporary path removed -> PASS
- Verification: `git rev-parse`, branch/upstream/origin checks, exact `git status`, per-file SHA-256 manifest, `fsutil file queryfileid`, and post-move path checks
- Evidence: this report and the command results from the 2026-08-30 migration task
- Boundary / next dependency: dirty source correctness and any auxiliary worktree cleanup require separate verification; Original and external release stages were untouched
