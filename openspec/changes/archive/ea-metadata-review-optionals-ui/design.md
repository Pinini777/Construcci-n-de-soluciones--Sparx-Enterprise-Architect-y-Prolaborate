# Design: EA Metadata Review Optionals and UI

## Technical Approach

Keep the modal WinForms/COM shape: `AddinoClass` validates `EA.Package` and loads COM-free rows; `MetadataReviewForm` owns editing, explicit Save, Reload, and presentation.
Current: private direct loader. Result: one shared recursive loader and reload-capable form.

```text
EA_MenuClick ─┬─> shared loader(root) ─> BindingList<Row> ─> modal form
Reload ───────┘                                      Save ─> GetElementByID/Update
```

## Architecture Decisions

| Decision | Choice and rationale | Rejected |
|---|---|---|
| Shared loading | An `internal delegate BindingList<MetadataElementRow> PackageLoader(EA.Package root, out List<string> warnings)` targets one iterative DFS in `AddinoClass`; open and Reload invoke it. This is C# 7.3-compatible without extra layers. | Form-owned traversal; direct-only mode; recursion checkbox. |
| Traversal | Pre-order DFS: each package's elements in EA collection order, then child packages in EA collection order (push children in reverse). `HashSet<int>` guards `PackageID`; another guards `ElementID`. No depth cap. `Depth` is omitted because nothing consumes it. | Recursive calls, `MaxDepth`, name-based identity. |
| Row visuals | `IsDirty` remains the only comparison authority. `CellValueChanged` invalidates the row; `CellFormatting` paints dirty rows pale amber (`#FFF4CE`). A presentation-only invalid-ID set paints the Name cell pale red with error text; it does not calculate dirtiness and clears when corrected. | Cached dirty flags or per-cell comparisons. |
| Reload | End grid/binding edits, then inspect `IsDirty`. Clean reloads; dirty shows Spanish Yes/No only. Yes discards/loads; No changes nothing. Never call Save/`Update`. | Auto-save, silent discard, Yes/No/Cancel. |
| Window style | Best-effort native caption coloring via the single bounded P/Invoke `DwmSetWindowAttribute` with `DWMWA_CAPTION_COLOR`/`DWMWA_TEXT_COLOR` (Windows 11 Build 22000+). Apply after handle creation in `OnHandleCreated`; swallow any DWM/interop/HRESULT failure and leave the standard native caption. No internal header panel, duplicate title, custom chrome, `FormBorderStyle.None`, manual buttons, additional P/Invoke, DWM hacks, or external libraries. Grid headers may use `#557DA5`/white when readable. | Internal header band (rejected by HG5 for overlapping grid and breaking resize); borderless form; fragile native code; libraries. |

## Responsibilities and Contracts

| File | Change |
|---|---|
| `Exercise_1_Addin/AddinoClass.cs` | Keep callbacks/validation; implement shared DFS and pass root/delegate. |
| `Exercise_1_Addin/MetadataElementRow.cs` | Add immutable, derived `PackagePath`; never persist it. Preserve original-value fields, `IsDirty`, and `AcceptChanges`. |
| `Exercise_1_Addin/MetadataReviewForm.cs` | Add `Paquete`, Save gate, visual events, Reload/refill, Spanish summaries. |
| `Exercise_1_Addin/MetadataReviewForm.Designer.cs` | Add Recargar and native-caption DWM P/Invoke; retain sizable native form, grid, and Cancel wiring. |

Per-package and per-element COM failures become warnings; only the affected item/branch is skipped. Repeated packages warn and stop that branch. Build rows off-grid. On success, detach the grid, refill the existing `BindingList`, reattach, and best-effort restore selected `ElementID`/column and scroll row. Aggregate warnings in Spanish; catastrophic Reload failure preserves old rows.

## Save, Dirty, and UI Flows

Save finalizes edits, collects dirty rows, preserves the no-change branch, then globally filters `string.IsNullOrWhiteSpace(Name)` before any `Element.Update()`. Invalid rows mean zero updates, one Spanish summary, retained edits/open form, and Name markers. Correction clears markers and restores normal Save. Afterward, independent row failures continue; only successes call `AcceptChanges`, clearing their dirty style.

Layout: native caption directly above a fill-docked grid; actions bottom-right; **Guardar** blue/white primary with an integrated flat border, **Recargar/Cancelar** native secondary. Grid headers may use `#557DA5`/white with `EnableHeadersVisualStyles=false` when theme-readable and fully visible. Tab order: grid, Guardar, Recargar, Cancelar; panels dock, buttons anchor right. No internal header panel or duplicate title.

## Testing Strategy and Human Gates

Order: **Name → recursive+Paquete → dirty → Reload → UI → full regression**. Successful x64 compilation is required before each gate; no automated test project exists.

| Gate | Manual proof retained |
|---|---|
| 1 — Name | Blank/whitespace/multiple-invalid produce zero Updates; untouched blank is ignored; correction saves. Retain message and EA before/after. |
| 2 — recursive+Paquete | Root/deep/sibling/empty trees, exact duplicate-free order/path, recoverable warnings. Retain IDs, paths, counts, warning. |
| 3 — dirty | Edit/multiple/revert/success/partial failure and Cancel/Esc/X. Retain screenshots and EA persistence. |
| 4 — Reload | Active edit; clean/external refresh; dirty Yes discard/No preserve; no Update. Retain prompts and values. |
| 5 — UI | Header/buttons/tab/resize/native chrome/accessibility, then full callback, selection, editability, Save/error/cancel regression. Retain screenshots/checklist. |

Rollback boundary is each increment's four-file diff; revert the failing increment without model/config migration. Residual risks are large-tree COM latency, unusual collection enumeration failures, theme contrast, and best-effort viewport restoration.

## Traceability

| Requirement / scenarios | Component | Gate |
|---|---|---|
| Strict Name validation (6 scenarios) | Form Save/validation marker | 1 |
| Direct element loading (7) | Shared loader/row/Paquete | 2 |
| Dirty indicator (6) | Row `IsDirty`/grid events | 3 |
| Reload (6) | Form/delegate/refill | 4 |
| Moderate visual refresh (5) | Designer/form styles | 5 |
| Excluded optional work (2) | Whole surface; no creation | 5 |

## Baseline, Threats, and Rollout

Preserve `Addino.AddinoClass`, namespace/assembly/GUID, five callbacks, .NET 4.7.2/x64/COM/Interop.EA, package selection, editable Name/Alias/Notes, read-only Type/Stereotype/Paquete, dirty-only explicit Save, modal behavior, and no persistence on Cancel/Esc/X. No `.csproj`, solution, AssemblyInfo, user, Exercise 2, archive, documentation/evidence/PDF, COM configuration, or canonical-spec change is planned. **Threat Matrix: N/A — no routing, shell, subprocess, VCS/PR automation, executable classification, or process-integration boundary.** No migration or feature flag; no open questions.
