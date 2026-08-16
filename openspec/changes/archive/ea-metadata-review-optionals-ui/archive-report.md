# Archive Report: EA Metadata Review Optionals and UI

**Change**: ea-metadata-review-optionals-ui
**Date**: 2026-08-15
**Mode**: hybrid
**Status**: archived

## Scope

EA Metadata Review form enhancements: strict name validation gate, recursive iterative DFS element loader with `Paquete` path, dirty indicator lifecycle, reload with Spanish confirmation dialog, and visual refresh with native caption fallback. No Git delivery action (commit or PR) was performed; artifacts were archived directly.

## Artifacts Archived

- `proposal.md`
- `specs/ea-metadata-review/spec.md`
- `design.md`
- `exploration.md`
- `tasks.md` — 33/33 implementation tasks complete
- `verify-report.md`
- `archive-report.md` (additive)

## Task Completion

All 33 implementation tasks across 6 phases marked complete in the persisted tasks artifact. No unchecked tasks.

## Verification

Per verify-report: build passed (x64, exit 0), human gates HG1–HG5 passed (including re-HG5). HG1-E PASS, HG2 cycle/repetition PASS, HG3-G PASS, HG4-B PASS. Only HG4-I remains N/A — not safely reproduced / static review, non-blocking. R-01 through R-13 planned-not-executed. No CRITICAL issues.

## Mechanical Archive Integrity Recovery

The original archive move completed correctly (files intact, byte-identical). The prior archive report was marked INCOMPLETE solely because the pre-move snapshot was cleaned before `diff.exe -r` could run — a process timing defect, not a data corruption.

Recovery procedure (2026-08-15T03:17:37):

1. Moved `openspec/changes/archive/2026-08-15-ea-metadata-review-optionals-ui` back to `openspec/changes/ea-metadata-review-optionals-ui` (Move-Item).
2. Created recursive pre-move snapshot at `$TEMP/sdd-archive-recovery-20260815031737/source` via `robocopy /MIR`.
3. Moved source back to `openspec/changes/archive/2026-08-15-ea-metadata-review-optionals-ui` (Move-Item).
4. Verified source absent.
5. Ran `C:\Program Files\Git\usr\bin\diff.exe -r` between snapshot and archive.

### diff.exe -r Readback Evidence

```
diff.exe exit code: 0
diff output: (empty — no differences)
```

Empty output with exit code 0 confirms byte-identity between pre-move snapshot and restored archive.

## Specs Synced

| Domain | Action | Details |
|--------|--------|---------|
| ea-metadata-review | Updated (4 added, 2 modified) | Delta spec merged into existing canonical `openspec/specs/ea-metadata-review/spec.md` — 4 requirements added (Strict pre-Save blank Name validation, Dirty-row visual indicator, Reload from current package, Moderate EA visual refresh), 2 requirements modified (Direct element loading, Excluded optional work) |

## Source of Truth Updated

- `openspec/specs/ea-metadata-review/spec.md` — reflects new validation gate, loader, dirty indicator, reload, and UI requirements.

## Protected Scope Preserved

No changes to: `.csproj`, `AssemblyInfo.cs`, COM registration, solution files, docs, evidence, `Exercise_2_queries/**`, project/config, archived changes, or Engram semantic facts. The canonical spec (`openspec/specs/ea-metadata-review/spec.md`) was the intentional archive-sync exception — delta requirements merged into it during the archive phase per the openspec convention.

## Exercise 1 Delivery PDF Recovery

Post-archive validation (2026-08-15) found three tracked Exercise 1 delivery PDFs deleted from the working tree, despite the archive contract requiring they remain untouched:

- `Exercise_1_Addin/docs/delivery/Pino_Evidencias_Pruebas_Funcionales_Addino.pdf`
- `Exercise_1_Addin/docs/delivery/Pino_Guia_Ejecucion_Addino.pdf`
- `Exercise_1_Addin/docs/delivery/Pino_Registro_Uso_IA.pdf`

Recovery procedure:

1. Verified deletion via `git status` (all three listed as `deleted:`).
2. Restored to HEAD bytes via `git restore` (native Git command, no model content routing).
3. Verified `git diff --name-only -- Exercise_1_Addin/docs/delivery/` is empty — no staged or unstaged differences.
4. Confirmed `git status` shows `nothing to commit, working tree clean` for the delivery directory.
5. Archive/report source paths `openspec/changes/archive/2026-08-15-ea-metadata-review-optionals-ui/` remain unchanged.
6. No staging, no commit — restoration only.

Result: All three PDFs restored. Protected scope preserved. No other files modified.

## Key Learnings

1. Pre-move snapshot must survive until diff.exe runs; cleanup traps must not fire before verification completes.
2. Mechanical archive copy contract requires diff.exe -r evidence in the phase result, not just file presence.
3. PowerShell `diff` alias must not be used; Git Bash diff.exe is the required readback tool on Windows.
