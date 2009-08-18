#!/usr/bin/env ipy

##############################################################################
# Written by:  Ray Wang <rawang@novell.com>
# Date:        06/23/2008
# Description: the sample for winforms control:
#              FileDialog
#              SaveFileDialog
##############################################################################

# The docstring below is used in the generated log file
"""
This sample will show "FileDialog" and "SaveFileDialog" control in the form.
It can be used for Autotest tools(e.g. Strongwind) to test the behaviors of controls.
"""

# imports
import clr
import System
import System.IO

clr.AddReference('System.Windows.Forms')
clr.AddReference('System.Drawing')

from System.Windows.Forms import *
from System.Drawing import *
from sys import path
from os.path import exists
from System.IO import StreamWriter

harness_dir = path[0]
i = harness_dir.rfind("/")
uiaqa_path = harness_dir[:i]

class SaveFileDialogSample(Form):
    """SaveFileDialog control class"""

    def __init__(self):
        """SaveFileDialogSample class init function."""

        # setup title
        self.Text = "SaveFileDialog control"

        # setup label
        self.label = Label()
        self.label.Text = "Click the button below, type a file name, press OK, then you could save these words\n"
        self.label.Width = 200 
        self.label.Height = 100 
        self.label.Dock = DockStyle.Top

        # setup button
        self.button = Button()
        self.button.Text = "Click me"
        self.button.Location = Point(0, 50)
        self.button.Click += self.save_file_dialog

        # add controls
        self.Controls.Add(self.button)
        self.Controls.Add(self.label)

    def save_file_dialog(self, sender, event):
        """open a savefiledialog dialog"""

        self.save_file_dialog = SaveFileDialog()
        self.save_file_dialog.InitialDirectory = "%s/winforms" % uiaqa_path
        self.save_file_dialog.Filter = "txt files (*.txt)|*.txt|All files (*.*)|*.*"
        self.save_file_dialog.FilterIndex = 2
        self.save_file_dialog.RestoreDirectory = True
        
        # save the label content to a file.
        try:
            if self.save_file_dialog.ShowDialog() == DialogResult.OK:
                filepath = self.save_file_dialog.FileName
                s_buf = StreamWriter(filepath)
                s_buf.Write(self.label.Text)
                s_buf.Flush()
                s_buf.Close()
                self.label.Text = "The path you selected is: " + filepath + '\n'
        except IOError, event:
            print 'An error occurred:', event

# run application
form = SaveFileDialogSample()
Application.EnableVisualStyles()
Application.Run(form)
