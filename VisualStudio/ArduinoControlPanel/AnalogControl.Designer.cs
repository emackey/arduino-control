namespace ArduinoControlPanel
{
    partial class AnalogControl
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

        #region Component Designer generated code

        /// <summary> 
        /// Required method for Designer support - do not modify 
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.groupBoxAnalog = new System.Windows.Forms.GroupBox();
            this.trackBarAnalog = new System.Windows.Forms.TrackBar();
            this.labelMax = new System.Windows.Forms.Label();
            this.labelMin = new System.Windows.Forms.Label();
            this.textBoxAnalog = new System.Windows.Forms.TextBox();
            this.groupBoxAnalog.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.trackBarAnalog)).BeginInit();
            this.SuspendLayout();
            // 
            // groupBoxAnalog
            // 
            this.groupBoxAnalog.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.groupBoxAnalog.Controls.Add(this.textBoxAnalog);
            this.groupBoxAnalog.Controls.Add(this.labelMin);
            this.groupBoxAnalog.Controls.Add(this.labelMax);
            this.groupBoxAnalog.Controls.Add(this.trackBarAnalog);
            this.groupBoxAnalog.Location = new System.Drawing.Point(3, 3);
            this.groupBoxAnalog.Name = "groupBoxAnalog";
            this.groupBoxAnalog.Size = new System.Drawing.Size(87, 256);
            this.groupBoxAnalog.TabIndex = 0;
            this.groupBoxAnalog.TabStop = false;
            this.groupBoxAnalog.Text = "(name)";
            // 
            // trackBarAnalog
            // 
            this.trackBarAnalog.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left)));
            this.trackBarAnalog.LargeChange = 16;
            this.trackBarAnalog.Location = new System.Drawing.Point(32, 19);
            this.trackBarAnalog.Maximum = 255;
            this.trackBarAnalog.Name = "trackBarAnalog";
            this.trackBarAnalog.Orientation = System.Windows.Forms.Orientation.Vertical;
            this.trackBarAnalog.Size = new System.Drawing.Size(45, 205);
            this.trackBarAnalog.TabIndex = 0;
            this.trackBarAnalog.TickFrequency = 16;
            this.trackBarAnalog.TickStyle = System.Windows.Forms.TickStyle.TopLeft;
            this.trackBarAnalog.Scroll += new System.EventHandler(this.trackBarAnalog_Scroll);
            // 
            // labelMax
            // 
            this.labelMax.AutoSize = true;
            this.labelMax.Location = new System.Drawing.Point(6, 26);
            this.labelMax.Name = "labelMax";
            this.labelMax.Size = new System.Drawing.Size(25, 13);
            this.labelMax.TabIndex = 1;
            this.labelMax.Text = "255";
            // 
            // labelMin
            // 
            this.labelMin.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.labelMin.AutoSize = true;
            this.labelMin.Location = new System.Drawing.Point(18, 204);
            this.labelMin.Name = "labelMin";
            this.labelMin.Size = new System.Drawing.Size(13, 13);
            this.labelMin.TabIndex = 2;
            this.labelMin.Text = "0";
            // 
            // textBoxAnalog
            // 
            this.textBoxAnalog.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.textBoxAnalog.Location = new System.Drawing.Point(6, 230);
            this.textBoxAnalog.MaxLength = 5;
            this.textBoxAnalog.Name = "textBoxAnalog";
            this.textBoxAnalog.Size = new System.Drawing.Size(71, 20);
            this.textBoxAnalog.TabIndex = 3;
            this.textBoxAnalog.TextChanged += new System.EventHandler(this.textBoxAnalog_TextChanged);
            // 
            // AnalogControl
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.groupBoxAnalog);
            this.Name = "AnalogControl";
            this.Size = new System.Drawing.Size(93, 262);
            this.groupBoxAnalog.ResumeLayout(false);
            this.groupBoxAnalog.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.trackBarAnalog)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.GroupBox groupBoxAnalog;
        private System.Windows.Forms.TextBox textBoxAnalog;
        private System.Windows.Forms.Label labelMin;
        private System.Windows.Forms.Label labelMax;
        private System.Windows.Forms.TrackBar trackBarAnalog;
    }
}
