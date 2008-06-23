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
Test accessibility of "SaveFileDialog" control
"""

# imports
import clr
clr.AddReference('System.Windows.Forms')
clr.AddReference('System.Drawing')

from System.Windows.Forms import (
    Application, Button, Form, SaveFileDialog, Label, DialogResult, DockStyle
)
from System.Drawing import Point
from System.IO import StreamWriter

class SaveFileDialogSample(Form):
    """SaveFileDialog control class"""

    def __init__(self):
        """SaveFileDialogSample class init function."""

        # setup title
        self.Text = "SaveFileDialog control"

        # setup label
        self.label = Label()
        self.label.Text = "Click the button below, type a file name, press OK, then you could save these words"
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
        self.save_file_dialog.Filter = "txt files (*.txt)|*.txt|All files (*.*)|*.*"
        self.save_file_dialog.FilterIndex = 1
        self.save_file_dialog.RestoreDirectory = True
        
        # save the label content to a file.
        try:
            if self.save_file_dialog.ShowDialog() == DialogResult.OK:
                filepath = self.save_file_dialog.FileName
                s_buf = StreamWriter(filepath)
                s_buf.Write(self.label.Text)
                s_buf.Flush()
                s_buf.Close()
        except IOError, event:
            print 'An error occurred:', event

# run application
form = SaveFileDialogSample()
Application.EnableVisualStyles()
Application.Run(form)
