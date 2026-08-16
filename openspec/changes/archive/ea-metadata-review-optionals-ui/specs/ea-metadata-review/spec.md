# Delta for ea-metadata-review

## ADDED Requirements

### Requirement: Strict pre-Save blank Name validation

The system MUST validate every dirty row's Name before any `Element.Update()` call. A Name that is empty or whitespace-only is invalid. If one or more dirty rows are invalid, the system SHALL NOT call `Element.Update()`, SHALL retain pending changes, SHALL keep the form open, and SHALL show one Spanish message identifying the invalid rows. Clean rows and untouched pre-existing blank Names SHALL NOT block Save.

#### Scenario: Single invalid dirty row

- GIVEN one dirty row with a blank Name and no other pending changes
- WHEN Save is clicked
- THEN zero `Element.Update()` calls occur, the form stays open, and one Spanish message identifies the invalid row

#### Scenario: Multiple invalid and one valid dirty row

- GIVEN three dirty rows where two have blank Names and one is valid
- WHEN Save is clicked
- THEN no `Element.Update()` begins for any row, pending changes remain, and one Spanish message lists both invalid rows

#### Scenario: Whitespace-only Name

- GIVEN a dirty row whose Name contains only whitespace
- WHEN Save is clicked
- THEN Save is blocked with zero `Element.Update()` calls and a Spanish message identifies the row

#### Scenario: Zero dirty rows

- GIVEN no dirty rows
- WHEN Save is clicked
- THEN the existing "no pending changes" message appears and the blank-Name validator does not run

#### Scenario: Untouched pre-existing blank Name

- GIVEN a row loaded with a blank Name that is not dirty and a different dirty valid row
- WHEN Save is clicked
- THEN the valid row is persisted normally and the untouched blank Name is not validated

#### Scenario: Correction followed by successful Save

- GIVEN a dirty row previously blocked for a blank Name has been corrected to a valid Name
- WHEN Save is clicked
- THEN `Element.Update()` succeeds for that row and the success message appears

### Requirement: Dirty-row visual indicator

The system MUST visually distinguish rows that are dirty. The indicator SHALL be driven solely by the row's dirty state. Reverting a row to its original values SHALL clear the indicator. Successful Save SHALL clear the indicator for persisted rows. Partial Save failures SHALL retain the indicator for failed rows. Cancel, Esc, and close SHALL discard changes without persistence.

#### Scenario: One edit shows indicator

- GIVEN a clean row
- WHEN the user edits Name, Alias, or Notes
- THEN the row shows the dirty indicator

#### Scenario: Multiple edits show indicators

- GIVEN two clean rows
- WHEN the user edits both rows
- THEN both rows show the dirty indicator

#### Scenario: Revert to original clears indicator

- GIVEN a dirty row
- WHEN the user restores the original values exactly
- THEN the dirty indicator disappears

#### Scenario: Save success clears indicator

- GIVEN two dirty rows that Save will persist successfully
- WHEN Save succeeds
- THEN neither row shows the dirty indicator

#### Scenario: Partial Save failure retains indicator

- GIVEN two dirty rows where one `Update()` fails and one succeeds
- WHEN Save completes
- THEN the failed row remains dirty and the succeeded row is clean

#### Scenario: Cancellation while dirty

- GIVEN a dirty row
- WHEN the user clicks Cancel, presses Escape, or closes the form
- THEN the form closes with no persistence

### Requirement: Reload from current package

The system MUST provide a Reload action that re-reads the package tree using the same semantics as initial load. When no rows are dirty, Reload SHALL execute immediately. When rows are dirty, Reload SHALL show a Spanish confirmation equivalent to "Hay cambios sin guardar. Recargar descartará esas modificaciones. ¿Desea continuar?" with Yes and No options. Yes SHALL discard pending changes and reload. No SHALL preserve the exact current state. Reload SHALL NOT auto-save or silently discard. After Reload, loaded rows SHALL be clean and SHALL reflect external EA changes.

#### Scenario: Clean reload

- GIVEN the form is open with no dirty rows
- WHEN Reload is clicked
- THEN the grid reloads from EA immediately

#### Scenario: External change reflected

- GIVEN an element was modified in EA while the form was open
- WHEN Reload is clicked
- THEN the grid shows the updated EA values

#### Scenario: Dirty reload Yes

- GIVEN the form has dirty rows and the user clicks Reload
- WHEN the user confirms Yes on the Spanish dialog
- THEN pending changes are discarded and the grid reloads from EA

#### Scenario: Dirty reload No

- GIVEN the form has dirty rows and the user clicks Reload
- WHEN the user selects No on the Spanish dialog
- THEN the grid remains exactly as before with pending changes intact

#### Scenario: Active edit confirmation

- GIVEN the user is actively editing a cell and clicks Reload
- WHEN the active edit ends and dirty rows exist
- THEN the Spanish confirmation appears before any reload

#### Scenario: No persistence during Reload

- GIVEN the form has dirty rows and the user confirms Yes on Reload
- WHEN the grid reloads
- THEN no `Element.Update()` occurs and the reloaded rows are clean

### Requirement: Moderate EA visual refresh

The system MUST display an EA-inspired caption treatment using `#557DA5` with white text. The primary target is the native window caption, set via the bounded DWM P/Invoke `DwmSetWindowAttribute` with `DWMWA_CAPTION_COLOR` and `DWMWA_TEXT_COLOR` (supported on Windows 11 Build 22000+). If DWM is absent, the attributes are unsupported, the call returns a non-success HRESULT, or any interop failure occurs, the system MUST safely fall back to the standard native caption and MUST still open normally. There MUST be no custom window chrome, `FormBorderStyle.None`, manual window buttons, additional P/Invoke, DWM hacks, or external visual libraries. The grid header MAY retain the `#557DA5`/white treatment when fully visible and readable. The Save action MUST be visibly primary. Reload and Cancel MUST be visibly secondary. The system SHALL preserve native Windows Forms chrome and behavior and SHALL NOT cause functional or accessibility regressions.

(Previously: header/caption band, Save, Reload, and Cancel used SHOULD; fallback language was less explicit. HG5 rejected the internal-header fallback due to overlap and resize defects; replaced with best-effort native-caption coloring.)

#### Scenario: Blue caption style present

- GIVEN the form is open on a system that supports the DWM caption attributes
- THEN the native caption shows `#557DA5` background with white text, the grid header may share the same `#557DA5` treatment, and no internal header panel or duplicate title is present

#### Scenario: Native caption fallback

- GIVEN the form is opened on a system where the DWM caption attributes are absent or the call fails
- THEN the form opens with the standard native caption, the grid remains fully visible, and no internal header panel or duplicate title is present

#### Scenario: Save action visually distinct

- GIVEN the form is open
- THEN the Save button is visibly identifiable as the primary action

#### Scenario: Secondary actions visually distinct

- GIVEN the form is open
- THEN Reload and Cancel are visibly identifiable as secondary actions

#### Scenario: Functionality intact

- GIVEN the visual style is applied
- WHEN the user edits, saves, reloads, and cancels
- THEN all behaviors remain unchanged and no accessibility regressions occur

## MODIFIED Requirements

### Requirement: Direct element loading

The system SHALL load the selected package as the root and include all direct and descendant elements without an arbitrary functional depth limit. Each element SHALL appear exactly once. The loader SHALL protect against repeated or cyclic packages by tracking visited package identities. The system SHALL display a read-only `Paquete` column containing the derived container path for each element. Name, Alias, and Notes SHALL remain editable; Type and Stereotype SHALL remain read-only.

(Previously: only direct children of the selected package loaded; no recursion and no `Paquete` column.)

#### Scenario: Package with direct elements

- GIVEN a package with direct children
- WHEN the editor opens
- THEN the grid lists Name, Alias, Notes, Type, Stereotype, and `Paquete` for each direct element

#### Scenario: Empty package

- GIVEN a package with no direct or descendant children
- WHEN the editor opens
- THEN an empty grid is shown without error

#### Scenario: Multiple nested levels

- GIVEN a package with subpackages nested three levels deep containing elements
- WHEN the editor opens
- THEN elements from all levels appear in the grid

#### Scenario: Sibling subpackages

- GIVEN a package with two subpackages each containing elements
- WHEN the editor opens
- THEN elements from both subpackages appear

#### Scenario: No duplicate elements

- GIVEN a package tree where an element could be reached by multiple paths
- WHEN the editor opens
- THEN each element appears exactly once

#### Scenario: Cycle and repetition protection

- GIVEN a corrupted or unusual model where a package references an ancestor
- WHEN the loader encounters the repeated package identity
- THEN it stops descending that branch and warns without looping

#### Scenario: Accurate package path

- GIVEN a descendant element three levels below the root
- WHEN the editor opens
- THEN the `Paquete` column shows the path from root to the element's direct parent

### Requirement: Excluded optional work

The system MUST NOT implement new-element creation. The prior exclusion of recursion, dirty-row highlighting, Reload, and Save-blocking empty-Name validation is superseded for this change; those capabilities are now required deliverables and SHALL be implemented as specified in the ADDED and MODIFIED requirements above.

(Previously: recursion, creation, reload, dirty-row highlighting, and Save-blocking empty-Name validation were all prohibited.)

#### Scenario: Approved capabilities are required deliverables

- GIVEN the editor is in use
- WHEN rows are edited, reloaded, and saved
- THEN recursion, dirty-row highlighting, Reload, and strict Name validation are present and functional, and creation remains absent

#### Scenario: New-element creation remains excluded

- GIVEN the editor is in use
- WHEN the user reviews available actions
- THEN no create-new-element action exists
