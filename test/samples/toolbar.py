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
import System
from sys import path
from os.path import exists


import clr
clr.AddReference('System.Windows.Forms')
clr.AddReference('System.Drawing')

from System.Windows.Forms import *
from System.Drawing import *
from System.IO import *

harness_dir = path[0]
i = harness_dir.rfind(Path.DirectorySeparatorChar)
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
            self.imagelist.Images.Add (Image.FromFile(uiaqa_path + 
                                       Path.DirectorySeparatorChar + "samples" + 
                                       Path.DirectorySeparatorChar + "listview-items-icons" + 
                                       Path.DirectorySeparatorChar + "32x32" +
                                       Path.DirectorySeparatorChar + i))

        self.toolbar.ImageList = self.imagelist

        # setup toolbarbuttons
        self.toolbar_btn1 = ToolBarButton()
        self.toolbar_btn2 = ToolBarButton()
        self.toolbar_btn3 = ToolBarButton()
        self.toolbar_btn4 = ToolBarButton()
        self.toolbar_btn5 = ToolBarButton()
        self.toolbar_btn1.Text = "Open"
        self.toolbar_btn2.Text = "Save"
        self.toolbar_btn3.Text = "Print"
        self.toolbar_btn4.Text = "nop"
        self.toolbar_btn1.ImageIndex = 0
        self.toolbar_btn2.ImageIndex = 1
        self.toolbar_btn3.ImageIndex = 2
        self.toolbar_btn4.ImageIndex = 3
        self.toolbar_btn1.Tag = 0
        self.toolbar_btn5.Style = ToolBarButtonStyle.Separator

        #create label1
        self.label1 = Label()
        self.label1.Text = "page:"
        self.label1.Size = Size(50,18)
        #self.label1.TextAlign = ContentAlignment.MiddleLeft;
        self.label1.Dock = DockStyle.Right

        #setup combobox
        self.combobox = ComboBox()
        self.combobox.Size = Size(50,18)
        self.combobox.Dock = DockStyle.Right
        self.combobox.DropDownStyle = ComboBoxStyle.DropDownList 
        self.combobox.SelectedIndexChanged += self.select
        # add items in ComboBox
        for i in range(10):
            self.combobox.Items.Add(str(i))
        
        # create dialogs
        self.openfiledialog = OpenFileDialog()
        self.savefiledialog = SaveFileDialog()
        self.printdialog = PrintDialog()

        # add controls
        self.toolbar.Buttons.Add(self.toolbar_btn1)
        self.toolbar.Buttons.Add(self.toolbar_btn2)
        self.toolbar.Buttons.Add(self.toolbar_btn3)
        self.toolbar.Buttons.Add(self.toolbar_btn4)
        self.toolbar.Buttons.Add(self.toolbar_btn5)
        self.toolbar.Controls.Add(self.label1)
        self.toolbar.Controls.Add(self.combobox)
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

    # ComboBox click event
    def select(self, sender, event):
        """select a item"""
        self.label1.Text = "page: " + self.combobox.Text

# run application
form = ToolBarSample()
Application.EnableVisualStyles()
Application.Run(form)
