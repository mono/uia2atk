#!/usr/bin/env ipy

##############################################################################
# Written by:  Calen Chen <cachen@novell.com>
# Date:        03/11/2008
# Description: This is a test application sample for winforms control:
#              StatusBar
#              StatusBarPanel
##############################################################################

import clr
import System

clr.AddReference('System.Windows.Forms')
clr.AddReference('System.Drawing')
clr.AddReference('System')

from System.Windows.Forms import *
from System.Drawing import *
from System import *


class StatusBarStatusBarPanelApp(Form):

    def __init__(self):
        self.Text = "Simple StatusBar&StatusBarPanel Example"
        self.Width = 400
        self.Height = 400
        self.FormBorderStyle = FormBorderStyle.Fixed3D

        self.mainLabel1 = Label()
        self.mainLabel1.Text = "Examples for: StatusBar and StatusBarPanel."
        self.mainLabel1.Location = Point(10,10)
        self.mainLabel1.AutoSize = True
        self.Controls.Add(self.mainLabel1)

        self.button1 = Button()
        self.button1.Text = "ToolTip button"
        self.button1.Location = Point(10,80)
        self.button1.AutoSize = True
        self.Controls.Add(self.button1)
        
        self.checkbox1 = CheckBox()
        self.checkbox1.Text = "Grape"
        self.checkbox1.Location = Point(10,140)
        self.checkbox1.AutoSize = True
        self.Controls.Add(self.checkbox1)

##set ToolTip and link to button1 and checkbox1
        self.tooltip1 = ToolTip()
        self.tooltip1.AutoPopDelay = 5000
        self.tooltip1.InitialDelay = 300
        self.tooltip1.ReshowDelay = 100
        self.tooltip1.ShowAlways = True
        self.tooltip1.SetToolTip(self.button1, "show button's tooltip")
        self.tooltip1.SetToolTip(self.checkbox1,"my favorite fruit")

##set StatusBar and StatusBarPanel. in statusbar1 add statusbarpanel1 displays status text for an application, statusbarpanel2 displays the current date and uses the ToolTipText property to display the current time. and add the panels to the statusbar1.
        self.statusbar1 = StatusBar()
        self.statusbarpanel1 = StatusBarPanel()
        self.statusbarpanel2 = StatusBarPanel()
        self.statusbarpanel1.BorderStyle = StatusBarPanelBorderStyle.Sunken
        #self.statusbarpanel1.Text = "Ready..."
        self.statusbarpanel1.Text = "Started: " + System.DateTime.Now.ToShortTimeString()
        self.statusbarpanel1.AutoSize = StatusBarPanelAutoSize.Spring
        self.statusbarpanel2.BorderStyle = StatusBarPanelBorderStyle.Raised

        self.statusbarpanel2.Text = System.DateTime.Today.ToLongDateString()
        self.statusbarpanel2.AutoSize = StatusBarPanelAutoSize.Contents
        self.statusbar1.ShowPanels = True
        self.statusbar1.Panels.Add(self.statusbarpanel1)
        self.statusbar1.Panels.Add(self.statusbarpanel2)
        self.Controls.Add(self.statusbar1)


form = StatusBarStatusBarPanelApp()
Application.Run(form)

