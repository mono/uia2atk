#!/usr/bin/env ipy

##############################################################################
# Written by:  Calen Chen <cachen@novell.com>
# Date:        03/11/2008
# Description: This is a test application sample for winforms control:
#              Form
##############################################################################

# The docstring below is used in the generated log file
"""
This sample will show "Form". It can be used for Autotest tools(e.g. Strongwind) to test the behaviors of controls.
"""


import clr

clr.AddReference('System.Windows.Forms')
clr.AddReference('System.Drawing')

from System.Drawing import *
from System.Windows.Forms import *
import System

class RunApp(Form):
    """Form controls class"""

    def __init__(self):
        """RunApp class init function."""

        self.count = 0
        self.Text = "Form control"

        # set up Button1 control
        self.button1 = Button()
        self.button1.Name = "button1"
        self.button1.Text = "button1"
        self.button1.Location = Point(10,40)
        self.button1.BackColor = Color.Green
        self.button1.ForeColor = Color.Red
        self.button1.Click += self.button1_click
        self.button1.Cursor = Cursors.Hand

        # set up Button2 control
        self.button2 = Button()
        self.button2.Name = "button2"
        self.button2.Text = "button2"
        self.button2.Location = Point(10,80)
        self.button2.BackColor = Color.Green
        self.button2.ForeColor = Color.Red
        self.button2.Click += self.button2_click
        self.button2.Cursor = Cursors.Hand

        # set up Button3 control
        self.button3 = Button()
        self.button3.Name = "button3"
        self.button3.Text = "button3"
        self.button3.Location = Point(10,120)
        self.button3.BackColor = Color.Green
        self.button3.ForeColor = Color.Red
        self.button3.Click += self.button3_click
        self.button3.Cursor = Cursors.Hand

        # add controls
        self.Controls.Add(self.button1)
        self.Controls.Add(self.button2)
        self.Controls.Add(self.button3)
    
    def button1_click(self, sender, event):
        print "Clicked Button 1"
        MessageBox.Show("successful clicked me", "Message Form")

    def button2_click(self, sender, event):
        print "Clicked Button 2"
        ef = ExtraForm()
        Form.Show(ef)

    def button3_click(self, sender, event):
        print "Clicked Button 3"
        ef = ExtraForm()
        Form.ShowDialog(ef)

class ExtraForm(Form):
    """Extra Form class"""

    def __init__(self):
        """ExtraForm class init function."""

        self.Text = "Extra Form"
        self.Height = 150
        self.StartPosition = FormStartPosition.CenterScreen


form = RunApp()
Application.Run(form)
