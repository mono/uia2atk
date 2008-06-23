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
        self.Text = "Splitter control"

        # setup labels
        self.label1 = Label()
        self.label1.Dock = DockStyle.Top
        self.label1.BackColor = Color.DarkCyan
        self.label1.Text = "label1 on one side against splitter"

        self.label2 = Label()
        self.label2.Dock = DockStyle.Top
        self.label2.Text = "label2 on the other side against splitter"
        self.label2.BackColor = Color.Coral


        # setup splitter
        self.splitter = Splitter()
        self.splitter.Dock = DockStyle.Top
        self.splitter.BorderStyle = BorderStyle.Fixed3D

        # add controls
        self.Controls.Add(self.label2)
        self.Controls.Add(self.splitter)
        self.Controls.Add(self.label1)

# run application
form = SplitterSample()
Application.EnableVisualStyles()
Application.Run(form)
