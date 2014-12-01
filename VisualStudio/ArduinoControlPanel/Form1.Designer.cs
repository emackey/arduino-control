namespace ArduinoControlPanel
{
    partial class Form1
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
            this.components = new System.ComponentModel.Container();
            this.panelForControls = new System.Windows.Forms.Panel();
            this.labelCOM = new System.Windows.Forms.Label();
            this.comboBoxCOM = new System.Windows.Forms.ComboBox();
            this.timerSerialAvailability = new System.Windows.Forms.Timer(this.components);
            this.SuspendLayout();
            // 
            // panelForControls
            // 
            this.panelForControls.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.panelForControls.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.panelForControls.Location = new System.Drawing.Point(12, 41);
            this.panelForControls.Name = "panelForControls";
            this.panelForControls.Size = new System.Drawing.Size(531, 209);
            this.panelForControls.TabIndex = 0;
            // 
            // labelCOM
            // 
            this.labelCOM.AutoSize = true;
            this.labelCOM.Location = new System.Drawing.Point(9, 12);
            this.labelCOM.Name = "labelCOM";
            this.labelCOM.Size = new System.Drawing.Size(57, 13);
            this.labelCOM.TabIndex = 1;
            this.labelCOM.Text = "Serial port:";
            // 
            // comboBoxCOM
            // 
            this.comboBoxCOM.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.comboBoxCOM.FormattingEnabled = true;
            this.comboBoxCOM.Location = new System.Drawing.Point(72, 9);
            this.comboBoxCOM.Name = "comboBoxCOM";
            this.comboBoxCOM.Size = new System.Drawing.Size(121, 21);
            this.comboBoxCOM.TabIndex = 2;
            this.comboBoxCOM.SelectedIndexChanged += new System.EventHandler(this.comboBoxCOM_SelectedIndexChanged);
            // 
            // timerSerialAvailability
            // 
            this.timerSerialAvailability.Interval = 500;
            this.timerSerialAvailability.Tick += new System.EventHandler(this.timerSerialAvailability_Tick);
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(555, 262);
            this.Controls.Add(this.comboBoxCOM);
            this.Controls.Add(this.labelCOM);
            this.Controls.Add(this.panelForControls);
            this.Name = "Form1";
            this.Text = "Form1";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.Form1_FormClosing);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Panel panelForControls;
        private System.Windows.Forms.Label labelCOM;
        private System.Windows.Forms.ComboBox comboBoxCOM;
        private System.Windows.Forms.Timer timerSerialAvailability;
    }
}

