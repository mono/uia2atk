namespace SampleForm {
	partial class Form1 {
		/// <summary>
		/// Required designer variable.
		/// </summary>
		private System.ComponentModel.IContainer components = null;

		/// <summary>
		/// Clean up any resources being used.
		/// </summary>
		/// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
		protected override void Dispose (bool disposing)
		{
			if (disposing && ( components != null )) {
				components.Dispose ();
			}
			base.Dispose (disposing);
		}

		#region Windows Form Designer generated code

		/// <summary>
		/// Required method for Designer support - do not modify
		/// the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent ()
		{
			this.button1 = new System.Windows.Forms.Button ();
			this.groupBox1 = new System.Windows.Forms.GroupBox ();
			this.panel1 = new System.Windows.Forms.Panel ();
			this.btnRemoveTextbox = new System.Windows.Forms.Button ();
			this.btnAddTextbox = new System.Windows.Forms.Button ();
			this.groupBox3 = new System.Windows.Forms.GroupBox ();
			this.button7 = new System.Windows.Forms.Button ();
			this.button6 = new System.Windows.Forms.Button ();
			this.groupBox2 = new System.Windows.Forms.GroupBox ();
			this.button5 = new System.Windows.Forms.Button ();
			this.checkBox1 = new System.Windows.Forms.CheckBox ();
			this.button4 = new System.Windows.Forms.Button ();
			this.button3 = new System.Windows.Forms.Button ();
			this.button2 = new System.Windows.Forms.Button ();
			this.label1 = new System.Windows.Forms.Label ();
			this.textBox1 = new System.Windows.Forms.TextBox ();
			this.textBox2 = new System.Windows.Forms.TextBox ();
			this.textBox3 = new System.Windows.Forms.TextBox ();
			this.treeView1 = new System.Windows.Forms.TreeView ();
			this.groupBox1.SuspendLayout ();
			this.panel1.SuspendLayout ();
			this.groupBox3.SuspendLayout ();
			this.groupBox2.SuspendLayout ();
			this.SuspendLayout ();
			// 
			// button1
			// 
			this.button1.Location = new System.Drawing.Point (102, 26);
			this.button1.Name = "button1";
			this.button1.Size = new System.Drawing.Size (75, 23);
			this.button1.TabIndex = 0;
			this.button1.Text = "b&utton1";
			this.button1.UseVisualStyleBackColor = true;
			this.button1.Click += new System.EventHandler (this.button1_Click);
			// 
			// groupBox1
			// 
			this.groupBox1.Controls.Add (this.groupBox3);
			this.groupBox1.Controls.Add (this.groupBox2);
			this.groupBox1.Controls.Add (this.button3);
			this.groupBox1.Controls.Add (this.button2);
			this.groupBox1.Location = new System.Drawing.Point (44, 153);
			this.groupBox1.Name = "groupBox1";
			this.groupBox1.Size = new System.Drawing.Size (489, 204);
			this.groupBox1.TabIndex = 1;
			this.groupBox1.TabStop = false;
			this.groupBox1.Text = "groupBox1";
			// 
			// panel1
			// 
			this.panel1.Controls.Add (this.btnRemoveTextbox);
			this.panel1.Controls.Add (this.btnAddTextbox);
			this.panel1.Location = new System.Drawing.Point (338, 26);
			this.panel1.Name = "panel1";
			this.panel1.Size = new System.Drawing.Size (130, 50);
			this.panel1.TabIndex = 4;
			// 
			// btnRemoveTextbox
			// 
			this.btnRemoveTextbox.Location = new System.Drawing.Point (62, 9);
			this.btnRemoveTextbox.Name = "btnRemoveTextbox";
			this.btnRemoveTextbox.Size = new System.Drawing.Size (63, 25);
			this.btnRemoveTextbox.TabIndex = 1;
			this.btnRemoveTextbox.Text = "Remove";
			this.btnRemoveTextbox.UseVisualStyleBackColor = true;
			this.btnRemoveTextbox.Click += new System.EventHandler (this.btnRemoveTextbox_Click);
			// 
			// btnAddTextbox
			// 
			this.btnAddTextbox.Location = new System.Drawing.Point (6, 9);
			this.btnAddTextbox.Name = "btnAddTextbox";
			this.btnAddTextbox.Size = new System.Drawing.Size (50, 25);
			this.btnAddTextbox.TabIndex = 0;
			this.btnAddTextbox.Text = "Add";
			this.btnAddTextbox.UseVisualStyleBackColor = true;
			this.btnAddTextbox.Click += new System.EventHandler (this.btnAddTextbox_Click);
			// 
			// groupBox3
			// 
			this.groupBox3.Controls.Add (this.button7);
			this.groupBox3.Controls.Add (this.button6);
			this.groupBox3.Enabled = false;
			this.groupBox3.Location = new System.Drawing.Point (272, 20);
			this.groupBox3.Name = "groupBox3";
			this.groupBox3.Size = new System.Drawing.Size (200, 100);
			this.groupBox3.TabIndex = 3;
			this.groupBox3.TabStop = false;
			this.groupBox3.Text = "groupBox3";
			// 
			// button7
			// 
			this.button7.Enabled = false;
			this.button7.Location = new System.Drawing.Point (7, 44);
			this.button7.Name = "button7";
			this.button7.Size = new System.Drawing.Size (75, 23);
			this.button7.TabIndex = 1;
			this.button7.Text = "button7";
			this.button7.UseVisualStyleBackColor = true;
			// 
			// button6
			// 
			this.button6.Location = new System.Drawing.Point (7, 13);
			this.button6.Name = "button6";
			this.button6.Size = new System.Drawing.Size (75, 23);
			this.button6.TabIndex = 0;
			this.button6.Text = "button6";
			this.button6.UseVisualStyleBackColor = true;
			// 
			// groupBox2
			// 
			this.groupBox2.Controls.Add (this.button5);
			this.groupBox2.Controls.Add (this.checkBox1);
			this.groupBox2.Controls.Add (this.button4);
			this.groupBox2.Location = new System.Drawing.Point (66, 93);
			this.groupBox2.Name = "groupBox2";
			this.groupBox2.Size = new System.Drawing.Size (200, 100);
			this.groupBox2.TabIndex = 2;
			this.groupBox2.TabStop = false;
			this.groupBox2.Text = "groupBox2";
			// 
			// button5
			// 
			this.button5.Enabled = false;
			this.button5.Location = new System.Drawing.Point (90, 20);
			this.button5.Name = "button5";
			this.button5.Size = new System.Drawing.Size (75, 23);
			this.button5.TabIndex = 2;
			this.button5.Text = "button5";
			this.button5.UseVisualStyleBackColor = true;
			// 
			// checkBox1
			// 
			this.checkBox1.AutoSize = true;
			this.checkBox1.Location = new System.Drawing.Point (7, 50);
			this.checkBox1.Name = "checkBox1";
			this.checkBox1.Size = new System.Drawing.Size (80, 17);
			this.checkBox1.TabIndex = 1;
			this.checkBox1.Text = "checkBox1";
			this.checkBox1.UseVisualStyleBackColor = true;
			// 
			// button4
			// 
			this.button4.Location = new System.Drawing.Point (7, 20);
			this.button4.Name = "button4";
			this.button4.Size = new System.Drawing.Size (75, 23);
			this.button4.TabIndex = 0;
			this.button4.Text = "button4";
			this.button4.UseVisualStyleBackColor = true;
			// 
			// button3
			// 
			this.button3.AccessibleDescription = "help text 3";
			this.button3.Enabled = false;
			this.button3.Location = new System.Drawing.Point (66, 63);
			this.button3.Name = "button3";
			this.button3.Size = new System.Drawing.Size (75, 23);
			this.button3.TabIndex = 1;
			this.button3.Text = "button3";
			this.button3.UseVisualStyleBackColor = true;
			// 
			// button2
			// 
			this.button2.Location = new System.Drawing.Point (66, 33);
			this.button2.Name = "button2";
			this.button2.Size = new System.Drawing.Size (128, 23);
			this.button2.TabIndex = 0;
			this.button2.Text = "button2";
			this.button2.UseVisualStyleBackColor = true;
			this.button2.Click += new System.EventHandler (this.button2_Click);
			// 
			// label1
			// 
			this.label1.AutoSize = true;
			this.label1.Location = new System.Drawing.Point (61, 31);
			this.label1.Name = "label1";
			this.label1.Size = new System.Drawing.Size (35, 13);
			this.label1.TabIndex = 2;
			this.label1.Text = "label1";
			// 
			// textBox1
			// 
			this.textBox1.Location = new System.Drawing.Point (102, 56);
			this.textBox1.Name = "textBox1";
			this.textBox1.Size = new System.Drawing.Size (100, 20);
			this.textBox1.TabIndex = 3;
			// 
			// textBox2
			// 
			this.textBox2.Enabled = false;
			this.textBox2.Location = new System.Drawing.Point (102, 83);
			this.textBox2.Name = "textBox2";
			this.textBox2.ReadOnly = true;
			this.textBox2.Size = new System.Drawing.Size (100, 20);
			this.textBox2.TabIndex = 4;
			this.textBox2.UseSystemPasswordChar = true;
			// 
			// textBox3
			// 
			this.textBox3.Location = new System.Drawing.Point (232, 26);
			this.textBox3.Multiline = true;
			this.textBox3.Name = "textBox3";
			this.textBox3.ScrollBars = System.Windows.Forms.ScrollBars.Both;
			this.textBox3.Size = new System.Drawing.Size (100, 90);
			this.textBox3.TabIndex = 7;
			this.textBox3.Text = "abcdefgabcdefgabcdef\n\n\n\n\n\n\n\ngabcdefgabcdefg";
			this.textBox3.WordWrap = false;
			// 
			// treeView1
			// 
			this.treeView1.Location = new System.Drawing.Point (44, 364);
			this.treeView1.Name = "treeView1";
			this.treeView1.Size = new System.Drawing.Size (121, 97);
			this.treeView1.TabIndex = 8;
			// 
			// Form1
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF (6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.ClientSize = new System.Drawing.Size (587, 530);
			this.Controls.Add (this.treeView1);
			this.Controls.Add (this.panel1);
			this.Controls.Add (this.textBox3);
			this.Controls.Add (this.textBox2);
			this.Controls.Add (this.textBox1);
			this.Controls.Add (this.label1);
			this.Controls.Add (this.groupBox1);
			this.Controls.Add (this.button1);
			this.Name = "Form1";
			this.Text = "TestForm1";
			this.groupBox1.ResumeLayout (false);
			this.panel1.ResumeLayout (false);
			this.groupBox3.ResumeLayout (false);
			this.groupBox2.ResumeLayout (false);
			this.groupBox2.PerformLayout ();
			this.ResumeLayout (false);
			this.PerformLayout ();

		}

		#endregion

		private System.Windows.Forms.Button button1;
		private System.Windows.Forms.GroupBox groupBox1;
		private System.Windows.Forms.Button button3;
		private System.Windows.Forms.Button button2;
		private System.Windows.Forms.Label label1;
		private System.Windows.Forms.TextBox textBox1;
		private System.Windows.Forms.TextBox textBox2;
		private System.Windows.Forms.TextBox textBox3;
		private System.Windows.Forms.GroupBox groupBox2;
		private System.Windows.Forms.Button button5;
		private System.Windows.Forms.CheckBox checkBox1;
		private System.Windows.Forms.Button button4;
		private System.Windows.Forms.GroupBox groupBox3;
		private System.Windows.Forms.Button button7;
		private System.Windows.Forms.Button button6;
		private System.Windows.Forms.Panel panel1;
		private System.Windows.Forms.Button btnRemoveTextbox;
		private System.Windows.Forms.Button btnAddTextbox;
		private System.Windows.Forms.TreeView treeView1;
	}
}

