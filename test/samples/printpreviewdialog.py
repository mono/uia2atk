#!/usr/bin/env ipy

##############################################################################
# Written by:  Ray Wang <rawang@novell.com>
# Date:        07/02/2008
# Description: the sample for winforms control:
#              PrintPreviewDialog
##############################################################################

# The docstring below is used in the generated log file
"""
Test accessibility of "PrintPreviewDialog" control
"""

# imports
import clr
clr.AddReference('System.Windows.Forms')
clr.AddReference('System.Drawing')
from System.Windows.Forms import (
    Application, Button, Form, Label, PrintPreviewDialog, DockStyle
)
from System.Drawing import Size, Point

class PrintPreviewDialogSample(Form):
    """PrintPreviewDialog control class"""

    def __init__(self):
        """PrintPreviewDialogSample class init function."""

        # setup title
        self.Text = "PrintPreviewDialog control"

        # setup button
        self.button = Button()
        self.button.Text = "PrintPreviewDialog"
        self.button.Click += self.click
        self.button.Width = 150

        # setup printpreviewdialog
        self.printpreviewdialog = PrintPreviewDialog()
        self.printpreviewdialog.ClientSize = Size(500, 300)
        self.printpreviewdialog.Location = Point(29, 29)
        self.printpreviewdialog.MinimumSize = Size(375, 250)
        self.printpreviewdialog.UseAntiAlias = True

        # add controls
        self.Controls.Add(self.button)

    def click(self, sender, event):
        self.printpreviewdialog.ShowDialog()

# run application
form = PrintPreviewDialogSample()
Application.EnableVisualStyles()
Application.Run(form)
