#!/usr/bin/env ipy
# -*- coding: utf-8 -*-

##############################################################################
# Written by:  Ray Wang <rawang@novell.com>
# Date:        Jun 16, 2008
# Description: the sample for winforms control:
#              Form
##############################################################################

# The docstring below is used in the generated log file
"""
Test accessibility of "Form" control
"""

# imports
import clr
clr.AddReference('System.Windows.Forms')
from System.Windows.Forms import Application, Form, FormBorderStyle

class FormSample(Form):
    """Form control class"""
    def __init__(self):
        self.Text = "Form control"
        # The FormBorderStyple choices are:
        # Fixed3D - A fixed, three-dimensional border.
        # FixedDialog - A thick, fixed dialog-style border.
        # FixedSingle - A fixed, single-line border.
        # FixedToolWindow - A tool window border that is not resizable.
        # None - No border.
        # Sizable - (The default) A resizable border.
        # SizableToolWindow - A resizable tool window border. A tool window
        #                     does not appear in the taskbar or in the window 
        #                     that appears when the user presses ALT+TAB.
        self.FormBorderStyle = FormBorderStyle.FixedSingle

form = FormSample()
Application.Run(form)
