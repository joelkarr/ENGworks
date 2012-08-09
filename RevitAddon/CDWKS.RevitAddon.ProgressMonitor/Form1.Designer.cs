using System;
using System.ComponentModel;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Net;
using System.Threading;
using System.Windows.Forms;
using CDWKS.Model.EF.FileQueue;
using Timer = System.Windows.Forms.Timer;

namespace CDWKS.RevitAddOn.ProgressMonitor
{
    partial class Form1
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private IContainer components = null;

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
            this.btnIndex = new System.Windows.Forms.Button();
            this.dataGridView1 = new System.Windows.Forms.DataGridView();
            this.rbFullIndex = new System.Windows.Forms.RadioButton();
            this.panel1 = new System.Windows.Forms.Panel();
            this.rbNewAndVersion = new System.Windows.Forms.RadioButton();
            this.rbNewOnly = new System.Windows.Forms.RadioButton();
            this.panel2 = new System.Windows.Forms.Panel();
            this.panel3 = new System.Windows.Forms.Panel();
            this.panel4 = new System.Windows.Forms.Panel();
            this.panel6 = new System.Windows.Forms.Panel();
            this.lblTimeStamp = new System.Windows.Forms.Label();
            this.lblFailed = new System.Windows.Forms.Label();
            this.lblDataSynced = new System.Windows.Forms.Label();
            this.lblSynced = new System.Windows.Forms.Label();
            this.lblQueued = new System.Windows.Forms.Label();
            this.timer2 = new System.Windows.Forms.Timer(this.components);
            this.timer3 = new System.Windows.Forms.Timer(this.components);
            this.fileQueueBindingSource = new System.Windows.Forms.BindingSource(this.components);
            this.statusDataGridViewTextBoxColumn = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.filePathDataGridViewTextBoxColumn = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.attemptsDataGridViewTextBoxColumn = new System.Windows.Forms.DataGridViewTextBoxColumn();
            ((System.ComponentModel.ISupportInitialize)(this.dataGridView1)).BeginInit();
            this.panel1.SuspendLayout();
            this.panel2.SuspendLayout();
            this.panel4.SuspendLayout();
            this.panel6.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.fileQueueBindingSource)).BeginInit();
            this.SuspendLayout();
            // 
            // btnIndex
            // 
            this.btnIndex.Location = new System.Drawing.Point(24, 28);
            this.btnIndex.Name = "btnIndex";
            this.btnIndex.Size = new System.Drawing.Size(153, 52);
            this.btnIndex.TabIndex = 0;
            this.btnIndex.Text = "Add Revit Families To Index";
            this.btnIndex.UseVisualStyleBackColor = true;
            this.btnIndex.Click += new System.EventHandler(this.btnIndex_Click);
            // 
            // dataGridView1
            // 
            this.dataGridView1.AllowUserToAddRows = false;
            this.dataGridView1.AllowUserToDeleteRows = false;
            this.dataGridView1.AutoGenerateColumns = false;
            this.dataGridView1.AutoSizeColumnsMode = System.Windows.Forms.DataGridViewAutoSizeColumnsMode.AllCells;
            this.dataGridView1.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dataGridView1.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.statusDataGridViewTextBoxColumn,
            this.filePathDataGridViewTextBoxColumn,
            this.attemptsDataGridViewTextBoxColumn});
            this.dataGridView1.DataSource = this.fileQueueBindingSource;
            this.dataGridView1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.dataGridView1.Location = new System.Drawing.Point(0, 0);
            this.dataGridView1.Name = "dataGridView1";
            this.dataGridView1.ReadOnly = true;
            this.dataGridView1.Size = new System.Drawing.Size(580, 392);
            this.dataGridView1.TabIndex = 1;
            // 
            // rbFullIndex
            // 
            this.rbFullIndex.AutoSize = true;
            this.rbFullIndex.Checked = true;
            this.rbFullIndex.Location = new System.Drawing.Point(28, 30);
            this.rbFullIndex.Name = "rbFullIndex";
            this.rbFullIndex.Size = new System.Drawing.Size(84, 17);
            this.rbFullIndex.TabIndex = 2;
            this.rbFullIndex.TabStop = true;
            this.rbFullIndex.Text = "Overwrite All";
            this.rbFullIndex.UseVisualStyleBackColor = true;
            // 
            // panel1
            // 
            this.panel1.Controls.Add(this.rbNewAndVersion);
            this.panel1.Controls.Add(this.rbNewOnly);
            this.panel1.Controls.Add(this.rbFullIndex);
            this.panel1.Location = new System.Drawing.Point(12, 121);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(200, 145);
            this.panel1.TabIndex = 3;
            this.panel1.Visible = false;
            // 
            // rbNewAndVersion
            // 
            this.rbNewAndVersion.AutoSize = true;
            this.rbNewAndVersion.Location = new System.Drawing.Point(28, 76);
            this.rbNewAndVersion.Name = "rbNewAndVersion";
            this.rbNewAndVersion.Size = new System.Drawing.Size(149, 17);
            this.rbNewAndVersion.TabIndex = 4;
            this.rbNewAndVersion.Text = "New and Version Updates";
            this.rbNewAndVersion.UseVisualStyleBackColor = true;
            // 
            // rbNewOnly
            // 
            this.rbNewOnly.AutoSize = true;
            this.rbNewOnly.Location = new System.Drawing.Point(28, 53);
            this.rbNewOnly.Name = "rbNewOnly";
            this.rbNewOnly.Size = new System.Drawing.Size(71, 17);
            this.rbNewOnly.TabIndex = 3;
            this.rbNewOnly.Text = "New Only";
            this.rbNewOnly.UseVisualStyleBackColor = true;
            // 
            // panel2
            // 
            this.panel2.Controls.Add(this.btnIndex);
            this.panel2.Location = new System.Drawing.Point(12, 33);
            this.panel2.Name = "panel2";
            this.panel2.Size = new System.Drawing.Size(200, 100);
            this.panel2.TabIndex = 4;
            // 
            // panel3
            // 
            this.panel3.Location = new System.Drawing.Point(13, 266);
            this.panel3.Name = "panel3";
            this.panel3.Size = new System.Drawing.Size(198, 52);
            this.panel3.TabIndex = 5;
            // 
            // panel4
            // 
            this.panel4.AutoScroll = true;
            this.panel4.Controls.Add(this.dataGridView1);
            this.panel4.Location = new System.Drawing.Point(228, 30);
            this.panel4.Name = "panel4";
            this.panel4.Size = new System.Drawing.Size(580, 392);
            this.panel4.TabIndex = 6;
            // 
            // panel6
            // 
            this.panel6.Controls.Add(this.lblTimeStamp);
            this.panel6.Controls.Add(this.lblFailed);
            this.panel6.Controls.Add(this.lblDataSynced);
            this.panel6.Controls.Add(this.lblSynced);
            this.panel6.Controls.Add(this.lblQueued);
            this.panel6.Location = new System.Drawing.Point(12, 253);
            this.panel6.Name = "panel6";
            this.panel6.Size = new System.Drawing.Size(182, 103);
            this.panel6.TabIndex = 9;
            // 
            // lblTimeStamp
            // 
            this.lblTimeStamp.AutoSize = true;
            this.lblTimeStamp.Location = new System.Drawing.Point(6, 4);
            this.lblTimeStamp.Name = "lblTimeStamp";
            this.lblTimeStamp.Size = new System.Drawing.Size(74, 13);
            this.lblTimeStamp.TabIndex = 4;
            this.lblTimeStamp.Text = "Last Updated:";
            // 
            // lblFailed
            // 
            this.lblFailed.AutoSize = true;
            this.lblFailed.Location = new System.Drawing.Point(11, 57);
            this.lblFailed.Name = "lblFailed";
            this.lblFailed.Size = new System.Drawing.Size(38, 13);
            this.lblFailed.TabIndex = 3;
            this.lblFailed.Text = "Failed:";
            // 
            // lblDataSynced
            // 
            this.lblDataSynced.AutoSize = true;
            this.lblDataSynced.Location = new System.Drawing.Point(11, 44);
            this.lblDataSynced.Name = "lblDataSynced";
            this.lblDataSynced.Size = new System.Drawing.Size(69, 13);
            this.lblDataSynced.TabIndex = 2;
            this.lblDataSynced.Text = "DataSynced:";
            // 
            // lblSynced
            // 
            this.lblSynced.AutoSize = true;
            this.lblSynced.Location = new System.Drawing.Point(11, 70);
            this.lblSynced.Name = "lblSynced";
            this.lblSynced.Size = new System.Drawing.Size(51, 13);
            this.lblSynced.TabIndex = 1;
            this.lblSynced.Text = "Success:";
            // 
            // lblQueued
            // 
            this.lblQueued.AutoSize = true;
            this.lblQueued.Location = new System.Drawing.Point(11, 31);
            this.lblQueued.Name = "lblQueued";
            this.lblQueued.Size = new System.Drawing.Size(51, 13);
            this.lblQueued.TabIndex = 0;
            this.lblQueued.Text = "Queued: ";
            // 
            // timer2
            // 
            this.timer2.Tick += new System.EventHandler(this.timer2_Tick);
            // 
            // timer3
            // 
            this.timer3.Tick += new System.EventHandler(this.timer3_Tick);
            // 
            // fileQueueBindingSource
            // 
            this.fileQueueBindingSource.DataSource = typeof(FileQueue);
            // 
            // statusDataGridViewTextBoxColumn
            // 
            this.statusDataGridViewTextBoxColumn.DataPropertyName = "Status";
            this.statusDataGridViewTextBoxColumn.HeaderText = "Status";
            this.statusDataGridViewTextBoxColumn.Name = "statusDataGridViewTextBoxColumn";
            this.statusDataGridViewTextBoxColumn.ReadOnly = true;
            this.statusDataGridViewTextBoxColumn.Width = 62;
            // 
            // filePathDataGridViewTextBoxColumn
            // 
            this.filePathDataGridViewTextBoxColumn.DataPropertyName = "FilePath";
            this.filePathDataGridViewTextBoxColumn.HeaderText = "FilePath";
            this.filePathDataGridViewTextBoxColumn.Name = "filePathDataGridViewTextBoxColumn";
            this.filePathDataGridViewTextBoxColumn.ReadOnly = true;
            this.filePathDataGridViewTextBoxColumn.Width = 70;
            // 
            // attemptsDataGridViewTextBoxColumn
            // 
            this.attemptsDataGridViewTextBoxColumn.DataPropertyName = "Attempts";
            this.attemptsDataGridViewTextBoxColumn.HeaderText = "Attempts";
            this.attemptsDataGridViewTextBoxColumn.Name = "attemptsDataGridViewTextBoxColumn";
            this.attemptsDataGridViewTextBoxColumn.ReadOnly = true;
            this.attemptsDataGridViewTextBoxColumn.Width = 73;
            // 
            // Form1
            // 
            this.ClientSize = new System.Drawing.Size(821, 434);
            this.Controls.Add(this.panel6);
            this.Controls.Add(this.panel4);
            this.Controls.Add(this.panel3);
            this.Controls.Add(this.panel2);
            this.Controls.Add(this.panel1);
            this.Name = "Form1";
            this.Text = "ENGworks BIMXchange Admin";
            ((System.ComponentModel.ISupportInitialize)(this.dataGridView1)).EndInit();
            this.panel1.ResumeLayout(false);
            this.panel1.PerformLayout();
            this.panel2.ResumeLayout(false);
            this.panel4.ResumeLayout(false);
            this.panel6.ResumeLayout(false);
            this.panel6.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.fileQueueBindingSource)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private Button btnIndex;
        private DataGridView dataGridView1;
        private RadioButton rbFullIndex;
        private Panel panel1;
        private RadioButton rbNewAndVersion;
        private RadioButton rbNewOnly;
        private Panel panel2;
        private Timer timer1;
        private Panel panel3;
        private Panel panel4;
        private Panel panel6;
        private Label lblFailed;
        private Label lblDataSynced;
        private Label lblSynced;
        private Label lblQueued;
        private Label lblTimeStamp;
        private Timer timer2;
        private Timer timer3;
        private DataGridViewTextBoxColumn statusDataGridViewTextBoxColumn;
        private DataGridViewTextBoxColumn filePathDataGridViewTextBoxColumn;
        private DataGridViewTextBoxColumn attemptsDataGridViewTextBoxColumn;
        private BindingSource fileQueueBindingSource;
    }
}