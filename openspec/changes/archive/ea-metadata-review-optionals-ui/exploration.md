# Exploration: `ea-metadata-review-optionals-ui`

Scope: extend the stable Addino Exercise 1 baseline (already verified, archived as `ea-metadata-review-exercise-1`) with four challenge §3.7 optionals plus a restrained EA-like visual refresh — and add the **operator-mandatory strict pre-Save empty-Name validation**: every dirty/persistable row must pass `string.IsNullOrWhiteSpace(row.Name)`; if any row fails, **no `Element.Update()` may begin**. **Research only.** No implementation, no Designer edits, no doc edits, no Exercise 2 / archived artifact edits.

---

## 1. Verified technical state (today, on disk)

| Aspect | Verified value | Evidence |
| --- | --- | --- |
| Project / framework / platform | C# class library, .NET Framework 4.7.2, WinForms, x64, COM-visible, EA 17.1 x64 target | `Addino.csproj` lines 6–54 |
| `Interop.EA` reference | Embedded, HintPath fixed to EA Trial | `Addino.csproj` lines 56–59 |
| COM identity preserved | `Addino.AddinoClass`, GUID `ac47175a-e262-45dd-8ac5-1a8570d2a078` | `Properties/AssemblyInfo.cs` lines 20, 23 |
| COM entry point | Five callbacks wired in `Addino.AddinoClass` | `AddinoClass.cs` lines 8–207 |
| Extensions menu action | `-&Addino` → `&Revisión de Metadatos de Elementos` (one item) | `AddinoClass.cs` lines 10–11, 20–38 |
| Package validation | `GetTreeSelectedItemType() == otPackage` then `GetTreeSelectedObject() as EA.Package` | `AddinoClass.cs` lines 64–97 |
| Direct element loading | `EA.Package.Elements` only (non-recursive) | `AddinoClass.cs` `LoadPackageElements`, lines 135–199 |
| Row model | `MetadataElementRow` (COM-free DTO) | `MetadataElementRow.cs` lines 1–63 |
| Edit columns | Name, Alias, Notes editable; Type, Stereotype read-only | `MetadataReviewForm.cs` `BindGrid`, lines 32–96 |
| Save loop | `EndEdit()` → filter `IsDirty` → `GetElementByID` → assign → `Update()` → `AcceptChanges()` per row | `MetadataReviewForm.cs` lines 107–203 |
| Cancel / Esc / X | `CancelarButton_Click` sets `DialogResult.Cancel`; `cancelarButton` is `Form.CancelButton`; no `Element.Update()` outside Save | `MetadataReviewForm.cs` lines 240–246; `MetadataReviewForm.Designer.cs` line 81 |
| Designer layout | 900×480, dock-fill grid, bottom panel with Cancel (right) and Guardar (right of grid) | `MetadataReviewForm.Designer.cs` lines 16–87 |
| Tests / automation | None (no test project, no runner) | `openspec/config.yaml`; `Addino.csproj` ItemGroups |
| External EA repo | `Repositorio Pasantías` lives outside this Git workspace and is used only for manual validation | challenge doc §2 |
| Archived base artifacts | `openspec/changes/archive/ea-metadata-review-exercise-1/` (proposal, exploration, design, spec, tasks, verify) and `openspec/specs/ea-metadata-review/spec.md` | filesystem listing |

### What the baseline already guarantees

- All mandatory challenge §3.3–§3.5 behavior is verified (`openspec/specs/ea-metadata-review/spec.md`).
- The base spec **explicitly excludes** the four optionals this change adds (`openspec/specs/ea-metadata-review/spec.md` line 58: "System MUST NOT implement recursion, creation, reload, dirty-row highlighting, or Save-blocking empty-Name validation").
- Therefore the four optionals can only be added by **MODIFIED** requirements in this change's delta spec, never by accident.

### Stale / inert items that must NOT be edited

- `obj/Debug/Addino.csproj.FileListAbsolute.txt` and `obj/x64/Debug/MyAddin.csproj.FileListAbsolute.txt` reference an old `C:\Proagile\Addin\MyAddin\…` path. These are regenerated on build and not part of source.
- Archived `openspec/changes/archive/ea-metadata-review-exercise-1/*` is audit history. Do not modify.
- `openspec/specs/application-*` and `application-lifecycle-governance` specs belong to Exercise 2 and are out of scope here.

---

## 2. Load flow today — verbatim trace

```
EA invokes EA_Connect(repository)                                 AddinoClass.cs:14
EA invokes EA_GetMenuItems(repository, location, menuName)         AddinoClass.cs:20
   ""           -> returns "-&Addino"                              AddinoClass.cs:27–28
   "-&Addino"   -> returns new[] { "&Revisión de Metadatos de Elementos" }   AddinoClass.cs:31–34
EA invokes EA_GetMenuState(...)                                    AddinoClass.cs:41
   -> isEnabled = true                                             AddinoClass.cs:49
EA invokes EA_MenuClick(repository, location, menuName, itemName)  AddinoClass.cs:53
   if itemName != MenuReview: return                                AddinoClass.cs:59–62
   if otPackage != GetTreeSelectedItemType(): Spanish info MessageBox + return   AddinoClass.cs:64–73
   selectedObject = GetTreeSelectedObject()                         AddinoClass.cs:75
   if null: Spanish warning + return                                AddinoClass.cs:76–85
   package = selectedObject as EA.Package                           AddinoClass.cs:87
   if package == null: Spanish warning + return                     AddinoClass.cs:88–97
   rows, warnings = LoadPackageElements(package, out warnings)      AddinoClass.cs:104
       elements = package.Elements                                   AddinoClass.cs:151
       foreach elementObject in elements:
           element = elementObject as EA.Element
           rows.Add(new MetadataElementRow(
               element.ElementID, element.Name, element.Alias, element.Notes,
               element.Type, element.Stereotype))                  AddinoClass.cs:177–183
   if load throws: Spanish error MessageBox + return                AddinoClass.cs:106–115
   if warnings non-empty: Spanish warning MessageBox (load-level)   AddinoClass.cs:117–126
   using (MetadataReviewForm(repository, rows)) -> ShowDialog       AddinoClass.cs:128–131
EA_Disconnect -> GC.Collect + GC.WaitForPendingFinalizers           AddinoClass.cs:202–206
```

Inside `MetadataReviewForm`:

```
Constructor: store _repository + _rows; InitializeComponent()      MetadataReviewForm.cs:14–19
OnLoad: base.OnLoad; BindGrid(); set guardarButton.DialogResult = None  MetadataReviewForm.cs:21–30
BindGrid:
   AutoGenerateColumns = false; DataSource = _rows; clear columns    MetadataReviewForm.cs:34–36
   add colName / colAlias / colNotes / colType / colStereotype        MetadataReviewForm.cs:38–88
   wire EditingControlShowing -> multiline on Notes                   MetadataReviewForm.cs:95, 98–105
GuardarButton_Click:
   EndEdit(); collect IsDirty rows                                    MetadataReviewForm.cs:111–122
   if none -> "No hay cambios pendientes" info; return                MetadataReviewForm.cs:123–132
   for each dirty row:
       element = repository.GetElementByID(row.ElementId)             MetadataReviewForm.cs:143
       assign Name/Alias/Notes; element.Update()                      MetadataReviewForm.cs:163–179
       on Update() == false / exception / null: record per-row failure, continue   MetadataReviewForm.cs:181–195
       on success: row.AcceptChanges(); successCount++                 MetadataReviewForm.cs:198–199
   ShowSaveResult(successCount, failures)                              MetadataReviewForm.cs:202
CancelarButton_Click:
   DialogResult = Cancel; Close()                                      MetadataReviewForm.cs:240–246
```

The pre-`Update()` Name validation point that this change will reuse is **inside `GuardarButton_Click`** at `MetadataReviewForm.cs:123` (the "no pending changes" branch) — that is the single natural choke point where every row is already enumerated before any `Update()` is issued. The operator's mandatory empty-Name validation must be inserted between line 122 and line 137 (per-row loop) and **must complete before any `Element.Update()` is invoked**: all dirty rows are checked first, and **no `Update()` begins if one or more rows fail**. Putting the check elsewhere would force duplicate per-row scans and would not guarantee the all-or-nothing guarantee.

---

## 3. File / responsibility map (today)

| File | Responsibility today | Will be touched by this change? |
| --- | --- | --- |
| `Exercise_1_Addin/AddinoClass.cs` | COM entry point, menu wiring, package validation, `LoadPackageElements` (direct only). Holds no UI state. | **Yes** — `LoadPackageElements` becomes recursive (adds `Package.Packages` walk); loader signature stays parameter-light so both initial open and Reload call the same method. |
| `Exercise_1_Addin/AddinoClass.cs` `EA_MenuClick` | Validates package, loads rows, constructs and shows `MetadataReviewForm`. | **No logic change**, but the form must be informed of which `EA.Package` it represents so Reload can re-read the same source. |
| `Exercise_1_Addin/MetadataElementRow.cs` | COM-free DTO with `ElementId`, `Name`, `Alias`, `Notes`, `Type`, `Stereotype`; `OriginalName/Alias/Notes`; `IsDirty`; `AcceptChanges()`. | **Light** — extend to optionally carry `PackagePath` and `Depth` so a recursive loader can present them, and keep `IsDirty` ordinal-only. |
| `Exercise_1_Addin/MetadataReviewForm.cs` | Hosts the grid, binds `_rows`, handles Save / Cancel, calls `EndEdit` and `GetElementByID`. | **Yes** — adds Reload handler, pre-Save Name validation, per-row dirty visual hook. |
| `Exercise_1_Addin/MetadataReviewForm.Designer.cs` | Hand-authored WinForms layout (no Designer serialization needed for this small surface). | **Yes** — add `recargarButton` to `buttonPanel`; adjust `ColumnHeadersDefaultCellStyle`; optionally set `DataGridView.EnableHeadersVisualStyles = false` to honor brand colors while keeping native chrome elsewhere. |
| `Exercise_1_Addin/Properties/AssemblyInfo.cs` | COM identity, `ComVisible(true)`. | **No.** |
| `Exercise_1_Addin/Addino.csproj`, `.sln`, `.slnx`, `Addino.csproj.user` | MSBuild plumbing and Debug\|x64 start program. | **No.** |
| `Exercise_1_Addin/docs/...` (challenge, source_material, delivery, evidence) | Challenge text, EA Object Model PDF, Proagile PDFs, AI log, video. | **No.** |
| `openspec/specs/ea-metadata-review/spec.md` | Baseline normative spec. | **Read-only context**; this change writes its delta spec under `openspec/changes/ea-metadata-review-optionals-ui/specs/ea-metadata-review/spec.md`. |
| `openspec/changes/archive/ea-metadata-review-exercise-1/*` | Audit history. | **No.** |

---

## 4. Analysis of the four optionals (each in isolation)

### 4.1 Empty-Name save validation (operator-mandatory, strict)

- **Operator objective (binding)**: prevent Save whenever any row that would be persisted has a `Name` that is empty or whitespace-only. This is the change's explicit, mandatory behavior; no soft alternative is on the table.
- **Where it goes**: pre-Save enumeration in `GuardarButton_Click` (`MetadataReviewForm.cs:123`). The check runs **before** any `Element.Update()` is invoked. The single natural choke point is between line 122 (after `EndEdit()`/dirty-row collection) and line 137 (per-row loop start), and it is the location that already enumerates every row that would be persisted.
- **Validation rule (strict)**: iterate every dirty row `r` and evaluate `string.IsNullOrWhiteSpace(r.Name)`. If **any** dirty row fails, the validator must:
  1. Collect the row's identifiers (`ElementID` and, when present, `PackagePath`) into a single failure list.
  2. **Stop before any `Update()`** — no `Element.Update()` call may begin while one or more invalid rows exist. The loop must not start; the failure list must be generated and reported before any COM round-trip is attempted.
  3. Show a **single Spanish summary** that lists every offending row.
  4. Leave `DialogResult = None`, do not call `AcceptChanges` on any row, leave the grid open with the offending cells selected so the user can correct them.
  5. The user manually corrects the blank Name(s) and re-attempts Save; validation runs again on the next click.
- **Why "every dirty row" not "every row"**: the operator's objective is "row that would be persisted". A row is persisted only when it is dirty. Rows that are not dirty (clean, untouched) are not in the persisted set, so their `Name` is left alone — this matches the existing `IsDirty` filter already in `GuardarButton_Click` (`MetadataReviewForm.cs:111–122`) and avoids false positives on pre-existing empty Names that the user never touched.
- **Why `IsNullOrWhiteSpace` not `IsNullOrEmpty`**: whitespace-only entries (spaces, tabs, NBSP) are trivially unusable as element names; `IsNullOrWhiteSpace` is the standard .NET predicate for "effectively empty" and matches the operator's "empty or whitespace-only" wording.
- **Scope interaction with other optionals**: independent of recursion, reload, and dirty highlight. Works for both direct-only and recursive loads because it only inspects dirty rows.
- **Pre-existing empty Names (EA-compatibility validation concern — not an exception)**: a row whose `Name` was already empty when loaded from EA (rare but legal in the EA model) is **not** in the dirty set and is **not** validated. The moment the user edits that row (touches any field — Name, Alias, Notes) and the row becomes dirty, the empty Name triggers the validator and Save is blocked. The user must type a non-blank Name to persist. This is documented behavior, not an exemption — the operator's objective is the rule, and the rule says "row that would be persisted". This concern is exercised by regression R-05c and captured in risk K-04.
- **Not a human decision**: the strict behavior is the operator's explicit requirement and is **not** presented as a soft-versus-strict choice. H-02 (strict-vs-soft) has been removed from the human-decision table in §11.

### 4.2 Visual indicator of modified rows (dirty highlight)

- **Existing surface**: `IsDirty` already exists on `MetadataElementRow` (`MetadataElementRow.cs:45–54`); `Save` already consumes it. The grid is the only consumer that needs to know.
- **Cheapest mechanism without duplicate logic**: subscribe to `metadataGridView.CellValueChanged` (fires after the binding pushes the edit into `MetadataElementRow`) and toggle a per-row style override. The `CellValueChanged` event arrives only after the row model already evaluated the new value, so `row.IsDirty` is the single source of truth — **no per-cell diffing logic is introduced**. Pair with `metadataGridView.CellFormatting` if a more conservative approach is desired (recompute style on every paint from `row.IsDirty`); `CellFormatting` adds a tiny paint-time cost but eliminates any reliance on the event firing order.
- **Revert highlight**: when `GuardarButton_Click` calls `row.AcceptChanges()` on success (`MetadataReviewForm.cs:198`), re-apply the default cell style to that row's cells so the highlight clears without re-binding the whole grid. Failures stay highlighted because their originals are unchanged.
- **Edge cases**: `CellValueChanged` does not fire if the user edits a cell and the value is identical (string equality) — that is correct and matches `IsDirty`'s ordinal comparison. If `EndEdit()` is called from `GuardarButton_Click` *before* the dirty check (line 111), the just-typed value is already in the row, so `CellValueChanged` for the active cell fires during `EndEdit()` and the highlight appears immediately on Save's path. No additional logic is needed.
- **Constraint**: must not regress the read-only columns (`Type`, `Stereotype`); only highlight on editable columns. Implementing it through `IsDirty` keeps the rule naturally limited to `Name/Alias/Notes`.

### 4.3 Recursive traversal of `Package.Packages` + `Package.Elements`

- **API surface**: `EA.Package.Packages` returns `EA.Collection` (confirmed via locally available `enterprise-architect-object-model.pdf` — `Package` exposes both `Elements` and `Packages`). Each child `EA.Package` exposes the same two collections, supporting DFS or BFS.
- **Stable EA identity**: keep using `element.ElementID` (`MetadataElementRow` already does). Package identity can use `package.PackageID`; do NOT key by Name because Names may collide across siblings and may be edited.
- **Cycle consideration**: EA packages form a strict tree in normal models, but defensive code should track visited `PackageID`s via a `HashSet<int>` to prevent an accidental cycle (e.g. a corrupted repository or a future EA API quirk) from looping forever. Real-world depth in `Repositorio Pasantías` is shallow (≤ 4), but the loader must not assume that.
- **Depth consideration**: cap recursion with `MaxDepth` (suggested 8) for safety. If the cap is exceeded, stop and add a Spanish load warning row rather than silently dropping children. This mirrors the existing per-element warning pattern at `AddinoClass.cs:185–188`.
- **Per-row schema for recursion**: add two optional fields to `MetadataElementRow`:
  - `PackagePath` — slash-joined `package.Name` chain (read-only). Used for the read-only `Paquete` column.
  - `Depth` — int (0 for direct children). Not strictly needed but cheap to store and helps future visual grouping.
- **Cycle/depth failure**: collect into the existing `warnings` list (`AddinoClass.cs:139`) so the load warning message at `AddinoClass.cs:117–126` continues to surface them in Spanish.
- **EA performance note**: `GetElementByID` per dirty row is already used in Save (`MetadataReviewForm.cs:143`); recursion multiplies the number of rows and therefore the number of `GetElementByID` calls in Save. The COM cost is O(rows), acceptable for the documented repository size, but document this as a known scaling note.

### 4.4 Reload button (re-read state from EA, never silent discard)

- **Reuse a single loader**: extract `LoadPackageElements(package, warnings)` into a public/internal helper that both `EA_MenuClick` and Reload call. The current method is already `private` (`AddinoClass.cs:135`); making it `internal` (or duplicating the call inside the form) is the only change needed in the form to invoke it.
- **Information the form needs to Reload**: the originally selected `EA.Package` reference (a `class`-typed COM object held by the form, or the loader re-invoked through a delegate). Holding the COM `EA.Package` is acceptable because the form's lifetime is shorter than the EA session and `EA_Disconnect` runs only after the form closes.
- **Unsaved local modifications**: never silently discard. The Reload handler must:
  1. `EndEdit()` to flush the active cell.
  2. If `_rows` has any `IsDirty`, show a Spanish `MessageBox` with three options equivalent to `Sí / No / Cancelar`: *Sí — descartar cambios y recargar*, *No — guardar antes de recargar*, *Cancelar — no recargar*. The challenge's `ask-on-risk` delivery strategy and the base spec's "Save stays explicit; form stays open" rule both forbid silent data loss.
  3. Branching: *Sí* swaps the `BindingList` (or clears + repopulates in place to preserve selection behavior); *No* runs `GuardarButton_Click` synchronously and, only on full success (zero failures), proceeds to reload; if Save produced failures, keep the form open with errors highlighted and tell the user Reload was cancelled; *Cancelar* is a no-op.
- **Re-binding strategy**: prefer in-place mutation of `_rows` (`Clear()` + `Add()` per element) so the grid's selection, scroll position, and any per-row formatting (`CellFormatting` highlight) are not lost. Re-assigning `DataSource` to a new `BindingList` would force re-binding events and is the common source of subtle bugs.
- **No new element creation**: this change does **not** add a button to add elements. Per orchestrator constraint, that optional is excluded.

---

## 5. UI analysis — restrained EA-like styling + Reload placement

### 5.1 Brand palette (suggested; confirm in Proposal)

- Header background `Color.FromArgb(85, 125, 165)` ≈ `#557DA5`. White header text.
- Primary action (Guardar) backcolor same blue, foreground white; default button look is acceptable; `FlatStyle.System` or `FlatStyle.Standard` is sufficient.
- Cancel and Reload remain default WinForms chrome (no aggressive styling). Sober look.
- Use `metadataGridView.EnableHeadersVisualStyles = false` so the header colors apply; otherwise Windows themes override them on modern Windows.

### 5.2 Minimal Designer changes (no full Designer regeneration)

| Change | Where | Why |
| --- | --- | --- |
| Add `recargarButton` (`Button`) to `buttonPanel`, anchored Top|Right, placed left of Cancel; `TabIndex = 0`, `UseVisualStyleBackColor = true`, `Text = "Recargar"`. Wire `Click += RecargarButton_Click`. | `MetadataReviewForm.Designer.cs` `InitializeComponent` (~line 37, next to `cancelarButton.Controls.Add(this.guardarButton)`). | Reload placement beside Cancel keeps the action cluster bottom-right where users expect secondary actions. |
| `metadataGridView.EnableHeadersVisualStyles = false; ColumnHeadersDefaultCellStyle.BackColor = Color.FromArgb(85,125,165); ColumnHeadersDefaultCellStyle.ForeColor = Color.White; ColumnHeadersDefaultCellStyle.Font = new Font(...).` | `MetadataReviewForm.Designer.cs` after the existing `metadataGridView` block. | Sober native look + brand header only. |
| `metadataGridView.Columns.Add(new DataGridViewTextBoxColumn { DataPropertyName = "PackagePath", HeaderText = "Paquete", ReadOnly = true, ... })` | `MetadataReviewForm.cs` `BindGrid`. | A read-only column is appropriate only for recursive mode — see §5.3. |
| `guardarButton.FlatStyle = FlatStyle.System; BackColor = Color.FromArgb(85,125,165); ForeColor = Color.White;` | Designer. | Optional brand accent on the primary action. |

No DPI matrices, no custom painting, no third-party styling library. Avoid pixel-perfect replication of EA's ribbon.

### 5.3 Is a read-only `Package` / `Path` column appropriate?

- **Without recursion** (current baseline): **no**, it would always show a single repeated value and add noise. Keep it out.
- **With recursion**: **yes**, but only when the load was recursive. Show `Paquete` (the chain from the originally selected package to the element's direct parent, e.g. `Model / Aplicaciones / Bases de Datos`). The header can be a separate `DataGridViewTextBoxColumn` bound to `PackagePath` (read-only, `AutoSizeMode.Fill`, `MinimumWidth = 120`). This is the standard convention for grouped/recursive grids in EA-aware tooling.

The cleanest path: gate the column on a constructor flag or on a property `MetadataReviewForm.Recursive` set from `EA_MenuClick` based on a user-controlled mode. **This is a design decision for Proposal**: should recursion be the new default or stay opt-in? See §11 human decisions.

### 5.4 DataGridView events that can present dirty state without duplicate logic

| Event | Use here? | Why |
| --- | --- | --- |
| `CellValueChanged` | **Yes (primary)** | Fires after the binding pushes the edit into the row, so `row.IsDirty` is correct at that moment. Single source of truth. |
| `CellParsing` / `CellValidating` / `CellValidated` | No | Adds per-cell logic that duplicates `IsDirty`. Reserve for the optional empty-Name validation if we want in-cell feedback (red cell tooltip), but the simpler design uses `RowValidating` or the pre-Save sweep. |
| `RowValidating` | Optional (only if we want in-line validation feedback) | Can block row-leave while Name is blank; couples to optional 4.1. |
| `CellFormatting` | **Yes (alternate)** | Paints based on `row.IsDirty`; slower than `CellValueChanged` because it runs on every paint, but completely event-order agnostic. |
| `RowPrePaint` / `RowPostPaint` | Avoid | Painting outside `CellFormatting` complicates theming. |
| `CurrentCellDirtyStateChanged` | Yes, if the user clicks away with `Commit` set; needed to call `CommitEdit` for `CellValueChanged` to fire on checkbox columns. Not needed here (no checkbox columns). |

### 5.5 Dirty highlight revert path on Save

When `AcceptChanges()` runs at `MetadataReviewForm.cs:198`, the row's `IsDirty` becomes `false`. The cached `CellStyle` set by the highlight handler remains unless we reset it. The simplest revert path is to invalidate the row's cells in the same place:

```text
row.AcceptChanges();          // MetadataReviewForm.cs:198 (existing)
metadataGridView.InvalidateRow(rowIndex);   // refresh CellFormatting
```

If the highlight was applied via direct `cell.Style = …` (set in `CellValueChanged`), explicitly reset `cell.Style` to `null` on the same row's editable cells at the same call site. Keep the rule in the form, not in the row DTO, so the DTO remains UI-agnostic.

---

## 6. Cross-cutting effects of recursion

| Surface | Effect |
| --- | --- |
| `Cancel` (`CancelarButton_Click`, `MetadataReviewForm.cs:240`) | No change. `DialogResult.Cancel` + `Close()` discards all rows regardless of source. |
| `Save` (`GuardarButton_Click`, `MetadataReviewForm.cs:107`) | Iterates the same `_rows`; recursion only enlarges the set. No new code path needed beyond the per-row loop that already exists. |
| `Row errors` (`MetadataReviewForm.cs:147–195`) | Each row keeps its independent failure path. Aggregation list grows proportionally. Spanish summary already iterates `failures`. |
| `Dirty tracking` | `IsDirty` remains per-row and is unaffected by recursion source. |
| `EndEdit()` (`MetadataReviewForm.cs:111`) | No change. Flushes the active cell regardless of how many rows exist. |
| `Reload` | Operates on the same `EA.Package` reference and reuses the same loader, so its behavior is independent of recursion (recursive load → recursive reload; direct load → direct reload). |
| `EA.Package` lifetime | The form holds a `class`-typed COM reference across Reload; that is safe because Reload completes synchronously and `EA_Disconnect` only runs after the dialog closes. |

---

## 7. Single loader reuse — concrete shape

Today:

```text
EA_MenuClick -> rows = LoadPackageElements(package, out warnings)
```

Required shape after the change:

```text
EA_MenuClick   -> rows = LoadElements(rootPackage, recursive, out warnings)
form.Reload()  -> LoadElements(rootPackage, recursive, out warnings); rows.Replace(newRows)
```

Two practical patterns:

- **Pattern A** (preferred): expose `AddinoClass.LoadElements(EA.Package, bool recursive, out List<string>)` as `internal`. The form holds a `Func<>` or an `Action` set by `EA_MenuClick` that invokes the same loader. Pros: zero coupling between WinForms and `Interop.EA` beyond the `EA.Repository` already passed. Cons: an extra lambda capture.
- **Pattern B**: keep the loader `private` and call `form.Reload()` via a method on the form that itself calls `_repository.GetPackageByID(_rootPackage.PackageID)` and re-iterates `Elements`/`Packages`. Pros: no AddinoClass API change. Cons: duplicates traversal logic in two places.

Recommendation: Pattern A; it preserves the "single loader" property that the orchestrator asked for and keeps Reload behavior identical to initial open.

---

## 8. Mandatory manual regressions in Enterprise Architect

These regressions must pass before any future archive. They extend (do not replace) the base spec's manual scenarios.

| # | Scenario | Expected |
| --- | --- | --- |
| R-01 | Open Addino on a package with direct + subpackage elements. | Recursive load shows the new `Paquete` column populated; element count = direct + all descendants (no duplicates). |
| R-02 | Same package, depth cap triggered (manually craft a deep chain or lower `MaxDepth` for the test). | Spanish load warning lists the depth-cap message; remaining rows still render. |
| R-03 | Edit a row's Name then Save. | Row's highlight clears; EA reflects the new value; `IsDirty` becomes false. |
| R-04 | Edit three rows; deliberately fail one (lock it in EA or cause `Update() == false`). | Two highlights clear, one remains; Spanish summary lists the failed row's identifier. |
| R-05 | Edit a Name to blank and click Guardar. | `string.IsNullOrWhiteSpace(row.Name)` returns `true`; failure list contains the row; **no `Element.Update()` is invoked**; Spanish summary lists the row; offending cell selected; `AcceptChanges` is NOT called. |
| R-05a | Edit multiple rows, blank two of them, click Guardar. | Both failing rows appear in the Spanish summary; **no `Element.Update()` begins for any row** (including the valid ones); offending cells selected. Proves the all-or-nothing guarantee. |
| R-05b | Edit a Name to whitespace-only (spaces / tabs / NBSP), click Guardar. | `IsNullOrWhiteSpace` catches whitespace; Save blocked; no `Update()`; identical UX to R-05. |
| R-05c | Load a row whose `OriginalName` was already empty (legal in EA), do not edit it; edit a different row's Name to a valid value; click Guardar. | The pre-existing empty Name is **not** in the dirty set and is **not** validated; the edited row saves normally. Proves the validator targets "rows that would be persisted", not every loaded row. |
| R-05d | Zero dirty rows, click Guardar. | Existing "No hay cambios pendientes" branch fires first; validator does not run; no `Update()` begins. |
| R-05e | Multiple dirty rows: all blank. | Single Spanish summary lists all of them; no `Update()` begins. |
| R-05f | Reach the validation gate via the **Reload** `Sí/No/Cancelar` flow (Reload → `Sí` discard, then re-save on a fresh load; or Reload → `No` save-first path). | Validation is the same gate as the initial Save; no bypass. |
| R-06 | Click Recargar with no pending changes. | Grid refreshes; selection/scroll preserved; no message. |
| R-07 | Click Recargar with pending changes; choose *Cancelar*. | No reload; grid unchanged. |
| R-08 | Click Recargar with pending changes; choose *No* (save first) and Save succeeds fully. | Reload proceeds; grid shows fresh EA state. |
| R-09 | Click Recargar with pending changes; choose *No* and Save reports failures. | Reload cancelled; original failed rows remain highlighted. |
| R-10 | Click Recargar with pending changes; choose *Sí* (discard). | Local edits dropped; grid reloaded. |
| R-11 | Press Esc or click X with pending edits. | Form closes with `DialogResult.Cancel`; no `Update()` calls; EA repository unchanged. |
| R-12 | Header color check on Windows 10 and 11. | `#557DA5` header with white text; native chrome elsewhere. |
| R-13 | Direct-only mode (if kept as an option). | `Paquete` column hidden; behavior matches the base spec exactly. |

These are executable only inside a live EA with `Repositorio Pasantías`. The orchestrator's `ask-on-risk` delivery strategy means each regression that produces unexpected results should pause implementation until the operator decides.

---

## 9. Documentation and testing assets to expand later

| Asset | Why expand later | When |
| --- | --- | --- |
| `Exercise_1_Addin/README.md` | Add a "Funcionalidades opcionales" subsection describing recursion, dirty highlight, Reload, and **strict** pre-Save empty-Name blocking (every dirty row, `string.IsNullOrWhiteSpace(row.Name)`, no soft warning, no `Update()` begins if any row fails); update the "Ventana de revisión de metadatos" table to include `Paquete` (recursive only). | During Apply or Verify after implementation. |
| `Exercise_1_Addin/AI_USAGE_LOG.md` | Append new `OPT-…` IDs for the four optionals (one per feature) plus the UI refresh; preserve the existing rule that AI log entries are added only after a formal ID is assigned. | After Assign in a later Apply phase; **do not** modify it now. |
| `Exercise_1_Addin/docs/evidence/` | New screenshots / video showing recursive load, dirty highlight, Reload prompt, blocked-empty-Name. | After Apply. |
| `Exercise_1_Addin/docs/delivery/Pino_Guia_Ejecucion_Addino.pdf` | Update to reflect the new Recargar button and the recursive `Paquete` column. | After Apply. |
| `openspec/specs/ea-metadata-review/spec.md` | Unchanged (baseline). The four optionals become **MODIFIED** requirements in this change's delta spec. | In Spec phase. |
| Tests | None. No test runner exists. The base config explicitly disables strict TDD; this change follows the same rule. | Not applicable. |

---

## 10. Risks and regressions

| # | Risk | Mitigation |
| --- | --- | --- |
| K-01 | Recursive load on large packages multiplies `GetElementByID` calls during Save and may noticeably slow the dialog. | Cap depth at 8; warn at cap; document in README. The Repositorio Pasantías size is small, so the absolute impact is bounded. |
| K-02 | Recursion could, in a corrupted repository, enter a cycle. | Track visited `PackageID`s in a `HashSet<int>`; add a warning if a cycle is detected and stop descending that branch. |
| K-03 | Reload silently discarding dirty rows would violate the base spec's "Save stays explicit" rule and the operator's safety expectation. | Three-option confirmation (`Sí/No/Cancelar`); only *Sí* discards; *No* saves first and reloads only on full success; *Cancelar* is a no-op. |
| K-04 | Pre-existing empty Names in EA (rare but legal in the EA model) become a Save-block when the user touches that row (it becomes dirty) and leaves the Name blank. | This is the **intended behavior** of the operator's mandatory objective — there is no exemption. It is surfaced as an **EA-compatibility validation concern** (regression R-05c) so the operator can confirm during validation that the validator does not flag untouched rows. Documented in README under "Validación de Nombre en blanco". No code-based override. |
| K-05 | `CellValueChanged` firing order with `EndEdit()` could leave a freshly edited cell un-highlighted if the highlight handler is wired after the row model is built. | Wire `CellValueChanged` in `BindGrid()` immediately after columns are configured, and force `metadataGridView.CommitEdit(DataGridViewDataErrorContexts.Commit)` before reading `IsDirty` if the user clicks Save without leaving the cell. The current `EndEdit()` at `MetadataReviewForm.cs:111` already covers this case. |
| K-06 | Designer edits risk losing the hand-authored `.Designer.cs` style. The current `.Designer.cs` is small (94 lines) and not generated by Visual Studio's Designer at runtime — edits must be made by hand to the existing block. | Treat `.Designer.cs` as source code; do not open the Form in the WinForms Designer; apply changes inside the existing `InitializeComponent()`. |
| K-07 | The base spec explicitly forbids these optionals; this change **must** ship a delta spec that supersedes those clauses via MODIFIED requirements. | The Spec phase must enumerate the existing "MUST NOT" lines and convert each into a "MAY / SHOULD" / "MUST" counterpart. |
| K-08 | `PackagePath` could leak EA model structure into a UI string. | `PackagePath` is a UI-only convenience; document it as derived data, never persisted. |
| K-09 | COM identity must stay intact (`Addino.AddinoClass`, GUID). Reload does not change COM registration, but operator-driven re-registration after a Debug build could break a running EA. | Document in the change that the operator must restart EA after rebuilding, exactly as today. |
| K-10 | Adding a Reload button shifts the button cluster; Esc/Cancel behavior must remain identical. | `cancelarButton` stays `Form.CancelButton`; Recargar sits to its left; Esc still maps to Cancel. |

---

## 11. Human decisions still needed (carry into Proposal)

| # | Decision | Why it matters | Default if no answer |
| --- | --- | --- | --- |
| H-01 | Should the new **recursive load** be the new default, or opt-in (e.g. a checkbox in the dialog or a second menu item)? | Affects whether `Paquete` column is always present and whether the base spec's "direct only" line is deleted or merely qualified. | Default: opt-in via a checkbox "Incluir subpaquetes" (preserves the base spec verbatim for users who don't need recursion). |
| H-02 | Header visual change acceptable on Windows 10/11? `EnableHeadersVisualStyles = false` is required to apply brand colors. | Operators using high-contrast themes may notice the override. | Default: apply; document in README; offer a one-line revert. |
| H-03 | Reload's `Sí/No/Cancelar` UX text — keep Spanish, button labels `Sí/No/Cancelar`, or use icons? | UI consistency with the existing Spanish-only dialog. | Default: Spanish text buttons (matches the rest of the dialog). |
| H-04 | Should `Paquete` column sort lexicographically by default? | Affects user expectation of order in recursive mode. | Default: leave sorting to user; do not pre-sort. |
| H-05 | Should the change be delivered as a single PR or chained PRs (one optional per slice)? | The 400-line review budget likely accommodates the four optionals in one PR if the brand styling is kept small; but if any optional expands unexpectedly, chaining protects review focus. | Default: single PR; switch to chained only if `sdd-tasks` forecasts > 400 lines for the union. |

> **H-02 (strict-vs-soft empty-Name) has been removed.** The operator's explicit objective is mandatory and binding: strict block on `string.IsNullOrWhiteSpace(row.Name)` for every dirty/persistable row, no soft alternative. There is no longer a strict-vs-soft choice to make. The remaining human decisions are H-01 and H-02..H-05 (above).

---

## 12. Technical order recommendation (NOT design — just sequencing for downstream phases)

1. **Spec first**: produce `MODIFIED` clauses that supersede the four "MUST NOT" lines in `openspec/specs/ea-metadata-review/spec.md`. Add scenarios R-01..R-13 (with R-05 split into R-05 / R-05a / R-05b / R-05c / R-05d / R-05e / R-05f). Capture H-01 and H-02..H-05 decisions.
2. **Design second**: lock the loader signature (`internal LoadElements(package, recursive, out warnings)`), the form's Reload UX, the **strict** empty-Name validation point (pre-Save sweep at `MetadataReviewForm.cs:123` with `string.IsNullOrWhiteSpace(row.Name)` and the all-or-nothing guarantee that no `Element.Update()` begins if any row fails), the highlight event (`CellValueChanged` + `CellFormatting` paint-time fallback), the `Paquete` column gating, and the brand-color palette.
3. **Tasks third**: keep slices small. Natural slicing if chained: (a) empty-Name validation (operator-mandatory, smallest scope, isolated); (b) recursive loader + `Paquete` column; (c) dirty highlight + revert on `AcceptChanges`; (d) Reload button + confirmation UX; (e) brand header styling. Verify the 400-line forecast before choosing single-PR vs chained.
4. **Apply fourth**: implement in this order —
   (a) **Empty-Name validation FIRST** (operator-mandatory, smallest scope, independent of all other optionals; lands the binding behavior earliest and reduces the risk of a downstream refactor accidentally regressing the gate).
   (b) Recursive loader (foundational — changes `_rows` size for downstream).
   (c) Reload (reuses the same loader).
   (d) Dirty highlight (reuses existing `IsDirty`).
   (e) Brand styling (cosmetic, last).
5. **Verify fifth**: extend the base verify report with R-01..R-13 manual checks; do not fabricate results.
6. **Archive last**: delta spec gets merged into the canonical `openspec/specs/ea-metadata-review/spec.md`.

This sequence is **engineering advice**, not a contract; sdd-design owns the final sequencing.

---

## 13. Real blockers

- **None technical.** Every optional is implementable inside the existing project, framework, platform, COM identity, and x64 boundary. No protected file must be edited (the four protected areas in §1 are stable and read-only here).
- **Only human decisions** (H-01 and H-02..H-05, after H-02 strict-vs-soft was removed) gate the next phase. They are not blockers in the "stop the work" sense; they are the standard ask-on-risk prompts that this delivery strategy requires.

---

## 14. Ready for Proposal

**Yes**, conditional on the operator resolving H-01 and H-02..H-05 in the Proposal phase. The empty-Name strict-vs-soft decision has been **resolved by the operator's explicit objective** (binding): strict block on `string.IsNullOrWhiteSpace(row.Name)` for every dirty/persistable row, no `Element.Update()` begins if any row fails, no soft alternative. The next SDD phase is **sdd-propose** for `ea-metadata-review-optionals-ui`, only on explicit user instruction.

---

## 15. Protected areas (do not touch)

- `Addino.AddinoClass` namespace + class name; `Addino.AddinoClass` registry identity.
- `AssemblyInfo.cs`: `[assembly: ComVisible(true)]`, `[assembly: Guid("ac47175a-e262-45dd-8ac5-1a8570d2a078")]`, version numbers.
- `Addino.csproj`: `TargetFrameworkVersion`, `PlatformTarget`, `RegisterForComInterop`, `EmbedInteropTypes`, `Interop.EA` HintPath, `LangVersion`, `AssemblyName`.
- `Addino.csproj.user`: `StartProgram` pointing to the Trial EA.
- `Exercise_1_Addin/docs/challenge/*`, `docs/source_material/*`, `docs/delivery/*`, `docs/evidence/*`, `README.md`, `AI_USAGE_LOG.md`.
- `openspec/specs/ea-metadata-review/spec.md` (canonical baseline; merges happen only at archive).
- `openspec/changes/archive/ea-metadata-review-exercise-1/*` and `openspec/changes/archive/ea-governance-queries-exercise-2/*`.
- `Exercise_2_queries/**` (different change).
- `.codegraph/`, `.atl/`, `.vs/`, `bin/`, `obj/` (regenerated / runtime state).

---

## 16. Summary (corrected binding facts)

After gate validation found requirement drift, the Explora phase was re-run inside the same change folder. The corrected binding facts are:

- **Operator objective is mandatory**: prevent Save whenever any row that would be persisted has a `Name` that is empty or whitespace-only. No soft alternative is on the table.
- **Predicate**: `string.IsNullOrWhiteSpace(row.Name)` evaluated for every dirty row in `GuardarButton_Click`, **before** any `Element.Update()` is invoked.
- **No partial save**: if one or more dirty rows fail, **no** `Element.Update()` begins. The Save button returns `DialogResult = None`, the grid stays open, the offending cells are selected, and a single Spanish summary lists every failing row.
- **"Persistable" definition**: a row is persistable if and only if `IsDirty == true`. Rows that are not dirty (clean, untouched) are not in the persisted set and are not validated. This avoids false positives on pre-existing empty Names that the user never touched.
- **No soft-warning alternative removed from the human-decision table**: H-02 (strict-vs-soft) is **removed**. The remaining human decisions are H-01 (recursion default) and H-02..H-05 (formerly H-03..H-06).
- **Pre-existing empty Names**: surfaced as an **EA-compatibility validation concern** (regression R-05c) and as risk **K-04** — not as an exception to the rule. When the user touches such a row and leaves `Name` blank, the validator blocks Save; the user must type a non-blank Name to persist. This is documented behavior, not an exemption.
- **Apply ordering**: empty-Name validation now lands **first** in the apply sequence (and first in the chained-PR slicing) because it is the operator's mandatory objective and is independent of the other optionals.
- **Validation scenarios**: R-05 was tightened and split into R-05, R-05a, R-05b, R-05c, R-05d, R-05e, R-05f to cover single-row, multi-row, whitespace-only, untouched pre-existing empty, zero-dirty, all-blank, and reload-path coverage respectively.
- **All other scope, evidence, and constraints from the previous version are retained unchanged**: the four optionals (recursive load, dirty highlight, Reload, empty-Name blocking), the restrained EA-like visual refresh, the file/responsibility map, the verification testing-capabilities baseline, the manual-regression scenarios R-01..R-04 and R-06..R-13, the protected areas, and the archived-baseline constraints.
