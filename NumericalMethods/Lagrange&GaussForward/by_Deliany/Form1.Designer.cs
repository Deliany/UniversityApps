namespace by_Deliany
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
            this.label1 = new System.Windows.Forms.Label();
            this.tabControl1 = new System.Windows.Forms.TabControl();
            this.tabPage1 = new System.Windows.Forms.TabPage();
            this.buttonLagrange = new System.Windows.Forms.Button();
            this.buttonDelCol = new System.Windows.Forms.Button();
            this.buttonAddCol = new System.Windows.Forms.Button();
            this.dataGridViewNodes = new System.Windows.Forms.DataGridView();
            this.tabPage2 = new System.Windows.Forms.TabPage();
            this.splitContainer1 = new System.Windows.Forms.SplitContainer();
            this.dataGridViewDifferenceTable = new System.Windows.Forms.DataGridView();
            this.dataGridViewGaussNodes = new System.Windows.Forms.DataGridView();
            this.label2 = new System.Windows.Forms.Label();
            this.buttonGauss = new System.Windows.Forms.Button();
            this.textBox1 = new System.Windows.Forms.TextBox();
            this.buttonDelNodeGauss = new System.Windows.Forms.Button();
            this.buttonAddNodeGauss = new System.Windows.Forms.Button();
            this.textBoxCheck = new System.Windows.Forms.TextBox();
            this.buttonCheck = new System.Windows.Forms.Button();
            this.labelCheck = new System.Windows.Forms.Label();
            this.tabControl1.SuspendLayout();
            this.tabPage1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dataGridViewNodes)).BeginInit();
            this.tabPage2.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer1)).BeginInit();
            this.splitContainer1.Panel1.SuspendLayout();
            this.splitContainer1.Panel2.SuspendLayout();
            this.splitContainer1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dataGridViewDifferenceTable)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.dataGridViewGaussNodes)).BeginInit();
            this.SuspendLayout();
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Font = new System.Drawing.Font("Arial Unicode MS", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.label1.Location = new System.Drawing.Point(45, 235);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(97, 25);
            this.label1.TabIndex = 0;
            this.label1.Text = "Polynom: ";
            // 
            // tabControl1
            // 
            this.tabControl1.Controls.Add(this.tabPage1);
            this.tabControl1.Controls.Add(this.tabPage2);
            this.tabControl1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tabControl1.Font = new System.Drawing.Font("Arial Unicode MS", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.tabControl1.Location = new System.Drawing.Point(0, 0);
            this.tabControl1.Name = "tabControl1";
            this.tabControl1.SelectedIndex = 0;
            this.tabControl1.Size = new System.Drawing.Size(683, 353);
            this.tabControl1.TabIndex = 1;
            // 
            // tabPage1
            // 
            this.tabPage1.BackColor = System.Drawing.SystemColors.Control;
            this.tabPage1.Controls.Add(this.buttonCheck);
            this.tabPage1.Controls.Add(this.textBoxCheck);
            this.tabPage1.Controls.Add(this.buttonLagrange);
            this.tabPage1.Controls.Add(this.buttonDelCol);
            this.tabPage1.Controls.Add(this.buttonAddCol);
            this.tabPage1.Controls.Add(this.dataGridViewNodes);
            this.tabPage1.Controls.Add(this.labelCheck);
            this.tabPage1.Controls.Add(this.label1);
            this.tabPage1.Location = new System.Drawing.Point(4, 30);
            this.tabPage1.Name = "tabPage1";
            this.tabPage1.Padding = new System.Windows.Forms.Padding(3);
            this.tabPage1.Size = new System.Drawing.Size(675, 319);
            this.tabPage1.TabIndex = 0;
            this.tabPage1.Text = "Lagrange";
            // 
            // buttonLagrange
            // 
            this.buttonLagrange.AutoSize = true;
            this.buttonLagrange.Location = new System.Drawing.Point(391, 177);
            this.buttonLagrange.Name = "buttonLagrange";
            this.buttonLagrange.Size = new System.Drawing.Size(106, 31);
            this.buttonLagrange.TabIndex = 2;
            this.buttonLagrange.Text = "Calculate";
            this.buttonLagrange.UseVisualStyleBackColor = true;
            this.buttonLagrange.Click += new System.EventHandler(this.buttonLagrange_Click);
            // 
            // buttonDelCol
            // 
            this.buttonDelCol.AutoSize = true;
            this.buttonDelCol.Location = new System.Drawing.Point(129, 177);
            this.buttonDelCol.Name = "buttonDelCol";
            this.buttonDelCol.Size = new System.Drawing.Size(106, 31);
            this.buttonDelCol.TabIndex = 2;
            this.buttonDelCol.Text = "Delete node";
            this.buttonDelCol.UseVisualStyleBackColor = true;
            this.buttonDelCol.Click += new System.EventHandler(this.buttonDelCol_Click);
            // 
            // buttonAddCol
            // 
            this.buttonAddCol.AutoSize = true;
            this.buttonAddCol.Location = new System.Drawing.Point(25, 177);
            this.buttonAddCol.Name = "buttonAddCol";
            this.buttonAddCol.Size = new System.Drawing.Size(89, 31);
            this.buttonAddCol.TabIndex = 2;
            this.buttonAddCol.Text = "Add node";
            this.buttonAddCol.UseVisualStyleBackColor = true;
            this.buttonAddCol.Click += new System.EventHandler(this.buttonAddCol_Click);
            // 
            // dataGridViewNodes
            // 
            this.dataGridViewNodes.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dataGridViewNodes.Dock = System.Windows.Forms.DockStyle.Top;
            this.dataGridViewNodes.Location = new System.Drawing.Point(3, 3);
            this.dataGridViewNodes.Name = "dataGridViewNodes";
            this.dataGridViewNodes.RowTemplate.DefaultCellStyle.SelectionBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(192)))), ((int)(((byte)(192)))), ((int)(((byte)(255)))));
            this.dataGridViewNodes.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.CellSelect;
            this.dataGridViewNodes.Size = new System.Drawing.Size(669, 158);
            this.dataGridViewNodes.TabIndex = 1;
            // 
            // tabPage2
            // 
            this.tabPage2.BackColor = System.Drawing.SystemColors.Control;
            this.tabPage2.Controls.Add(this.splitContainer1);
            this.tabPage2.Location = new System.Drawing.Point(4, 30);
            this.tabPage2.Name = "tabPage2";
            this.tabPage2.Padding = new System.Windows.Forms.Padding(3);
            this.tabPage2.Size = new System.Drawing.Size(675, 319);
            this.tabPage2.TabIndex = 1;
            this.tabPage2.Text = "Gauss Forward";
            // 
            // splitContainer1
            // 
            this.splitContainer1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.splitContainer1.Location = new System.Drawing.Point(3, 3);
            this.splitContainer1.Name = "splitContainer1";
            this.splitContainer1.Orientation = System.Windows.Forms.Orientation.Horizontal;
            // 
            // splitContainer1.Panel1
            // 
            this.splitContainer1.Panel1.Controls.Add(this.dataGridViewDifferenceTable);
            this.splitContainer1.Panel1.Controls.Add(this.dataGridViewGaussNodes);
            // 
            // splitContainer1.Panel2
            // 
            this.splitContainer1.Panel2.Controls.Add(this.label2);
            this.splitContainer1.Panel2.Controls.Add(this.buttonGauss);
            this.splitContainer1.Panel2.Controls.Add(this.textBox1);
            this.splitContainer1.Panel2.Controls.Add(this.buttonDelNodeGauss);
            this.splitContainer1.Panel2.Controls.Add(this.buttonAddNodeGauss);
            this.splitContainer1.Size = new System.Drawing.Size(669, 313);
            this.splitContainer1.SplitterDistance = 188;
            this.splitContainer1.TabIndex = 0;
            // 
            // dataGridViewDifferenceTable
            // 
            this.dataGridViewDifferenceTable.BackgroundColor = System.Drawing.Color.Thistle;
            this.dataGridViewDifferenceTable.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dataGridViewDifferenceTable.Dock = System.Windows.Forms.DockStyle.Fill;
            this.dataGridViewDifferenceTable.Location = new System.Drawing.Point(240, 0);
            this.dataGridViewDifferenceTable.Name = "dataGridViewDifferenceTable";
            this.dataGridViewDifferenceTable.RowTemplate.DefaultCellStyle.SelectionBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(192)))), ((int)(((byte)(192)))), ((int)(((byte)(255)))));
            this.dataGridViewDifferenceTable.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.CellSelect;
            this.dataGridViewDifferenceTable.Size = new System.Drawing.Size(429, 188);
            this.dataGridViewDifferenceTable.TabIndex = 1;
            // 
            // dataGridViewGaussNodes
            // 
            this.dataGridViewGaussNodes.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dataGridViewGaussNodes.Dock = System.Windows.Forms.DockStyle.Left;
            this.dataGridViewGaussNodes.Location = new System.Drawing.Point(0, 0);
            this.dataGridViewGaussNodes.Name = "dataGridViewGaussNodes";
            this.dataGridViewGaussNodes.RowTemplate.DefaultCellStyle.SelectionBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(192)))), ((int)(((byte)(192)))), ((int)(((byte)(255)))));
            this.dataGridViewGaussNodes.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.CellSelect;
            this.dataGridViewGaussNodes.Size = new System.Drawing.Size(240, 188);
            this.dataGridViewGaussNodes.TabIndex = 0;
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Font = new System.Drawing.Font("Arial Unicode MS", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.label2.Location = new System.Drawing.Point(253, 68);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(76, 25);
            this.label2.TabIndex = 3;
            this.label2.Text = "Result: ";
            // 
            // buttonGauss
            // 
            this.buttonGauss.AutoSize = true;
            this.buttonGauss.Location = new System.Drawing.Point(405, 17);
            this.buttonGauss.Name = "buttonGauss";
            this.buttonGauss.Size = new System.Drawing.Size(86, 31);
            this.buttonGauss.TabIndex = 2;
            this.buttonGauss.Text = "Calculate";
            this.buttonGauss.UseVisualStyleBackColor = true;
            this.buttonGauss.Click += new System.EventHandler(this.buttonGauss_Click);
            // 
            // textBox1
            // 
            this.textBox1.Location = new System.Drawing.Point(329, 19);
            this.textBox1.Name = "textBox1";
            this.textBox1.Size = new System.Drawing.Size(52, 29);
            this.textBox1.TabIndex = 1;
            // 
            // buttonDelNodeGauss
            // 
            this.buttonDelNodeGauss.AutoSize = true;
            this.buttonDelNodeGauss.Location = new System.Drawing.Point(134, 17);
            this.buttonDelNodeGauss.Name = "buttonDelNodeGauss";
            this.buttonDelNodeGauss.Size = new System.Drawing.Size(106, 31);
            this.buttonDelNodeGauss.TabIndex = 0;
            this.buttonDelNodeGauss.Text = "Delete node";
            this.buttonDelNodeGauss.UseVisualStyleBackColor = true;
            this.buttonDelNodeGauss.Click += new System.EventHandler(this.buttonDelNodeGauss_Click);
            // 
            // buttonAddNodeGauss
            // 
            this.buttonAddNodeGauss.AutoSize = true;
            this.buttonAddNodeGauss.Location = new System.Drawing.Point(21, 17);
            this.buttonAddNodeGauss.Name = "buttonAddNodeGauss";
            this.buttonAddNodeGauss.Size = new System.Drawing.Size(89, 31);
            this.buttonAddNodeGauss.TabIndex = 0;
            this.buttonAddNodeGauss.Text = "Add node";
            this.buttonAddNodeGauss.UseVisualStyleBackColor = true;
            this.buttonAddNodeGauss.Click += new System.EventHandler(this.buttonAddNodeGauss_Click);
            // 
            // textBoxCheck
            // 
            this.textBoxCheck.Location = new System.Drawing.Point(50, 272);
            this.textBoxCheck.Name = "textBoxCheck";
            this.textBoxCheck.Size = new System.Drawing.Size(52, 29);
            this.textBoxCheck.TabIndex = 3;
            // 
            // buttonCheck
            // 
            this.buttonCheck.AutoSize = true;
            this.buttonCheck.Location = new System.Drawing.Point(119, 270);
            this.buttonCheck.Name = "buttonCheck";
            this.buttonCheck.Size = new System.Drawing.Size(75, 31);
            this.buttonCheck.TabIndex = 4;
            this.buttonCheck.Text = "Check";
            this.buttonCheck.UseVisualStyleBackColor = true;
            this.buttonCheck.Click += new System.EventHandler(this.buttonCheck_Click);
            // 
            // labelCheck
            // 
            this.labelCheck.AutoSize = true;
            this.labelCheck.Font = new System.Drawing.Font("Arial Unicode MS", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.labelCheck.Location = new System.Drawing.Point(209, 272);
            this.labelCheck.Name = "labelCheck";
            this.labelCheck.Size = new System.Drawing.Size(128, 25);
            this.labelCheck.TabIndex = 0;
            this.labelCheck.Text = "Check result: ";
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(683, 353);
            this.Controls.Add(this.tabControl1);
            this.Name = "Form1";
            this.Text = "Form1";
            this.tabControl1.ResumeLayout(false);
            this.tabPage1.ResumeLayout(false);
            this.tabPage1.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dataGridViewNodes)).EndInit();
            this.tabPage2.ResumeLayout(false);
            this.splitContainer1.Panel1.ResumeLayout(false);
            this.splitContainer1.Panel2.ResumeLayout(false);
            this.splitContainer1.Panel2.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer1)).EndInit();
            this.splitContainer1.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.dataGridViewDifferenceTable)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.dataGridViewGaussNodes)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.TabControl tabControl1;
        private System.Windows.Forms.TabPage tabPage1;
        private System.Windows.Forms.TabPage tabPage2;
        private System.Windows.Forms.DataGridView dataGridViewNodes;
        private System.Windows.Forms.Button buttonLagrange;
        private System.Windows.Forms.Button buttonDelCol;
        private System.Windows.Forms.Button buttonAddCol;
        private System.Windows.Forms.SplitContainer splitContainer1;
        private System.Windows.Forms.DataGridView dataGridViewDifferenceTable;
        private System.Windows.Forms.DataGridView dataGridViewGaussNodes;
        private System.Windows.Forms.Button buttonGauss;
        private System.Windows.Forms.TextBox textBox1;
        private System.Windows.Forms.Button buttonDelNodeGauss;
        private System.Windows.Forms.Button buttonAddNodeGauss;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Button buttonCheck;
        private System.Windows.Forms.TextBox textBoxCheck;
        private System.Windows.Forms.Label labelCheck;

    }
}

