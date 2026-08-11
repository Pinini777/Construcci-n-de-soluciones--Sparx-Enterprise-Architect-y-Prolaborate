# Tasks: EA Metadata Review Exercise 1

## Review Workload Forecast

| Field | Value |
|-------|-------|
| Estimated changed lines | ~450-550 |
| 400-line budget risk | Medium |
| Chained PRs recommended | No |
| Suggested split | Single size-exception PR; 3 checks |
| Delivery strategy | exception-ok |
| Chain strategy | size-exception |

Decision needed before apply: No
Chained PRs recommended: No
Chain strategy: size-exception
400-line budget risk: Medium

### Suggested Work Units

| Unit | Goal | Likely PR | Focused test command | Runtime harness | Rollback boundary |
|------|------|-----------|----------------------|-----------------|-------------------|
| 1 | Callbacks, validation, load, DTO, csproj/sln | size-exception commit 1 | `msbuild Addino.csproj /t:Build /p:Configuration=Debug /p:Platform=x64` | N/A | `AddinoClass.cs`, `MetadataElementRow.cs`, `Addino.csproj`, `Addino.sln` |
| 2 | Modal Spanish grid, columns, binding | size-exception commit 2 | Build; open via EA Extensions | Manual EA column check | `MetadataReviewForm.cs`, `.Designer.cs` |
| 3 | Save, errors, README, AI log, evidence | size-exception commit 3 | Build; edit/save rows | Manual EA lifecycle | `MetadataReviewForm.cs` save, `README.md`, `AI_USAGE_LOG.md`, final evidence |

## Phase 1: Foundation

- [x] 1.1 `AddinoClass.cs`: replace menus with `MenuReview`; keep 5 callbacks. Verify: only review action, builds.
- [x] 1.2 `AddinoClass.cs`: validate `EA.Package`; Spanish message otherwise. Verify: non-package stops.
- [x] 1.3 `MetadataElementRow.cs`: identity, editable fields, originals, `IsDirty`, `AcceptChanges()`. Verify: dirty/reset.
- [x] 1.4 `Addino.csproj`, `Addino.sln`: add `Compile`, create solution. Verify: x64 MSBuild.
- [x] 1.5 `AddinoClass.cs`, `MetadataElementRow.cs`: load ONLY direct `EA.Package.Elements` (no subpackages/recursion); COM-free row per child with `ElementId`, `Name`, `Alias`, `Notes`, `Type`, `Stereotype`; collection before form; empty ok; Spanish handling, no leak. Needs: 1.2, 1.3. Verify: direct-only, empty grid, errors handled.

## Phase 2: Core UI

- [ ] 2.1 `MetadataReviewForm.cs`, `.Designer.cs`: modal Spanish grid, Save/Cancel. Needs: 1.3-1.5. Verify: opens modally.
- [ ] 2.2 `MetadataReviewForm.cs`, `.Designer.cs`: bind rows; Name/Alias editable, Notes multiline, Type/Stereotype readonly. Verify: editability rules.
- [ ] 2.3 `MetadataReviewForm.cs`: end edit on Save; no `AcceptButton`. Verify: Enter in Notes, Save includes active value.
- [ ] 2.4 `MetadataReviewForm.cs`: Cancel/Esc/X discard local changes without `Update()`. Verify: EA unchanged.

## Phase 3: Save & Error Handling

- [ ] 3.1 `MetadataReviewForm.cs`: end edit, iterate only dirty rows. Verify: no-changes message when clean.
- [ ] 3.2 `MetadataReviewForm.cs`: per dirty row get element by ID, set fields, `Update()`, continue on fail. Verify: failures identified.
- [ ] 3.3 `MetadataReviewForm.cs`: `AcceptChanges()` on success, keep values on failure, Spanish summary. Verify: success/no-changes/partial messages.
- [ ] 3.4 `MetadataReviewForm.cs`: zero dirty rows: no `Update()`, no pending changes. Verify: repeated Save no-op.

## Phase 4: Delivery Docs & Manual Validation

- [ ] 4.1 `README.md`: Spanish README: prerequisites, EA opening, selection, menu path, edit/save/cancel, validation, persist check. Verify: sections present.
- [ ] 4.2 `AI_USAGE_LOG.md`: ≥5 rows incl. `Update() == false`. Verify: required fields.
- [ ] 4.3 EA: `Debug|x64` build, COM registration, 10 checks. Needs: 3.3, 2.4. Verify: checklist complete.
- [ ] 4.4 `README.md`/notes: surname packaging; keep `Addino` names. Verify: strategy written.
- [ ] 4.5 Evidence: screenshots + video of selection → grid → edit → Save → EA reflection. No auto/synthesis. Needs: 4.3. Verify: actual evidence exists — existing screenshots and short video show package selection → grid open → edit → Save → change reflected in EA.
