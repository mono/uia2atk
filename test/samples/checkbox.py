#!/usr/bin/env ipy

##############################################################################
# Written by:  Calen Chen <cachen@novell.com>
# Date:        07/24/2008
# Description: This is a test application sample for winforms control:
#              CheckBox
##############################################################################

# The docstring below is used in the generated log file
"""
This sample will show "CheckBox" controls in the form.
It can be used for Autotest tools(e.g. Strongwind) to test the behaviors of controls.
"""

import clr

clr.AddReference('System.Windows.Forms')
clr.AddReference('System.Drawing')

from System.Drawing import *
from System.Windows.Forms import *


class CheckBoxApp(Form):

    def __init__(self):
        self.Text = "CheckBox control"

        self.Width = 380
        self.Height = 200

        #checkbox control
        self.mainLabel1 = Label()
        self.mainLabel1.Text = "Examples for: CheckBox"
        self.mainLabel1.Location = Point(10,10)
        self.mainLabel1.AutoSize = True
        self.Controls.Add(self.mainLabel1)

        self.checkLabel = Label()
        self.checkLabel.Text = "Multi-choose:"
        self.checkLabel.Location = Point(10, 30)
        self.checkLabel.AutoSize = True

        self.check1 = CheckBox()
        self.check1.Text = "Bananas"
        self.check1.Location = Point(10, 60)
        self.check1.Width = 90

        self.check2 = CheckBox()
        self.check2.Text = "Chicken"
        self.check2.Location = Point(110, 60)
        self.check2.Width = 90

        self.check3 = CheckBox()
        self.check3.Text = "Stuffed Peppers"
        self.check3.Location = Point(210, 60)
        self.check3.Width = 90
        self.check3.Checked = True

        self.check4 = CheckBox()
        self.check4.Text = "Beef"
        self.check4.Location = Point(10, 90)
        self.check4.Width = 90
        self.check4.Enabled = False

        self.Controls.Add(self.checkLabel)
        self.Controls.Add(self.check1)
        self.Controls.Add(self.check2)
        self.Controls.Add(self.check3)
        self.Controls.Add(self.check4)

form = CheckBoxApp()
Application.Run(form)
