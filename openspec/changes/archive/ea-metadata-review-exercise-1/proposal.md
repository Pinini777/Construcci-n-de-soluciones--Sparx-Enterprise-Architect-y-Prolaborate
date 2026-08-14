# Proposal: EA Metadata Review Exercise 1

## Intent

Replace the tutorial demo commands with a package-scoped Enterprise Architect metadata editor.

## Scope

### In Scope
- Replace demo commands with an enabled Extensions action; retain `EA_Connect`, `EA_GetMenuItems`, `EA_GetMenuState`, `EA_MenuClick`, and `EA_Disconnect` callbacks.
- Validate the selected Project Browser item as an `EA.Package`; otherwise show a Spanish message and stop.
- Modal Spanish Windows Forms grid of direct elements: editable Name/Alias/multiline Notes; read-only Type/Stereotype.
- Edit a local DTO `BindingList`; retain internal modified-row detection so Save calls `Element.Update()` only for changed rows, aggregating errors.
- Deliver a classic Visual Studio solution for Addino, execution README, functional screenshots/video, and separate AI Usage Log. Final delivered artifacts and packaging follow the challenge surname convention; resolve the exact packaging strategy before delivery. Keep `Addino`, project, namespace, assembly, classes, and source-file names unchanged for stable development.

### Out of Scope
- Recursive loading, reload/add controls, visual dirty-row indicators/highlighting, and non-empty Name validation.
- Exercise 2/Prolaborate, retaining or extending tutorial demo actions, and unnecessary framework/x64/COM changes. Allow minimal adjustments only for portability or execution.

## Capabilities

### New Capabilities
- `ea-metadata-review`: Package metadata review, safe editing, explicit persistence, and delivery documents.

### Modified Capabilities
None; `openspec/specs/` has no existing capability specifications.

## Approach

Route callbacks through the action after type/object validation. Load `EA.Package.Elements` into local rows; only Save independently calls `Update()` with result/exception checks. Cancel/Esc/X do not invoke `Update()` or persist unsaved local changes; they do not undo previously saved EA data. Retain the valid `Interop.EA` path; adjust minimally only for portability/open/build. Use external Repositorio Pasantías only for manual validation.

## Affected Areas

| Area | Impact | Description |
|------|--------|-------------|
| `AddinoClass.cs` | Modified | Action routing and validation. |
| WinForms `*.cs`, `Addino.csproj` | New/Modified | Grid, DTO binding, save flow, source entries. |
| `Addino.sln` | New | Classic Visual Studio solution; final delivered artifacts and packaging follow the surname convention, with exact packaging resolved before delivery. Internal Addino project, namespace, assembly, class, and source-file names remain stable. |
| `README.md` | New | Execution instructions for unassisted use. |
| Screenshots/video | New | Functional evidence of the completed flow and EA impact. |
| AI Usage Log | New | Audit record of significant AI-assisted decisions and evidence. |

## Risks

| Risk | Likelihood | Mitigation |
|------|------------|------------|
| Absent or invalid selection | Med | Show a clear message and stop before opening the dialog. |
| Locked element or `Update()` failure | Med | Continue saving other rows and report per-row errors. |
| EA installation differs from the validated target | Med | Enterprise Architect 17.1 x64 is the currently validated challenge target; Trial edition is not a requirement. Do not claim compatibility for other EA versions or architectures until separately validated with COM/Automation and `Interop.EA`. |

## Rollback Plan

Revert code/configuration with Git. Remediate already-saved EA metadata separately.

## Dependencies

- .NET Framework 4.7.2; EA COM/Automation and `Interop.EA`; Visual Studio build/COM-registration prerequisites; external Repositorio Pasantías. Enterprise Architect 17.1 x64 is the currently validated challenge target; Trial edition is not a requirement. No compatibility claim is made for other EA versions or architectures unless separately validated.

## Success Criteria

- [ ] The add-in registers and starts without alerts/blocks; `EA_Connect`, `EA_GetMenuItems`, `EA_GetMenuState`, `EA_MenuClick`, and `EA_Disconnect` callbacks remain available.
- [ ] An absent selection or one other than `EA.Package` shows a clear Spanish message, stops the flow, and does not open the editor.
- [ ] The Extensions action validates the package and shows only direct elements: editable Name/Alias/multiline Notes; read-only Type/Stereotype.
- [ ] Edits remain local until Save; Cancel, Esc, and window close do not invoke `Update()` or persist unsaved local changes, and do not undo previously saved EA data.
- [ ] Save uses `Update()` for changed rows: successes show a clear message; failures retain independent processing and show relevant row details.
- [ ] README enables unassisted execution: prerequisites; opening EA; selecting package; menu route; grid; edit/save/cancel; validation/errors; persisted-change verification.
- [ ] Screenshots and short video show the complete flow and reflected EA impact.
- [ ] A classic Visual Studio solution opens/builds for Addino; final delivered artifacts and packaging follow the challenge surname convention, with exact packaging resolved before delivery. Keep Addino, project, namespace, assembly, class, and source-file names unchanged for stable development; source is clean, organized, strictly typed, without tutorial demo actions.
- [ ] The separate AI Usage Log records at least five significant interactions; each includes ID, objective, tool/model, strategy or prompt, decision made, evidence, and result.
