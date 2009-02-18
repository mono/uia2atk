#!/usr/bin/env ipy

##############################################################################
# Written by:  Calen Chen <cachen@novell.com>
# Date:        02/18/2009
# Description: the sample for winforms control:
#              NotifyIcon
##############################################################################

# The docstring below is used in the generated log file
"""
This sample will show "NotifyIcon" control in the form.
It can be used for Autotest tools(e.g. Strongwind) to test the behaviors of controls.
"""

# imports
import clr
import System

clr.AddReference('System.Windows.Forms')
clr.AddReference('System.Drawing')
clr.AddReference('System')

from System.Windows.Forms import *
from System.Drawing import *
from System import *

class NotifyIconSample(Form):
    """NotifyIcon control class"""

    def __init__(self):
        """NotifyIconSample class init function."""

        # setup label
        self.label = Label()
        self.label.Text = "Please click button to rise notifyicon or balloon"
        self.label.Location = Point(10,50)
        self.label.AutoSize = True
        self.Controls.Add(self.label)
        
        # setup title
        self.Text = "NotifyIcon control"

        #click button to rise notifyicon
        self.button = Button()
        self.button.Text = "notifyicon"
        self.button.Location = Point(10,100)
        self.button.Click += self.on_tick
        self.Controls.Add(self.button)

        #click button1 to rise notifyicon_balloon
        self.button1 = Button()
        self.button1.Text = "balloon"
        self.button1.Location = Point(10,150)
        self.button1.Click += self.on_tick
        self.Controls.Add(self.button1)

        # setup notifyicon with "Quit" menuitem
        self.contextmenu = ContextMenu()
        self.menuitem = MenuItem()
        self.menuitem.Index = 0
        self.menuitem.Text = "Quit"
        self.menuitem.Click += self.menuitem_click
        self.contextmenu.MenuItems.Add(self.menuitem)

        self.notifyicon = NotifyIcon()
        self.notifyicon.Icon = SystemIcons.Exclamation
        self.notifyicon.ContextMenu = self.contextmenu
        self.notifyicon.Text = "Form"
        self.notifyicon.DoubleClick += self.notifyicon_doubleclick

        # setup balloon tip
        self.notifyicon1 = NotifyIcon()
        self.notifyicon1.BalloonTipTitle = "Hello"
        self.notifyicon1.BalloonTipText = "I'm NotifyIcon"
        self.notifyicon1.BalloonTipIcon = ToolTipIcon.Error


    def on_tick(self, sender, event):
        if sender == self.button:
            self.notifyicon.Visible = True
        elif sender == self.button1:
            self.notifyicon1.Visible = True
            self.notifyicon1.ShowBalloonTip(1000)
            self.notifyicon.Visible = False

    def notifyicon_doubleclick(self, sender, event):
        self.Close()

    def menuitem_click(self, sender, event):
        self.Close()


# run application
form = NotifyIconSample()
Application.EnableVisualStyles()
Application.Run(form)
