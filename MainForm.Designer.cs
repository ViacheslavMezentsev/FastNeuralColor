using System;
using System.ComponentModel;
using System.Drawing;
using System.Threading;
using System.Windows.Forms;

namespace FastNeuralColor
{
    partial class MainForm
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose( bool disposing )
        {
            if ( disposing && ( components != null ) )
            {
                components.Dispose();
            }
            base.Dispose( disposing );
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(MainForm));
            this.MainToolStrip = new System.Windows.Forms.ToolStrip();
            this.ImageToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.OpenImageToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.SaveImageToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.StartStopToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator1 = new System.Windows.Forms.ToolStripSeparator();
            this.ExitToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.MainPictureBox = new System.Windows.Forms.PictureBox();
            this.MainProgressBar = new System.Windows.Forms.ProgressBar();
            this.MainToolStrip.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.MainPictureBox)).BeginInit();
            this.SuspendLayout();
            // 
            // MainToolStrip
            // 
            this.MainToolStrip.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.ImageToolStripMenuItem});
            this.MainToolStrip.Location = new System.Drawing.Point(0, 0);
            this.MainToolStrip.Name = "MainToolStrip";
            this.MainToolStrip.Size = new System.Drawing.Size(720, 25);
            this.MainToolStrip.TabIndex = 0;
            // 
            // ImageToolStripMenuItem
            // 
            this.ImageToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.OpenImageToolStripMenuItem,
            this.SaveImageToolStripMenuItem,
            this.StartStopToolStripMenuItem,
            this.toolStripSeparator1,
            this.ExitToolStripMenuItem});
            this.ImageToolStripMenuItem.Image = ((System.Drawing.Image)(resources.GetObject("ImageToolStripMenuItem.Image")));
            this.ImageToolStripMenuItem.Name = "ImageToolStripMenuItem";
            this.ImageToolStripMenuItem.Size = new System.Drawing.Size(111, 25);
            this.ImageToolStripMenuItem.Text = "Изображение";
            // 
            // OpenImageToolStripMenuItem
            // 
            this.OpenImageToolStripMenuItem.Image = ((System.Drawing.Image)(resources.GetObject("OpenImageToolStripMenuItem.Image")));
            this.OpenImageToolStripMenuItem.Name = "OpenImageToolStripMenuItem";
            this.OpenImageToolStripMenuItem.Size = new System.Drawing.Size(180, 22);
            this.OpenImageToolStripMenuItem.Text = "Открыть";
            this.OpenImageToolStripMenuItem.Click += new System.EventHandler(this.OpenImage);
            // 
            // SaveImageToolStripMenuItem
            // 
            this.SaveImageToolStripMenuItem.Enabled = false;
            this.SaveImageToolStripMenuItem.Image = ((System.Drawing.Image)(resources.GetObject("SaveImageToolStripMenuItem.Image")));
            this.SaveImageToolStripMenuItem.Name = "SaveImageToolStripMenuItem";
            this.SaveImageToolStripMenuItem.Size = new System.Drawing.Size(132, 22);
            this.SaveImageToolStripMenuItem.Text = "Сохранить";
            this.SaveImageToolStripMenuItem.Click += new System.EventHandler(this.SaveImage);
            // 
            // StartStopToolStripMenuItem
            // 
            this.StartStopToolStripMenuItem.Image = ((System.Drawing.Image)(resources.GetObject("StartStopToolStripMenuItem.Image")));
            this.StartStopToolStripMenuItem.Name = "StartStopToolStripMenuItem";
            this.StartStopToolStripMenuItem.Size = new System.Drawing.Size(132, 22);
            this.StartStopToolStripMenuItem.Text = "Старт";
            this.StartStopToolStripMenuItem.Click += new System.EventHandler(this.StartColorization);
            // 
            // toolStripSeparator1
            // 
            this.toolStripSeparator1.Name = "toolStripSeparator1";
            this.toolStripSeparator1.Size = new System.Drawing.Size(129, 6);
            // 
            // ExitToolStripMenuItem
            // 
            this.ExitToolStripMenuItem.Image = ((System.Drawing.Image)(resources.GetObject("ExitToolStripMenuItem.Image")));
            this.ExitToolStripMenuItem.Name = "ExitToolStripMenuItem";
            this.ExitToolStripMenuItem.Size = new System.Drawing.Size(132, 22);
            this.ExitToolStripMenuItem.Text = "Выход";
            this.ExitToolStripMenuItem.Click += new System.EventHandler(this.Close);
            // 
            // MainPictureBox
            // 
            this.MainPictureBox.BackColor = System.Drawing.Color.DimGray;
            this.MainPictureBox.Dock = System.Windows.Forms.DockStyle.Fill;
            this.MainPictureBox.Location = new System.Drawing.Point(0, 25);
            this.MainPictureBox.Name = "MainPictureBox";
            this.MainPictureBox.Size = new System.Drawing.Size(720, 455);
            this.MainPictureBox.SizeMode = System.Windows.Forms.PictureBoxSizeMode.Zoom;
            this.MainPictureBox.TabIndex = 1;
            this.MainPictureBox.TabStop = false;
            // 
            // MainProgressBar
            // 
            this.MainProgressBar.Location = new System.Drawing.Point(12, 448);
            this.MainProgressBar.Name = "MainProgressBar";
            this.MainProgressBar.Size = new System.Drawing.Size(700, 20);
            this.MainProgressBar.TabIndex = 2;
            this.MainProgressBar.Visible = false;
            // 
            // MainForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.White;
            this.ClientSize = new System.Drawing.Size(720, 480);
            this.Controls.Add(this.MainProgressBar);
            this.Controls.Add(this.MainPictureBox);
            this.Controls.Add(this.MainToolStrip);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "MainForm";
            this.Text = "PABCSoft - FastNeuralColor (C# Edition)";
            this.MainToolStrip.ResumeLayout(false);
            this.MainToolStrip.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.MainPictureBox)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private Data ModelData;
        private ToolStrip MainToolStrip;
        private ToolStripMenuItem ImageToolStripMenuItem;
        private ToolStripMenuItem OpenImageToolStripMenuItem;
        private ToolStripMenuItem SaveImageToolStripMenuItem;
        private PictureBox MainPictureBox;
        private ProgressBar MainProgressBar;
        private Thread ColorizationThread;
        private ToolStripMenuItem StartStopToolStripMenuItem;
        private ToolStripSeparator toolStripSeparator1;
        private ToolStripMenuItem ExitToolStripMenuItem;
    }
}

