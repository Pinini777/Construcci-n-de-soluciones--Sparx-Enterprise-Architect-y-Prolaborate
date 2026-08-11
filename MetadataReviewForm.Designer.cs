namespace Addino
{
    partial class MetadataReviewForm
    {
        private System.ComponentModel.IContainer components = null;

        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        private void InitializeComponent()
        {
            this.metadataGridView = new System.Windows.Forms.DataGridView();
            this.buttonPanel = new System.Windows.Forms.Panel();
            this.cancelarButton = new System.Windows.Forms.Button();
            this.guardarButton = new System.Windows.Forms.Button();
            ((System.ComponentModel.ISupportInitialize)(this.metadataGridView)).BeginInit();
            this.buttonPanel.SuspendLayout();
            this.SuspendLayout();
            //
            // metadataGridView
            //
            this.metadataGridView.Dock = System.Windows.Forms.DockStyle.Fill;
            this.metadataGridView.Location = new System.Drawing.Point(0, 0);
            this.metadataGridView.Name = "metadataGridView";
            this.metadataGridView.Size = new System.Drawing.Size(900, 430);
            this.metadataGridView.TabIndex = 0;
            //
            // buttonPanel
            //
            this.buttonPanel.Controls.Add(this.cancelarButton);
            this.buttonPanel.Controls.Add(this.guardarButton);
            this.buttonPanel.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.buttonPanel.Location = new System.Drawing.Point(0, 430);
            this.buttonPanel.Name = "buttonPanel";
            this.buttonPanel.Size = new System.Drawing.Size(900, 50);
            this.buttonPanel.TabIndex = 1;
            //
            // cancelarButton
            //
            this.cancelarButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.cancelarButton.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.cancelarButton.Location = new System.Drawing.Point(792, 12);
            this.cancelarButton.Name = "cancelarButton";
            this.cancelarButton.Size = new System.Drawing.Size(96, 26);
            this.cancelarButton.TabIndex = 2;
            this.cancelarButton.Text = "Cancelar";
            this.cancelarButton.UseVisualStyleBackColor = true;
            this.cancelarButton.Click += new System.EventHandler(this.CancelarButton_Click);
            //
            // guardarButton
            //
            this.guardarButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.guardarButton.DialogResult = System.Windows.Forms.DialogResult.OK;
            this.guardarButton.Location = new System.Drawing.Point(690, 12);
            this.guardarButton.Name = "guardarButton";
            this.guardarButton.Size = new System.Drawing.Size(96, 26);
            this.guardarButton.TabIndex = 1;
            this.guardarButton.Text = "Guardar";
            this.guardarButton.UseVisualStyleBackColor = true;
            this.guardarButton.Click += new System.EventHandler(this.GuardarButton_Click);
            //
            // MetadataReviewForm
            //
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(900, 480);
            this.Controls.Add(this.metadataGridView);
            this.Controls.Add(this.buttonPanel);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.Sizable;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "MetadataReviewForm";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "Revisión de Metadatos";
            this.CancelButton = this.cancelarButton;
            ((System.ComponentModel.ISupportInitialize)(this.metadataGridView)).EndInit();
            this.buttonPanel.ResumeLayout(false);
            this.buttonPanel.PerformLayout();
            this.ResumeLayout(false);

        }

        private System.Windows.Forms.DataGridView metadataGridView;
        private System.Windows.Forms.Panel buttonPanel;
        private System.Windows.Forms.Button cancelarButton;
        private System.Windows.Forms.Button guardarButton;
    }
}
