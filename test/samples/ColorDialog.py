#!/usr/bin/env ipy

###ColorDialog

import clr
clr.AddReference('System.Windows.Forms')
clr.AddReference('System.Drawing')
clr.AddReference('System')

from System.Windows.Forms import *
from System.Drawing import *
from System import *

class TestColorDialog(Form):
	def __init__(self):
		self.Text = "Simple ColorDialog Example"
		self.Width = 400
		self.Height = 500
		self.FormBorderStyle = FormBorderStyle.Fixed3D

		self.mainLabel1 = Label()
		self.mainLabel1.Text = "Examples for: ColorDialog"
		self.mainLabel1.Location = Point(10,10)
		self.mainLabel1.AutoSize = True
		self.Controls.Add(self.mainLabel1)
		
		self.textbox1 = TextBox()
		self.textbox1.Text = ""
		self.textbox1.Location = Point(10,50)
		self.textbox1.Size = Size(150,100)
		self.textbox1.BackColor = Color.Red
		self.Controls.Add(self.textbox1)

		self.button1 = Button()
		self.button1.Text = "ColorDialog Button"
		self.button1.AutoSize = True
		self.button1.Location = Point(200,50)
		self.button1.Click += self.bc
		self.Controls.Add(self.button1)		

		self.colordialog1 = ColorDialog()
		self.colordialog1.Color = Color.Red

	def bc(self, sender, event):
		if(self.colordialog1.ShowDialog() == self.DialogResult.OK):
			self.textbox1.BackColor = self.colordialog1.Color
			Console.WriteLine(self.colordialog1.Color)

form = TestColorDialog()
Application.Run(form)

