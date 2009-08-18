#!/usr/bin/env ipy

##############################################################################
# Written by:  Ray Wang <rawang@novell.com>
# Date:        06/23/2008
# Description: the sample for winforms control:
#              Splitter
##############################################################################

# The docstring below is used in the generated log file
"""
Test accessibility of "Splitter" control
"""

# imports
import clr
clr.AddReference('System.Windows.Forms')
clr.AddReference('System.Drawing')
from System.Windows.Forms import Application, DockStyle, Form, Splitter, Label, BorderStyle
from System.Drawing import Color

class SplitterSample(Form):
    """Splitter control class"""

    def __init__(self):
        """SplitterSample class init function."""

        # setup title
        self.Text = "Horizontal Splitter"

        # setup labels
        self.label0 = Label()
        self.label0.Dock = DockStyle.Top
        self.label0.BackColor = Color.DarkCyan
        self.label0.Text = "label0 on one side against splitter"

        self.label1 = Label()
        self.label1.Dock = DockStyle.Top
        self.label1.BackColor = Color.DarkCyan
        self.label1.Text = "label1 on one side against splitter"

	self.label2 = Label()
        self.label2.Dock = DockStyle.Top
        self.label2.BackColor = Color.DarkCyan
        self.label2.Text = "label2 on one side against splitter"
	
	self.label3 = Label()
        self.label3.Dock = DockStyle.Top
        self.label3.BackColor = Color.DarkCyan
        self.label3.Text = "label3 on one side against splitter"

        self.label4 = Label()
        self.label4.Dock = DockStyle.Fill
        self.label4.Text = "label4 on the other side against splitter"
        self.label4.BackColor = Color.Coral


        # setup splitter
        self.splitter = Splitter()
        self.splitter.Dock = DockStyle.Top
        self.splitter.BorderStyle = BorderStyle.Fixed3D

        # add controls
        self.Controls.Add(self.label4)
        self.Controls.Add(self.splitter)
        self.Controls.Add(self.label0)
        self.Controls.Add(self.label1)
        self.Controls.Add(self.label2)
        self.Controls.Add(self.label3)

# run application
form = SplitterSample()
Application.EnableVisualStyles()
Application.Run(form)
