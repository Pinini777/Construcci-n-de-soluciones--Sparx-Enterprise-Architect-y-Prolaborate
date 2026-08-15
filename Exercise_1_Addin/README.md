# Addino — Enterprise Architect Metadata Review

Addino is a C# WinForms COM add-in for Sparx Enterprise Architect. It opens a modal metadata editor for the selected package, including all descendant packages, while keeping persistence explicit and row-based.

## Environment

- Windows x64
- Sparx Enterprise Architect 17.1 x64 (validated environment)
- .NET Framework 4.7.2
- Visual Studio or MSBuild with .NET Framework tooling
- `Interop.EA.dll`
- Permission to register the COM add-in when required

The project targets C# 7.3 and x64. It does not change the add-in class, assembly identity, GUID, COM registration model, or Enterprise Architect callbacks.

## Build

From `Exercise_1_Addin`:

```cmd
msbuild Addino.csproj /t:Build /p:Configuration=Debug /p:Platform=x64
```

The expected assembly is:

```text
bin\x64\Debug\Addino.dll
```

## Install and register

1. Build the project as `Debug | x64`.
2. Ensure Enterprise Architect can load the COM class `Addino.AddinoClass`.
3. Register the add-in for 64-bit Enterprise Architect under:

   ```text
   HKEY_CURRENT_USER\SOFTWARE\Sparx Systems\EAAddins64
   ```

4. Start or restart Enterprise Architect after registration.

The project uses `RegisterForComInterop`; registration may require an elevated Visual Studio or build environment. The configured `Interop.EA.dll` hint path is environment-specific and may need to match the local Enterprise Architect installation.

## Open the metadata review

1. Open a repository in Enterprise Architect.
2. Select an `EA.Package` in the Project Browser.
3. Open **Specialize > Addino > Revisión de Metadatos de Elementos**.
4. The modal **Revisión de Metadatos** window opens.

If the current selection is not a package, Addino shows a validation message and does not open the editor.

## Package loading and columns

The selected package is the traversal root. Addino performs an iterative, pre-order traversal of the complete descendant package tree:

- direct elements are listed before descendant-package elements;
- sibling order follows Enterprise Architect collection order;
- visited `PackageID` values stop repeated or cyclic branches;
- emitted `ElementID` values prevent duplicate rows;
- recoverable package or element read failures are reported and the affected item or branch is skipped;
- there is no functional depth limit.

| Column | Editable | Meaning |
|---|---:|---|
| Nombre | Yes | Element name |
| Alias | Yes | Element alias |
| Notas | Yes | Element notes; multiline text is supported |
| Tipo | No | Enterprise Architect element type |
| Estereotipo | No | Applied stereotype |
| Paquete | No | Derived path from the selected root to the element's direct parent |

`Paquete` is display-only and is never persisted to Enterprise Architect.

## Dirty rows and Save

Edits remain in memory until **Guardar** is selected. A pale amber indicator marks editable cells in rows whose current Name, Alias, or Notes differ from their originally loaded values. Reverting all three values clears the indicator.

Before any `Element.Update()` call, Save finalizes the active edit and validates every dirty row. A dirty row whose Name is empty or whitespace-only blocks the entire Save operation:

- zero rows are updated;
- all pending edits remain available;
- invalid Name cells are highlighted;
- one Spanish message identifies all invalid rows;
- the form remains open for correction.

An untouched pre-existing blank Name is clean and does not block a different valid dirty row. If no rows are dirty, Addino reports that there are no pending changes and performs no writes.

After the global Name gate passes, each dirty row is saved independently through `GetElementByID` and `Element.Update()`:

- successful rows accept their new baseline and become clean;
- failed rows remain dirty for correction or retry;
- one row failure does not stop later rows;
- the final Spanish summary reports successes and failures.

## Reload

**Recargar** uses the same package-tree loader as the initial open.

- With no dirty rows, Reload runs immediately.
- With dirty rows, Addino asks: `Hay cambios sin guardar. Recargar descartará esas modificaciones. ¿Desea continuar?`
- **Yes** discards pending edits and reloads current Enterprise Architect values.
- **No** preserves the exact current grid state.
- Reload never calls Save or `Element.Update()`.
- A successful reload produces clean rows and reflects external Enterprise Architect changes.
- New rows are materialized before the visible list is replaced, so a catastrophic loader failure preserves the old grid.

Selection, active column, and scroll restoration after Reload are best effort.

## Cancel and close

The **Cancelar** button, Escape key, and native window close button discard unsaved in-memory edits. None of these paths calls `Element.Update()`. Rows already saved before closing remain persisted.

## Native visual treatment

The form keeps standard sizable WinForms chrome. On supported Windows 11 systems, one bounded `DwmSetWindowAttribute` integration requests a `#557DA5` native caption with white text. If DWM is absent, unsupported, or fails, Addino opens normally with the standard native caption.

There is no internal title header, duplicate title, borderless custom chrome, manual window buttons, extra DWM integration, or external visual library. **Guardar** is the blue primary action; **Recargar** and **Cancelar** remain native secondary actions. The grid fills the form and the action panel remains anchored at the bottom-right during resize.

## Creation and other limits

- Addino does not create or delete Enterprise Architect elements.
- It does not provide recursion toggles or a `MaxDepth` setting.
- It edits only Name, Alias, and Notes.
- COM access is synchronous; very large trees or slow repositories can make loading or saving take noticeable time.
- Recoverable COM read failures can produce a partial grid with warnings.
- Catastrophic Reload failure handling is implemented statically but was not safely fault-injected in the validated EA environment.
- Native caption coloring depends on operating-system DWM support and falls back to standard chrome.
- There is no automated test project or coverage runner; validation uses the x64 build plus approved manual Enterprise Architect human gates.

## Author

Ezequiel Pino — Proagile 2026 Technical Practice Challenge
