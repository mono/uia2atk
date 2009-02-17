#!/usr/bin/env ipy

##############################################################################
# Written by:  Calen Chen <cachen@novell.com>
# Date:        02/10/2009
# Description: This is a test application sample for winforms control:
#              OpenFileDialog
##############################################################################

import clr
import System
import System.IO

clr.AddReference('System.Windows.Forms')
clr.AddReference('System.Drawing')

from System.Windows.Forms import *
from System.Drawing import *
from sys import path
from os.path import exists

harness_dir = path[0]
i = harness_dir.rfind("/")
uiaqa_path = harness_dir[:i]


class MenuStripOpenFileDialogApp(Form):

    def __init__(self):
        self.Text = "OpenfileDialog control"
        self.Width = 300
        self.Height = 300
        self.FormBorderStyle = FormBorderStyle.Fixed3D

        #add button
        self.button = Button()
        self.button.Text = "OpenDialog"
        self.button.Location = Point(10, 20)
        self.button.Click += self.Open_Document_Clicked

        #add button to enable HelpButton as visible
        self.button1 = Button()
        self.button1.Text = "EnableVisible"
        self.button1.Location = Point(100, 20)
        self.button1.AutoSize = True
        self.button1.Click += self.Open_Document_Clicked

        #add textbox
        self.textbox = TextBox()
        self.textbox.Text = "Click Button to invoke OpenFileDialog"
        self.textbox.Location = Point(10, 50)
        self.textbox.Size = Size(260,200)
        self.textbox.Multiline = True

        self.Controls.Add(self.button)
        self.Controls.Add(self.button1)
        self.Controls.Add(self.textbox)

    def Open_Document_Clicked(self, sender, event):

        if sender:
            self.openfiledialog = OpenFileDialog()
            self.openfiledialog.InitialDirectory = "%s/samples" % uiaqa_path
            self.openfiledialog.RestoreDirectory = True
            if sender == self.button1:
                self.openfiledialog.ShowHelp = True
                self.openfiledialog.ShowReadOnly = True

        if(self.openfiledialog.ShowDialog() == DialogResult.OK):
            self.stream = System.IO.StreamReader(self.openfiledialog.OpenFile())
            self.textbox.Text = self.stream.ReadToEnd()
            self.stream.Close()

form = MenuStripOpenFileDialogApp()
Application.Run(form)


