#!/usr/bin/env ipy

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
        self.Text = "Check Button"
        self.Width = 169
        self.Height = 171
        self.BorderStyle = BorderStyle.FixedSingle

        self.check1 = CheckBox()
        self.check1.Text = "check button 1"
        self.check1.TabIndex = 0
        self.check1.Location = Point(22,25)
        self.check1.Click += self.checkbox_click

        self.check2 = CheckBox()
        self.check2.Text = "check button 2"
        self.check2.TabIndex = 1
        self.check2.Location = Point(22,60)
        self.check2.Click += self.checkbox_click

        self.check3 = CheckBox()
        self.check3.Text = "check button 3"
        self.check3.TabIndex = 1
        self.check3.Location = Point(22, 80)
        self.check3.Click += self.checkbox_click

        self.button = Button()
        self.button.Text = "Quit"
        self.button.TabIndex = 2
        self.button.Location = Point(22,95)
        self.button.Size = Size(118,30)
        self.button.Click += self.button_click

        self.Controls.Add(self.button)
        self.Controls.Add(self.check3)
        self.Controls.Add(self.check2)
        self.Controls.Add(self.check1)

    def button_click(self, sender, event):
        print "Quit Button Clicked"
        Application.Exit()

    def checkbox_click(self, sender, event):
        if sender.Checked is True:
            print sender.Text + " was toggled ON"
        else:
            print sender.Text + " was toggled OFF"

Application.EnableVisualStyles()
form = CheckBoxApp()
Application.Run(form)
