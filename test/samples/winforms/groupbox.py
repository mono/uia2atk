#!/usr/bin/env ipy

##############################################################################
# Written by:  Calen Chen <cachen@novell.com>
# Date:        03/11/2008
# Description: This is a test application sample for winforms control:
#              GroupBox
##############################################################################

# The docstring below is used in the generated log file
"""
This sample will show "GroupBox" controls in the form.
It can be used for Autotest tools(e.g. Strongwind) to test the behaviors of controls.
"""

import clr

clr.AddReference('System.Windows.Forms')
clr.AddReference('System.Drawing')

from System.Drawing import *
from System.Windows.Forms import *


class GroupBoxApp(Form):
    """GroupBoxApp controls class"""

    def __init__(self):
        """GroupBoxApp class init function."""

        self.count1 = 0
        self.count2 = 0

        # setup title
        self.Text = "GroupBox with Button"

        # setup groupboxs
        self.groupbox1 = GroupBox()
        self.groupbox1.Name = "groupbox1"
        self.groupbox1.Text = "GroupBox1"
        self.groupbox1.Location = Point(10,10)
        self.groupbox1.FlatStyle = FlatStyle.Flat

        self.groupbox2 = GroupBox()
        self.groupbox2.Name = "groupbox2"
        self.groupbox2.Text = "GroupBox2"
        self.groupbox2.Location = Point(10,120)
        self.groupbox2.FlatStyle = FlatStyle.Flat

        # setup buttons
        self.button1 = Button()
        self.button1.Text = "button1"
        self.button1.Location = Point(20,60)
        self.button1.Click += self.on_click

        self.button2 = Button()
        self.button2.Text = "button2"
        self.button2.Location = Point(20,60)
        self.button2.Click += self.on_click

        # setup labels
        self.label1 = Label()
        self.label1.Text = "the first Groupbox"
        self.label1.AutoSize = True
        self.label1.Location = Point(20,30)

        self.label2 = Label()
        self.label2.Text = "the second Groupbox"
        self.label2.AutoSize = True
        self.label2.Location = Point(20,30)

        # add controls
        self.groupbox1.Controls.Add(self.label1)
        self.groupbox1.Controls.Add(self.button1)

        self.groupbox2.Controls.Add(self.label2)
        self.groupbox2.Controls.Add(self.button2)

        self.Controls.Add(self.groupbox1)
        self.Controls.Add(self.groupbox2)
    
    def on_click(self, sender, event):
        if sender == self.button1:
            self.count1 += 1
            self.label1.Text = "%d" % self.count1
        else:
            self.count2 += 1
            self.label2.Text = "%d" % self.count2

form = GroupBoxApp()
Application.Run(form)
