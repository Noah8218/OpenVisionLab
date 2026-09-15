# OpenVisionLab OVL-23 — Learn document copy policy owner

- **Status:** Complete for one independently verifiable smoke responsibility.
- **Scope:** `tools/PipelineViewerScreenshotSmoke/Program.cs` no longer owns the Learn internal-copy phrase list or matching/reporting logic. The new `LearnDocumentationCopyPolicy` owns document-file checks, visible-copy checks, phrase matching, and the existing error messages.
- **Intentional boundary:** `Program` still owns WPF visual-tree traversal through `CollectVisibleLearnCopy`. It supplies collected learner-visible strings to the policy owner. Window creation, topic/document resolution, capture orchestration, and Recipe/UI behavior remain in their existing owners.
- **Dependency direction:** `Program` -> `LearnDocumentationCopyPolicy`; the policy has no OpenVision window or control dependency and can be exercised without creating WPF UI. The contract entry point runs before WPF application initialization.
- **Observable contract:** Existing Learn topic/document checks still execute in the same call order. Forbidden phrase order and case-insensitive matching are unchanged. Existing exception text and context are preserved.
- **Duplicate-work rule:** The related phrase checks in `OpenVisionReadinessCheck` are a separate repository-readiness tool boundary. They were not merged or re-split because doing so would add a cross-tool dependency without a demonstrated shared runtime owner. Do not repeat or further split this policy unless a new defect or independent ownership need is demonstrated.

## Focused proof

`LearnDocumentationCopyPolicyContract` verifies:

1. Program delegates document and visible-copy checks to the new policy owner.
2. Program no longer declares the old phrase list or assertion methods.
3. The policy retains the phrase list, file read, and ordinal-ignore-case matching rule.
4. Clean document text is accepted.
5. Internal document text is rejected with the existing context-bearing error.
6. Internal evidence language in visible copy is rejected while clean copy is accepted.
7. Case-insensitive matching returns the first configured phrase.

Evidence: `D:\OpenVisionLab-TestData\OpenVisionLab_Dev\ovl23-learn-document-copy-policy-contract-20260909\learn-document-copy-policy-contract.txt`.

## Verification

- Debug build: `dotnet build tools\PipelineViewerScreenshotSmoke\PipelineViewerScreenshotSmoke.csproj -c Debug -p:Platform='Any CPU' -m:1 -nr:false --nologo` — 0 errors, 1 pre-existing `CS8600` warning in `Program.cs`.
- Debug focused contract: `--learn-document-copy-contract` — `LEARN_DOCUMENT_COPY_POLICY_CONTRACT=PASS|checks=7`.
- Existing OVL-22 runner contract rerun — `SMOKE_TARGET_RUNNER_CONTRACT=PASS|checks=10`.
- Release build: `dotnet build tools\PipelineViewerScreenshotSmoke\PipelineViewerScreenshotSmoke.csproj -c Release -p:Platform='Any CPU' -m:1 -nr:false --nologo` — 0 errors, 1 pre-existing `CS8600` warning in `Program.cs` at line 10706.
- Release focused contract: `--learn-document-copy-contract` — `LEARN_DOCUMENT_COPY_POLICY_CONTRACT=PASS|checks=7`.
- Existing OVL-22 runner contract rerun in Release — `SMOKE_TARGET_RUNNER_CONTRACT=PASS|checks=10`.
- `Invoke-RefactorAudit.ps1 -Verify -OutputDirectory D:\OpenVisionLab-TestData\OpenVisionLab_Dev\ovl23-refactor-audit-20260909` — `REFACTOR_AUDIT=PASS|CSharpFiles=791|XamlFiles=59|PartialDeclarations=108|ViewModelUiIoFiles=1|ProjectCycles=0|ShellStorageCalls=0`.
- `TestDocumentationIndex.ps1` — `DocumentationIndex=PASS IndexedPaths=242 Routes=13 RootRedirects=102`.

## Junior readability review

**PASS for this boundary.** A maintainer can find the policy by responsibility name, see the two supported inputs (document path or visible strings), and run the command-line contract without constructing a window. The remaining WPF traversal is explicitly named at its call site rather than hidden inside the policy.

## Next slice

Inspect the remaining `PipelineViewerScreenshotSmoke/Program.cs` fixture, capture, and reporting groups for one concrete independent owner. Use file size only as an investigation signal; do not split a cohesive group or repeat completed OVL-12 through OVL-23 boundaries.
