#!/usr/bin/env ipy

##############################################################################
# Written by:  Ray Wang <rawang@novell.com>
# Date:        06/30/2008
# Description: the sample for winforms control:
#              TableLayoutPanel
##############################################################################

# Since we do not typically use the ScrollableControl class directly. 
# The TableLayoutPanel and Panel classes inherit from this class.
# So we implement TableLayoutPanel to indicate the features of ScrollableControl

# The docstring below is used in the generated log file
"""
Test accessibility of "TableLayoutPanel" control
"""

# imports
import clr
clr.AddReference('System.Windows.Forms')
clr.AddReference('System.Drawing')
from System.Windows.Forms import (
    Application, Form, Label, TableLayoutPanel, DockStyle, ColumnStyle, RowStyle, SizeType, BorderStyle
)
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
        self.tablelayoutpanel.ColumnCount = 2
        self.tablelayoutpanel.RowCount = 2
        self.tablelayoutpanel.ColumnStyles.Add(ColumnStyle(SizeType.Percent, 100.0))
        self.tablelayoutpanel.ColumnStyles.Add(ColumnStyle(SizeType.Percent, 100.0))
        self.tablelayoutpanel.RowStyles.Add(RowStyle(SizeType.Percent, 100.0))
        self.tablelayoutpanel.RowStyles.Add(RowStyle(SizeType.Percent, 100.0))

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
        self.label1.Dock = DockStyle.Fill
        self.label2.Dock = DockStyle.Fill
        self.label3.Dock = DockStyle.Fill
        self.label4.Dock = DockStyle.Fill

        # add controls
        # Controls.add (control, column, row)
        self.tablelayoutpanel.Controls.Add(self.label1, 0, 0)
        self.tablelayoutpanel.Controls.Add(self.label2, 0, 1)
        self.tablelayoutpanel.Controls.Add(self.label3, 1, 0)
        self.tablelayoutpanel.Controls.Add(self.label4, 1, 1)
        self.Controls.Add(self.tablelayoutpanel)


# run application
form = TableLayoutPanelSample()
Application.EnableVisualStyles()
Application.Run(form)
