#!/usr/bin/env ipy

##############################################################################
# Written by:  Ray Wang <rawang@novell.com>
# Date:        07/24/2008
# Description: the sample for winforms control:
#              PrintPreviewControl
##############################################################################

# The docstring below is used in the generated log file
"""
This sample will show "PrintPreviewControl" control in the form.
It can be used for Autotest tools(e.g. Strongwind) to test the behaviors of controls.
"""

# imports
import clr
clr.AddReference('System.Windows.Forms')
clr.AddReference('System.Drawing')

from System.Drawing import Size, Point, Font, FontStyle, Brushes
from System.Drawing.Printing import PrintDocument
from System.Windows.Forms import (
    Application, Button, Form, Label, PrintPreviewControl, DockStyle
)

class PrintPreviewControlSample(Form):
    """PrintPreviewControl control class"""

    def __init__(self):
        """PrintPreviewControlSample class init function."""

        # initialize a PrintDocument object
        doc_to_print = PrintDocument()

        # setup title
        self.Text = "PrintPreviewControl control"

        # setup button
        self.button = Button()
        self.button.Text = 'Button'
        self.button.Click += self.on_click

        # setup print_preview_control
        self.print_preview_control = PrintPreviewControl()
        self.print_preview_control.Text = "PrintPreviewPage"
        self.print_preview_control.Location = Point(30, 80)
        self.print_preview_control.Dock = DockStyle.Fill

        # Set the UseAntiAlias property to true so fonts are smoothed
        # by the operating system.
        self.print_preview_control.UseAntiAlias = True

        # Set the Document property to the PrintDocument 
        # for which the PrintPage event has been handled.
        self.print_preview_control.Document = doc_to_print

        #set Zoom to show scrollbar
        self.print_preview_control.Zoom = 0.5

        # Associate the event-handling method with the
        # document's PrintPage event.
        doc_to_print.PrintPage += self.doc_to_print_PrintPage

        # add controls
        self.Controls.Add(self.button)

    def doc_to_print_PrintPage(self, sender, event):
        # Insert code to render the page here.
        # This code will be called when the control is drawn.
        # The following code will render a simple
        # message on the document in the control.
        text = "In doc_to_print_PrintPage method."
        print_font = Font("Arial", 10, FontStyle.Bold)
        event.Graphics.DrawString(text, print_font, Brushes.Black, 10, 10)

    def on_click(self, sender, event):
        self.Controls.Add(self.print_preview_control)
        self.print_preview_control.Show()

# run application
form = PrintPreviewControlSample()
Application.EnableVisualStyles()
Application.Run(form)
