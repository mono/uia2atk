#!/usr/bin/env ipy

##############################################################################
# Written by:  Calen Chen <cachen@novell.com>
# Date:        03/11/2008
# Description: This is a test application sample for winforms control:
#              NumericUpDown
##############################################################################

import clr

clr.AddReference('System.Windows.Forms')
clr.AddReference('System.Drawing')

from System.Drawing import *
from System.Windows.Forms import *


class NumericUpDownApp(Form):

    def __init__(self):
        self.Text = "NumericUpDown Example"

        self.Width = 400
        self.Height = 400

        self.mainLabel1 = Label()
        self.mainLabel1.Text = "Examples for: NumericUpDown"
        self.mainLabel1.Location = Point(10,10)
        self.mainLabel1.AutoSize = True
        self.Controls.Add(self.mainLabel1)

        self.numercupdown1 = NumericUpDown()
        self.numercupdown1.Name = "NumercUpDown1"
        self.numercupdown1.Location = Point(10,50)
        self.numercupdown1.Value = 10
        self.numercupdown1.Maximum = 2500
        self.numercupdown1.Minimum = -100
        self.numercupdown1.Increment  = 20
        self.numercupdown1.ReadOnly = False
        self.numercupdown1.UserEdit = True

        self.Controls.Add(self.numercupdown1)

        self.numercupdown2 = NumericUpDown()
        self.numercupdown2.Name = "NumercUpDown2"
        self.numercupdown2.Location = Point(10,100)
        self.numercupdown2.Value = 10
        self.numercupdown2.Maximum = 2500
        self.numercupdown2.Minimum = -100
        self.numercupdown2.ReadOnly = True
        self.numercupdown2.UserEdit = True

        self.Controls.Add(self.numercupdown2)


form = NumericUpDownApp()
Application.Run(form)
