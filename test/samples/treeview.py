#!/usr/bin/env ipy

##############################################################################
# Written by:  Ray Wang <rawang@novell.com>
# Date:        06/20/2008
# Description: the sample for winforms control:
#              TreeView
##############################################################################

# The docstring below is used in the generated log file
"""
Test accessibility of "TreeView" control
"""

# imports
import clr
clr.AddReference('System.Windows.Forms')
from System.Windows.Forms import Application, DockStyle, Form, TreeView

class TreeViewSample(Form):
    """TreeView control class"""

    def __init__(self):
        """TreeViewSample class init function."""

        # setup title
        self.Text = "TreeView control"

        # init treeview
        self.init_treeview()

        self.Width = 300


    def init_treeview(self):
        """initialize TreeView control"""

        # setup treeview
        self.treeview = TreeView()
        self.treeview.Dock = DockStyle.Left
        self.treeview.Width = 300
        self.treeview.BeginUpdate()
        self.treeview.Nodes.Add("Parent 1")
        self.treeview.Nodes[0].Nodes.Add("Child 1");
        self.treeview.Nodes[0].Nodes.Add("Child 2");
        self.treeview.Nodes[0].Nodes[1].Nodes.Add("Grandchild");
        self.treeview.Nodes[0].Nodes[1].Nodes[0].Nodes.Add("Great Grandchild");
        self.treeview.Nodes.Add("Parent 2")
        self.treeview.Nodes[1].Nodes.Add("Child 3");
        self.treeview.EndUpdate()

        # add controls
        self.Controls.Add(self.treeview)


# run application
form = TreeViewSample()
Application.EnableVisualStyles()
Application.Run(form)
