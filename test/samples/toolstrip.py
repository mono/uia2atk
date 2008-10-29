#!/usr/bin/env ipy

##############################################################################
# Written by:  Calen Chen <cachen@novell.com>
# Date:        05/11/2008
# Description: This is a test application sample for winforms control:
#              ToolStripLabel
##############################################################################

import clr
import System
import System.IO

clr.AddReference('System.Windows.Forms')
clr.AddReference('System.Drawing')

from System.Windows.Forms import *
from System.Drawing import *
import System.Drawing.Text


class RunApp(Form):

    def __init__(self):
        self.Text = "ToolStrip control"
        self.Width = 300
        self.Height = 100
        self.FormBorderStyle = FormBorderStyle.Fixed3D

##ToolStrip:
        self.ts = ToolStrip()
        self.Controls.Add(self.ts)


form = RunApp()
Application.Run(form)


