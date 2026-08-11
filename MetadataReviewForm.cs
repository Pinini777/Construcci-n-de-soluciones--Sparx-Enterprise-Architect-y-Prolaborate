using System;
using System.ComponentModel;
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
            // Work Unit 2: end active edit so the active value is included,
            // but do not call Element.Update() or otherwise persist.
            metadataGridView.EndEdit();
            DialogResult = DialogResult.OK;
            Close();
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
