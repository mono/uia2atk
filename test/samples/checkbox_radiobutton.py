#!/usr/bin/env ipy

##############################################################################
# Written by:  Calen Chen <cachen@novell.com>
# Date:        07/24/2008
# Description: This is a test application sample for winforms control:
#              CheckBox
#              RadioButton
##############################################################################

# The docstring below is used in the generated log file
"""
This sample will show "CheckBox" and "RadioButton" controls in the form.
It can be used for Autotest tools(e.g. Strongwind) to test the behaviors of controls.
"""

import clr

clr.AddReference('System.Windows.Forms')
clr.AddReference('System.Drawing')

from System.Drawing import *
from System.Windows.Forms import *


class CheckBoxApp(Form):

    def __init__(self):
        self.Text = "CheckBox_RadioButton controls"

        self.Width = 380
        self.Height = 300

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

        #radiobutton control

        self.mainLabel2 = Label()
        self.mainLabel2.Text = "Examples for: RadioLabel"
        self.mainLabel2.Location = Point(10, 130)
        self.mainLabel2.AutoSize = True

        self.radioLabel1 = Label()
        self.radioLabel1.Text = "Tell Me Your Gender:"
        self.radioLabel1.Location = Point(10, 150)
        self.radioLabel1.AutoSize = True

        self.radio1 = RadioButton()
        self.radio1.Text = "Male"
        self.radio1.Location = Point(10, 170)
        self.radio1.Checked = True
        self.radio1.CheckedChanged += self.checkedChanged

        self.radio2 = RadioButton()
        self.radio2.Text = "Female"
        self.radio2.Location = Point(110, 170)
        self.radio2.CheckedChanged += self.checkedChanged

        self.radio3 = RadioButton()
        self.radio3.Text = "Disabled"
        self.radio3.Location = Point(210, 170)
        self.radio3.AutoSize = True
        self.radio3.Enabled = False

        self.radioLabel2 = Label()
        self.radioLabel2.Text = "Go On:____"
        self.radioLabel2.Location = Point(10, 200)
        self.radioLabel2.AutoSize = True
        self.radioLabel2.Font = Font("Arial", 10, FontStyle.Bold)
        self.radioLabel2.ForeColor = Color.Red

        self.Controls.Add(self.mainLabel2)
        self.Controls.Add(self.radioLabel1)
        self.Controls.Add(self.radioLabel2)
        self.Controls.Add(self.radio1)
        self.Controls.Add(self.radio2)
        self.Controls.Add(self.radio3)


    def checkedChanged(self, sender, args):
        if not sender.CheckedChanged:
            return
        if sender.Text == "Female":
            self.radioLabel2.Text = "You are %s" % self.radio2.Text 
        else:
            self.radioLabel2.Text = "You are %s" % self.radio1.Text

form = CheckBoxApp()
Application.Run(form)
