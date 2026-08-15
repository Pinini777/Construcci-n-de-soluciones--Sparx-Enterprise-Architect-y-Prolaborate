# Proposal: EA Metadata Review Optionals and UI

## Intent

Extend the stable Exercise 1 metadata editor so operators can review a complete package tree, safely manage pending edits, and use a restrained EA-inspired interface—without changing add-in identity or persistence boundaries.

## Scope

### In Scope
- Strict pre-Save validation: inspect every dirty row with `string.IsNullOrWhiteSpace(Name)`; on any failure, perform zero `Element.Update()` calls, retain changes, highlight/select invalid rows, keep the form open, and show one Spanish summary. Untouched pre-existing blank Names do not block other saves.
- Default full-tree loading: iteratively traverse the root package and all descendants, protect with visited `PackageID`s, show each element once, and expose a read-only `Paquete` path column.
- Dirty-row highlight driven solely by `MetadataElementRow.IsDirty`; clear it after successful `AcceptChanges`, retain it for failures.
- Reload through the same loader used at open: reload immediately when clean; otherwise show “Hay cambios sin guardar. Recargar descartará esas modificaciones. ¿Desea continuar?”. Yes discards and reloads; No preserves exact state. Never auto-save or silently discard.
- Restrained WinForms refresh: `#557DA5` blue header with white text, blue primary Guardar, sober secondary actions. Best-effort native caption coloring via DWM `DWMWA_CAPTION_COLOR`/`DWMWA_TEXT_COLOR` with standard native-caption fallback; no internal header, custom chrome, or additional P/Invoke.

### Out of Scope
- Element creation, recursion mode/MaxDepth controls, external UI libraries, custom window chrome, broad refactors, COM/configuration changes, Exercise 2, and current docs/evidence updates.

## Capabilities

### New Capabilities
None.

### Modified Capabilities
- `ea-metadata-review`: replace excluded optional behavior with recursive review, strict dirty-row Name validation, dirty indication, Reload safety, and restrained UI requirements.

## Approach

Keep `Addino.AddinoClass` callbacks and the .NET Framework 4.7.2 x64 COM baseline intact. Share one iterative, cycle-safe loader between initial open and Reload; derive package paths only for display. Preserve dirty-only, independent-row Save failures after the all-row validation gate.

## Affected Areas

| Area | Impact | Description |
|---|---|---|
| `Exercise_1_Addin/AddinoClass.cs` | Modified | Shared iterative tree loader. |
| `Exercise_1_Addin/MetadataElementRow.cs` | Modified | Derived package path; existing dirty authority. |
| `Exercise_1_Addin/MetadataReviewForm*.cs` | Modified | Validation, Reload, visuals. |

## Risks

| Risk | Likelihood | Mitigation |
|---|---|---|
| Cycles/large trees | Med | Visited IDs; iterative traversal; EA review. |
| Reload data loss | Low | Explicit Yes/No confirmation only. |
| Fragile caption styling | Med | Best-effort DWM native caption; standard native-caption fallback on failure. |

## Rollback Plan

Revert the change files to restore the archived baseline; no model migration or COM registration change is introduced.

## Dependencies

- Live Enterprise Architect manual validation; later documentation and evidence updates only after implementation.

## Success Criteria

- [ ] Increments run in order: Name validation, recursive loader/Paquete, dirty highlight, Reload, visual refresh.
- [ ] EA human gate after each increment proves: zero Update on invalid Name; highlight lifecycle; exact duplicate-free tree; Reload external state/no silent discard; Save/Cancel/Esc/X regression; visual review.
- [ ] All baseline callbacks, editability, dirty-only Save, and independent row failures remain intact.
- [ ] No unresolved product decisions remain.
