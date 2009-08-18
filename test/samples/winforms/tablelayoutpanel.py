#!/usr/bin/env ipy

##############################################################################
# Written by:  Ray Wang <rawang@novell.com>
# Date:        06/30/2008
# Description: the sample for winforms control:
#              TableLayoutPanel
##############################################################################

# The docstring below is used in the generated log file
"""
Test accessibility of "TableLayoutPanel" control
"""

# imports
import clr
clr.AddReference('System.Windows.Forms')
clr.AddReference('System.Drawing')
from System.Windows.Forms import *
from System.Drawing import Color

class TableLayoutPanelSample(Form):
    """TableLayoutPanel control class"""

    def __init__(self):
        """TableLayoutPanelSample class init function."""

        # setup title
        self.Text = "TableLayoutPanel control"

        # setup tablelayoutpanel
        self.tablelayoutpanel = TableLayoutPanel()
        self.tablelayoutpanel.Dock = DockStyle.Fill
        self.tablelayoutpanel.GrowStyle = TableLayoutPanelGrowStyle.AddColumns
        self.tablelayoutpanel.ColumnCount = 2
        self.tablelayoutpanel.RowCount = 4
        self.tablelayoutpanel.ColumnStyles.Add(ColumnStyle(SizeType.Percent, 100.0))
        self.tablelayoutpanel.ColumnStyles.Add(ColumnStyle(SizeType.Percent, 100.0))
        self.tablelayoutpanel.RowStyles.Add(RowStyle(SizeType.Percent, 100.0))
        self.tablelayoutpanel.RowStyles.Add(RowStyle(SizeType.Percent, 100.0))

        # setup labels
        self.label1 = Label()
        self.label1.Text = "label1"
        self.label1.BackColor = Color.Red
        self.label1.Dock = DockStyle.Fill

        self.label2 = Label()
        self.label2.BackColor = Color.Green
        self.label2.Dock = DockStyle.Fill
        self.label2.Text = "label2"

        self.label3 = Label()
        self.label3.Text = "label3"
        self.label3.BackColor = Color.Yellow
        self.label3.Dock = DockStyle.Fill

        self.label4 = Label()
        self.label4.Text = "label4"
        self.label4.BackColor = Color.Blue
        self.label4.Dock = DockStyle.Fill

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

        # add controls
        # Controls.add (control, column, row)
        self.tablelayoutpanel.Controls.Add(self.label1, 0, 0)
        self.tablelayoutpanel.Controls.Add(self.button1, 0, 1)
        self.tablelayoutpanel.Controls.Add(self.label2, 1, 0)
        self.tablelayoutpanel.Controls.Add(self.button2, 1, 1)
        self.tablelayoutpanel.Controls.Add(self.label3, 0, 2)
        self.tablelayoutpanel.Controls.Add(self.button3, 0, 3)
        self.tablelayoutpanel.Controls.Add(self.label4, 1, 2)
        self.tablelayoutpanel.Controls.Add(self.button4, 1, 3)

        self.Controls.Add(self.tablelayoutpanel)

    def button_click(self, sender, event):
        if sender.Text == "button1":
            self.label1.Text = "I am in cell1"
        elif sender.Text == "button2":
            self.label2.Text = "I am in cell2"
        elif sender.Text == "button3":
            self.label3.Text = "I am in cell3"
        elif sender.Text == "button4":
            self.label4.Text = "I am in cell4"


# run application
form = TableLayoutPanelSample()
Application.EnableVisualStyles()
Application.Run(form)
