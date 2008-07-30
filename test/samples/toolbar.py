#!/usr/bin/env ipy

##############################################################################
# Written by:  Ray Wang <rawang@novell.com>
# Date:        07/30/2008
# Description: the sample for winforms control:
#              ToolBar
#              ToolBarButton
##############################################################################

# The docstring below is used in the generated log file
"""
This sample will show "ToolBar" and "ToolBarButton" controls in the form.
It can be used for Autotest tools(e.g. Strongwind) to test the behaviors of controls.
"""

# imports
import clr
clr.AddReference('System.Windows.Forms')
clr.AddReference('System.Drawing')

from System.Windows.Forms import (
    Application, Label, Form, ToolBar, ToolBarButton, OpenFileDialog, SaveFileDialog, PrintDialog, DialogResult
)
from System.Drawing import Point

class ToolBarSample(Form):
    """ToolBar control class"""

    def __init__(self):
        """ToolBarSample class init function."""

        # setup form
        self.Text = "ToolBar control"

        # setup label
        self.label = Label()
        self.label.Text = ""
        self.label.AutoSize = True
        self.label.Height = 200 
        self.label.Width = self.Width - 10
        self.label.Location = Point (10, 50)

        # Create and initialize the ToolBar and ToolBarButton controls.
        self.toolbar = ToolBar()
        self.toolbar.ButtonClick += self.on_click

        # setup toolbarbuttons
        self.toolbar_btn1 = ToolBarButton()
        self.toolbar_btn2 = ToolBarButton()
        self.toolbar_btn3 = ToolBarButton()
        self.toolbar_btn1.Text = "Open"
        self.toolbar_btn2.Text = "Save"
        self.toolbar_btn3.Text = "Print"
        
        # create dialogs
        self.openfiledialog = OpenFileDialog()
        self.savefiledialog = SaveFileDialog()
        self.printdialog = PrintDialog()

        # add controls
        self.toolbar.Buttons.Add(self.toolbar_btn1)
        self.toolbar.Buttons.Add(self.toolbar_btn2)
        self.toolbar.Buttons.Add(self.toolbar_btn3)
        self.Controls.Add(self.toolbar)
        self.Controls.Add(self.label)

    def on_click(self, sender, event):
        btn = self.toolbar.Buttons.IndexOf(event.Button)
        if btn == 0:
            if self.openfiledialog.ShowDialog() == DialogResult.OK:
                self.label.Text = self.openfiledialog.FileName
        elif btn == 1:
            if self.savefiledialog.ShowDialog() == DialogResult.OK:
                self.label.Text = self.savefiledialog.FileName
        elif btn == 2:
            self.printdialog.ShowDialog()

# run application
form = ToolBarSample()
Application.EnableVisualStyles()
Application.Run(form)
