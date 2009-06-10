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
			this.button3 = new System.Windows.Forms.Button ();
			this.button2 = new System.Windows.Forms.Button ();
			this.label1 = new System.Windows.Forms.Label ();
			this.textBox1 = new System.Windows.Forms.TextBox ();
			this.textBox2 = new System.Windows.Forms.TextBox ();
			this.textBox3 = new System.Windows.Forms.TextBox ();
			this.groupBox1.SuspendLayout ();
			this.SuspendLayout ();
			// 
			// button1
			// 
			this.button1.Location = new System.Drawing.Point (238, 54);
			this.button1.Name = "button1";
			this.button1.Size = new System.Drawing.Size (75, 23);
			this.button1.TabIndex = 0;
			this.button1.Text = "b&utton1";
			this.button1.UseVisualStyleBackColor = true;
			// 
			// groupBox1
			// 
			this.groupBox1.Controls.Add (this.button3);
			this.groupBox1.Controls.Add (this.button2);
			this.groupBox1.Location = new System.Drawing.Point (131, 167);
			this.groupBox1.Name = "groupBox1";
			this.groupBox1.Size = new System.Drawing.Size (200, 100);
			this.groupBox1.TabIndex = 1;
			this.groupBox1.TabStop = false;
			this.groupBox1.Text = "groupBox1";
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
			// 
			// label1
			// 
			this.label1.AutoSize = true;
			this.label1.Location = new System.Drawing.Point (197, 59);
			this.label1.Name = "label1";
			this.label1.Size = new System.Drawing.Size (35, 13);
			this.label1.TabIndex = 2;
			this.label1.Text = "label1";
			// 
			// textBox1
			// 
			this.textBox1.Location = new System.Drawing.Point (238, 84);
			this.textBox1.Name = "textBox1";
			this.textBox1.Size = new System.Drawing.Size (100, 20);
			this.textBox1.TabIndex = 3;
			// 
			// textBox2
			// 
			this.textBox2.Location = new System.Drawing.Point (238, 111);
			this.textBox2.Name = "textBox2";
			this.textBox2.Size = new System.Drawing.Size (100, 20);
			this.textBox2.TabIndex = 4;
			this.textBox2.UseSystemPasswordChar = true;
			// 
			// textBox3
			// 
			this.textBox3.Location = new System.Drawing.Point (338, 177);
			this.textBox3.Multiline = true;
			this.textBox3.Name = "textBox3";
			this.textBox3.ScrollBars = System.Windows.Forms.ScrollBars.Both;
			this.textBox3.Size = new System.Drawing.Size (100, 90);
			this.textBox3.TabIndex = 7;
			this.textBox3.Text = "abcdefgabcdefgabcdef\n\n\n\n\n\n\n\ngabcdefgabcdefg";
			this.textBox3.WordWrap = false;
			// 
			// Form1
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF (6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.ClientSize = new System.Drawing.Size (587, 384);
			this.Controls.Add (this.textBox3);
			this.Controls.Add (this.textBox2);
			this.Controls.Add (this.textBox1);
			this.Controls.Add (this.label1);
			this.Controls.Add (this.groupBox1);
			this.Controls.Add (this.button1);
			this.Name = "Form1";
			this.Text = "TestForm1";
			this.groupBox1.ResumeLayout (false);
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
	}
}

