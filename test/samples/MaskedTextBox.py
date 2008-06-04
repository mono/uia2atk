#!/usr/bin/env ipy

###MaskedTextBox

import clr
clr.AddReference('System.Windows.Forms')
clr.AddReference('System.Drawing')
clr.AddReference('System')

from System.Windows.Forms import *
from System.Drawing import *
from System import *

class TestMaskedTextBox(Form):
	def __init__(self):
		self.Text = "Simple MaskedTextBox Example"
		self.Width = 400
		self.Height = 400
		self.FormBorderStyle = FormBorderStyle.Fixed3D

		self.mainLabel1 = Label()
		self.mainLabel1.Text = "Examples for: MaskedTextBox"
		self.mainLabel1.Location = Point(10,10)
		self.mainLabel1.AutoSize = True
		self.Controls.Add(self.mainLabel1)

##Mask value = data	
		self.maskedTextBox1 = MaskedTextBox()
		self.maskedTextBox1.Text = ""
		self.maskedTextBox1.Location = Point(10,60)
		self.maskedTextBox1.Width = 180
		self.maskedTextBox1.Height = 30
		self.maskedTextBox1.Mask = "00/00/0000"
		self.maskedTextBox1.MaskInputRejected += MaskInputRejectedEventHandler(self.mm)
		self.Controls.Add(self.maskedTextBox1)


##Mask value = tel
		self.maskedTextBox2 = MaskedTextBox()
		self.maskedTextBox2.Text = ""
		self.maskedTextBox2.Location = Point(10,120)
		self.maskedTextBox2.Width = 180
		self.maskedTextBox2.Height = 30
		self.maskedTextBox2.Mask = "(086)-000-00000000"
		self.Controls.Add(self.maskedTextBox2)		
		self.tooltip2 = ToolTip()
		self.tooltip2.ShowAlways = True
		self.tooltip2.SetToolTip(self.maskedTextBox2, "inter your phone number")

##Mask value = price
		self.maskedTextBox3 = MaskedTextBox()
		self.maskedTextBox3.Text = ""
		self.maskedTextBox3.Location = Point(10,180)
		self.maskedTextBox3.Width = 180
		self.maskedTextBox3.Height = 30
		self.maskedTextBox3.Mask = "$999,999.00"
		self.Controls.Add(self.maskedTextBox3)


##Mask value = letter
		self.maskedTextBox4 = MaskedTextBox()
		self.maskedTextBox4.Text = ""
		self.maskedTextBox4.Location = Point(10,240)
		self.maskedTextBox4.Width = 180
		self.maskedTextBox4.Height = 30
		self.maskedTextBox4.Mask = "LLLLLLL"
		self.Controls.Add(self.maskedTextBox4)


	def mm(self, sender, MaskInputRejectedEventArgs):
		if(self.maskedTextBox1.MaskFull):
			print "End of Field"
		else:
			print "please input the data"
			#self.form1 = Form()
			#self.form1.ShowDialog()



form = TestMaskedTextBox()
Application.Run(form)

