###ToolTip

import clr
clr.AddReference('System.Windows.Forms')
clr.AddReference('System.Drawing')

from System.Windows.Forms import *
from System.Drawing import *

class TestToolTip(Form):
	def __init__(self):
		self.Text = "Simple ToolTip Example"
		self.Width = 400
		self.Height = 400
		self.FormBorderStyle = FormBorderStyle.Fixed3D

		self.mainLabel1 = Label()
		self.mainLabel1.Text = "Examples for: ToolTip\nSite your mouse to button and checkbox, you will see some info"
		self.mainLabel1.Location = Point(10,10)
		self.mainLabel1.AutoSize = True
		self.Controls.Add(self.mainLabel1)

		self.button1 = Button()
		self.button1.Text = "ToolTip button"
		self.button1.Location = Point(10,80)
		self.button1.AutoSize = True
		self.Controls.Add(self.button1)
		
		self.checkbox1 = CheckBox()
		self.checkbox1.Text = "Grape"
		self.checkbox1.Location = Point(10,140)
		self.checkbox1.AutoSize = True
		self.Controls.Add(self.checkbox1)

##set ToolTip and link to button1 and checkbox1
		self.tooltip1 = ToolTip()
		self.tooltip1.AutoPopDelay = 5000
		self.tooltip1.InitialDelay = 300
		self.tooltip1.ReshowDelay = 100
		self.tooltip1.ShowAlways = True
		self.tooltip1.SetToolTip(self.button1, "show button's tooltip")
		self.tooltip1.SetToolTip(self.checkbox1,"my favorite fruit")


form = TestToolTip()
Application.Run(form)

