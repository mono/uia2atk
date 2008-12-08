#!/usr/bin/env ipy

##############################################################################
# Written by:  Ray Wang <rawang@novell.com>
# Date:        08/06/2008
# Description: the sample for winforms control:
#              DomainUpDown
##############################################################################

# The docstring below is used in the generated log file
"""
This sample will show "DomainUpDown" control in the form.
It can be used for Autotest tools(e.g. Strongwind) to test the behaviors of controls.
"""

# imports
import clr
clr.AddReference('System.Windows.Forms')
clr.AddReference('System.Drawing')

from System.Windows.Forms import Application, Form, Label, DomainUpDown
from System.Drawing import Point, Size

class DomainUpDownSample(Form):
    """DomainUpDown control class"""

    def __init__(self):
        """DomainUpDownSample class init function."""

        # set up form
        self.Text = "DomainUpDown control"

        # set up label
        #self.label = Label()
        #self.label.Size = Size(250, 30)
        #self.label.Text = "Up/Down DomainUpDown control to see the items"
        #self.label.Location = Point(10, 0)

        # set up domainupdown
        self.domain_up_down = DomainUpDown()
        self.domain_up_down.Location = Point(10, 50)
        self.domain_up_down.Sorted = True
        self.domain_up_down.Items.Add("Austin")
        self.domain_up_down.Items.Add("Beijing")
        self.domain_up_down.Items.Add("Cambridge")
        self.domain_up_down.Items.Add("Madrid")
        self.domain_up_down.Items.Add("Provo")
        self.domain_up_down.Items.Add("San Diego")
    #    self.domain_up_down.SelectedItemChanged += self.selected_item_changed

        # ReadOnly DomainUpDown
        self.readonly_domain_up_down = DomainUpDown()
        self.readonly_domain_up_down.Location = Point(10, 100)
        self.readonly_domain_up_down.Sorted = True
        self.readonly_domain_up_down.Items.Add("Austin")
        self.readonly_domain_up_down.Items.Add("Beijing")
        self.readonly_domain_up_down.Items.Add("Cambridge")
        self.readonly_domain_up_down.Items.Add("Madrid")
        self.readonly_domain_up_down.Items.Add("Provo")
        self.readonly_domain_up_down.Items.Add("San Diego")
    #    self.readonly_domain_up_down.SelectedItemChanged += self.selected_item_changed
        self.readonly_domain_up_down.ReadOnly = True

        # add control
        #self.Controls.Add(self.label)
        self.Controls.Add(self.readonly_domain_up_down)
        self.Controls.Add(self.domain_up_down)

    #def selected_item_changed(self, sender, event):
    #    self.label.Text = "SelectedIndex: " + \
    #        str(sender.SelectedIndex) + \
    #        "\n" + \
    #        "SelectedItem: "  + \
    #        str(sender.SelectedItem)

# run application
form = DomainUpDownSample()
Application.EnableVisualStyles()
Application.Run(form)
