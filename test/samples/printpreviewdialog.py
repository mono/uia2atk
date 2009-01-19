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
from System.Drawing import Size, Point, Font, FontStyle, Brushes
from System.Drawing.Printing import PrintDocument

class PrintPreviewDialogSample(Form):
    """PrintPreviewDialog control class"""

    def __init__(self):
        """PrintPreviewDialogSample class init function."""
        # initialize a PrintDocument object
        self.doc_to_print = PrintDocument()

        # setup title
        self.Text = "PrintPreviewDialog control"

        # setup button
        self.button = Button()
        self.button.Text = "PrintPreviewDialog"
        self.button.Click += self.click
        self.button.Width = 150

        # setup printpreviewdialog
        self.printpreviewdialog = PrintPreviewDialog()
        self.printpreviewdialog.Text = "PrintPreviewDialog"
        self.printpreviewdialog.ClientSize = Size(500, 300)
        self.printpreviewdialog.Location = Point(29, 29)
        self.printpreviewdialog.MinimumSize = Size(375, 250)
        self.printpreviewdialog.UseAntiAlias = True
        self.printpreviewdialog.Document = self.doc_to_print
        self.doc_to_print.PrintPage += self.doc_to_print_PrintPage

        # add controls
        self.Controls.Add(self.button)

    def doc_to_print_PrintPage(self, sender, event):
        # Insert code to render the page here.
        # This code will be called when the control is drawn.
        # The following code will render a simple
        # message on the document in the control.
        text = "In doc_to_print_PrintPage method."
        print_font = Font("Arial", 20, FontStyle.Bold)
        event.Graphics.DrawString(text, print_font, Brushes.Black, 50, 10)

    def click(self, sender, event):
        self.printpreviewdialog.Document = self.doc_to_print
        self.printpreviewdialog.ShowDialog()

# run application
form = PrintPreviewDialogSample()
Application.EnableVisualStyles()
Application.Run(form)
