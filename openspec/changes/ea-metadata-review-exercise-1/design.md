# Design: EA Metadata Review Exercise 1

## Technical Approach

Extend the existing COM callback class with one modal WinForms editor. `AddinoClass` validates the selected object, copies direct package elements into plain local rows, and opens the form with the EA repository plus a `BindingList<MetadataElementRow>`. The form owns editing and the explicit per-row save loop. This is deliberately proportional: no interfaces, repositories, dependency injection, generic layers, or artificial services.

Preserve C# 7.3, .NET Framework 4.7.2, WinForms, `Interop.EA`, COM visibility/registration, x64, and the EA 17.1 x64 target. Preserve the current embedded Interop reference and HintPath unless a demonstrated portability/build need requires the smallest path adjustment; make no Trial-edition compatibility claim.

## Architecture Decisions

| Decision | Alternatives / tradeoff | Rationale |
|---|---|---|
| Callback class + form + row DTO | Layered services improve substitution but add unjustified indirection | The feature has one entry point, one dialog, and one persistence boundary. |
| DTO stores `ElementId`, current strings, and original strings; never `EA.Element` or another COM object | Binding COM objects is shorter but can leak writes/lifetime concerns | Stable EA identity survives name/alias edits and keeps all edits local. |
| Independent dirty-row saves | Transaction-like all-or-nothing behavior is unavailable through this API | Matches the frozen spec's partial-success lifecycle. |

## Data Flow

`EA_MenuClick` → require `Repository.GetTreeSelectedItemType() == EA.ObjectType.otPackage` → only then call `GetTreeSelectedObject()` and verify/cast it to `EA.Package` → direct `EA.Package.Elements` only → copy rows → Spanish modal dialog → explicit Save → `Repository.GetElementByID(ElementId)` temporarily → assign editable fields → `Update()`.

If the item-type check fails, or the selected object cannot be obtained and verified/cast as `EA.Package`, the flow shows a Spanish message and stops without opening the form. Collection-level load failure stops with a Spanish error; an unreadable child is omitted and identified in a load warning. An empty package opens an empty grid without error.

## File Changes

| File | Action | Description |
|---|---|---|
| `AddinoClass.cs` | Modify | Keep all five callback contracts; expose one enabled Extensions action and remove Say Hello/Goodbye; validate/load/open. |
| `MetadataReviewForm.cs`, `MetadataReviewForm.Designer.cs` | Create | Modal Spanish grid and save/cancel behavior. |
| `MetadataElementRow.cs` | Create | COM-free local state, identity, dirty comparison, and baseline acceptance. |
| `Addino.csproj` | Modify | Add explicit `Compile` entries for WinForms files; preserve baseline settings. |
| `Addino.sln` | Create | Classic Visual Studio solution for the existing project; `Addino.slnx` may remain. |
| `README.md`, `AI_USAGE_LOG.md` | Create | Execution guidance and required interaction log. |
| Final screenshots/video | Plan only | Later capture selection → grid → edit → save → reflection in EA; no evidence is produced in this phase. |

This design phase changes no proposal, spec, exploration, or production code.

## Interfaces / Contracts

`MetadataElementRow` has immutable `ElementId`; editable `Name`, `Alias`, `Notes`; read-only `Type`, `Stereotype`; and independently settable internal `OriginalName`, `OriginalAlias`, `OriginalNotes`. `IsDirty` uses ordinal comparisons of each editable value against its original. `AcceptChanges()` copies all current editable values into the original baseline after successful persistence.

The grid explicitly configures Name/Alias editable, Notes editable with wrapping and multiline editing, and Type/Stereotype read-only. Save first commits the active cell and binding edit. The form has no `AcceptButton`; Enter never saves and remains available to Notes editing. Escape, Cancel, and X close and discard pending local values without `Update()`; already-saved EA values remain.

Save visits only dirty rows. It retrieves each `EA.Element` by `ElementId` for that attempt, sets only Name/Alias/Notes, and calls `Update()`. Success calls `AcceptChanges()` so a later unchanged Save does not update it. Missing/unrecoverable elements, `Update() == false`, COM exceptions, and locked/unwritable elements are recorded per row; processing continues. Failures retain both current local data and the prior original baseline. Spanish summaries distinguish full success, no pending changes, and partial success with element identifiers and actionable errors.

## Testing Strategy

| Layer | Planned verification |
|---|---|
| Logic | Dirty comparisons, `AcceptChanges()`, retry after failure, and second Save without updates. |
| Build/integration | Existing x64 MSBuild command, five callbacks, explicit source inclusion, and COM baseline inspection. |
| Manual EA (later) | Invalid/empty/load-error cases; modal grid/editability; Enter/Notes safety; success/false/exception/lock/partial saves; Cancel/Esc/X; persisted reflection. Screenshots/video are produced only after implementation. |

## Threat Matrix

N/A — this change adds no shell/subprocess execution, VCS/PR automation, executable-file classification, or command-routing boundary; EA callbacks remain in-process application callbacks.

## Migration / Rollout

No data migration or feature flag. Roll back source/configuration with Git; already-persisted EA metadata requires separate remediation.

## Traceability, Tradeoffs, and Risks

The flow and contracts implement every frozen `ea-metadata-review` requirement, while excluding recursion, creation, reload, dirty highlighting, and empty-Name blocking. The main tradeoff is direct form orchestration over test seams; mitigation is a COM-free row model, a narrow explicit save boundary, build checks, and live-EA scenarios. Remaining risks are machine-specific Interop paths, COM registration permissions, and repository locking; preserve the validated setup, adjust only on demonstrated portability need, and report Spanish per-row failures.

## Open Questions

None.
