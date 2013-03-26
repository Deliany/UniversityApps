namespace BidirectionalSearch
{
    partial class MainForm
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(MainForm));
            this.pictureBoxGraph = new System.Windows.Forms.PictureBox();
            this.toolStrip1 = new System.Windows.Forms.ToolStrip();
            this.toolStripButtonLoadGraph = new System.Windows.Forms.ToolStripButton();
            this.toolStripLabelPathFrom = new System.Windows.Forms.ToolStripLabel();
            this.toolStripTextBoxFrom = new System.Windows.Forms.ToolStripTextBox();
            this.toolStripLabelTo = new System.Windows.Forms.ToolStripLabel();
            this.toolStripTextBoxTo = new System.Windows.Forms.ToolStripTextBox();
            this.toolStripButtonSearch = new System.Windows.Forms.ToolStripButton();
            this.toolStripLabelPathDistance = new System.Windows.Forms.ToolStripLabel();
            this.toolStripTextBoxPathDistance = new System.Windows.Forms.ToolStripTextBox();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBoxGraph)).BeginInit();
            this.toolStrip1.SuspendLayout();
            this.SuspendLayout();
            // 
            // pictureBoxGraph
            // 
            this.pictureBoxGraph.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.pictureBoxGraph.BackColor = System.Drawing.Color.White;
            this.pictureBoxGraph.Location = new System.Drawing.Point(0, 28);
            this.pictureBoxGraph.Name = "pictureBoxGraph";
            this.pictureBoxGraph.Size = new System.Drawing.Size(842, 525);
            this.pictureBoxGraph.TabIndex = 2;
            this.pictureBoxGraph.TabStop = false;
            this.pictureBoxGraph.SizeChanged += new System.EventHandler(this.pictureBoxGraph_SizeChanged);
            this.pictureBoxGraph.Paint += new System.Windows.Forms.PaintEventHandler(this.pictureBoxGraph_Paint);
            this.pictureBoxGraph.MouseDown += new System.Windows.Forms.MouseEventHandler(this.pictureBoxGraph_MouseDown);
            this.pictureBoxGraph.MouseMove += new System.Windows.Forms.MouseEventHandler(this.pictureBoxGraph_MouseMove);
            this.pictureBoxGraph.MouseUp += new System.Windows.Forms.MouseEventHandler(this.pictureBoxGraph_MouseUp);
            // 
            // toolStrip1
            // 
            this.toolStrip1.GripStyle = System.Windows.Forms.ToolStripGripStyle.Hidden;
            this.toolStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.toolStripButtonLoadGraph,
            this.toolStripLabelPathFrom,
            this.toolStripTextBoxFrom,
            this.toolStripLabelTo,
            this.toolStripTextBoxTo,
            this.toolStripButtonSearch,
            this.toolStripLabelPathDistance,
            this.toolStripTextBoxPathDistance});
            this.toolStrip1.Location = new System.Drawing.Point(0, 0);
            this.toolStrip1.Name = "toolStrip1";
            this.toolStrip1.Size = new System.Drawing.Size(842, 25);
            this.toolStrip1.TabIndex = 3;
            this.toolStrip1.Text = "toolStrip1";
            // 
            // toolStripButtonLoadGraph
            // 
            this.toolStripButtonLoadGraph.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text;
            this.toolStripButtonLoadGraph.Image = ((System.Drawing.Image)(resources.GetObject("toolStripButtonLoadGraph.Image")));
            this.toolStripButtonLoadGraph.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.toolStripButtonLoadGraph.Name = "toolStripButtonLoadGraph";
            this.toolStripButtonLoadGraph.Size = new System.Drawing.Size(71, 22);
            this.toolStripButtonLoadGraph.Text = "Load graph";
            this.toolStripButtonLoadGraph.Click += new System.EventHandler(this.toolStripButtonLoadGraph_Click);
            // 
            // toolStripLabelPathFrom
            // 
            this.toolStripLabelPathFrom.Name = "toolStripLabelPathFrom";
            this.toolStripLabelPathFrom.Size = new System.Drawing.Size(66, 22);
            this.toolStripLabelPathFrom.Text = "Path from: ";
            // 
            // toolStripTextBoxFrom
            // 
            this.toolStripTextBoxFrom.Name = "toolStripTextBoxFrom";
            this.toolStripTextBoxFrom.Size = new System.Drawing.Size(100, 25);
            // 
            // toolStripLabelTo
            // 
            this.toolStripLabelTo.Name = "toolStripLabelTo";
            this.toolStripLabelTo.Size = new System.Drawing.Size(24, 22);
            this.toolStripLabelTo.Text = "to: ";
            // 
            // toolStripTextBoxTo
            // 
            this.toolStripTextBoxTo.Name = "toolStripTextBoxTo";
            this.toolStripTextBoxTo.Size = new System.Drawing.Size(100, 25);
            // 
            // toolStripButtonSearch
            // 
            this.toolStripButtonSearch.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text;
            this.toolStripButtonSearch.Image = ((System.Drawing.Image)(resources.GetObject("toolStripButtonSearch.Image")));
            this.toolStripButtonSearch.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.toolStripButtonSearch.Name = "toolStripButtonSearch";
            this.toolStripButtonSearch.Size = new System.Drawing.Size(49, 22);
            this.toolStripButtonSearch.Text = "Search!";
            this.toolStripButtonSearch.Click += new System.EventHandler(this.toolStripButtonSearch_Click);
            // 
            // toolStripLabelPathDistance
            // 
            this.toolStripLabelPathDistance.Name = "toolStripLabelPathDistance";
            this.toolStripLabelPathDistance.Size = new System.Drawing.Size(81, 22);
            this.toolStripLabelPathDistance.Text = "Path distance:";
            // 
            // toolStripTextBoxPathDistance
            // 
            this.toolStripTextBoxPathDistance.Name = "toolStripTextBoxPathDistance";
            this.toolStripTextBoxPathDistance.Size = new System.Drawing.Size(100, 25);
            // 
            // MainForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(842, 553);
            this.Controls.Add(this.toolStrip1);
            this.Controls.Add(this.pictureBoxGraph);
            this.Name = "MainForm";
            this.Text = "Bidirectional search project";
            ((System.ComponentModel.ISupportInitialize)(this.pictureBoxGraph)).EndInit();
            this.toolStrip1.ResumeLayout(false);
            this.toolStrip1.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.PictureBox pictureBoxGraph;
        private System.Windows.Forms.ToolStrip toolStrip1;
        private System.Windows.Forms.ToolStripButton toolStripButtonLoadGraph;
        private System.Windows.Forms.ToolStripLabel toolStripLabelPathFrom;
        private System.Windows.Forms.ToolStripTextBox toolStripTextBoxFrom;
        private System.Windows.Forms.ToolStripLabel toolStripLabelTo;
        private System.Windows.Forms.ToolStripTextBox toolStripTextBoxTo;
        private System.Windows.Forms.ToolStripButton toolStripButtonSearch;
        private System.Windows.Forms.ToolStripLabel toolStripLabelPathDistance;
        private System.Windows.Forms.ToolStripTextBox toolStripTextBoxPathDistance;
    }
}

