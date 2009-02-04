#!/usr/bin/env ipy

##############################################################################
# Written by:  Ray Wang <rawang@novell.com>
# Date:        07/30/2008
# Description: the sample for winforms control:
#              ToolBar
#              ToolBarButton
##############################################################################

# The docstring below is used in the generated log file
"""
This sample will show "ToolBar" and "ToolBarButton" controls in the form.
It can be used for Autotest tools(e.g. Strongwind) to test the behaviors of controls.
"""

# imports
import os
from sys import path
from os.path import exists

import clr
clr.AddReference('System.Windows.Forms')
clr.AddReference('System.Drawing')

from System.Windows.Forms import *
from System.Drawing import *
import System

harness_dir = path[0]
i = harness_dir.rfind("/")
uiaqa_path = harness_dir[:i]

class ToolBarSample(Form):
    """ToolBar control class"""

    def __init__(self):
        """ToolBarSample class init function."""

        # setup form
        self.Text = "ToolBar control"

        # setup label
        self.label = Label()
        self.label.Text = "ToolBar and ToolBarButton example"
        self.label.AutoSize = True
        self.label.Height = 200 
        self.label.Width = self.Width - 10
        self.label.Location = Point (10, 80)

        # Create and initialize the ToolBar and ToolBarButton controls.
        self.toolbar = ToolBar()
        self.toolbar.ButtonClick += self.on_click

        # image list
        self.imagelist = ImageList()
        self.imagelist.ColorDepth = ColorDepth.Depth32Bit;
        self.imagelist.ImageSize = Size(32, 32)

        # small images
        names = [
                "abiword_48.png",
                "bmp.png",
                "disks.png",
                "evolution.png"
            ]

        for i in names:
            self.imagelist.Images.Add (Image.FromFile("%s/samples/listview-items-icons/32x32/" % uiaqa_path + i))

        self.toolbar.ImageList = self.imagelist

        # setup toolbarbuttons
        self.toolbar_btn1 = ToolBarButton()
        self.toolbar_btn2 = ToolBarButton()
        self.toolbar_btn3 = ToolBarButton()
        self.toolbar_btn4 = ToolBarButton()
        self.toolbar_btn1.Text = "Open"
        self.toolbar_btn2.Text = "Save"
        self.toolbar_btn3.Text = "Print"
        self.toolbar_btn4.Text = "nop"
        self.toolbar_btn1.ImageIndex = 0
        self.toolbar_btn2.ImageIndex = 1
        self.toolbar_btn3.ImageIndex = 2
        self.toolbar_btn4.ImageIndex = 3
        
        # create dialogs
        self.openfiledialog = OpenFileDialog()
        self.savefiledialog = SaveFileDialog()
        self.printdialog = PrintDialog()

        # add controls
        self.toolbar.Buttons.Add(self.toolbar_btn1)
        self.toolbar.Buttons.Add(self.toolbar_btn2)
        self.toolbar.Buttons.Add(self.toolbar_btn3)
        self.toolbar.Buttons.Add(self.toolbar_btn4)
        self.Controls.Add(self.toolbar)
        self.Controls.Add(self.label)

    def on_click(self, sender, event):
        btn = self.toolbar.Buttons.IndexOf(event.Button)
        if btn == 0:
            if self.openfiledialog.ShowDialog() == DialogResult.OK:
                self.label.Text = self.openfiledialog.FileName
        elif btn == 1:
            if self.savefiledialog.ShowDialog() == DialogResult.OK:
                self.label.Text = self.savefiledialog.FileName
        elif btn == 2:
            self.printdialog.ShowDialog()

# run application
form = ToolBarSample()
Application.EnableVisualStyles()
Application.Run(form)
