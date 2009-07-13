#!/usr/bin/env ipy

##############################################################################
# Written by:  Ray Wang <rawang@novell.com>
# Date:        07/01/2008
# Description: the sample for winforms control:
#              PropertyGrid
##############################################################################

# The docstring below is used in the generated log file
"""
This sample will show "PropertyGrid" control in the form.
It can be used for Autotest tools(e.g. Strongwind) to test the behaviors of 
controls.
"""

# imports
import clr
clr.AddReference('System.Windows.Forms')
clr.AddReference('System.Drawing')

from System.Windows.Forms import (
    Application, Form, Label, PropertyGrid, DockStyle, TextBox
)
from System.Drawing import * 

class PropertyGridSample(Form):
    """PropertyGrid control class"""

    def __init__(self):
        """PropertyGridSample class init function."""
        self.count = 0

        # setup title
        self.Text = "PropertyGrid control"
        self.Height = 500

        # setup text
        self.text = TextBox()
        #self.text.AutoSize = True
        self.text.Height = 100
        self.text.Dock = DockStyle.Top
        self.text.Text = "The Property Grid of text control"

        # setup propertygrid
        self.propertygrid = PropertyGrid()
        self.propertygrid.CommandsVisibleIfAvailable = True
        self.propertygrid.Dock = DockStyle.Top
        self.propertygrid.TabIndex = 1
        self.propertygrid.Text = "Property Grid"
        self.propertygrid.SelectedObject = self.text
        self.propertygrid.PropertyValueChanged += self.color_change

        # add controls
        self.Controls.Add(self.propertygrid)
        self.Controls.Add(self.text)

    def color_change(self, control, event):
        self.text.Text = "The color of label is: %s %s %s" % \
               (self.text.BackColor.R,self.text.BackColor.G,self.text.BackColor.B)


# run application
form = PropertyGridSample()
Application.EnableVisualStyles()
Application.Run(form)
