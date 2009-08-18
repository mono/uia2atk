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
        self.ClientSize = Size(600, 500)

        # Description label
        self.dscpt_label = Label()
        self.dscpt_label.Location = Point(10, 0)
        self.dscpt_label.Size = Size(450, 50)
        self.dscpt_label.BorderStyle = BorderStyle.Fixed3D
        self.dscpt_label.Text = "If you leave empty in one textbox and switch focus to another textbox, an icon will appear beside the first textbox. Move mouse over the icon and a tooltip appears."

        # Name label
        self.name_label = Label()
        self.name_label.Location = Point(30, 62)
        self.name_label.AutoSize = True
        self.name_label.Text = "Name:"

        # ErrorBlinkStyle.AlwaysBlink Label
        self.blink_style_label0 = Label()
        self.blink_style_label0.Location = Point(300, 62);
        self.blink_style_label0.Size = Size(160, 23)
        self.blink_style_label0.Text = "AlwaysBlink"

        # ErrorBlinkStyle.AlwaysBlink Label
        self.blink_style_label1 = Label()
        self.blink_style_label1.Location = Point(300, 94);
        self.blink_style_label1.Size = Size(160, 23)
        self.blink_style_label1.Text = "BlinkIfDifferentError"

	# ErrorBlinkStyle.AlwaysBlink Label
        self.blink_style_label2 = Label()
        self.blink_style_label2.Location = Point(300, 126);
        self.blink_style_label2.Size = Size(160, 23)
        self.blink_style_label2.Text = "NeverBlink"

        # age Label
        self.age_label = Label()
        self.age_label.Location = Point(30, 94)
        self.age_label.AutoSize = True
        self.age_label.Text = "Age:"

        # age Label
        self.weight_label = Label()
        self.weight_label.Location = Point(30, 126)
        self.age_label.AutoSize = True
        self.weight_label.Text = "Weight:"

        # height Label
        self.height_label = Label()
        self.height_label.Location = Point(30, 158)
        self.age_label.AutoSize = True
        self.height_label.Text = "Height:"

        # depth Label
        self.depth_label = Label()
        self.depth_label.Location = Point(30, 190)
        self.age_label.AutoSize = True
        self.depth_label.Text = "Depth:"

        # Name TextBox
        self.name_textbox = TextBox()
        self.name_textbox.Location = Point(152, 62)
        self.name_textbox.Size = Size(120, 20)
        self.name_textbox.TabIndex = 0
        self.name_textbox.Validated += self.validate_name

        # age TextBox
        self.age_textbox = TextBox()
        self.age_textbox.Location = Point(152, 94)
        self.age_textbox.Size = Size(120, 20)
        self.age_textbox.TabIndex = 1

        # weight TextBox
        self.weight_textbox = TextBox()
        self.weight_textbox.Location = Point(152, 126)
        self.weight_textbox.Size = Size(120, 20)
        self.weight_textbox.TabIndex = 2
        self.weight_textbox.Validated += self.validate_weight

        # height TextBox
        self.height_textbox = TextBox()
        self.height_textbox.Location = Point(152, 158)
        self.height_textbox.Size = Size(120, 20)
        self.height_textbox.TabIndex = 3
        self.height_textbox.Validated += self.validate_height

        # depth TextBox
        self.depth_textbox = TextBox()
        self.depth_textbox.Location = Point(152, 190)
        self.depth_textbox.Size = Size(120, 20)
        self.depth_textbox.TabIndex = 4
        self.depth_textbox.Validated += self.validate_depth

        self.age_textbox.Validated += self.validate_age

        # Create and set the ErrorProvider for each data entry control.
        self.name_error_provider = ErrorProvider()
        self.name_error_provider.SetIconAlignment(self.name_textbox, 
                                                ErrorIconAlignment.TopRight)
        self.name_error_provider.SetIconPadding (self.name_textbox, 2)
        self.name_error_provider.BlinkRate = 1000
        self.name_error_provider.BlinkStyle = ErrorBlinkStyle.AlwaysBlink

        self.age_error_provider = ErrorProvider()
        self.age_error_provider.SetIconAlignment(self.age_textbox, 
                                                ErrorIconAlignment.TopLeft)
        self.age_error_provider.SetIconPadding (self.age_textbox, 2)
        self.age_error_provider.BlinkRate = 500
        self.age_error_provider.BlinkStyle = \
                                         ErrorBlinkStyle.BlinkIfDifferentError

        self.weight_error_provider = ErrorProvider()
        self.weight_error_provider.SetIconAlignment(self.weight_textbox, 
                                                ErrorIconAlignment.MiddleRight)
        self.weight_error_provider.SetIconPadding (self.weight_textbox, 2)
        self.weight_error_provider.BlinkRate = 1000
        self.weight_error_provider.BlinkStyle = \
                                         ErrorBlinkStyle.NeverBlink

        self.height_error_provider = ErrorProvider()
        self.height_error_provider.SetIconAlignment(self.height_textbox, 
                                                ErrorIconAlignment.MiddleLeft)
        self.height_error_provider.SetIconPadding (self.height_textbox, 2)
        self.height_error_provider.BlinkStyle = \
                                         ErrorBlinkStyle.NeverBlink

        self.depth_error_provider = ErrorProvider()
        self.depth_error_provider.SetIconAlignment(self.depth_textbox, 
                                                ErrorIconAlignment.BottomRight)
        self.depth_error_provider.SetIconPadding (self.depth_textbox, 2)
        self.depth_error_provider.BlinkStyle = \
                                         ErrorBlinkStyle.NeverBlink

        # add controls
        self.Controls.Add(self.dscpt_label)
        self.Controls.Add(self.name_label)
        self.Controls.Add(self.age_label)
        self.Controls.Add(self.weight_label)
        self.Controls.Add(self.height_label)
        self.Controls.Add(self.depth_label)
        self.Controls.Add(self.blink_style_label0)
        self.Controls.Add(self.blink_style_label1)
        self.Controls.Add(self.blink_style_label2)
        self.Controls.Add(self.name_textbox)
        self.Controls.Add(self.age_textbox)
        self.Controls.Add(self.weight_textbox)
        self.Controls.Add(self.height_textbox)
        self.Controls.Add(self.depth_textbox)

    def validate_name(self, sender, event):
        if self.is_text_valid(sender):
            self.name_error_provider.SetError(self.name_textbox, "")
        else:
            self.name_error_provider.SetError(self.name_textbox, 
                                              "Name required")

    def validate_weight(self, sender, event):
        if self.is_text_valid(sender):
            self.weight_error_provider.SetError(self.weight_textbox, "")
        else:
            self.weight_error_provider.SetError(self.weight_textbox, 
                                              "Weight required")

    def validate_height(self, sender, event):
        if self.is_text_valid(sender):
            self.height_error_provider.SetError(self.height_textbox, "")
        else:
            self.height_error_provider.SetError(self.height_textbox, 
                                              "Height required")

    def validate_age(self, sender, event):
        if self.is_text_valid(sender):
            self.age_error_provider.SetError(self.age_textbox, "")
        else:
            self.age_error_provider.SetError(self.age_textbox, 
                                              "Age required")
    
    def validate_depth(self, sender, event):
        if self.is_text_valid(sender):
            self.depth_error_provider.SetError(self.depth_textbox, "")
        else:
            self.depth_error_provider.SetError(self.depth_textbox, 
                                              "Depth required")

    def is_text_valid(self, sender):
        return (sender.Text.Length > 0)

# run application
form = ErrorProviderSample()
Application.EnableVisualStyles()
Application.Run(form)
