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
import os
import sys

clr.AddReference('System.Windows.Forms')
clr.AddReference('System.Drawing')

from System.Drawing import *
from System.Windows.Forms import *

harness_dir = sys.path[0]
uiaqa_path = os.path.dirname(harness_dir)

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

        # setup radiobutton
        self.radio1 = RadioButton()
        self.radio1.Text = "Male"
        self.radio1.Location = Point(20, 30)
        self.radio1.Checked = True
        #self.radio1.CheckedChanged += self.checkedChanged

        self.radio2 = RadioButton()
        self.radio2.Text = "Female"
        self.radio2.Location = Point(20, 60)
        #self.radio2.CheckedChanged += self.checkedChanged

        self.radio3 = RadioButton()
        self.radio3.Text = "Disabled"
        self.radio3.Location = Point(20, 30)
        self.radio3.AutoSize = True
        self.radio3.Enabled = True
        #self.radio3.CheckedChanged += self.checkedChanged

        self.radio4 = RadioButton()
        self.radio4.Text = "Lizard"
        self.radio4.Location = Point(20, 60)
        self.radio4.AutoSize = True
        self.radio4.Image = Image.FromFile("%s/samples/opensuse60x38.gif" % uiaqa_path)
        #self.radio4.CheckedChanged += self.checkedChanged

        # add controls
        self.groupbox1.Controls.Add(self.radio1)
        self.groupbox1.Controls.Add(self.radio2)

        self.groupbox2.Controls.Add(self.radio3)
        self.groupbox2.Controls.Add(self.radio4)

        self.Controls.Add(self.groupbox1)
        self.Controls.Add(self.groupbox2)
    
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

form = GroupBoxApp()
Application.Run(form)
