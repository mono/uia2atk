#!/usr/bin/env ipy

##############################################################################
# Written by:  Ray Wang <rawang@novell.com>
# Date:        09/01/2008
# Description: the sample for winforms control:
#              PageSetupDialog
##############################################################################

# The docstring below is used in the generated log file
"""
This sample will show "PageSetupDialog" control in the form.
It can be used for Autotest tools(e.g. Strongwind) to test the behaviors of controls.
"""

# imports
import clr
clr.AddReference('System.Windows.Forms')
clr.AddReference('System.Drawing')

from System.Windows.Forms import (
    Application, Button, Form, PageSetupDialog, DialogResult, ListBox, 
    BorderStyle
)
from System.Drawing import Point, Size
from System.Drawing.Printing import PageSettings, PrinterSettings

class PageSetupDialogSample(Form):
    """PageSetupDialog control class"""

    def __init__(self):
        """PageSetupDialogSample class init function."""

        # setup title
        self.Text = "PageSetupDialog control"

        # setup button
        self.button = Button()
        self.button.Text = "Click me"
        self.button.Location = Point(10, 210)
        self.button.Click += self.page_setup_dialog

        # setup listbox
        self.listbox = ListBox()
        self.listbox.Size = Size(self.Width - 10, 200)
        self.listbox.BorderStyle = BorderStyle.FixedSingle

        # add controls
        self.Controls.Add(self.listbox)
        self.Controls.Add(self.button)

    def page_setup_dialog(self, sender, event):
        """open a page_setup_dialog dialog"""

        # initialize page_setup_dialog
        self.page_setup_dialog = PageSetupDialog()
        self.page_setup_dialog.PageSettings = PageSettings()
        self.page_setup_dialog.PrinterSettings = PrinterSettings()

        # If the result is OK, display selected settings in ListBox. 
        # These values can be used when printing the document.
        if self.page_setup_dialog.ShowDialog() == DialogResult.OK:
            results = (
                "PageMargins: ", 
                self.page_setup_dialog.PageSettings.Margins, '',
                "PagerSize: ", 
                self.page_setup_dialog.PageSettings.PaperSize, '',
                "Landscape: ", 
                self.page_setup_dialog.PageSettings.Landscape, '',
                "PrinterName:", 
                self.page_setup_dialog.PrinterSettings.PrinterName, '',
                "PrintRange: ", 
                self.page_setup_dialog.PrinterSettings.PrintRange
                )
            self.listbox.Items.AddRange(results)

# run application
form = PageSetupDialogSample()
Application.EnableVisualStyles()
Application.Run(form)
