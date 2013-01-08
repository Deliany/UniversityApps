namespace NewtonModified_Method
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
            this.NewtonEquationTextBox = new System.Windows.Forms.TextBox();
            this.label1 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.label4 = new System.Windows.Forms.Label();
            this.NewtonX = new System.Windows.Forms.TextBox();
            this.label5 = new System.Windows.Forms.Label();
            this.NewtonY = new System.Windows.Forms.TextBox();
            this.label6 = new System.Windows.Forms.Label();
            this.NewtonZ = new System.Windows.Forms.TextBox();
            this.label7 = new System.Windows.Forms.Label();
            this.NewtonUpDown = new System.Windows.Forms.NumericUpDown();
            this.nextNewtonButton = new System.Windows.Forms.Button();
            this.clearNewtonButton = new System.Windows.Forms.Button();
            this.iterNewtonCount = new System.Windows.Forms.Label();
            this.NewtonResultTextBox = new System.Windows.Forms.TextBox();
            this.tabControl1 = new System.Windows.Forms.TabControl();
            this.tabPage1 = new System.Windows.Forms.TabPage();
            this.tabPage2 = new System.Windows.Forms.TabPage();
            this.ResultJacobianTextBox = new System.Windows.Forms.TextBox();
            this.label8 = new System.Windows.Forms.Label();
            this.EpsilonTextBox = new System.Windows.Forms.TextBox();
            this.calcNewtonButton = new System.Windows.Forms.Button();
            ((System.ComponentModel.ISupportInitialize)(this.NewtonUpDown)).BeginInit();
            this.tabControl1.SuspendLayout();
            this.tabPage1.SuspendLayout();
            this.tabPage2.SuspendLayout();
            this.SuspendLayout();
            // 
            // NewtonEquationTextBox
            // 
            this.NewtonEquationTextBox.Font = new System.Drawing.Font("Arial Unicode MS", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.NewtonEquationTextBox.Location = new System.Drawing.Point(28, 23);
            this.NewtonEquationTextBox.Multiline = true;
            this.NewtonEquationTextBox.Name = "NewtonEquationTextBox";
            this.NewtonEquationTextBox.Size = new System.Drawing.Size(302, 92);
            this.NewtonEquationTextBox.TabIndex = 0;
            this.NewtonEquationTextBox.Text = "x^2+y^2+z^2-1\r\n2*x^2+y^2-4*z\r\n3*x^2-4*y+z^2";
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Font = new System.Drawing.Font("Arial Unicode MS", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.label1.Location = new System.Drawing.Point(336, 26);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(39, 25);
            this.label1.TabIndex = 1;
            this.label1.Text = "= 0";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Font = new System.Drawing.Font("Arial Unicode MS", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.label2.Location = new System.Drawing.Point(336, 76);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(39, 25);
            this.label2.TabIndex = 1;
            this.label2.Text = "= 0";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Font = new System.Drawing.Font("Arial Unicode MS", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.label3.Location = new System.Drawing.Point(336, 51);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(39, 25);
            this.label3.TabIndex = 1;
            this.label3.Text = "= 0";
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Font = new System.Drawing.Font("Arial Unicode MS", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.label4.Location = new System.Drawing.Point(28, 137);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(33, 25);
            this.label4.TabIndex = 2;
            this.label4.Text = "x=";
            // 
            // NewtonX
            // 
            this.NewtonX.Font = new System.Drawing.Font("Arial Unicode MS", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.NewtonX.Location = new System.Drawing.Point(56, 134);
            this.NewtonX.Name = "NewtonX";
            this.NewtonX.Size = new System.Drawing.Size(49, 33);
            this.NewtonX.TabIndex = 3;
            this.NewtonX.Text = "0,5";
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Font = new System.Drawing.Font("Arial Unicode MS", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.label5.Location = new System.Drawing.Point(125, 137);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(33, 25);
            this.label5.TabIndex = 2;
            this.label5.Text = "y=";
            // 
            // NewtonY
            // 
            this.NewtonY.Font = new System.Drawing.Font("Arial Unicode MS", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.NewtonY.Location = new System.Drawing.Point(155, 134);
            this.NewtonY.Name = "NewtonY";
            this.NewtonY.Size = new System.Drawing.Size(49, 33);
            this.NewtonY.TabIndex = 3;
            this.NewtonY.Text = "0,5";
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Font = new System.Drawing.Font("Arial Unicode MS", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.label6.Location = new System.Drawing.Point(221, 137);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(33, 25);
            this.label6.TabIndex = 2;
            this.label6.Text = "z=";
            // 
            // NewtonZ
            // 
            this.NewtonZ.Font = new System.Drawing.Font("Arial Unicode MS", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.NewtonZ.Location = new System.Drawing.Point(250, 134);
            this.NewtonZ.Name = "NewtonZ";
            this.NewtonZ.Size = new System.Drawing.Size(49, 33);
            this.NewtonZ.TabIndex = 3;
            this.NewtonZ.Text = "0,5";
            // 
            // label7
            // 
            this.label7.AutoSize = true;
            this.label7.Font = new System.Drawing.Font("Arial Unicode MS", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.label7.Location = new System.Drawing.Point(28, 181);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(190, 25);
            this.label7.TabIndex = 4;
            this.label7.Text = "number of equations:";
            // 
            // NewtonUpDown
            // 
            this.NewtonUpDown.Font = new System.Drawing.Font("Arial Unicode MS", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.NewtonUpDown.Location = new System.Drawing.Point(224, 179);
            this.NewtonUpDown.Maximum = new decimal(new int[] {
            3,
            0,
            0,
            0});
            this.NewtonUpDown.Minimum = new decimal(new int[] {
            2,
            0,
            0,
            0});
            this.NewtonUpDown.Name = "NewtonUpDown";
            this.NewtonUpDown.Size = new System.Drawing.Size(41, 33);
            this.NewtonUpDown.TabIndex = 5;
            this.NewtonUpDown.Value = new decimal(new int[] {
            3,
            0,
            0,
            0});
            this.NewtonUpDown.ValueChanged += new System.EventHandler(this.NewtonUpDown_ValueChanged);
            // 
            // nextNewtonButton
            // 
            this.nextNewtonButton.Font = new System.Drawing.Font("Arial Unicode MS", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.nextNewtonButton.Location = new System.Drawing.Point(406, 26);
            this.nextNewtonButton.Name = "nextNewtonButton";
            this.nextNewtonButton.Size = new System.Drawing.Size(133, 39);
            this.nextNewtonButton.TabIndex = 6;
            this.nextNewtonButton.Text = "Next iteration";
            this.nextNewtonButton.UseVisualStyleBackColor = true;
            this.nextNewtonButton.Click += new System.EventHandler(this.nextNewtonButton_Click);
            // 
            // clearNewtonButton
            // 
            this.clearNewtonButton.Font = new System.Drawing.Font("Arial Unicode MS", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.clearNewtonButton.Location = new System.Drawing.Point(478, 79);
            this.clearNewtonButton.Name = "clearNewtonButton";
            this.clearNewtonButton.Size = new System.Drawing.Size(72, 31);
            this.clearNewtonButton.TabIndex = 6;
            this.clearNewtonButton.Text = "Clear";
            this.clearNewtonButton.UseVisualStyleBackColor = true;
            this.clearNewtonButton.Click += new System.EventHandler(this.clearNewtonButton_Click);
            // 
            // iterNewtonCount
            // 
            this.iterNewtonCount.AutoSize = true;
            this.iterNewtonCount.Font = new System.Drawing.Font("Arial Unicode MS", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.iterNewtonCount.Location = new System.Drawing.Point(416, 123);
            this.iterNewtonCount.Name = "iterNewtonCount";
            this.iterNewtonCount.Size = new System.Drawing.Size(110, 25);
            this.iterNewtonCount.TabIndex = 4;
            this.iterNewtonCount.Text = "iterations: 0";
            // 
            // NewtonResultTextBox
            // 
            this.NewtonResultTextBox.Location = new System.Drawing.Point(4, 3);
            this.NewtonResultTextBox.Multiline = true;
            this.NewtonResultTextBox.Name = "NewtonResultTextBox";
            this.NewtonResultTextBox.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
            this.NewtonResultTextBox.Size = new System.Drawing.Size(258, 157);
            this.NewtonResultTextBox.TabIndex = 7;
            // 
            // tabControl1
            // 
            this.tabControl1.Controls.Add(this.tabPage1);
            this.tabControl1.Controls.Add(this.tabPage2);
            this.tabControl1.Location = new System.Drawing.Point(578, 12);
            this.tabControl1.Name = "tabControl1";
            this.tabControl1.SelectedIndex = 0;
            this.tabControl1.Size = new System.Drawing.Size(273, 191);
            this.tabControl1.TabIndex = 8;
            // 
            // tabPage1
            // 
            this.tabPage1.Controls.Add(this.NewtonResultTextBox);
            this.tabPage1.Font = new System.Drawing.Font("Arial Unicode MS", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.tabPage1.Location = new System.Drawing.Point(4, 22);
            this.tabPage1.Name = "tabPage1";
            this.tabPage1.Padding = new System.Windows.Forms.Padding(3);
            this.tabPage1.Size = new System.Drawing.Size(265, 165);
            this.tabPage1.TabIndex = 0;
            this.tabPage1.Text = "Result Values";
            this.tabPage1.UseVisualStyleBackColor = true;
            // 
            // tabPage2
            // 
            this.tabPage2.Controls.Add(this.ResultJacobianTextBox);
            this.tabPage2.Location = new System.Drawing.Point(4, 22);
            this.tabPage2.Name = "tabPage2";
            this.tabPage2.Padding = new System.Windows.Forms.Padding(3);
            this.tabPage2.Size = new System.Drawing.Size(265, 165);
            this.tabPage2.TabIndex = 1;
            this.tabPage2.Text = "Jacobian matrix";
            this.tabPage2.UseVisualStyleBackColor = true;
            // 
            // ResultJacobianTextBox
            // 
            this.ResultJacobianTextBox.Location = new System.Drawing.Point(4, 3);
            this.ResultJacobianTextBox.Multiline = true;
            this.ResultJacobianTextBox.Name = "ResultJacobianTextBox";
            this.ResultJacobianTextBox.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
            this.ResultJacobianTextBox.Size = new System.Drawing.Size(258, 157);
            this.ResultJacobianTextBox.TabIndex = 7;
            // 
            // label8
            // 
            this.label8.AutoSize = true;
            this.label8.Font = new System.Drawing.Font("Arial Unicode MS", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.label8.Location = new System.Drawing.Point(285, 181);
            this.label8.Name = "label8";
            this.label8.Size = new System.Drawing.Size(54, 25);
            this.label8.TabIndex = 9;
            this.label8.Text = "eps: ";
            // 
            // EpsilonTextBox
            // 
            this.EpsilonTextBox.Font = new System.Drawing.Font("Arial Unicode MS", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.EpsilonTextBox.Location = new System.Drawing.Point(341, 178);
            this.EpsilonTextBox.Name = "EpsilonTextBox";
            this.EpsilonTextBox.Size = new System.Drawing.Size(61, 33);
            this.EpsilonTextBox.TabIndex = 10;
            this.EpsilonTextBox.Text = "1e-3";
            this.EpsilonTextBox.Validated += new System.EventHandler(this.EpsilonTextBox_Validated);
            // 
            // calcNewtonButton
            // 
            this.calcNewtonButton.Font = new System.Drawing.Font("Arial Unicode MS", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.calcNewtonButton.Location = new System.Drawing.Point(399, 79);
            this.calcNewtonButton.Name = "calcNewtonButton";
            this.calcNewtonButton.Size = new System.Drawing.Size(73, 31);
            this.calcNewtonButton.TabIndex = 11;
            this.calcNewtonButton.Text = "Calc";
            this.calcNewtonButton.UseVisualStyleBackColor = true;
            this.calcNewtonButton.Click += new System.EventHandler(this.calcNewtonButton_Click);
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(862, 224);
            this.Controls.Add(this.calcNewtonButton);
            this.Controls.Add(this.EpsilonTextBox);
            this.Controls.Add(this.label8);
            this.Controls.Add(this.tabControl1);
            this.Controls.Add(this.clearNewtonButton);
            this.Controls.Add(this.nextNewtonButton);
            this.Controls.Add(this.NewtonUpDown);
            this.Controls.Add(this.iterNewtonCount);
            this.Controls.Add(this.label7);
            this.Controls.Add(this.NewtonY);
            this.Controls.Add(this.NewtonZ);
            this.Controls.Add(this.NewtonX);
            this.Controls.Add(this.label5);
            this.Controls.Add(this.label6);
            this.Controls.Add(this.label4);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.NewtonEquationTextBox);
            this.Name = "Form1";
            this.Text = "Newton Modified Method";
            ((System.ComponentModel.ISupportInitialize)(this.NewtonUpDown)).EndInit();
            this.tabControl1.ResumeLayout(false);
            this.tabPage1.ResumeLayout(false);
            this.tabPage1.PerformLayout();
            this.tabPage2.ResumeLayout(false);
            this.tabPage2.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.TextBox NewtonEquationTextBox;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.TextBox NewtonX;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.TextBox NewtonY;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.TextBox NewtonZ;
        private System.Windows.Forms.Label label7;
        private System.Windows.Forms.NumericUpDown NewtonUpDown;
        private System.Windows.Forms.Button nextNewtonButton;
        private System.Windows.Forms.Button clearNewtonButton;
        private System.Windows.Forms.Label iterNewtonCount;
        private System.Windows.Forms.TextBox NewtonResultTextBox;
        private System.Windows.Forms.TabControl tabControl1;
        private System.Windows.Forms.TabPage tabPage1;
        private System.Windows.Forms.TabPage tabPage2;
        private System.Windows.Forms.TextBox ResultJacobianTextBox;
        private System.Windows.Forms.Label label8;
        private System.Windows.Forms.TextBox EpsilonTextBox;
        private System.Windows.Forms.Button calcNewtonButton;
    }
}

