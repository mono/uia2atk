#!/usr/bin/env ipy

###StatusStrip

import clr
import System
clr.AddReference('System.Windows.Forms')
clr.AddReference('System.Drawing')

from System.Windows.Forms import *
from System.Drawing import *


class TestStatusStrip(Form):
	def __init__(self):
		self.Text = "Simple StatusStrip Example"
		self.Width = 400
		self.Height = 400
		self.FormBorderStyle = FormBorderStyle.Fixed3D
		self.Name = "Form1"

		self.mainLabel1 = Label()
		self.mainLabel1.Text = "Examples for: StatusStrip."
		self.mainLabel1.Location = Point(10,60)
		self.mainLabel1.AutoSize = True
		self.Controls.Add(self.mainLabel1)


##set StatusStrip:
		self.statusstrip1 = StatusStrip()
		self.statusstrip1.GripStyle = ToolStripGripStyle.Visible
		self.statusstrip1.Name = "toolstrip1"

##set ToolStripStatusLabel:
		self.toolstripstatuslabel1 = ToolStripStatusLabel()
		self.toolstripstatuslabel1.Text = "ToolStripLabel Text..."
		self.toolstripstatuslabel1.BorderStyle = Border3DStyle.Raised
		self.toolstripstatuslabel1.BorderSides = ToolStripStatusLabelBorderSides.Bottom
		self.toolstripstatuslabel1.Spring = True

##set ToolStripButton:
		self.toolstripbutton1 = ToolStripButton("Click Me")
		self.toolstripbutton1.Click += self.bc

		self.statusstrip1.Items.Add(self.toolstripbutton1)
		self.statusstrip1.Items.Add(self.toolstripstatuslabel1)
		self.statusstrip1.Items.Add("One")
		self.statusstrip1.Items.Add("Two")
		self.statusstrip1.Items.Add("Three")
		self.Controls.Add(self.statusstrip1)


	def bc(object,sender,event):
		MessageBox.Show("the first plugin")
	

form = TestStatusStrip()
Application.Run(form)


