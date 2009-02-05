#!/usr/bin/env ipy

##############################################################################
# Written by:  Brian G. Merrell <bgmerrell@novell.com>
# Date:        01/29/2009
# Description: a sample for winforms control:
#              Splitter
##############################################################################

# The docstring below is used in the generated log file
"""
"Splitter" control sample
"""

# imports
import clr
import System
clr.AddReference('System.Windows.Forms')
clr.AddReference('System.Drawing')
from System.Windows.Forms import (Application, DockStyle, Form, Splitter,
                                  Label, BorderStyle, TreeView, Button,
                                  Control)
from System.Drawing import Color

class SplitterSample(Form):
    """Splitter control class"""

    def __init__(self):
        """SplitterSample class init function."""

        # Create TreeView, Button, and Splitter controls.
        self.treeView1 = TreeView()
        self.button1 = Button()
        self.splitter1 = Splitter()

        # Set the TreeView control to dock to the left side of the form.
        self.treeView1.Dock = DockStyle.Left;
        # Set the Splitter to dock to the left side of the TreeView control.
        self.splitter1.Dock = DockStyle.Left;
        # Set the minimum size the Button control can be sized to.
        self.splitter1.MinExtra = 100;
        # Set the minimum size the TreeView control can be sized to.
        self.splitter1.MinSize = 75;
        # Set the Button control to fill the remaining space on the form.
        self.button1.Dock = DockStyle.Fill;
        # Add nodes to the TreeView
        self.treeView1.Nodes.Add("TreeView Node");
        self.treeView1.Nodes.Add("Another Node");
        # Give the button some text
        self.button1.Text = "Right Side"

        # Add the controls in reverse order to the form to ensure proper
        # location.
        array_Control = System.Array[Control]
        self.Controls.AddRange(array_Control((self.button1,
                                              self.splitter1,
                                              self.treeView1)))

# run application
form = SplitterSample()
Application.EnableVisualStyles()
Application.Run(form)
