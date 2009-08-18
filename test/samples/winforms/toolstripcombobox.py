#!/usr/bin/env ipy

##############################################################################
# Written by:  Calen Chen <cachen@novell.com>
# Date:        05/11/2008
# Description: This is a test application sample for winforms control:
#              ToolStrip
#              ToolStripComboBox
##############################################################################

# The docstring below is used in the generated log file
"""
This sample will show "ToolStripComboBox" controls in the form.
It can be used for Autotest tools(e.g. Strongwind) to test the behaviors of controls.
"""

import clr

clr.AddReference('System.Windows.Forms')
clr.AddReference('System.Drawing')

from System.Windows.Forms import *
from System.Drawing import *


class ToolStripComboBoxSample(Form):

    def __init__(self):

        # setup form
        self.Text = "ToolStripComboBox Control"
        self.Width = 500
        self.Height = 100 

        # setup label
        self.label = Label()
        self.label.AutoSize = True
        self.label.Dock = DockStyle.Top
        self.label.Text = "Please Select one Font Size from the ComboBox"
    
        # ToolStrip:
        self.toolstrip = ToolStrip()

        # ToolStripComboBox
        self.toolstrip_combobox = ToolStripComboBox()
        self.toolstrip_combobox.DropDownStyle = ComboBoxStyle.DropDownList
        self.toolstrip_combobox.Items.Add("6")
        self.toolstrip_combobox.Items.Add("8")
        self.toolstrip_combobox.Items.Add("10")
        self.toolstrip_combobox.Items.Add("12")
        self.toolstrip_combobox.Items.Add("14")
        self.toolstrip_combobox.SelectedIndex = 1
        self.toolstrip_combobox.AutoSize = False
        self.toolstrip_combobox.Width = 45
        self.toolstrip_combobox.SelectedIndexChanged += self.toolstrip_combobox_SelectedIndexChanged
        
        # add Controls
        self.toolstrip.Items.Add(self.toolstrip_combobox)
        self.Controls.Add(self.label)
        self.Controls.Add(self.toolstrip)

    def toolstrip_combobox_SelectedIndexChanged(self, sender, event):
        if self.toolstrip_combobox.SelectedIndex >= 0:
            self.label.Text = "The font size is %d" % int(self.toolstrip_combobox.SelectedItem)
            self.label.Font = Font(self.label.Font.Name, 
                                   int(self.toolstrip_combobox.SelectedItem))

form = ToolStripComboBoxSample()
Application.Run(form)
