namespace WindowsFormsApplication1
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Form1));
            this.fASIBTDEPARTMENTSBindingSource = new System.Windows.Forms.BindingSource(this.components);
            this.dataSet1 = new WindowsFormsApplication1.DataSet1();
            this.fAS_IBT_DEPARTMENTSTableAdapter = new WindowsFormsApplication1.DataSet1TableAdapters.FAS_IBT_DEPARTMENTSTableAdapter();
            this.cmdExit = new System.Windows.Forms.Button();
            this.timer1 = new System.Windows.Forms.Timer(this.components);
            this.lblGeneral = new System.Windows.Forms.Label();
            this.lblLife = new System.Windows.Forms.Label();
            this.lblNonTCS = new System.Windows.Forms.Label();
            this.notifyIcon1 = new System.Windows.Forms.NotifyIcon(this.components);
            ((System.ComponentModel.ISupportInitialize)(this.fASIBTDEPARTMENTSBindingSource)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.dataSet1)).BeginInit();
            this.SuspendLayout();
            // 
            // fASIBTDEPARTMENTSBindingSource
            // 
            this.fASIBTDEPARTMENTSBindingSource.DataMember = "FAS_IBT_DEPARTMENTS";
            this.fASIBTDEPARTMENTSBindingSource.DataSource = this.dataSet1;
            // 
            // dataSet1
            // 
            this.dataSet1.DataSetName = "DataSet1";
            this.dataSet1.SchemaSerializationMode = System.Data.SchemaSerializationMode.IncludeSchema;
            // 
            // fAS_IBT_DEPARTMENTSTableAdapter
            // 
            this.fAS_IBT_DEPARTMENTSTableAdapter.ClearBeforeFill = true;
            // 
            // cmdExit
            // 
            this.cmdExit.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(192)))), ((int)(((byte)(64)))), ((int)(((byte)(0)))));
            this.cmdExit.ForeColor = System.Drawing.SystemColors.ButtonHighlight;
            this.cmdExit.Location = new System.Drawing.Point(454, 24);
            this.cmdExit.Name = "cmdExit";
            this.cmdExit.Size = new System.Drawing.Size(126, 96);
            this.cmdExit.TabIndex = 3;
            this.cmdExit.Text = "Exit";
            this.cmdExit.UseVisualStyleBackColor = false;
            this.cmdExit.Click += new System.EventHandler(this.cmdExit_Click);
            // 
            // timer1
            // 
            this.timer1.Enabled = true;
            this.timer1.Interval = 20000;
            this.timer1.Tick += new System.EventHandler(this.timer1_Tick);
            // 
            // lblGeneral
            // 
            this.lblGeneral.BackColor = System.Drawing.Color.Gray;
            this.lblGeneral.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblGeneral.ForeColor = System.Drawing.SystemColors.ButtonHighlight;
            this.lblGeneral.Location = new System.Drawing.Point(27, 24);
            this.lblGeneral.Name = "lblGeneral";
            this.lblGeneral.Size = new System.Drawing.Size(371, 21);
            this.lblGeneral.TabIndex = 4;
            this.lblGeneral.Text = "General Receipt Batch";
            // 
            // lblLife
            // 
            this.lblLife.BackColor = System.Drawing.Color.Gray;
            this.lblLife.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblLife.ForeColor = System.Drawing.SystemColors.ButtonHighlight;
            this.lblLife.Location = new System.Drawing.Point(27, 61);
            this.lblLife.Name = "lblLife";
            this.lblLife.Size = new System.Drawing.Size(371, 21);
            this.lblLife.TabIndex = 5;
            this.lblLife.Text = "Life Receipt Batch";
            // 
            // lblNonTCS
            // 
            this.lblNonTCS.BackColor = System.Drawing.Color.Gray;
            this.lblNonTCS.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblNonTCS.ForeColor = System.Drawing.SystemColors.ButtonHighlight;
            this.lblNonTCS.Location = new System.Drawing.Point(27, 97);
            this.lblNonTCS.Name = "lblNonTCS";
            this.lblNonTCS.Size = new System.Drawing.Size(371, 21);
            this.lblNonTCS.TabIndex = 6;
            this.lblNonTCS.Text = "NonTCS Receipt Batch";
            // 
            // notifyIcon1
            // 
            this.notifyIcon1.BalloonTipIcon = System.Windows.Forms.ToolTipIcon.Info;
            this.notifyIcon1.BalloonTipText = "dddd";
            this.notifyIcon1.BalloonTipTitle = "dddd";
            this.notifyIcon1.Icon = ((System.Drawing.Icon)(resources.GetObject("notifyIcon1.Icon")));
            this.notifyIcon1.Text = "IBT Batch Pooling App";
            this.notifyIcon1.Visible = true;
            this.notifyIcon1.DoubleClick += new System.EventHandler(this.notifyIcon1_DoubleClick);
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(601, 141);
            this.Controls.Add(this.lblNonTCS);
            this.Controls.Add(this.lblLife);
            this.Controls.Add(this.lblGeneral);
            this.Controls.Add(this.cmdExit);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.Fixed3D;
            this.Name = "Form1";
            this.Text = "IBT Automated Receipting Application";
            this.Load += new System.EventHandler(this.Form1_Load);
            ((System.ComponentModel.ISupportInitialize)(this.fASIBTDEPARTMENTSBindingSource)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.dataSet1)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private DataSet1 dataSet1;
        private System.Windows.Forms.BindingSource fASIBTDEPARTMENTSBindingSource;
        private DataSet1TableAdapters.FAS_IBT_DEPARTMENTSTableAdapter fAS_IBT_DEPARTMENTSTableAdapter;
        private System.Windows.Forms.Button cmdExit;
        private System.Windows.Forms.Timer timer1;
        private System.Windows.Forms.Label lblGeneral;
        private System.Windows.Forms.Label lblLife;
        private System.Windows.Forms.Label lblNonTCS;
        private System.Windows.Forms.NotifyIcon notifyIcon1;
    }
}