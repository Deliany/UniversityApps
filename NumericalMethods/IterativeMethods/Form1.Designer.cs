namespace IterativeMethods
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
            this.zedIterative = new ZedGraph.ZedGraphControl();
            this.tabControl1 = new System.Windows.Forms.TabControl();
            this.tabPage1 = new System.Windows.Forms.TabPage();
            this.iterativeLogger = new System.Windows.Forms.ListBox();
            this.iterativeInput = new System.Windows.Forms.TextBox();
            this.button1 = new System.Windows.Forms.Button();
            this.label1 = new System.Windows.Forms.Label();
            this.tabPage2 = new System.Windows.Forms.TabPage();
            this.zedChord = new ZedGraph.ZedGraphControl();
            this.tabPage3 = new System.Windows.Forms.TabPage();
            this.ChordLogger = new System.Windows.Forms.ListBox();
            this.zedNewton = new ZedGraph.ZedGraphControl();
            this.NewtonLogger = new System.Windows.Forms.ListBox();
            this.tabControl1.SuspendLayout();
            this.tabPage1.SuspendLayout();
            this.tabPage2.SuspendLayout();
            this.tabPage3.SuspendLayout();
            this.SuspendLayout();
            // 
            // zedIterative
            // 
            this.zedIterative.Dock = System.Windows.Forms.DockStyle.Top;
            this.zedIterative.Location = new System.Drawing.Point(3, 3);
            this.zedIterative.Name = "zedIterative";
            this.zedIterative.ScrollGrace = 0D;
            this.zedIterative.ScrollMaxX = 0D;
            this.zedIterative.ScrollMaxY = 0D;
            this.zedIterative.ScrollMaxY2 = 0D;
            this.zedIterative.ScrollMinX = 0D;
            this.zedIterative.ScrollMinY = 0D;
            this.zedIterative.ScrollMinY2 = 0D;
            this.zedIterative.Size = new System.Drawing.Size(771, 374);
            this.zedIterative.TabIndex = 0;
            // 
            // tabControl1
            // 
            this.tabControl1.Controls.Add(this.tabPage1);
            this.tabControl1.Controls.Add(this.tabPage2);
            this.tabControl1.Controls.Add(this.tabPage3);
            this.tabControl1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tabControl1.Location = new System.Drawing.Point(0, 0);
            this.tabControl1.Name = "tabControl1";
            this.tabControl1.SelectedIndex = 0;
            this.tabControl1.Size = new System.Drawing.Size(785, 545);
            this.tabControl1.TabIndex = 1;
            // 
            // tabPage1
            // 
            this.tabPage1.Controls.Add(this.iterativeLogger);
            this.tabPage1.Controls.Add(this.iterativeInput);
            this.tabPage1.Controls.Add(this.button1);
            this.tabPage1.Controls.Add(this.label1);
            this.tabPage1.Controls.Add(this.zedIterative);
            this.tabPage1.Location = new System.Drawing.Point(4, 22);
            this.tabPage1.Name = "tabPage1";
            this.tabPage1.Padding = new System.Windows.Forms.Padding(3);
            this.tabPage1.Size = new System.Drawing.Size(777, 519);
            this.tabPage1.TabIndex = 0;
            this.tabPage1.Text = "Simple Iteration";
            this.tabPage1.UseVisualStyleBackColor = true;
            // 
            // iterativeLogger
            // 
            this.iterativeLogger.FormattingEnabled = true;
            this.iterativeLogger.Location = new System.Drawing.Point(345, 383);
            this.iterativeLogger.Name = "iterativeLogger";
            this.iterativeLogger.Size = new System.Drawing.Size(359, 108);
            this.iterativeLogger.TabIndex = 4;
            // 
            // iterativeInput
            // 
            this.iterativeInput.Location = new System.Drawing.Point(145, 383);
            this.iterativeInput.Name = "iterativeInput";
            this.iterativeInput.Size = new System.Drawing.Size(100, 20);
            this.iterativeInput.TabIndex = 3;
            // 
            // button1
            // 
            this.button1.Location = new System.Drawing.Point(145, 434);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(75, 23);
            this.button1.TabIndex = 2;
            this.button1.Text = "Compute";
            this.button1.UseVisualStyleBackColor = true;
            this.button1.Click += new System.EventHandler(this.button1_Click);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(83, 386);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(20, 13);
            this.label1.TabIndex = 1;
            this.label1.Text = "X0";
            // 
            // tabPage2
            // 
            this.tabPage2.Controls.Add(this.ChordLogger);
            this.tabPage2.Controls.Add(this.zedChord);
            this.tabPage2.Location = new System.Drawing.Point(4, 22);
            this.tabPage2.Name = "tabPage2";
            this.tabPage2.Padding = new System.Windows.Forms.Padding(3);
            this.tabPage2.Size = new System.Drawing.Size(777, 519);
            this.tabPage2.TabIndex = 1;
            this.tabPage2.Text = "Chord Method";
            this.tabPage2.UseVisualStyleBackColor = true;
            // 
            // zedChord
            // 
            this.zedChord.Dock = System.Windows.Forms.DockStyle.Top;
            this.zedChord.Location = new System.Drawing.Point(3, 3);
            this.zedChord.Name = "zedChord";
            this.zedChord.ScrollGrace = 0D;
            this.zedChord.ScrollMaxX = 0D;
            this.zedChord.ScrollMaxY = 0D;
            this.zedChord.ScrollMaxY2 = 0D;
            this.zedChord.ScrollMinX = 0D;
            this.zedChord.ScrollMinY = 0D;
            this.zedChord.ScrollMinY2 = 0D;
            this.zedChord.Size = new System.Drawing.Size(771, 374);
            this.zedChord.TabIndex = 0;
            // 
            // tabPage3
            // 
            this.tabPage3.Controls.Add(this.NewtonLogger);
            this.tabPage3.Controls.Add(this.zedNewton);
            this.tabPage3.Location = new System.Drawing.Point(4, 22);
            this.tabPage3.Name = "tabPage3";
            this.tabPage3.Size = new System.Drawing.Size(777, 519);
            this.tabPage3.TabIndex = 2;
            this.tabPage3.Text = "Newton Method";
            this.tabPage3.UseVisualStyleBackColor = true;
            // 
            // ChordLogger
            // 
            this.ChordLogger.FormattingEnabled = true;
            this.ChordLogger.Location = new System.Drawing.Point(162, 402);
            this.ChordLogger.Name = "ChordLogger";
            this.ChordLogger.Size = new System.Drawing.Size(159, 95);
            this.ChordLogger.TabIndex = 1;
            // 
            // zedNewton
            // 
            this.zedNewton.Dock = System.Windows.Forms.DockStyle.Top;
            this.zedNewton.Location = new System.Drawing.Point(0, 0);
            this.zedNewton.Name = "zedNewton";
            this.zedNewton.ScrollGrace = 0D;
            this.zedNewton.ScrollMaxX = 0D;
            this.zedNewton.ScrollMaxY = 0D;
            this.zedNewton.ScrollMaxY2 = 0D;
            this.zedNewton.ScrollMinX = 0D;
            this.zedNewton.ScrollMinY = 0D;
            this.zedNewton.ScrollMinY2 = 0D;
            this.zedNewton.Size = new System.Drawing.Size(777, 368);
            this.zedNewton.TabIndex = 0;
            // 
            // NewtonLogger
            // 
            this.NewtonLogger.FormattingEnabled = true;
            this.NewtonLogger.Location = new System.Drawing.Point(149, 377);
            this.NewtonLogger.Name = "NewtonLogger";
            this.NewtonLogger.Size = new System.Drawing.Size(296, 134);
            this.NewtonLogger.TabIndex = 1;
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(785, 545);
            this.Controls.Add(this.tabControl1);
            this.Name = "Form1";
            this.Text = "Form1";
            this.tabControl1.ResumeLayout(false);
            this.tabPage1.ResumeLayout(false);
            this.tabPage1.PerformLayout();
            this.tabPage2.ResumeLayout(false);
            this.tabPage3.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private ZedGraph.ZedGraphControl zedIterative;
        private System.Windows.Forms.TabControl tabControl1;
        private System.Windows.Forms.TabPage tabPage1;
        private System.Windows.Forms.TabPage tabPage2;
        private System.Windows.Forms.TextBox iterativeInput;
        private System.Windows.Forms.Button button1;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.ListBox iterativeLogger;
        private ZedGraph.ZedGraphControl zedChord;
        private System.Windows.Forms.TabPage tabPage3;
        private System.Windows.Forms.ListBox ChordLogger;
        private System.Windows.Forms.ListBox NewtonLogger;
        private ZedGraph.ZedGraphControl zedNewton;


    }
}

