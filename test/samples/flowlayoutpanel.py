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
from System.Windows.Forms import *
from System.Drawing import Color

class FlowLayoutPanelSample(Form):
    """FlowLayoutPanel control class"""

    def __init__(self):
        """FlowLayoutPanelSample class init function."""

        # setup title
        self.Text = "FlowLayoutPanel control"

        # setup labels
        self.label1 = Label()
        self.label1.Text = "label1"
        self.label1.BackColor = Color.Red
        self.label1.Dock = DockStyle.Top

        self.label2 = Label()
        self.label2.Text = "label2"
        self.label2.BackColor = Color.Green
        self.label2.Dock = DockStyle.Top

        self.label3 = Label()
        self.label3.Text = "label3"
        self.label3.BackColor = Color.Yellow
        self.label3.Dock = DockStyle.Top

        self.label4 = Label()
        self.label4.Text = "label4"
        self.label4.BackColor = Color.Blue
        self.label4.Dock = DockStyle.Top

        # setup buttons
        self.button1 = Button()
        self.button1.Text = "button1"
        self.button1.Click += self.button_click

        self.button2 = Button()
        self.button2.Text = "button2"
        self.button2.Click += self.button_click

        self.button3 = Button()
        self.button3.Text = "button3"
        self.button3.Click += self.button_click

        self.button4 = Button()
        self.button4.Text = "button4"
        self.button4.Click += self.button_click

        # setup flowlayoutpanels
        self.flowlayoutpanel1 = FlowLayoutPanel()
        self.flowlayoutpanel1.Dock = DockStyle.Bottom
        self.flowlayoutpanel1.Height = self.Height / 4
        self.flowlayoutpanel1.FlowDirection = FlowDirection.TopDown
        self.flowlayoutpanel1.BorderStyle = BorderStyle.FixedSingle

        self.flowlayoutpanel2 = FlowLayoutPanel()
        self.flowlayoutpanel2.Dock = DockStyle.Bottom
        self.flowlayoutpanel2.Height = self.Height / 4
        self.flowlayoutpanel2.FlowDirection = FlowDirection.BottomUp 
        self.flowlayoutpanel2.BorderStyle = BorderStyle.FixedSingle

        self.flowlayoutpanel3 = FlowLayoutPanel()
        self.flowlayoutpanel3.Dock = DockStyle.Bottom
        self.flowlayoutpanel3.Height = self.Height / 4 - 20
        self.flowlayoutpanel3.FlowDirection = FlowDirection.LeftToRight
        self.flowlayoutpanel3.BorderStyle = BorderStyle.FixedSingle

        self.flowlayoutpanel4 = FlowLayoutPanel()
        self.flowlayoutpanel4.Dock = DockStyle.Bottom
        self.flowlayoutpanel4.Height = self.Height / 4 - 20
        self.flowlayoutpanel4.FlowDirection = FlowDirection.RightToLeft
        self.flowlayoutpanel4.BorderStyle = BorderStyle.FixedSingle

        # add controls

        self.flowlayoutpanel1.Controls.Add(self.label1)
        self.flowlayoutpanel1.Controls.Add(self.button1)

        self.flowlayoutpanel2.Controls.Add(self.label2)
        self.flowlayoutpanel2.Controls.Add(self.button2)

        self.flowlayoutpanel3.Controls.Add(self.label3)
        self.flowlayoutpanel3.Controls.Add(self.button3)

        self.flowlayoutpanel4.Controls.Add(self.label4)
        self.flowlayoutpanel4.Controls.Add(self.button4)

        self.Controls.Add(self.flowlayoutpanel1)
        self.Controls.Add(self.flowlayoutpanel2)
        self.Controls.Add(self.flowlayoutpanel3)
        self.Controls.Add(self.flowlayoutpanel4)

    def button_click(self, sender, event):
        if sender.Text == "button1":
            self.label1.Text = "TopDown"
        elif sender.Text == "button2":
            self.label2.Text = "BottomUp"
        elif sender.Text == "button3":
            self.label3.Text = "LeftToRight"
        elif sender.Text == "button4":
            self.label4.Text = "RightToLeft"

# run application
form = FlowLayoutPanelSample()
Application.EnableVisualStyles()
Application.Run(form)
