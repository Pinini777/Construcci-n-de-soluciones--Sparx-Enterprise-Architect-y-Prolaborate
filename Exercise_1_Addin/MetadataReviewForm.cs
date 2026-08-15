using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Runtime.InteropServices;
using System.Text;
using System.Windows.Forms;

namespace Addino
{
    public partial class MetadataReviewForm : Form
    {
        private readonly EA.Repository _repository;
        private readonly EA.Package _rootPackage;
        private readonly PackageLoader _loader;
        private readonly BindingList<MetadataElementRow> _rows;
        private readonly HashSet<int> _invalidNameRows = new HashSet<int>();

        // Best-effort native caption coloring via the only allowed DWM API.
        // These attributes are supported on Windows 11 Build 22000+; any failure
        // (missing DWM, unsupported attribute, HRESULT error, or interop failure)
        // is swallowed so the form still opens with the standard native caption.
        private const int DWMWA_CAPTION_COLOR = 35;
        private const int DWMWA_TEXT_COLOR = 36;

        [DllImport("dwmapi.dll", PreserveSig = true)]
        private static extern int DwmSetWindowAttribute(IntPtr hwnd, int attr, ref int attrValue, int attrSize);

        public MetadataReviewForm(EA.Repository repository, BindingList<MetadataElementRow> rows)
        {
            _repository = repository;
            _rootPackage = null;
            _loader = null;
            _rows = rows ?? new BindingList<MetadataElementRow>();
            InitializeComponent();
        }

        internal MetadataReviewForm(
            EA.Repository repository,
            EA.Package rootPackage,
            PackageLoader loader,
            BindingList<MetadataElementRow> rows)
        {
            _repository = repository;
            _rootPackage = rootPackage;
            _loader = loader;
            _rows = rows ?? new BindingList<MetadataElementRow>();
            InitializeComponent();
        }

        protected override void OnHandleCreated(EventArgs e)
        {
            base.OnHandleCreated(e);
            TrySetCaptionColors();
        }

        private void TrySetCaptionColors()
        {
            try
            {
                if (!IsHandleCreated)
                {
                    return;
                }

                IntPtr hWnd = this.Handle;

                // #557DA5 as a COLORREF: 0x00bbggrr.
                int captionColor = 0x00A57D55;
                int textColor = 0x00FFFFFF;

                DwmSetWindowAttribute(hWnd, DWMWA_CAPTION_COLOR, ref captionColor, sizeof(int));
                DwmSetWindowAttribute(hWnd, DWMWA_TEXT_COLOR, ref textColor, sizeof(int));
            }
            catch
            {
                // Best-effort fallback: leave the standard native caption unchanged.
            }
        }

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);
            BindGrid();

            // Prevent the Guardar button from closing the form automatically.
            // Save stays explicit: it shows a result message and leaves the form
            // open so the user can retry failed rows or keep editing.
            guardarButton.DialogResult = DialogResult.None;
        }

        private void BindGrid()
        {
            metadataGridView.AutoGenerateColumns = false;
            metadataGridView.DataSource = _rows;
            metadataGridView.Columns.Clear();

            metadataGridView.Columns.Add(new DataGridViewTextBoxColumn
            {
                DataPropertyName = "Name",
                HeaderText = "Nombre",
                Name = "colName",
                ReadOnly = false,
                AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill,
                MinimumWidth = 120
            });

            metadataGridView.Columns.Add(new DataGridViewTextBoxColumn
            {
                DataPropertyName = "Alias",
                HeaderText = "Alias",
                Name = "colAlias",
                ReadOnly = false,
                AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill,
                MinimumWidth = 120
            });

            DataGridViewTextBoxColumn notesColumn = new DataGridViewTextBoxColumn
            {
                DataPropertyName = "Notes",
                HeaderText = "Notas",
                Name = "colNotes",
                ReadOnly = false,
                AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill,
                MinimumWidth = 180
            };
            notesColumn.DefaultCellStyle.WrapMode = DataGridViewTriState.True;
            metadataGridView.Columns.Add(notesColumn);

            metadataGridView.Columns.Add(new DataGridViewTextBoxColumn
            {
                DataPropertyName = "Type",
                HeaderText = "Tipo",
                Name = "colType",
                ReadOnly = true,
                AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill,
                MinimumWidth = 100
            });

            metadataGridView.Columns.Add(new DataGridViewTextBoxColumn
            {
                DataPropertyName = "Stereotype",
                HeaderText = "Estereotipo",
                Name = "colStereotype",
                ReadOnly = true,
                AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill,
                MinimumWidth = 100
            });

            metadataGridView.Columns.Add(new DataGridViewTextBoxColumn
            {
                DataPropertyName = "PackagePath",
                HeaderText = "Paquete",
                Name = "colPackage",
                ReadOnly = true,
                AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill,
                MinimumWidth = 150
            });

            metadataGridView.AutoSizeRowsMode = DataGridViewAutoSizeRowsMode.AllCells;
            metadataGridView.AllowUserToAddRows = false;
            metadataGridView.AllowUserToDeleteRows = false;
            metadataGridView.MultiSelect = false;
            metadataGridView.SelectionMode = DataGridViewSelectionMode.CellSelect;
            metadataGridView.EditingControlShowing += MetadataGridView_EditingControlShowing;
            metadataGridView.CellFormatting += MetadataGridView_CellFormatting;
            metadataGridView.CellValueChanged += MetadataGridView_CellValueChanged;
        }

        private void MetadataGridView_CellFormatting(object sender, DataGridViewCellFormattingEventArgs e)
        {
            if (e.RowIndex < 0)
            {
                return;
            }

            if (!(metadataGridView.Rows[e.RowIndex].DataBoundItem is MetadataElementRow row))
            {
                return;
            }

            string columnName = metadataGridView.Columns[e.ColumnIndex].Name;

            // Dirty highlight: pale amber on editable cells. IsDirty is the sole authority;
            // this event only presents the current row state and never mutates the model.
            if (row.IsDirty && IsEditableColumn(columnName))
            {
                e.CellStyle.BackColor = Color.FromArgb(255, 244, 206);
                e.CellStyle.SelectionBackColor = Color.FromArgb(255, 244, 206);
            }

            // Name-invalid marker: pale red with final visual priority. This is presentation-only
            // and independent from dirty-state tracking.
            if (columnName == "colName" && _invalidNameRows.Contains(row.ElementId))
            {
                e.CellStyle.BackColor = Color.FromArgb(255, 205, 210);
                e.CellStyle.SelectionBackColor = Color.FromArgb(255, 205, 210);
            }
        }

        private void MetadataGridView_CellValueChanged(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex < 0)
            {
                return;
            }

            if (!(metadataGridView.Rows[e.RowIndex].DataBoundItem is MetadataElementRow row))
            {
                return;
            }

            // Preserve Phase 1 behavior: clear the presentation-only invalid-Name marker
            // as soon as the Name is corrected. This does not calculate or maintain dirty state.
            if (metadataGridView.Columns[e.ColumnIndex].Name == "colName" &&
                _invalidNameRows.Contains(row.ElementId) &&
                !string.IsNullOrWhiteSpace(row.Name))
            {
                _invalidNameRows.Remove(row.ElementId);
            }

            // Refresh the entire row so CellFormatting reflects the current IsDirty state.
            metadataGridView.InvalidateRow(e.RowIndex);
        }

        private bool IsEditableColumn(string columnName)
        {
            return columnName == "colName" ||
                   columnName == "colAlias" ||
                   columnName == "colNotes";
        }

        private void MetadataGridView_EditingControlShowing(object sender, DataGridViewEditingControlShowingEventArgs e)
        {
            if (metadataGridView.CurrentCell?.OwningColumn.Name == "colNotes" && e.Control is TextBox textBox)
            {
                textBox.Multiline = true;
                textBox.AcceptsReturn = true;
            }
        }

        private void GuardarButton_Click(object sender, EventArgs e)
        {
            // End the active grid/binding edit so the value being typed is
            // pushed to the row before we evaluate IsDirty.
            metadataGridView.EndEdit();

            // Reset presentation-only validation markers. They will be re-applied
            // below if any dirty row still has a blank Name.
            _invalidNameRows.Clear();

            List<MetadataElementRow> dirtyRows = new List<MetadataElementRow>();

            foreach (MetadataElementRow row in _rows)
            {
                if (row.IsDirty)
                {
                    dirtyRows.Add(row);
                }
            }

            if (dirtyRows.Count == 0)
            {
                MessageBox.Show(
                    "No hay cambios pendientes para guardar.",
                    "Addino",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Information);

                return;
            }

            // Global pre-Save gate: every dirty row must have a non-empty Name.
            // If any fail, perform zero Element.Update() calls and keep the form open.
            List<MetadataElementRow> invalidNameRows = new List<MetadataElementRow>();

            foreach (MetadataElementRow row in dirtyRows)
            {
                if (string.IsNullOrWhiteSpace(row.Name))
                {
                    invalidNameRows.Add(row);
                }
            }

            if (invalidNameRows.Count > 0)
            {
                StringBuilder message = new StringBuilder();
                message.AppendLine("No se puede guardar porque los siguientes elementos tienen un Nombre vacío:");
                message.AppendLine();

                foreach (MetadataElementRow row in invalidNameRows)
                {
                    string displayName = string.IsNullOrWhiteSpace(row.Name)
                        ? "Nombre vacío"
                        : row.Name;

                    message.AppendLine($"• Elemento ID {row.ElementId} ({displayName})");
                    _invalidNameRows.Add(row.ElementId);
                }

                MessageBox.Show(
                    message.ToString(),
                    "Addino",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Warning);

                FocusInvalidNameRow(invalidNameRows[0]);
                metadataGridView.Refresh();
                return;
            }

            int successCount = 0;
            List<string> failures = new List<string>();

            foreach (MetadataElementRow row in dirtyRows)
            {
                EA.Element element;

                try
                {
                    element = _repository.GetElementByID(row.ElementId);
                }
                catch (Exception ex)
                {
                    failures.Add(
                        $"Elemento ID {row.ElementId} ({row.Name}): no se pudo recuperar del repositorio. {ex.Message}");

                    continue;
                }

                if (element == null)
                {
                    failures.Add(
                        $"Elemento ID {row.ElementId} ({row.Name}): no se encontró en el repositorio.");

                    continue;
                }

                try
                {
                    element.Name = row.Name ?? string.Empty;
                    element.Alias = row.Alias ?? string.Empty;
                    element.Notes = row.Notes ?? string.Empty;
                }
                catch (Exception ex)
                {
                    failures.Add(
                        $"Elemento ID {row.ElementId} ({row.Name}): error al asignar campos. {ex.Message}");

                    continue;
                }

                bool updated;

                try
                {
                    updated = element.Update();
                }
                catch (Exception ex)
                {
                    failures.Add(
                        $"Elemento ID {row.ElementId} ({row.Name}): error de COM/E-A al guardar. {ex.Message}");

                    continue;
                }

                if (!updated)
                {
                    failures.Add(
                        $"Elemento ID {row.ElementId} ({row.Name}): no se pudo guardar. El elemento puede estar bloqueado o no permitir escritura.");

                    continue;
                }

                // Only successful updates clear the pending/dirty state.
                row.AcceptChanges();
                successCount++;
            }

            ShowSaveResult(successCount, failures);
        }

        private void ShowSaveResult(int successCount, List<string> failures)
        {
            if (failures.Count == 0)
            {
                MessageBox.Show(
                    $"Se guardaron correctamente los cambios de {successCount} elemento(s).",
                    "Addino",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Information);

                return;
            }

            StringBuilder message = new StringBuilder();
            message.AppendLine("Algunos cambios no se pudieron guardar:");
            message.AppendLine();

            foreach (string failure in failures)
            {
                message.AppendLine($"• {failure}");
            }

            if (successCount > 0)
            {
                message.AppendLine();
                message.AppendLine($"Elementos guardados correctamente: {successCount}.");
            }

            MessageBox.Show(
                message.ToString(),
                "Addino",
                MessageBoxButtons.OK,
                MessageBoxIcon.Warning);
        }

        private void FocusInvalidNameRow(MetadataElementRow targetRow)
        {
            for (int i = 0; i < metadataGridView.Rows.Count; i++)
            {
                if (metadataGridView.Rows[i].DataBoundItem is MetadataElementRow row &&
                    row.ElementId == targetRow.ElementId)
                {
                    DataGridViewCell nameCell = metadataGridView.Rows[i].Cells["colName"];
                    metadataGridView.CurrentCell = nameCell;
                    metadataGridView.FirstDisplayedScrollingRowIndex = i;
                    break;
                }
            }
        }

        private void RecargarButton_Click(object sender, EventArgs e)
        {
            // End active DataGridView and binding edit so IsDirty evaluates the current value
            // and any pending keystroke is committed before we ask for confirmation or load.
            metadataGridView.EndEdit();

            if (BindingContext[_rows] is CurrencyManager currencyManager)
            {
                currencyManager.EndCurrentEdit();
            }

            bool hasDirtyRows = false;

            foreach (MetadataElementRow row in _rows)
            {
                if (row.IsDirty)
                {
                    hasDirtyRows = true;
                    break;
                }
            }

            if (hasDirtyRows)
            {
                DialogResult choice = MessageBox.Show(
                    "Hay cambios sin guardar. Recargar descartará esas modificaciones. ¿Desea continuar?",
                    "Addino",
                    MessageBoxButtons.YesNo,
                    MessageBoxIcon.Question,
                    MessageBoxDefaultButton.Button2);

                if (choice != DialogResult.Yes)
                {
                    return;
                }
            }

            PerformReload();
        }

        private void PerformReload()
        {
            if (_loader == null || _rootPackage == null)
            {
                MessageBox.Show(
                    "No se puede recargar porque no hay un paquete raíz disponible.",
                    "Addino",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Warning);

                return;
            }

            // Capture current view state for best-effort restoration after refill.
            int? selectedElementId = null;
            string currentColumnName = null;
            int? firstDisplayedScrollingRow = null;

            if (metadataGridView.CurrentRow?.DataBoundItem is MetadataElementRow currentRow)
            {
                selectedElementId = currentRow.ElementId;
            }

            if (metadataGridView.CurrentCell != null)
            {
                currentColumnName = metadataGridView.Columns[metadataGridView.CurrentCell.ColumnIndex].Name;
            }

            if (metadataGridView.FirstDisplayedScrollingRowIndex >= 0)
            {
                firstDisplayedScrollingRow = metadataGridView.FirstDisplayedScrollingRowIndex;
            }

            // Materialize the new load before touching the visible BindingList.
            BindingList<MetadataElementRow> loadedRows;
            List<string> warnings;

            try
            {
                loadedRows = _loader(_rootPackage, out warnings);
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    $"Error al recargar los elementos del paquete: {ex.Message}",
                    "Addino",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error);

                return;
            }

            if (loadedRows == null)
            {
                MessageBox.Show(
                    "Error al recargar: el cargador no devolvió resultados. Se conservan los datos actuales.",
                    "Addino",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error);

                return;
            }

            // Success: refill the existing BindingList instance in place.
            _rows.RaiseListChangedEvents = false;
            _rows.Clear();

            foreach (MetadataElementRow row in loadedRows)
            {
                _rows.Add(row);
            }

            _rows.RaiseListChangedEvents = true;
            _rows.ResetBindings();

            // Loaded rows are clean; clear presentation-only invalid-Name markers.
            _invalidNameRows.Clear();

            // Best-effort restore of selection, current column, and scroll position.
            RestoreGridViewState(selectedElementId, currentColumnName, firstDisplayedScrollingRow);

            // Surface any loader warnings with explicit incomplete-load wording.
            if (warnings != null && warnings.Count > 0)
            {
                string details = string.Join(Environment.NewLine, warnings);

                MessageBox.Show(
                    $"La recarga se completó, pero algunos elementos no pudieron leerse y fueron omitidos. Es posible que la carga esté incompleta:{Environment.NewLine}{details}",
                    "Addino",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Warning);
            }
        }

        private void RestoreGridViewState(int? elementId, string columnName, int? firstDisplayedScrollingRow)
        {
            try
            {
                if (elementId.HasValue)
                {
                    int targetRowIndex = -1;

                    for (int i = 0; i < metadataGridView.Rows.Count; i++)
                    {
                        if (metadataGridView.Rows[i].DataBoundItem is MetadataElementRow row &&
                            row.ElementId == elementId.Value)
                        {
                            targetRowIndex = i;
                            break;
                        }
                    }

                    if (targetRowIndex >= 0)
                    {
                        int targetColumnIndex = 0;

                        if (!string.IsNullOrEmpty(columnName))
                        {
                            for (int i = 0; i < metadataGridView.Columns.Count; i++)
                            {
                                if (metadataGridView.Columns[i].Name == columnName)
                                {
                                    targetColumnIndex = i;
                                    break;
                                }
                            }
                        }

                        DataGridViewCell targetCell = metadataGridView.Rows[targetRowIndex].Cells[targetColumnIndex];
                        metadataGridView.CurrentCell = targetCell;
                    }
                }

                if (firstDisplayedScrollingRow.HasValue &&
                    firstDisplayedScrollingRow.Value >= 0 &&
                    firstDisplayedScrollingRow.Value < metadataGridView.Rows.Count)
                {
                    metadataGridView.FirstDisplayedScrollingRowIndex = firstDisplayedScrollingRow.Value;
                }
            }
            catch
            {
                // Best-effort only: never fail reload because of selection/scroll restoration.
            }
        }

        private void CancelarButton_Click(object sender, EventArgs e)
        {
            // Work Unit 2: discard local edits by closing with Cancel.
            // No Element.Update() path exists in this form.
            DialogResult = DialogResult.Cancel;
            Close();
        }
    }
}
