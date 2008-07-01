#!/usr/bin/env ipy

##############################################################################
# Written by:  Ray Wang <rawang@novell.com>
# Date:        07/01/2008
# Description: the sample for winforms control:
#              FlowLayoutPanel
##############################################################################

# The docstring below is used in the generated log file
"""
Test accessibility of "FlowLayoutPanel" control
"""

# imports
import clr
clr.AddReference('System.Windows.Forms')
clr.AddReference('System.Drawing')
from System.Windows.Forms import (
    Application, Form, Label, FlowLayoutPanel, DockStyle, FlowDirection
)
from System.Drawing import Color

class FlowLayoutPanelSample(Form):
    """FlowLayoutPanel control class"""

    def __init__(self):
        """FlowLayoutPanelSample class init function."""

        # setup title
        self.Text = "FlowLayoutPanel control"

        # setup labels
        self.label1 = Label()
        self.label2 = Label()
        self.label3 = Label()
        self.label4 = Label()
        self.label1.Text = "label1"
        self.label2.Text = "label2"
        self.label3.Text = "label3"
        self.label4.Text = "label4"
        self.label1.BackColor = Color.Red
        self.label2.BackColor = Color.Green
        self.label3.BackColor = Color.Yellow
        self.label4.BackColor = Color.Blue
        self.label1.Dock = DockStyle.Top
        self.label2.Dock = DockStyle.Top
        self.label3.Dock = DockStyle.Top
        self.label4.Dock = DockStyle.Top

        # setup flowlayoutpanel
        self.flowlayoutpanel = FlowLayoutPanel()
        self.flowlayoutpanel.Dock = DockStyle.Fill
        self.flowlayoutpanel.TabIndex = 0
        self.flowlayoutpanel.FlowDirection = FlowDirection.TopDown #BottomUp 

        # add controls
        self.flowlayoutpanel.Controls.Add(self.label1)
        self.flowlayoutpanel.Controls.Add(self.label2)
        self.flowlayoutpanel.Controls.Add(self.label3)
        self.flowlayoutpanel.Controls.Add(self.label4)
        self.Controls.Add(self.flowlayoutpanel)


# run application
form = FlowLayoutPanelSample()
Application.EnableVisualStyles()
Application.Run(form)
