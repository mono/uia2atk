#!/usr/bin/env ipy

##############################################################################
# Written by:  Calen Chen <cachen@novell.com>
# Date:        07/24/2008
# Description: This is a test application sample for winforms control:
#              RadioButton
##############################################################################

# The docstring below is used in the generated log file
"""
This sample will show "RadioButton" controls in the form.
It can be used for Autotest tools(e.g. Strongwind) to test the behaviors of controls.
"""

import clr
import os
from sys import path
from os.path import exists

clr.AddReference('System.Windows.Forms')
clr.AddReference('System.Drawing')

from System.Drawing import *
from System.Windows.Forms import *

harness_dir = path[0]
uiaqa_path = os.path.dirname(harness_dir)

class RadioButtonApp(Form):

    def __init__(self):
        self.Text = "RadioButton control"

        self.Width = 380
        self.Height = 200

        #radiobutton control

        self.mainLabel2 = Label()
        self.mainLabel2.Text = "Examples for: RadioLabel"
        self.mainLabel2.Location = Point(10, 30)
        self.mainLabel2.AutoSize = True

        self.radioLabel1 = Label()
        self.radioLabel1.Text = "Tell Me Your Gender:"
        self.radioLabel1.Location = Point(10, 50)
        self.radioLabel1.AutoSize = True

        self.radio1 = RadioButton()
        self.radio1.Text = "Male"
        self.radio1.Location = Point(10, 70)
        self.radio1.Checked = True
        self.radio1.CheckedChanged += self.checkedChanged

        self.radio2 = RadioButton()
        self.radio2.Text = "Female"
        self.radio2.Location = Point(110, 70)
        self.radio2.CheckedChanged += self.checkedChanged

        self.radio3 = RadioButton()
        self.radio3.Text = "Disabled"
        self.radio3.Location = Point(210, 70)
        self.radio3.AutoSize = True
        self.radio3.Enabled = False

        self.radio4 = RadioButton()
        self.radio4.Text = "Lizard"
        self.radio4.Location = Point(10, 100)
        self.radio4.AutoSize = True
        self.radio4.Image = Image.FromFile("%s/winforms/opensuse60x38.gif" % uiaqa_path)
        self.radio4.CheckedChanged += self.checkedChanged

        self.radioLabel2 = Label()
        self.radioLabel2.Text = "Go On:____"
        self.radioLabel2.Location = Point(10, 140)
        self.radioLabel2.AutoSize = True
        self.radioLabel2.Font = Font("Arial", 10, FontStyle.Bold)
        self.radioLabel2.ForeColor = Color.Red

        self.Controls.Add(self.mainLabel2)
        self.Controls.Add(self.radioLabel1)
        self.Controls.Add(self.radioLabel2)
        self.Controls.Add(self.radio1)
        self.Controls.Add(self.radio2)
        self.Controls.Add(self.radio3)
        self.Controls.Add(self.radio4)

    def checkedChanged(self, sender, args):
        if not sender.CheckedChanged:
            return
        if sender.Text == "Female":
            self.radioLabel2.Text = "You are %s" % self.radio2.Text 
        elif sender.Text == "Male":
            self.radioLabel2.Text = "You are %s" % self.radio1.Text
        elif sender.Text == "Lizard":
            self.radioLabel2.Text = "You are a lizard"
        else:
            self.radioLabel2.Text = "Error"

form = RadioButtonApp()
Application.Run(form)
