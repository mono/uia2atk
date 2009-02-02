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
                                  Label, BorderStyle, TreeView, ListView,
                                  Control)
from System.Drawing import Color

class SplitterSample(Form):
    """Splitter control class"""

    def __init__(self):
        """SplitterSample class init function."""

        # Create TreeView, ListView, and Splitter controls.
        self.treeView1 = TreeView()
        self.listView1 = ListView()
        self.splitter1 = Splitter()

        # Set the TreeView control to dock to the left side of the form.
        self.treeView1.Dock = DockStyle.Left;
        # Set the Splitter to dock to the left side of the TreeView control.
        self.splitter1.Dock = DockStyle.Left;
        # Set the minimum size the ListView control can be sized to.
        self.splitter1.MinExtra = 100;
        # Set the minimum size the TreeView control can be sized to.
        self.splitter1.MinSize = 75;
        # Set the ListView control to fill the remaining space on the form.
        self.listView1.Dock = DockStyle.Fill;
        # Add a TreeView and a ListView item to identify the controls on the
        # form.
        self.treeView1.Nodes.Add("TreeView Node");
        self.listView1.Items.Add("ListView Item");

        # Add the controls in reverse order to the form to ensure proper
        # location.
        array_Control = System.Array[Control]
        self.Controls.AddRange(array_Control((self.listView1,
                                              self.splitter1,
                                              self.treeView1)))

        # add controls
        #self.Controls.Add(self.label4)
        #self.Controls.Add(self.splitter)
        #self.Controls.Add(self.label0)
        #self.Controls.Add(self.label1)
        #self.Controls.Add(self.label2)
        #self.Controls.Add(self.label3)

# run application
form = SplitterSample()
Application.EnableVisualStyles()
Application.Run(form)
