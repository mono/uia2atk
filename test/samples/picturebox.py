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
from System.Windows.Forms import (
    Application, Button, DockStyle, Form, PictureBox, PictureBoxSizeMode
)
from System.Drawing import Image

harness_dir = path[0]
i = harness_dir.rfind("/")
uiaqa_path = harness_dir[:i]

#uiaqa_path = os.environ.get("UIAQA_HOME")

class PictureBoxSample(Form):
    """PictureBox control class"""
    toggle = True
    path_to_file = "%s/samples/desktop-blue_soccer.jpg" % uiaqa_path
    image = None

    def __init__(self):
        """PictureBoxSample class init function."""

        # setup title
        self.Text = "PictureBox control"
        self.Height = 600 
        self.Width = 600

        # setup button
        self.button = Button()
        self.button.Text = "Toggle"
        self.button.Click += self.on_click

        # setup picturebox
        self.picturebox = PictureBox()
        self.picturebox.SizeMode = PictureBoxSizeMode.StretchImage
        self.picturebox.Image = Image.FromFile(self.path_to_file)
        self.picturebox.Dock = DockStyle.Fill

        # add controls
        self.Controls.Add(self.button)
        self.Controls.Add(self.picturebox)

        self.change_picture()

    def change_picture(self):
        # load picture
        self.picturebox.Image = Image.FromFile(self.path_to_file)
        self.Show()

    def on_click(self, sender, event):
        if self.toggle == True:
            self.toggle = False
            self.path_to_file = "%s/samples/universe.jpg" % uiaqa_path
            self.change_picture()
        else:
            self.toggle = True
            self.path_to_file = "%s/samples/desktop-blue_soccer.jpg" % uiaqa_path
            self.change_picture()

# run application
form = PictureBoxSample()
Application.EnableVisualStyles()
Application.Run(form)
