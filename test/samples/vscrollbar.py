#!/usr/bin/env ipy

##############################################################################
# Written by:  Ray Wang <rawang@novell.com>
# Date:        06/23/2008
# Description: the sample for winforms control:
#              ScrollBar
#              VScrollBar
##############################################################################

# The docstring below is used in the generated log file
"""
Test accessibility of "VScrollBar" control
"""

# imports
import clr
clr.AddReference('System.Windows.Forms')
from System.Windows.Forms import Application, DockStyle, Form, VScrollBar

class VScrollBarSample(Form):
    """VScrollBar control class"""

    def __init__(self):
        """VScrollBarSample class init function."""

        # setup title
        self.Text = "VScrollBar control"

        # setup vscrollbar
        self.vscrollbar = VScrollBar()
        self.vscrollbar.Dock = DockStyle.Right

        # add controls
        self.Controls.Add(self.vscrollbar)

# run application
form = VScrollBarSample()
Application.EnableVisualStyles()
Application.Run(form)
