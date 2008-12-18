#!/usr/bin/env ipy

##############################################################################
# Written by:  Ray Wang <rawang@novell.com>
# Date:        08/07/2008
# Description: the sample for winforms control:
#              DateTimePicker
##############################################################################

# The docstring below is used in the generated log file
"""
This sample will show "DateTimePicker" control in the form.
It can be used for Autotest tools(e.g. Strongwind) to test the behaviors of controls.
"""

# imports
import clr
clr.AddReference('System.Windows.Forms')
clr.AddReference('System.Drawing')

from System.Windows.Forms import (
    Application, Form, Label, DateTimePicker, DateTimePickerFormat
)
from System.Drawing import Point, Size

class DateTimePickerSample(Form):
    """DateTimePicker control class"""

    def __init__(self):
        """DateTimePickerSample class init function."""

        # set up form
        self.Text = "DateTimePicker control"

        # set up label
        self.label = Label()
        self.label.Text = "The Date you select is:"
        self.label.Size = Size(260, 30)
        self.label.Location = Point(0, 50)

        # set up domainupdown
        self.date_time_picker = DateTimePicker()
        self.date_time_picker.CustomFormat = "MMMM dd, yyyy - dddd"
        self.date_time_picker.Format = DateTimePickerFormat.Custom
        self.date_time_picker.ShowUpDown = True
        self.date_time_picker.ShowCheckBox = True
        self.date_time_picker.Width = self.Width - 10
        self.date_time_picker.ValueChanged += self.dt_value_changed

        # add controls
        self.Controls.Add(self.label)
        self.Controls.Add(self.date_time_picker)

    def dt_value_changed(self, sender, event):
        self.label.Text = "The Date you select is: %s" % \
                                            self.date_time_picker.Value

# run application
form = DateTimePickerSample()
Application.EnableVisualStyles()
Application.Run(form)
