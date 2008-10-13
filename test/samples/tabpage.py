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
This sample will show "TabControl" and "TabPage"  control in the form.
It can be used for Autotest tools(e.g. Strongwind) to test the behaviors of controls.
"""

# imports
import clr
clr.AddReference('System.Windows.Forms')
from System.Windows.Forms import (
    Application, Form, TabPage, TabControl, StatusBar, StatusBarPanel, 
    StatusBarPanelBorderStyle, StatusBarPanelAutoSize
)

class TabControlTabPageSample(Form):
    """TabControlTabPage control class"""

    def __init__(self):
        """TabControlTabPageSample class init function."""

        # setup title
        self.Text = "TabControl & TabPage control"

        # setup tabcontrol
        self.tabcontrol = TabControl()
        self.tabcontrol.Width = 260
        self.tabcontrol.Height = 240

        # setup tabpage
        for i in range(5):
            self.tabpage = TabPage()
            self.tabpage.Text = "Tab %s" % i
            self.tabpage.Enter += self.on_click

            # add controls
            self.tabcontrol.TabPages.Add(self.tabpage)

        # setup status bar
        self.statusbar = StatusBar()
        self.statusbar_panel = StatusBarPanel()
        self.statusbar_panel.BorderStyle = StatusBarPanelBorderStyle.Sunken
        self.statusbar_panel.Text = "Select a Tab" 
        self.statusbar_panel.AutoSize = StatusBarPanelAutoSize.Spring
        self.statusbar.ShowPanels = True

        # add controls
        self.statusbar.Panels.Add(self.statusbar_panel)
        self.Controls.Add(self.statusbar)
        self.Controls.Add(self.tabcontrol)

    def on_click(self, sender, event):
        self.statusbar_panel.Text = "The preview tab is: " + sender.Text

# run application
form = TabControlTabPageSample()
Application.EnableVisualStyles()
Application.Run(form)
