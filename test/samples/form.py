#!/usr/bin/env ipy

##############################################################################
# Written by:  Brian Merrell <bgmerrell@gmail.com>
# Date:        09/12/2008
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
from System.Windows.Forms import Application, Form

class FormSample(Form):
    """Form control class"""

    def __init__(self):
        """FormSample class init function."""

        # setup title
        self.Text = "Form control"

# run application
form = FormSample()
Application.Run(form)
