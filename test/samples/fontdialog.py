#!/usr/bin/env ipy
# -*- coding: utf-8 -*-

##############################################################################
# Written by:  Ray Wang <rawang@novell.com>
# Date:        Jun 16, 2008
# Description: the sample for winforms control:
#              FontDialog
##############################################################################

# The docstring below is used in the generated log file
"""
Test accessibility of "FontDialog" control
"""

# imports
import clr
clr.AddReference('System.Windows.Forms')
clr.AddReference('System.Drawing')

from System.Windows.Forms import Application, Button, Form, FontDialog, Label, DialogResult
from System.Drawing import Point

class FontDialogSample(Form):
    """FontDialog control class"""

    def __init__(self):
        """FontDialogSample class init function."""

        # setup title
        self.Text = "FontDialog control test sample"

        # setup label
        self.label = Label()
        self.label.Text = "Click the button below, select a style, " + \
                          "and then you could see the difference. :)"
        self.label.Location = Point(25, 25)
        self.label.Width = 200 
        self.label.Height = 100 

        # setup button
        self.button = Button()
        self.button.Text = "Click me"
        self.button.Location = Point(25, 125)
        self.button.Click += self.openDialog

        # add controls
        self.Controls.Add(self.label)
        self.Controls.Add(self.button)

    def openDialog(self, sender, args):
        """open a FontDialog dialog"""

        # preserve the previous label font to FontDialog dialog.
        self.FontDialog = FontDialog()
        self.FontDialog.ShowColor = True
        self.FontDialog.Font = self.label.Font
        self.FontDialog.Color = self.label.ForeColor
        
        # set up the label font which select in FontDialog dialog.
        if (self.FontDialog.ShowDialog() == DialogResult.OK):
            self.label.Font = self.FontDialog.Font
            self.label.ForeColor = self.FontDialog.Color

# run application
form = FontDialogSample()
Application.Run(form)
