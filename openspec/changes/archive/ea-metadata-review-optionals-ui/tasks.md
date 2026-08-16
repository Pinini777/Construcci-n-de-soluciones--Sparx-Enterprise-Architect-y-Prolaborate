# Tasks: EA Metadata Review Optionals and UI

## Review Workload Forecast

| Field | Value |
|-------|-------|
| Estimated changed lines | ~300–400 |
| 400-line budget risk | Medium |
| Chained PRs recommended | No |
| Suggested split | Single PR; slice per phase if any increment exceeds ~120 lines |
| Delivery strategy | ask-on-risk |
| Chain strategy | single-pr |

Decision needed before apply: No
Chained PRs recommended: No
Chain strategy: single-pr
400-line budget risk: Medium

### Suggested Work Units

| Unit | Goal | Likely PR | Focused test command | Runtime harness | Rollback boundary |
|------|------|-----------|----------------------|-----------------|-------------------|
| 1 | Name validation gate | PR 1 | `msbuild Exercise_1_Addin\Addino.csproj /t:Build /p:Configuration=Debug /p:Platform=x64` | Human Gate 1 in EA | `MetadataReviewForm.cs` validation block only |
| 2 | Recursive loader + Paquete | PR 1 | Same x64 build command | Human Gate 2 in EA | `AddinoClass.cs` loader + `MetadataElementRow.PackagePath` |
| 3 | Dirty indicator | PR 1 | Same x64 build command | Human Gate 3 in EA | `MetadataReviewForm.cs` grid events only |
| 4 | Reload | PR 1 | Same x64 build command | Human Gate 4 in EA | `MetadataReviewForm.cs` reload handler only |
| 5 | Visual refresh | PR 1 | Same x64 build command | Human Gate 5 in EA | `MetadataReviewForm.Designer.cs` header/button styles only |

## Completion Criteria

All five functional increments pass x64 build and their named human gate in Enterprise Architect. No `Element.Update()` runs when Name validation fails. Loader shows each element once with correct `Paquete` path. Dirty highlight reacts only to `IsDirty`. Reload never silently discards or auto-saves. UI retains native chrome. Exercise 2 and archived artifacts remain untouched.

## Protected Scope

Modify only `Exercise_1_Addin/AddinoClass.cs`, `Exercise_1_Addin/MetadataElementRow.cs`, `Exercise_1_Addin/MetadataReviewForm.cs`, and `Exercise_1_Addin/MetadataReviewForm.Designer.cs`. Do not touch `.csproj`, `AssemblyInfo.cs`, COM registration, solution files, docs, evidence, `openspec/specs/ea-metadata-review/spec.md`, archived changes, or `Exercise_2_queries/**`. If a task requires a protected change, stop Apply and report the blocker.

## Task Summary

- 33 total tasks distributed across 6 phases: Phase 1 (6), Phase 2 (7), Phase 3 (5), Phase 4 (7), Phase 5 (6), Phase 6 (2).

## Phase 1: Name validation

- [x] 1.1 Finalize active cell and collect dirty rows in `GuardarButton_Click` before any `Element.Update()`.
- [x] 1.2 Add global `string.IsNullOrWhiteSpace(Name)` sweep over every dirty row before the Update loop.
- [x] 1.3 On any failure: perform zero `Element.Update()` calls, aggregate invalid row identifiers, keep form open, show one Spanish summary.
- [x] 1.4 Add a presentation-only Name-invalid marker (pale red) separate from `IsDirty`; clear it when the Name is corrected.
- [x] 1.5 Preserve the zero-dirty-row branch with no validator invocation.
- [x] 1.6 Build x64 from cwd `Exercise_1_Addin` (`msbuild Addino.csproj /t:Build /p:Configuration=Debug /p:Platform=x64`) and run Human Gate 1: blank, whitespace, multiple invalid+valid, zero Update, untouched clean blank, correction saves. Record exit code/errors/warnings and obtain operator approval before advancing.
  - HG1-A PASS, HG1-B PASS, HG1-C PASS, HG1-D PASS, HG1-E N/A / not reproduced, HG1-F PASS, HG1-G PASS.

## Phase 2: Recursive loader + Paquete

- [x] 2.1 Refactor `LoadPackageElements` into a shared internal iterative DFS loader usable by open and Reload.
- [x] 2.2 Traverse root/package elements before descendants, preserving EA collection order; push child packages in reverse to maintain order.
- [x] 2.3 Guard with `HashSet<int>` for `PackageID` and `ElementID`; warn and skip repeated/cyclic package branches only.
- [x] 2.4 Skip per-element only when a COM read fails; aggregate per-package incomplete-load warnings in Spanish.
- [x] 2.5 Add immutable, derived `PackagePath` to `MetadataElementRow`; bind read-only `Paquete` column in `BindGrid`.
- [x] 2.6 Keep `Name`/`Alias`/`Notes` editable and `Type`/`Stereotype`/`Paquete` read-only.
- [x] 2.7 Build x64 from cwd `Exercise_1_Addin` (`msbuild Addino.csproj /t:Build /p:Configuration=Debug /p:Platform=x64`) and run Human Gate 2: direct/deep/siblings/empty tree, exact path, element count, no duplicates, warnings. Record exit code/errors/warnings and obtain operator approval.
  - HG2-A PASS, HG2-B PASS, HG2-C PASS, HG2-D PASS, HG2-E PASS, HG2-F PASS, HG2-G PASS, HG2-H PASS, HG2-I PASS. Warnings/cycles N/A / static review.

## Phase 3: Dirty indicator

- [x] 3.1 Wire `CellValueChanged` only to refresh/invalidate the affected row; keep `IsDirty` as the sole authority.
- [x] 3.2 Use `CellFormatting` to paint dirty rows with `#FFF4CE` across editable cells.
- [x] 3.3 Clear the indicator on `AcceptChanges` after successful Save; retain it on failure or manual reversion.
- [x] 3.4 Keep Name-invalid marker independent and with visual priority over dirty highlight.
- [x] 3.5 Build x64 from cwd `Exercise_1_Addin` (`msbuild Addino.csproj /t:Build /p:Configuration=Debug /p:Platform=x64`) and run Human Gate 3: one/multiple dirty rows, revert, Save success, partial failure, Cancel, Esc, X. Record exit code/errors/warnings and obtain operator approval.
  - HG3-A PASS, HG3-B PASS, HG3-C PASS, HG3-D PASS, HG3-E PASS, HG3-F PASS, HG3-G N/A / not safely reproduced / static review, HG3-H PASS.

## Phase 4: Reload

- [x] 4.1 Add `Recargar` button to the bottom panel and wire `RecargarButton_Click`.
- [x] 4.2 End active edit first; reload immediately when no rows are dirty.
- [x] 4.3 When dirty, show Spanish Yes/No only: "Hay cambios sin guardar. Recargar descartará esas modificaciones. ¿Desea continuar?".
- [x] 4.4 Yes discards pending changes and reloads; No is an exact no-op; never Save or Update during Reload.
- [x] 4.5 Refill the existing `BindingList` in place through the shared root loader; on catastrophic failure keep the old grid.
- [x] 4.6 Best-effort restore selected `ElementID`, current column, and scroll position; display reload warnings.
- [x] 4.7 Build x64 from cwd `Exercise_1_Addin` (`msbuild Addino.csproj /t:Build /p:Configuration=Debug /p:Platform=x64`) and run Human Gate 4: clean reload, external change, active edit, dirty Yes/No, zero Update, catastrophic failure. Record exit code/errors/warnings and obtain operator approval.
  - HG4-A PASS, HG4-B N/A / not safely reproduced, HG4-C PASS, HG4-D PASS, HG4-E PASS, HG4-F PASS, HG4-G PASS, HG4-H PASS, HG4-I N/A / not safely reproduced / static review, HG4-J PASS.

## Phase 5: UI

- [x] 5.1 ~~Add an internal top header panel with `#557DA5` background and white title text; retain native caption and border.~~ HG5 rejected: internal header overlaps grid, hides headers/first rows, duplicates title, and breaks resize. Replaced with best-effort native caption coloring only.
- [x] 5.2 Style `Guardar` as blue/white primary with an integrated flat border; keep `Recargar` and `Cancelar` as native secondary buttons.
- [x] 5.3 Apply `#557DA5`/white to grid headers only when fully visible and readable via `EnableHeadersVisualStyles = false`; otherwise leave native.
- [x] 5.4 Place actions bottom-right; set tab order grid → Guardar → Recargar → Cancelar.
- [x] 5.5 Use dock/anchor for resize; no external libraries, custom chrome, or additional P/Invoke beyond the single bounded `DwmSetWindowAttribute` call.
- [x] 5.6 Build x64 from cwd `Exercise_1_Addin` (`msbuild Addino.csproj /t:Build /p:Configuration=Debug /p:Platform=x64`) and run re-Human Gate 5: verify no internal header/title duplicate, grid headers/rows fully visible, native caption best-effort/fallback, `Guardar` finish, resize behavior, and full HG1–4 functional regression. Record exit code/errors/warnings and obtain operator approval.
  - re-HG5 PASS — native caption/fallback, no overlap, resize, grid/header/buttons legibility, and HG1–HG4 regression all passed in Enterprise Architect.

## Phase 6: Full regression plan

- [x] 6.1 List baseline + optional manual checks R-01..R-13 from spec/design as planned, not executed, and define evidence to capture per gate.
- [x] 6.2 Defer `README.md`, `AI_USAGE_LOG.md`, evidence screenshots/video, and delivery PDF updates until after functional approval.

### Planned-not-executed baseline + optional regression checklist

These checks were derived from the approved Spec and Design but were **not executed** as part of this change. They are retained as a closure baseline for any future full regression pass.

| Check | Source / focus | Expected evidence if run |
|---|---|---|
| R-01 | Spec: Strict Name validation — single blank dirty row blocks Save | Screenshot of Spanish error message; EA audit log showing zero `Element.Update()` for the row. |
| R-02 | Spec: Strict Name validation — whitespace-only Name | Same as R-01, with the Name value visible in the grid or repro notes. |
| R-03 | Spec: Strict Name validation — multiple invalid rows aggregated | Screenshot listing all invalid row identifiers in one message; zero updates confirmed. |
| R-04 | Spec: Strict Name validation — untouched pre-existing blank Name ignored | Save succeeds for the valid dirty row; untouched blank row is not listed in the error. |
| R-05 | Spec: Dirty indicator — edit/revert/save/partial failure lifecycle | Screenshots before/after revert, after successful Save, and after partial failure; grid row background states. |
| R-06 | Spec: Reload — clean reload reflects external EA changes | Before/after grid values for an element modified externally; no dirty indicator after reload. |
| R-07 | Spec: Reload — dirty Yes discards/No preserves | Screenshot of Spanish Yes/No dialog; grid values unchanged after No, refreshed after Yes. |
| R-08 | Spec: Direct element loading — direct/deep/sibling elements with exact `Paquete` path | Grid export or screenshots showing element order, counts, and `Paquete` values. |
| R-09 | Spec: Direct element loading — cycle/repetition protection and warnings | EA model IDs of repeated packages; warning message text; no infinite loop. |
| R-10 | Spec/Design: Visual refresh — native caption color/fallback, no internal header | Screenshot of form caption (or standard native caption on fallback systems); no duplicate title/overlap. |
| R-11 | Design: UI — primary/secondary buttons, tab order, resize behavior | Screenshot of button panel; Tab focus recording; resized form showing anchored controls. |
| R-12 | Design: Accessibility/contrast — header/button legibility, focus rectangles | Close-up screenshots of grid headers and focused buttons; contrast check notes. |
| R-13 | Spec: Excluded optional work — no new-element creation action exists | Screenshot of the form showing only Guardar/Recargar/Cancelar; no create button or menu. |

### Deferred documentation and evidence

The following artifacts remain unmodified by this change and are scheduled for update only after functional approval:

- `README.md`
- `AI_USAGE_LOG.md`
- Evidence screenshots and screen-capture video
- Delivery PDFs

No functional files, `Exercise_2_queries/**`, project configuration, COM registration, canonical spec, or archived artifacts were changed during Phase 6.
