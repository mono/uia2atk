#!/usr/bin/env ipy

##############################################################################
# Written by:  Ray Wang <rawang@novell.com>
# Date:        06/23/2008
# Description: the sample for winforms control:
#              TabControl
#              TabPage
##############################################################################

# The docstring below is used in the generated log file
"""
Test accessibility of "TabPage" control
"""

# imports
import clr
clr.AddReference('System.Windows.Forms')
from System.Windows.Forms import Application, Form, TabPage, TabControl

class TabControlTabPageSample(Form):
    """TabControlTabPage control class"""

    def __init__(self):
        """TabControlTabPageSample class init function."""

        # setup title
        self.Text = "TabControl & TabPage control"

        # setup tabcontrol
        self.tabcontrol = TabControl()
        self.tabcontrol.Width = 285
        self.tabcontrol.Height = 265

        # setup tabpage
        for i in range(5):
            self.tabpage = TabPage()
            self.tabpage.Text = "Tab %s" % i

            # add controls
            self.tabcontrol.TabPages.Add(self.tabpage)

        # add controls
        self.Controls.Add(self.tabcontrol)

# run application
form = TabControlTabPageSample()
Application.EnableVisualStyles()
Application.Run(form)
