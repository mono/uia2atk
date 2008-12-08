#!/usr/bin/env ipy

##############################################################################
# Written by:  Calen Chen <cachen@novell.com>
# Date:        03/11/2008
# Description: This is a test application sample for winforms control:
#              Panel
##############################################################################

# The docstring below is used in the generated log file
"""
This sample will show "Panel" control in the form.
It can be used for Autotest tools(e.g. Strongwind) to test the behaviors of controls.
"""

import clr

clr.AddReference('System.Windows.Forms')
clr.AddReference('System.Drawing')

from System.Drawing import *
from System.Windows.Forms import *


class PanelCheckBoxRadioButtonApp(Form):

    def newPanel(self, x, y):
        panel = Panel()
        panel.Width = 400
        panel.Height = 150
        panel.Location = Point(x, y)
        panel.BorderStyle = BorderStyle.Fixed3D
        return panel

    def __init__(self):

        # setup frame
        self.Text = "Panel control"
        self.Width = 400
        self.Height = 300

        # panel - 1
        self.checkPanel = self.newPanel(0, 0)
        self.checkPanel.Text = "panel1"
        self.checkPanel.Name = "panel1"

        # panel - 2
        self.radioPanel = self.newPanel(0, 150)
        self.radioPanel.Text = "panel2"
        self.radioPanel.Name = "panel2"

        print self.checkPanel.Name
        print self.radioPanel.Name

        self.setupCheckButtons()
        self.setupRadioButtons()

        self.Controls.Add(self.checkPanel)
        self.Controls.Add(self.radioPanel)

    def setupCheckButtons(self):

        self.checkLabel = Label()
        self.checkLabel.Text = "multi-choose:"
        self.checkLabel.Location = Point(25, 25)
        self.checkLabel.AutoSize = True

        self.check1 = CheckBox()
        self.check1.Text = "Bananas"
        self.check1.Location = Point(25, 50)
        self.check1.Width = 90

        self.check2 = CheckBox()
        self.check2.Text = "Chicken"
        self.check2.Location = Point(125, 50)
        self.check2.Width = 110

        self.check3 = CheckBox()
        self.check3.Text = "Stuffed Peppers"
        self.check3.Location = Point(240, 50)
        self.check3.Width = 120
        self.check3.Checked = True

        self.check4 = CheckBox()
        self.check4.Text = "Beef"
        self.check4.Location = Point(25, 100)
        self.check4.Width = 120
        self.check4.Enabled = False

        self.checkPanel.Controls.Add(self.checkLabel)
        self.checkPanel.Controls.Add(self.check1)
        self.checkPanel.Controls.Add(self.check2)
        self.checkPanel.Controls.Add(self.check3)
        self.checkPanel.Controls.Add(self.check4)

    def setupRadioButtons(self):

        self.radioLabel1 = Label()
        self.radioLabel1.Text = "Tell Me Your Gender:"
        self.radioLabel1.Location = Point(25, 25)
        self.radioLabel1.AutoSize = True

        self.radio1 = RadioButton()
        self.radio1.Text = "Male"
        self.radio1.Location = Point(25, 50)
        self.radio1.Checked = True
        self.radio1.CheckedChanged += self.checkedChanged

        self.radio2 = RadioButton()
        self.radio2.Text = "Female"
        self.radio2.Location = Point(150, 50)
        self.radio2.CheckedChanged += self.checkedChanged

        self.radio3 = RadioButton()
        self.radio3.Text = "Disabled"
        self.radio3.Location = Point(260, 50)
        self.radio3.AutoSize = True
        self.radio3.Enabled = False

        self.radioLabel2 = Label()
        self.radioLabel2.Text = "Go On:____"
        self.radioLabel2.Location = Point(25, 80)
        self.radioLabel2.AutoSize = True
        self.radioLabel2.Font = Font("Arial", 10, FontStyle.Bold)
        self.radioLabel2.ForeColor = Color.Red

        self.radioPanel.Controls.Add(self.radioLabel1)
        self.radioPanel.Controls.Add(self.radioLabel2)
        self.radioPanel.Controls.Add(self.radio1)
        self.radioPanel.Controls.Add(self.radio2)
        self.radioPanel.Controls.Add(self.radio3)

    def checkedChanged(self, sender, args):
        if not sender.CheckedChanged:
            return
        if sender.Text == "Female":
            self.radioLabel2.Text = "You are %s" % self.radio2.Text 
        else:
            self.radioLabel2.Text = "You are %s" % self.radio1.Text

form = PanelCheckBoxRadioButtonApp()
Application.Run(form)
