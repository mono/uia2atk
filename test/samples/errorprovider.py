#!/usr/bin/env ipy

##############################################################################
# Written by:  Ray Wang <rawang@novell.com>
# Date:        08/05/2008
# Description: the sample for winforms control:
#              ErrorProvider
##############################################################################

# The docstring below is used in the generated log file
"""
This sample will show "ErrorProvider" control in the form.
It can be used for Autotest tools(e.g. Strongwind) to test the behaviors of controls.
"""

# imports
import clr
clr.AddReference('System.Windows.Forms')
clr.AddReference('System.Drawing')

from System.Windows.Forms import (
    Application, BorderStyle, Form, Label, TextBox, ErrorProvider, ErrorIconAlignment, ErrorBlinkStyle
)
from System.Drawing import Point, Size

class ErrorProviderSample(Form):
    """ErrorProvider control class"""

    def __init__(self):
        """ErrorProviderSample class init function."""

        # set up form
        self.Text = "ErrorProvider control"
        self.ClientSize = Size(464, 150)

        # Description label
        self.dscpt_label = Label()
        self.dscpt_label.Location = Point(10, 0)
        self.dscpt_label.Size = Size(450, 50)
        self.dscpt_label.BorderStyle = BorderStyle.Fixed3D
        self.dscpt_label.Text = "If you leave empty in \"Name:\" textbox, and switch focus to \"Foobar\" textbox, then there will be a blink icon right besides to \"Name:\" textbox. Move mouse over the icon, the tip will be displayed."

        # Name label
        self.name_label = Label()
        self.name_label.Location = Point(56, 62)
        self.name_label.Size = Size(40, 23)
        self.name_label.Text = "Name:"

        # ErrorBlinkStyle.AlwaysBlink Label
        self.blink_label = Label()
        self.blink_label.Location = Point(264, 62);
        self.blink_label.Size = Size(160, 23)
        self.blink_label.Text = "ErrorBlinkStyle.AlwaysBlink"

        # foobar Label
        self.foo_label = Label()
        self.foo_label.Location = Point(56, 94)
        self.foo_label.Size = Size(40, 23)
        self.foo_label.Text = "Foobar:"

        # Name TextBox
        self.name_textbox = TextBox()
        self.name_textbox.Location = Point(112, 62)
        self.name_textbox.Size = Size(120, 20)
        self.name_textbox.TabIndex = 0
        self.name_textbox.Validated += self.validated

        # foobar TextBox
        self.foo_textbox = TextBox()
        self.foo_textbox.Location = Point(112, 94)
        self.foo_textbox.Size = Size(120, 20)

        # Create and set the ErrorProvider for each data entry control.
        self.name_error_provider = ErrorProvider()
        self.name_error_provider.SetIconAlignment(self.name_textbox, 
                                                ErrorIconAlignment.MiddleRight)
        self.name_error_provider.SetIconPadding (self.name_textbox, 2)
        self.name_error_provider.BlinkRate = 1000
        self.name_error_provider.BlinkStyle = ErrorBlinkStyle.AlwaysBlink

        # add controls
        self.Controls.Add(self.dscpt_label)
        self.Controls.Add(self.name_label)
        self.Controls.Add(self.foo_label)
        self.Controls.Add(self.blink_label)
        self.Controls.Add(self.name_textbox)
        self.Controls.Add(self.foo_textbox)

    def validated(self, sender, event):
        if self.is_name_valid():
            self.name_error_provider.SetError(self.name_textbox, "")
        else:
            self.name_error_provider.SetError(self.name_textbox, 
                                              "Name is required")

    def is_name_valid(self):
        return (self.name_textbox.Text.Length > 0)

# run application
form = ErrorProviderSample()
Application.EnableVisualStyles()
Application.Run(form)
