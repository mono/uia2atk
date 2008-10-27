#!/usr/bin/env ipy

##############################################################################
# Written by:  Calen Chen <cachen@novell.com>
# Date:        10/22/2008
# Description: This is a test application sample for winforms control:
#              LinkLabel
##############################################################################

# The docstring below is used in the generated log file
"""
This sample will show "LinkLabel" controls in the form.
It can be used for Autotest tools(e.g. Strongwind) to test the behaviors of controls.
"""


import clr
import os
from sys import path
from os.path import exists

clr.AddReference('System.Windows.Forms')
clr.AddReference('System.Drawing')

from System.Drawing import *
from System.Windows.Forms import *
import System

class RunApp(Form):
    """LinkLabel control class"""

    def __init__(self):
        """RunApp class init function."""

        self.count = 0
        self.Text = "LinkLabel control"
        self.Height = 200
        self.Width = 350

        # set up LinkLabel control
        self.linklabel1 = LinkLabel()
        self.linklabel1.Location = Point(10,20)
        self.linklabel1.AutoSize = True
        self.linklabel1.Name = "www"
        self.linklabel1.Text = "openSUSE:www.opensuse.org   webmail:gmail.novell.com"
        self.linklabel1.Links.Add(9, 16, "www.opensuse.org")
        self.linklabel1.Links.Add(33, 19, "gmail.novell.com")
        #self.linklabel1.LinkClicked += self.linklabel_clicked
        self.linklabel1.Links[1].Enabled = False
        #self.linklabel1.LinkArea = LinkArea(9, 28)
        #self.linklabel1.Links[0].LinkData = "www.opensuse.org"
        self.linklabel1.LinkClicked += self.linklabel1_clicked

        # set up LinkLabel control
        self.linklabel2 = LinkLabel()
        self.linklabel2.Location = Point(10,70)
        self.linklabel2.AutoSize = True
        self.linklabel2.Name = "calculator"
        self.linklabel2.Text = "calculator:"

        AddLinkString = "/usr/bin/gcalctool"
        LinkIndex = self.linklabel2.Text.Length
        self.linklabel2.Text += AddLinkString

        #self.linklabel2.Links.Add(10, 28, "/usr/bin/gcalctool")
        self.linklabel2.Links.Add(LinkIndex, (AddLinkString.Length), "/usr/bin/gcalctool")
        self.linklabel2.LinkClicked += self.linklabel2_clicked
        self.linklabel2.Links[0].Enabled = True

        # set up LinkLabel control
        self.linklabel3 = LinkLabel()
        self.linklabel3.Location = Point(10,100)
        self.linklabel3.AutoSize = True
        self.linklabel3.Name = "gmail"
        self.linklabel3.Text = "gmail:gmail.novell.com"
        self.linklabel3.Links.Add(6, 20, "gmail.novell.com")
        self.linklabel3.LinkClicked += self.linklabel3_clicked
        self.linklabel3.Links[0].Enabled = False

        self.Controls.Add(self.linklabel1)
        self.Controls.Add(self.linklabel2)
        self.Controls.Add(self.linklabel3)

    def linklabel1_clicked(self, sender, LinkClicked):
        self.linklabel1.Links[0].Visited = True
        target1 = self.linklabel1.Links[0].LinkData

        System.Diagnostics.Process.Start(target1)

    def linklabel2_clicked(self, sender, LinkClicked):
        self.linklabel2.Links[0].Visited = True
        target1 = self.linklabel2.Links[0].LinkData

        if (target1.StartsWith("/usr")):
            System.Diagnostics.Process.Start(target1)
        else:
            MessageBox.Show("Item clicked: " + target1)

    def linklabel3_clicked(self, sender, LinkClicked):
        self.linklabel3.Links[0].Visited = True
        target1 = self.linklabel3.Links[0].LinkData

        if (target1.StartsWith("gmail")):
            System.Diagnostics.Process.Start(target1)
        else:
            MessageBox.Show("Item clicked: " + target1)

form = RunApp()
Application.Run(form)

