#!/usr/bin/env ipy

##############################################################################
# Written by:  Calen Chen <cachen@novell.com>
# Date:        05/11/2008
# Description: This is a test application sample for winforms control:
#              ToolTip
##############################################################################

import clr
clr.AddReference('System.Windows.Forms')
clr.AddReference('System.Drawing')

from System.Windows.Forms import *
from System.Drawing import *

class ToolTipApp(Form):

    def __init__(self):
        self.Text = "Simple ToolTip Example"
        self.Width = 400
        self.Height = 400
        self.FormBorderStyle = FormBorderStyle.Fixed3D

        self.mainLabel1 = Label()
        self.mainLabel1.Text = "Examples for: ToolTip"
        self.mainLabel1.Location = Point(10,10)
        self.mainLabel1.AutoSize = True
        self.Controls.Add(self.mainLabel1)

        self.mainLabel2 = Label()
        self.mainLabel2.Text = "Hover your mouse over button and checkbox to see tooltips"
        self.mainLabel2.Location = Point(10,40)
        self.mainLabel2.AutoSize = True
        self.Controls.Add(self.mainLabel2)

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
        self.tooltip1.AutoPopDelay = 10000
        self.tooltip1.InitialDelay = 300
        self.tooltip1.ReshowDelay = 100
        self.tooltip1.ShowAlways = True
        self.tooltip1.SetToolTip(self.button1, "show button's tooltip")
        self.tooltip1.SetToolTip(self.checkbox1,"my favorite fruit")


form = ToolTipApp()
Application.Run(form)

