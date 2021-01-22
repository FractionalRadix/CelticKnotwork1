
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
            this.mtbNrOfRows.Location = new System.Drawing.Point(94, 29);
            this.mtbNrOfRows.Mask = "00000";
            this.mtbNrOfRows.Name = "mtbNrOfRows";
            this.mtbNrOfRows.Size = new System.Drawing.Size(100, 23);
            this.mtbNrOfRows.TabIndex = 1;
            this.mtbNrOfRows.ValidatingType = typeof(int);
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1240, 627);
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
    }
}

