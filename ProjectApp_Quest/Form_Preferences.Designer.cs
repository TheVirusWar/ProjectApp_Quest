
namespace ProjectApp_Quest
{
    partial class Form_Preferences
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
            this.label1 = new System.Windows.Forms.Label();
            this.textBoxRecentNumber = new System.Windows.Forms.TextBox();
            this.ButtonPreferencesOK = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(12, 34);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(152, 13);
            this.label1.TabIndex = 0;
            this.label1.Text = "Number of recent files tracked:";
            // 
            // textBoxRecentNumber
            // 
            this.textBoxRecentNumber.Location = new System.Drawing.Point(170, 31);
            this.textBoxRecentNumber.Name = "textBoxRecentNumber";
            this.textBoxRecentNumber.Size = new System.Drawing.Size(70, 20);
            this.textBoxRecentNumber.TabIndex = 1;
            // 
            // ButtonPreferencesOK
            // 
            this.ButtonPreferencesOK.Location = new System.Drawing.Point(264, 161);
            this.ButtonPreferencesOK.Name = "ButtonPreferencesOK";
            this.ButtonPreferencesOK.Size = new System.Drawing.Size(75, 23);
            this.ButtonPreferencesOK.TabIndex = 2;
            this.ButtonPreferencesOK.Text = "OK";
            this.ButtonPreferencesOK.UseVisualStyleBackColor = true;
            this.ButtonPreferencesOK.Click += new System.EventHandler(this.ButtonPreferencesOK_Click);
            // 
            // Form_Preferences
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(355, 200);
            this.Controls.Add(this.ButtonPreferencesOK);
            this.Controls.Add(this.textBoxRecentNumber);
            this.Controls.Add(this.label1);
            this.Name = "Form_Preferences";
            this.Text = "Form_Preferences";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.TextBox textBoxRecentNumber;
        private System.Windows.Forms.Button ButtonPreferencesOK;
    }
}