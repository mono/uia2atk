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
        self.numercupdown1.Location = Point(10,50)
        self.numercupdown1.Value = 10
        self.numercupdown1.Maximum = 2500
        self.numercupdown1.Minimum = -100
        self.numercupdown1.UserEdit = True

        self.Controls.Add(self.numercupdown1)


form = NumericUpDownApp()
Application.Run(form)
