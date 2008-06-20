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
import clr
clr.AddReference('System.Windows.Forms')
clr.AddReference('System.Drawing')
from System.Windows.Forms import (
    Application, DockStyle, Form, PictureBox, PictureBoxSizeMode
)
from System.Drawing import Image

path_to_file = "./desktop-blue_soccer.jpg"

class PictureBoxSample(Form):
    """PictureBox control class"""

    def __init__(self):
        """PictureBoxSample class init function."""

        # load picture
        image = Image.FromFile(path_to_file)

        # setup title
        self.Text = "PictureBox control"
        self.Height = image.Height
        self.Width = image.Width

        # setup picturebox
        self.picturebox = PictureBox()
        self.picturebox.SizeMode = PictureBoxSizeMode.StretchImage
        self.picturebox.Image = image
        self.picturebox.Dock = DockStyle.Fill

        # add controls
        self.Controls.Add(self.picturebox)

        self.Show()

# run application
form = PictureBoxSample()
Application.EnableVisualStyles()
Application.Run(form)
