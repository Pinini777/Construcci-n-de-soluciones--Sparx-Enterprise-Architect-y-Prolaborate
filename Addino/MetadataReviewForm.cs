using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;
using System.Windows.Forms;

namespace Addino
{
    public partial class MetadataReviewForm : Form
    {
        private readonly EA.Repository _repository;
        private readonly BindingList<MetadataElementRow> _rows;

        public MetadataReviewForm(EA.Repository repository, BindingList<MetadataElementRow> rows)
        {
            _repository = repository;
            _rows = rows ?? new BindingList<MetadataElementRow>();
            InitializeComponent();
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

            metadataGridView.AutoSizeRowsMode = DataGridViewAutoSizeRowsMode.AllCells;
            metadataGridView.AllowUserToAddRows = false;
            metadataGridView.AllowUserToDeleteRows = false;
            metadataGridView.MultiSelect = false;
            metadataGridView.SelectionMode = DataGridViewSelectionMode.CellSelect;
            metadataGridView.EditingControlShowing += MetadataGridView_EditingControlShowing;
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

        private void CancelarButton_Click(object sender, EventArgs e)
        {
            // Work Unit 2: discard local edits by closing with Cancel.
            // No Element.Update() path exists in this form.
            DialogResult = DialogResult.Cancel;
            Close();
        }
    }
}
