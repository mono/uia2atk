#!/usr/bin/env ipy

##############################################################################
# Written by:  Brian G. Merrell <bgmerrell@novell.com>
# Date:        12/05/2008
# Description: the sample for winforms control:
#              HelpProvider
##############################################################################

# The docstring below is used in the generated log file
"""
This sample will show "HelpProvider" control in the form.
It can be used for Autotest tools (e.g. Strongwind) to test the behaviors of
controls.
"""

# imports
import clr
import System
clr.AddReference('System.Windows.Forms')
clr.AddReference('System.Drawing')

from System.Windows.Forms import (Application, BorderStyle, Form, Label,
                                  TextBox, HelpProvider, FormBorderStyle, 
                                  Control)
from System.Drawing import Point, Size

class HelpProviderSample(Form):
    """HelpProvider control class"""

    def __init__(self):
        """HelpProviderSample class init function."""
        self.addressTextBox = TextBox()
        self.addressTextBox.AccessibleName = "Street Text Box"
        self.helpLabel = Label()
        self.label2 = Label()
        self.cityTextBox = TextBox()
        self.label3 = Label()
        self.stateTextBox = TextBox()
        self.zipTextBox = TextBox()

        # Help Label
        self.helpLabel.BorderStyle = BorderStyle.Fixed3D
        self.helpLabel.Location = Point(8, 80)
        self.helpLabel.Size = Size(272, 72)
        self.helpLabel.Text = " ".join(["Click on a focusable control and",
                                        "then press F1 to see a tooltip for",
                                        "that control."])

        # Address Label
        self.label2.Location = Point(16, 8)
        self.label2.Size = Size(100, 16)
        self.label2.Text = "Address:"

        # Comma Label
        self.label3.Location = Point(136, 56)
        self.label3.Size = Size(16, 16)
        self.label3.Text = ","

        # Create the HelpProvider.
        self.helpProvider1 = HelpProvider()

        # Tell the HelpProvider what controls to provide help for, and
        # what the help string is.
        self.helpProvider1.SetShowHelp(self.addressTextBox, True)
        self.helpProvider1.SetHelpString(self.addressTextBox, "Enter the street address in this text box.")

        self.helpProvider1.SetShowHelp(self.cityTextBox, True)
        self.helpProvider1.SetHelpString(self.cityTextBox, "Enter the city here.")

        self.helpProvider1.SetShowHelp(self.stateTextBox, True)
        self.helpProvider1.SetHelpString(self.stateTextBox, "Enter the state in this text box.")

        self.helpProvider1.SetShowHelp(self.zipTextBox, True)
        self.helpProvider1.SetHelpString(self.zipTextBox, "Enter the zip code here.")

        # Set what the Help file will be for the HelpProvider.
        self.helpProvider1.HelpNamespace = "mspaint.chm"

        # Sets properties for the different address fields.

        # Address TextBox
        self.addressTextBox.Location = Point(16, 24)
        self.addressTextBox.Size = Size(264, 20)
        self.addressTextBox.TabIndex = 0
        self.addressTextBox.Text = "1800 S Novell Place"

        # City TextBox
        self.cityTextBox.Location = Point(16, 48)
        self.cityTextBox.Size = Size(120, 20)
        self.cityTextBox.TabIndex = 3
        self.cityTextBox.Text = ""

        # State TextBox
        self.stateTextBox.Location = Point(152, 48)
        self.stateTextBox.MaxLength = 2
        self.stateTextBox.Size = Size(32, 20)
        self.stateTextBox.TabIndex = 5

        # Zip TextBox
        self.zipTextBox.Location = Point(192, 48)
        self.zipTextBox.Size = Size(88, 20)
        self.zipTextBox.TabIndex = 6
        self.zipTextBox.Text = ""

        # Add the controls to the form.
        array_Control = System.Array[Control]
        self.Controls.AddRange(array_Control((self.zipTextBox,
                               self.stateTextBox,
                               self.label3, self.cityTextBox,
                               self.label2, self.helpLabel,
                               self.addressTextBox)))

        # Set the form to look like a dialog, and show the HelpButton.    
        self.FormBorderStyle = FormBorderStyle.FixedDialog
        self.HelpButton = True
        self.MaximizeBox = False
        self.MinimizeBox = False
        self.ClientSize = Size(292, 160)
        self.Text = "Help Provider Demonstration"
 
# run application
form = HelpProviderSample()
Application.EnableVisualStyles()
Application.Run(form)
