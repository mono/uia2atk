#!/usr/bin/env ipy

##############################################################################
# Written by:  Calen Chen <cachen@novell.com>
# Date:        03/11/2008
# Description: This is a test application sample for winforms control:
#              ColorDialog
##############################################################################

# The docstring below is used in the generated log file
"""
This sample will show "ColorDialog" control in the form.
It can be used for Autotest tools(e.g. Strongwind) to test the behaviors of controls.
"""

import clr

clr.AddReference('System.Windows.Forms')
clr.AddReference('System.Drawing')

from System.Windows.Forms import *
from System.Drawing import *


class RunApp(Form):
    """ColorDialog controls class"""

    def __init__(self):
        """RunApp class init function."""

        self.Text = "ColorDialog control"

        self.mainLabel1 = Label()
        self.mainLabel1.Text = "Examples for: ColorDialog"
        self.mainLabel1.Location = Point(10,10)
        self.mainLabel1.AutoSize = True
        self.Controls.Add(self.mainLabel1)
        
        self.textbox1 = TextBox()
        self.textbox1.Text = ""
        self.textbox1.Location = Point(10,50)
        self.textbox1.Size = Size(150,100)
        self.textbox1.BackColor = Color.Red
        self.Controls.Add(self.textbox1)

        self.button1 = Button()
        self.button1.Text = "ColorDialog Button"
        self.button1.AutoSize = True
        self.button1.Location = Point(10,100)
        self.button1.Click += self.button1Click
        self.Controls.Add(self.button1)        

        self.colordialog1 = ColorDialog()
        self.colordialog1.Color = Color.Red

    def button1Click(self, sender, event):
        if(self.colordialog1.ShowDialog() == self.DialogResult.OK):
            self.textbox1.BackColor = self.colordialog1.Color
            self.mainLabel1.Text = str(self.colordialog1.Color)

form = RunApp()
Application.Run(form)
