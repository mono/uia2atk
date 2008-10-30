#!/usr/bin/env ipy

##############################################################################
# Written by:  Ray Wang <rawang@novell.com>
# Date:        06/20/2008
# Description: the sample for winforms control:
#              PictureBox
##############################################################################

# The docstring below is used in the generated log file
"""
Test accessibility of "PictureBox" control
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

harness_dir = path[0]
i = harness_dir.rfind("/")
uiaqa_path = harness_dir[:i]

#uiaqa_path = os.environ.get("UIAQA_HOME")

class PictureBoxSample(Form):
    """PictureBox control class"""
    toggle = True
    path_to_file = "%s/samples/desktop-blue_soccer400x500.jpg" % uiaqa_path
    image = None

    def __init__(self):
        """PictureBoxSample class init function."""

        # setup title
        self.Text = "PictureBox control"
        self.Height = 500 
        self.Width = 450

        # setup button
        self.button = Button()
        self.button.Text = "openSUSE"
        self.button.Click += self.on_click
        self.button.Image = Image.FromFile("%s/samples/opensuse60x38.gif" % uiaqa_path)
        self.button.AutoSize = True

        # setup label
        self.label = Label()
        self.label.Location = Point(10, 100)
        self.label.ForeColor = Color.Red
        self.label.Text = "You are watching %s" % self.path_to_file
        self.label.AutoSize = True

        # setup picturebox
        self.picturebox = PictureBox()
        self.picturebox.SizeMode = PictureBoxSizeMode.StretchImage
        self.picturebox.Image = Image.FromFile(self.path_to_file)
        self.picturebox.Dock = DockStyle.Fill

        # add controls
        self.Controls.Add(self.button)
        self.Controls.Add(self.label)
        self.Controls.Add(self.picturebox)

        self.change_picture()

    def change_picture(self):
        # load picture
        self.picturebox.Image = Image.FromFile(self.path_to_file)
        self.Show()

    def on_click(self, sender, event):
        if self.toggle == True:
            self.toggle = False
            self.path_to_file = "%s/samples/universe300x400.jpg" % uiaqa_path
            self.change_picture()
            self.label.Text = "You are watching %s" % self.path_to_file
        else:
            self.toggle = True
            self.path_to_file = "%s/samples/desktop-blue_soccer400x500.jpg" % uiaqa_path
            self.change_picture()
            self.label.Text = "You are watching %s" % self.path_to_file

# run application
form = PictureBoxSample()
Application.EnableVisualStyles()
Application.Run(form)
