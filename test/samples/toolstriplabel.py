#!/usr/bin/env ipy
# vim: set tabstop=4 shiftwidth=4 expandtab
##############################################################################
# Written by:  Ray Wang <cachen@novell.com>
# Date:        12/10/2008
# Description: The sample for winforms control:
#              ToolStrip
#              ToolStripLabel
##############################################################################

# The docstring below is used in the generated log file
"""
This sample will show "ToolStripLabel" controls in the form.
It can be used for Autotest tools(e.g. Strongwind) to test the behaviors of controls.
"""

import clr

clr.AddReference('System.Windows.Forms')
clr.AddReference('System.Drawing')

from System.Windows.Forms import *
from System.Drawing import *
import System


class ToolStripLabelSample(Form):

    def __init__(self):

        # setup form
        self.Text = "ToolStripLabel Control"

        # ToolStrip:
        self.toolstrip = ToolStrip()
        self.toolstrip.Text = "ToolStrip"

        # ToolStripLabel
        self.toolstrip_label = ToolStripLabel()
        self.toolstrip_label.IsLink = True
        self.toolstrip_label.LinkBehavior = LinkBehavior.AlwaysUnderline
        self.toolstrip_label.Tag = "http://www.mono-project.com/Accessibility"
        self.toolstrip_label.Text = "Mono\nAccessibility"
        self.toolstrip_label.TextAlign = System.Drawing.ContentAlignment.MiddleLeft
        self.toolstrip_label.Click += self.toolstrip_click

        self.toolstrip_label_image = ToolStripLabel()
        self.toolstrip_label_image.Text = "ToolStripLabel with image"
        self.toolstrip_label_image.Image = Bitmap.FromFile("apple-red.png")
        self.toolstrip_label_image.ImageAlign = System.Drawing.ContentAlignment.MiddleRight

        # add Controls
        self.toolstrip.Items.Add(self.toolstrip_label)
        self.toolstrip.Items.Add(self.toolstrip_label_image)
        self.Controls.Add(self.toolstrip)

    def toolstrip_click(self, sender, event):

        # Start Internet Explorer and navigate to the URL in the
        # tag property.
        System.Diagnostics.Process.Start("firefox", sender.Tag.ToString())

        # Set the LinkVisited property to true to change the color.
        sender.LinkVisited = True


form = ToolStripLabelSample()
Application.Run(form)


