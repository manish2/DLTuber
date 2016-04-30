namespace DLTuber
{
    partial class FormStatus
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
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
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.statusBar = new System.Windows.Forms.ProgressBar();
            this.vidTitle = new System.Windows.Forms.Label();
            this.cancelButton = new System.Windows.Forms.Button();
            this.statusLabel = new System.Windows.Forms.Label();
            this.SuspendLayout();
            // 
            // statusBar
            // 
            this.statusBar.Location = new System.Drawing.Point(9, 57);
            this.statusBar.Margin = new System.Windows.Forms.Padding(2);
            this.statusBar.Name = "statusBar";
            this.statusBar.Size = new System.Drawing.Size(350, 19);
            this.statusBar.TabIndex = 0;
            // 
            // vidTitle
            // 
            this.vidTitle.AutoSize = true;
            this.vidTitle.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.vidTitle.Location = new System.Drawing.Point(11, 31);
            this.vidTitle.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.vidTitle.Name = "vidTitle";
            this.vidTitle.Size = new System.Drawing.Size(32, 15);
            this.vidTitle.TabIndex = 1;
            this.vidTitle.Text = "Title:";
            // 
            // cancelButton
            // 
            this.cancelButton.Location = new System.Drawing.Point(284, 94);
            this.cancelButton.Name = "cancelButton";
            this.cancelButton.Size = new System.Drawing.Size(75, 23);
            this.cancelButton.TabIndex = 2;
            this.cancelButton.Text = "Cancel";
            this.cancelButton.UseVisualStyleBackColor = true;
            // 
            // statusLabel
            // 
            this.statusLabel.AutoSize = true;
            this.statusLabel.Location = new System.Drawing.Point(12, 94);
            this.statusLabel.Name = "statusLabel";
            this.statusLabel.Size = new System.Drawing.Size(0, 13);
            this.statusLabel.TabIndex = 3;
            // 
            // FormStatus
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(370, 129);
            this.Controls.Add(this.statusLabel);
            this.Controls.Add(this.cancelButton);
            this.Controls.Add(this.vidTitle);
            this.Controls.Add(this.statusBar);
            this.Margin = new System.Windows.Forms.Padding(2);
            this.Name = "FormStatus";
            this.Text = "Download Progress";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.ProgressBar statusBar;
        private System.Windows.Forms.Label vidTitle;
        private System.Windows.Forms.Button cancelButton;
        private System.Windows.Forms.Label statusLabel;
    }
}