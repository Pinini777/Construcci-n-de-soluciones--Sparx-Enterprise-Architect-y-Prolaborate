# EA Metadata Review Capability

## Purpose
Editor for direct children of an `EA.Package`.

## Requirements

### EA callbacks and Extensions menu action
Add-in MUST implement `EA_Connect`, `EA_GetMenuItems`, `EA_GetMenuState`, `EA_MenuClick`, and `EA_Disconnect`, with one enabled Extensions action.

- **Add-in loads**: GIVEN the add-in is registered, WHEN EA initializes, THEN callbacks are reachable and Extensions action is enabled

### Package selection validation
`EA_MenuClick` MUST accept only `EA.Package` selections; otherwise it SHALL show a Spanish message and stop.

- **Valid package selection**: GIVEN an `EA.Package` selection, WHEN invoked, THEN editor opens
- **Invalid selection**: GIVEN an invalid selection, WHEN invoked, THEN a Spanish message is shown and editor stays closed

### Direct element loading
Only direct children SHALL load.

- **Package with direct elements**: GIVEN a package with direct children, WHEN editor opens, THEN grid lists Name, Alias, Notes, Type, Stereotype
- **Empty package**: GIVEN a package with no direct children, WHEN editor opens, THEN an empty grid is shown without error

### Grid editability
Name/Alias MUST be editable, Notes multiline editable, Type/Stereotype read-only. Local edits MUST NOT call `Element.Update()` before Save. Save MUST write only the associated source element.

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

### Target platform and solution baseline
Solution MUST target C# on .NET Framework 4.7.2, Windows Forms, `Interop.EA`, COM add-in registration, and EA 17.1 x64, and be clean, organized, and strictly typed. A Trial edition is not required and untested compatibility MUST NOT be claimed.

- **Baseline verification**: GIVEN source, WHEN `.sln` opens and builds in Visual Studio, THEN it succeeds and matches baseline requirement

### Delivery artifacts
Change MUST deliver a `.sln`, README, evidence, and AI Usage Log. README MUST cover prerequisites, EA opening, package selection, Extensions path, edit/save/cancel, validation/errors, persisted-change verification. Evidence MUST include screenshots and a short video demonstrating the complete flow: package selection, grid opening, metadata editing, saving, and changes reflected in Enterprise Architect. AI Usage Log MUST record at least five significant AI interactions with ID, objective, tool/model, strategy/prompt, decision, evidence, result. Final packaging MUST follow the challenge surname convention; internal Addino names MUST remain unchanged.

- **Delivery verification**: GIVEN implementation is complete, WHEN artifacts are assembled, THEN the README meets its documented requirements, screenshots and a short video demonstrate package selection, grid opening, metadata editing, saving, and changes reflected in Enterprise Architect, and the AI Usage Log contains at least five significant interactions with all required fields

### Excluded optional work
System MUST NOT implement recursion, creation, reload, dirty-row highlighting, or Save-blocking empty-Name validation.

- **No optional features**: GIVEN editor is in use, WHEN rows are edited and saved, THEN no excluded features are present
