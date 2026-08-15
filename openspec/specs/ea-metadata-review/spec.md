# EA Metadata Review Capability

## Purpose
Editor for the selected `EA.Package` plus all descendants.

## Requirements

### EA callbacks and Extensions menu action
Add-in MUST implement `EA_Connect`, `EA_GetMenuItems`, `EA_GetMenuState`, `EA_MenuClick`, and `EA_Disconnect`, with one enabled Extensions action.

- **Add-in loads**: GIVEN the add-in is registered, WHEN EA initializes, THEN callbacks are reachable and Extensions action is enabled

### Package selection validation
`EA_MenuClick` MUST accept only `EA.Package` selections; otherwise it SHALL show a Spanish message and stop.

- **Valid package selection**: GIVEN an `EA.Package` selection, WHEN invoked, THEN editor opens
- **Invalid selection**: GIVEN an invalid selection, WHEN invoked, THEN a Spanish message is shown and editor stays closed

### Direct element loading
The system SHALL load the selected package as the root and include all direct and descendant elements without an arbitrary functional depth limit. Each element SHALL appear exactly once. The loader SHALL protect against repeated or cyclic packages by tracking visited package identities. The system SHALL display a read-only `Paquete` column containing the derived container path for each element. Name, Alias, and Notes SHALL remain editable; Type and Stereotype SHALL remain read-only.

- **Package with direct elements**: GIVEN a package with direct children, WHEN the editor opens, THEN the grid lists Name, Alias, Notes, Type, Stereotype, and `Paquete` for each direct element
- **Empty package**: GIVEN a package with no direct or descendant children, WHEN the editor opens, THEN an empty grid is shown without error
- **Multiple nested levels**: GIVEN a package with subpackages nested three levels deep containing elements, WHEN the editor opens, THEN elements from all levels appear in the grid
- **Sibling subpackages**: GIVEN a package with two subpackages each containing elements, WHEN the editor opens, THEN elements from both subpackages appear
- **No duplicate elements**: GIVEN a package tree where an element could be reached by multiple paths, WHEN the editor opens, THEN each element appears exactly once
- **Cycle and repetition protection**: GIVEN a corrupted or unusual model where a package references an ancestor, WHEN the loader encounters the repeated package identity, THEN it stops descending that branch and warns without looping
- **Accurate package path**: GIVEN a descendant element three levels below the root, WHEN the editor opens, THEN the `Paquete` column shows the path from root to the element's direct parent

### Grid editability
Name/Alias MUST be editable, Notes multiline editable, Type/Stereotype/Paquete read-only. Local edits MUST NOT call `Element.Update()` before Save. Save MUST write only the associated source element.

- **Allowed edits**: GIVEN grid has rows, WHEN user edits each column and saves, THEN editable columns accept input, read-only columns reject it, no `Element.Update()` occurs before Save, and Save writes only the associated source element

### Editor modality and UI language
Editor MUST be a modal Windows Form with UI text in Spanish.

- **Modal Spanish editor**: GIVEN a valid package selection, WHEN editor opens, THEN it is modal and UI text is Spanish

### Local edit lifecycle
Changes MUST remain local until Save. Cancel, Esc, and close SHALL discard them without `Element.Update()`, preserving saved data.

- **Cancel lifecycle**: GIVEN saved and pending edits, WHEN Cancel, Esc, or close is used, THEN pending changes are discarded without `Element.Update()`

### Save lifecycle
Save MUST commit the active edit, call `Element.Update()` only on modified rows, clear pending state for successes, show a success message on no failures, continue on failure. Failures (false, exception, lock/unwritable) are identified; successes are non-pending and failures remain pending. A Save with no new edits MUST NOT call `Element.Update()` and MUST report no pending changes.

- **Normal save**: GIVEN modified rows with an active edit, WHEN Save is clicked, THEN active value is included, `Update()` runs only on modified rows, successful rows clear pending state, and a clear success message appears on no failures
- **Failures continue**: GIVEN `Update()` returns false for one element, throws for another, and another is locked/unwritable, WHEN Save runs, THEN remaining rows continue, each failed element is identified, successes are non-pending, and failures remain pending
- **No new changes**: GIVEN a Save with no new edits, WHEN Save is clicked again, THEN no `Element.Update()` is called and no pending changes are reported

### Strict pre-Save blank Name validation
The system MUST validate every dirty row's Name before any `Element.Update()` call. A Name that is empty or whitespace-only is invalid. If one or more dirty rows are invalid, the system SHALL NOT call `Element.Update()`, SHALL retain pending changes, SHALL keep the form open, and SHALL show one Spanish message identifying the invalid rows. Clean rows and untouched pre-existing blank Names SHALL NOT block Save.

- **Single invalid dirty row**: GIVEN one dirty row with a blank Name and no other pending changes, WHEN Save is clicked, THEN zero `Element.Update()` calls occur, the form stays open, and one Spanish message identifies the invalid row
- **Multiple invalid and one valid dirty row**: GIVEN three dirty rows where two have blank Names and one is valid, WHEN Save is clicked, THEN no `Element.Update()` begins for any row, pending changes remain, and one Spanish message lists both invalid rows
- **Whitespace-only Name**: GIVEN a dirty row whose Name contains only whitespace, WHEN Save is clicked, THEN Save is blocked with zero `Element.Update()` calls and a Spanish message identifies the row
- **Zero dirty rows**: GIVEN no dirty rows, WHEN Save is clicked, THEN the existing "no pending changes" message appears and the blank-Name validator does not run
- **Untouched pre-existing blank Name**: GIVEN a row loaded with a blank Name that is not dirty and a different dirty valid row, WHEN Save is clicked, THEN the valid row is persisted normally and the untouched blank Name is not validated
- **Correction followed by successful Save**: GIVEN a dirty row previously blocked for a blank Name has been corrected to a valid Name, WHEN Save is clicked, THEN `Element.Update()` succeeds for that row and the success message appears

### Dirty-row visual indicator
The system MUST visually distinguish rows that are dirty. The indicator SHALL be driven solely by the row's dirty state. Reverting a row to its original values SHALL clear the indicator. Successful Save SHALL clear the indicator for persisted rows. Partial Save failures SHALL retain the indicator for failed rows. Cancel, Esc, and close SHALL discard changes without persistence.

- **One edit shows indicator**: GIVEN a clean row, WHEN the user edits Name, Alias, or Notes, THEN the row shows the dirty indicator
- **Multiple edits show indicators**: GIVEN two clean rows, WHEN the user edits both rows, THEN both rows show the dirty indicator
- **Revert to original clears indicator**: GIVEN a dirty row, WHEN the user restores the original values exactly, THEN the dirty indicator disappears
- **Save success clears indicator**: GIVEN two dirty rows that Save will persist successfully, WHEN Save succeeds, THEN neither row shows the dirty indicator
- **Partial Save failure retains indicator**: GIVEN two dirty rows where one `Update()` fails and one succeeds, WHEN Save completes, THEN the failed row remains dirty and the succeeded row is clean
- **Cancellation while dirty**: GIVEN a dirty row, WHEN the user clicks Cancel, presses Escape, or closes the form, THEN the form closes with no persistence

### Reload from current package
The system MUST provide a Reload action that re-reads the package tree using the same semantics as initial load. When no rows are dirty, Reload SHALL execute immediately. When rows are dirty, Reload SHALL show a Spanish confirmation equivalent to "Hay cambios sin guardar. Recargar descartará esas modificaciones. ¿Desea continuar?" with Yes and No options. Yes SHALL discard pending changes and reload. No SHALL preserve the exact current state. Reload SHALL NOT auto-save or silently discard. After Reload, loaded rows SHALL be clean and SHALL reflect external EA changes.

- **Clean reload**: GIVEN the form is open with no dirty rows, WHEN Reload is clicked, THEN the grid reloads from EA immediately
- **External change reflected**: GIVEN an element was modified in EA while the form was open, WHEN Reload is clicked, THEN the grid shows the updated EA values
- **Dirty reload Yes**: GIVEN the form has dirty rows and the user clicks Reload, WHEN the user confirms Yes on the Spanish dialog, THEN pending changes are discarded and the grid reloads from EA
- **Dirty reload No**: GIVEN the form has dirty rows and the user clicks Reload, WHEN the user selects No on the Spanish dialog, THEN the grid remains exactly as before with pending changes intact
- **Active edit confirmation**: GIVEN the user is actively editing a cell and clicks Reload, WHEN the active edit ends and dirty rows exist, THEN the Spanish confirmation appears before any reload
- **No persistence during Reload**: GIVEN the form has dirty rows and the user confirms Yes on Reload, WHEN the grid reloads, THEN no `Element.Update()` occurs and the reloaded rows are clean

### Moderate EA visual refresh
The system MUST display an EA-inspired caption treatment using `#557DA5` with white text. The primary target is the native window caption, set via the bounded DWM P/Invoke `DwmSetWindowAttribute` with `DWMWA_CAPTION_COLOR` and `DWMWA_TEXT_COLOR` (supported on Windows 11 Build 22000+). If DWM is absent, the attributes are unsupported, the call returns a non-success HRESULT, or any interop failure occurs, the system MUST safely fall back to the standard native caption and MUST still open normally. There MUST be no custom window chrome, `FormBorderStyle.None`, manual window buttons, additional P/Invoke, DWM hacks, or external visual libraries. The grid header MAY retain the `#557DA5`/white treatment when fully visible and readable. The Save action MUST be visibly primary. Reload and Cancel MUST be visibly secondary. The system SHALL preserve native Windows Forms chrome and behavior and SHALL NOT cause functional or accessibility regressions.

- **Blue caption style present**: GIVEN the form is open on a system that supports the DWM caption attributes, THEN the native caption shows `#557DA5` background with white text, the grid header may share the same `#557DA5` treatment, and no internal header panel or duplicate title is present
- **Native caption fallback**: GIVEN the form is opened on a system where the DWM caption attributes are absent or the call fails, THEN the form opens with the standard native caption, the grid remains fully visible, and no internal header panel or duplicate title is present
- **Save action visually distinct**: GIVEN the form is open, THEN the Save button is visibly identifiable as the primary action
- **Secondary actions visually distinct**: GIVEN the form is open, THEN Reload and Cancel are visibly identifiable as secondary actions
- **Functionality intact**: GIVEN the visual style is applied, WHEN the user edits, saves, reloads, and cancels, THEN all behaviors remain unchanged and no accessibility regressions occur

### Target platform and solution baseline
Solution MUST target C# on .NET Framework 4.7.2, Windows Forms, `Interop.EA`, COM add-in registration, and EA 17.1 x64, and be clean, organized, and strictly typed. A Trial edition is not required and untested compatibility MUST NOT be claimed.

- **Baseline verification**: GIVEN source, WHEN `.sln` opens and builds in Visual Studio, THEN it succeeds and matches baseline requirement

### Delivery artifacts
Change MUST deliver a `.sln`, README, evidence, and AI Usage Log. README MUST cover prerequisites, EA opening, package selection, Extensions path, edit/save/cancel, validation/errors, persisted-change verification. Evidence MUST include screenshots and a short video demonstrating the complete flow: package selection, grid opening, metadata editing, saving, and changes reflected in Enterprise Architect. AI Usage Log MUST record at least five significant AI interactions with ID, objective, tool/model, strategy/prompt, decision, evidence, result. Final packaging MUST follow the challenge surname convention; internal Addino names MUST remain unchanged.

- **Delivery verification**: GIVEN implementation is complete, WHEN artifacts are assembled, THEN the README meets its documented requirements, screenshots and a short video demonstrate package selection, grid opening, metadata editing, saving, and changes reflected in Enterprise Architect, and the AI Usage Log contains at least five significant interactions with all required fields

### Excluded optional work
The system MUST NOT implement new-element creation. The prior exclusion of recursion, dirty-row highlighting, Reload, and Save-blocking empty-Name validation is superseded for this change; those capabilities are now required deliverables and SHALL be implemented as specified in the Direct element loading, Dirty-row visual indicator, Reload from current package, and Strict pre-Save blank Name validation requirements above.

- **Approved capabilities are required deliverables**: GIVEN the editor is in use, WHEN rows are edited, reloaded, and saved, THEN recursion, dirty-row highlighting, Reload, and strict Name validation are present and functional, and creation remains absent
- **New-element creation remains excluded**: GIVEN the editor is in use, WHEN the user reviews available actions, THEN no create-new-element action exists
