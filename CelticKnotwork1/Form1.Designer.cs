
namespace CelticKnotwork1
{
    partial class Form1
    {
        /// <summary>
        ///  Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        ///  Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        ///  Required method for Designer support - do not modify
        ///  the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            this.timer1 = new System.Windows.Forms.Timer(this.components);
            this.lblNrOfRows = new System.Windows.Forms.Label();
            this.mtbNrOfRows = new System.Windows.Forms.MaskedTextBox();
            this.mtbNrOfColumns = new System.Windows.Forms.MaskedTextBox();
            this.mtbBorderWidth = new System.Windows.Forms.MaskedTextBox();
            this.lblNrOfColumns = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.chbDoubleLines = new System.Windows.Forms.CheckBox();
            this.btnStartDrawing = new System.Windows.Forms.Button();
            this.saveFileDialog1 = new System.Windows.Forms.SaveFileDialog();
            this.btExportToSvg = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // lblNrOfRows
            // 
            this.lblNrOfRows.AutoSize = true;
            this.lblNrOfRows.Location = new System.Drawing.Point(22, 29);
            this.lblNrOfRows.Name = "lblNrOfRows";
            this.lblNrOfRows.Size = new System.Drawing.Size(65, 15);
            this.lblNrOfRows.TabIndex = 0;
            this.lblNrOfRows.Text = "Nr of rows:";
            // 
            // mtbNrOfRows
            // 
            this.mtbNrOfRows.Location = new System.Drawing.Point(114, 26);
            this.mtbNrOfRows.Mask = "00000";
            this.mtbNrOfRows.Name = "mtbNrOfRows";
            this.mtbNrOfRows.Size = new System.Drawing.Size(100, 23);
            this.mtbNrOfRows.TabIndex = 1;
            this.mtbNrOfRows.Text = "15";
            this.mtbNrOfRows.ValidatingType = typeof(int);
            // 
            // mtbNrOfColumns
            // 
            this.mtbNrOfColumns.Location = new System.Drawing.Point(114, 63);
            this.mtbNrOfColumns.Mask = "00000";
            this.mtbNrOfColumns.Name = "mtbNrOfColumns";
            this.mtbNrOfColumns.Size = new System.Drawing.Size(100, 23);
            this.mtbNrOfColumns.TabIndex = 2;
            this.mtbNrOfColumns.Text = "47";
            this.mtbNrOfColumns.ValidatingType = typeof(int);
            // 
            // mtbBorderWidth
            // 
            this.mtbBorderWidth.Location = new System.Drawing.Point(114, 99);
            this.mtbBorderWidth.Mask = "00000";
            this.mtbBorderWidth.Name = "mtbBorderWidth";
            this.mtbBorderWidth.Size = new System.Drawing.Size(100, 23);
            this.mtbBorderWidth.TabIndex = 3;
            this.mtbBorderWidth.Text = "3";
            this.mtbBorderWidth.ValidatingType = typeof(int);
            // 
            // lblNrOfColumns
            // 
            this.lblNrOfColumns.AutoSize = true;
            this.lblNrOfColumns.Location = new System.Drawing.Point(22, 66);
            this.lblNrOfColumns.Name = "lblNrOfColumns";
            this.lblNrOfColumns.Size = new System.Drawing.Size(86, 15);
            this.lblNrOfColumns.TabIndex = 4;
            this.lblNrOfColumns.Text = "Nr of columns:";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(22, 99);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(78, 15);
            this.label2.TabIndex = 5;
            this.label2.Text = "Border width:";
            // 
            // chbDoubleLines
            // 
            this.chbDoubleLines.AutoSize = true;
            this.chbDoubleLines.Checked = true;
            this.chbDoubleLines.CheckState = System.Windows.Forms.CheckState.Checked;
            this.chbDoubleLines.Location = new System.Drawing.Point(78, 140);
            this.chbDoubleLines.Name = "chbDoubleLines";
            this.chbDoubleLines.Size = new System.Drawing.Size(91, 19);
            this.chbDoubleLines.TabIndex = 6;
            this.chbDoubleLines.Text = "Double lines";
            this.chbDoubleLines.UseVisualStyleBackColor = true;
            // 
            // btnStartDrawing
            // 
            this.btnStartDrawing.Location = new System.Drawing.Point(78, 178);
            this.btnStartDrawing.Name = "btnStartDrawing";
            this.btnStartDrawing.Size = new System.Drawing.Size(91, 23);
            this.btnStartDrawing.TabIndex = 7;
            this.btnStartDrawing.Text = "Draw it!";
            this.btnStartDrawing.UseVisualStyleBackColor = true;
            this.btnStartDrawing.Click += new System.EventHandler(this.btnStartDrawing_Click);
            // 
            // btExportToSvg
            // 
            this.btExportToSvg.Location = new System.Drawing.Point(78, 253);
            this.btExportToSvg.Name = "btExportToSvg";
            this.btExportToSvg.Size = new System.Drawing.Size(91, 23);
            this.btExportToSvg.TabIndex = 8;
            this.btExportToSvg.Text = "Export to SVG";
            this.btExportToSvg.UseVisualStyleBackColor = true;
            this.btExportToSvg.Click += new System.EventHandler(this.btExportToSvg_Click);
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1024, 627);
            this.Controls.Add(this.btExportToSvg);
            this.Controls.Add(this.btnStartDrawing);
            this.Controls.Add(this.chbDoubleLines);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.lblNrOfColumns);
            this.Controls.Add(this.mtbBorderWidth);
            this.Controls.Add(this.mtbNrOfColumns);
            this.Controls.Add(this.mtbNrOfRows);
            this.Controls.Add(this.lblNrOfRows);
            this.Name = "Form1";
            this.Text = "Celtic Knotwork Border Designer";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Timer timer1;
        private System.Windows.Forms.Label lblNrOfRows;
        private System.Windows.Forms.MaskedTextBox mtbNrOfRows;
        private System.Windows.Forms.MaskedTextBox mtbNrOfColumns;
        private System.Windows.Forms.MaskedTextBox mtbBorderWidth;
        private System.Windows.Forms.Label lblNrOfColumns;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.CheckBox chbDoubleLines;
        private System.Windows.Forms.Button btnStartDrawing;
        private System.Windows.Forms.SaveFileDialog saveFileDialog1;
        private System.Windows.Forms.Button btExportToSvg;
    }
}

