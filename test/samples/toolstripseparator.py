#!/usr/bin/env ipy

##############################################################################
# Written by:  Calen Chen <cachen@novell.com>
# Date:        02/03/2008
# Description: This is a test application sample for winforms control:
#              ToolStripSeparator
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
        self.Text = "ToolStripSeparator control"
        self.Width = 300
        self.Height = 300
        self.FormBorderStyle = FormBorderStyle.Fixed3D

        # ToolStrip
        self.ts = ToolStrip()
        self.Controls.Add(self.ts)

        # ToolStripLabel1
        self.tsl1 = ToolStripLabel("Font:")
        self.ts.Items.Add(self.tsl1)

        # ToolStripSeparator1
        self.toolstripseparator1 = ToolStripSeparator()
        self.toolstripseparator1.Enabled = False
        self.ts.Items.Add(self.toolstripseparator1)

        # ToolStripLabel1
        self.tsl2 = ToolStripLabel("Size:")
        self.ts.Items.Add(self.tsl2)

        # ToolStripSeparator2
        self.toolstripseparator2 = ToolStripSeparator()
        self.ts.Items.Add(self.toolstripseparator2)

        # ToolStripLabel3
        self.tsl3 = ToolStripLabel("Color:")
        self.ts.Items.Add(self.tsl3)

	# ToolStripSeparator3
        self.toolstripseparator3 = ToolStripSeparator()
        self.ts.Items.Add(self.toolstripseparator3)

form = RunApp()
Application.Run(form)


