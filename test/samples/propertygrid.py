#!/usr/bin/env ipy

##############################################################################
# Written by:  Ray Wang <rawang@novell.com>
# Date:        07/01/2008
# Description: the sample for winforms control:
#              PropertyGrid
##############################################################################

# The docstring below is used in the generated log file
"""
Test accessibility of "PropertyGrid" control
"""

# imports
import clr
clr.AddReference('System.Windows.Forms')
from System.Windows.Forms import (
    Application, Form, Label, PropertyGrid, DockStyle
)

class PropertyGridSample(Form):
    """PropertyGrid control class"""

    def __init__(self):
        """PropertyGridSample class init function."""

        # setup title
        self.Text = "PropertyGrid control"
        self.Height = 460

        # setup labels
        self.label = Label()
        self.label.Dock = DockStyle.Top
        self.label.Text = "The Property Grid of Label control"

        # setup propertygrid
        self.propertygrid = PropertyGrid()
        self.propertygrid.CommandsVisibleIfAvailable = True
        self.propertygrid.Dock = DockStyle.Top
        self.propertygrid.TabIndex = 1
        self.propertygrid.Text = "Property Grid"
        self.propertygrid.SelectedObject = self.label

        # add controls
        self.Controls.Add(self.propertygrid)
        self.Controls.Add(self.label)


# run application
form = PropertyGridSample()
Application.EnableVisualStyles()
Application.Run(form)
