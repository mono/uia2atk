###GroupBox

import clr
clr.AddReference('System.Windows.Forms')
clr.AddReference('System.Drawing')

from System.Drawing import *
from System.Windows.Forms import *
import System


class ButtonLable(Form):
	def __init__(self):
		self.Text = "GroupBox with Button"
		self.Height = 500
		self.Width = 400
		self.FormBorderStyle = FormBorderStyle.FixedDialog
		self.FormBorderStyle = FormBorderStyle.Fixed3D


##GroupBox control with Button and Label
		self.groupbox1 = GroupBox()
		self.button1 = Button()
		self.label1 = Label()
		self.button1.Text = "button1"
		self.label1.Text = "this is the first Groupbox"
		self.label1.AutoSize = True
		self.button1.Location = Point(20,60)
		self.groupbox1.Location = Point(10,10)
		self.label1.Location = Point(20,30)
		self.groupbox1.FlatStyle = FlatStyle.Flat
		self.groupbox1.Controls.Add(self.button1)
		self.groupbox1.Controls.Add(self.label1)
		self.Controls.Add(self.groupbox1)

		self.groupbox2 = GroupBox()
		self.button2 = Button()
		self.label2 = Label()
		self.button2.Text = "button2"
		self.label2.Text = "this is the second Groupbox"
		self.label2.AutoSize = True
		self.button2.Location = Point(20,60)
		self.groupbox2.Location = Point(10,120)
		self.label2.Location = Point(20,30)
		self.groupbox2.FlatStyle = FlatStyle.Flat
		self.groupbox2.Controls.Add(self.button2)
		self.groupbox2.Controls.Add(self.label2)
		self.Controls.Add(self.groupbox2)

	
	#def b1(self, sender, event):
	#	self.count += 1
	#	self.label.Text = "You have clicked b4 %s times" % self.count


form = ButtonLable()
Application.Run(form)
