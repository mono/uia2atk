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

        # form
        self.Text = "NumericUpDown Example"

        # label
        self.mainLabel1 = Label()
        self.mainLabel1.Text = "Examples for: NumericUpDown"
        self.mainLabel1.Location = Point(10,10)
        self.mainLabel1.AutoSize = True
        self.Controls.Add(self.mainLabel1)

        # editable NumericUpDown
        self.numericupdown1 = NumericUpDown()
        self.numericupdown1.Name = "NumercUpDown1"
        self.numericupdown1.Location = Point(10,50)
        self.numericupdown1.Value = 10
        self.numericupdown1.Maximum = 2500
        self.numericupdown1.Minimum = -100
        self.numericupdown1.Increment  = 20
        self.numericupdown1.ReadOnly = False
        self.numericupdown1.UserEdit = True

        # uneditable NumericUpDown
        self.numericupdown2 = NumericUpDown()
        self.numericupdown2.Name = "NumercUpDown2"
        self.numericupdown2.Location = Point(10,100)
        self.numericupdown2.Value = 10
        self.numericupdown2.Maximum = 2500
        self.numericupdown2.Minimum = -100
        self.numericupdown2.ReadOnly = True
        self.numericupdown2.UserEdit = True

        # add controls to form
        self.Controls.Add(self.numericupdown1)
        self.Controls.Add(self.numericupdown2)


form = NumericUpDownApp()
Application.Run(form)
