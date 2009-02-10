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
        self.Text = "OpenfileDialog controls"
        self.Width = 300
        self.Height = 300
        self.FormBorderStyle = FormBorderStyle.Fixed3D

        #add button
        self.button = Button()
        self.button.Text = "Button"
        self.button.Location = Point(10, 20)
        self.button.Click += self.Open_Document_Clicked

        #add textbox
        self.textbox = TextBox()
        self.textbox.Text = "Click Button to invoke OpenFileDialog"
        self.textbox.Location = Point(10, 50)
        self.textbox.Size = Size(260,200)
        self.textbox.Multiline = True

        self.Controls.Add(self.button)
        self.Controls.Add(self.textbox)

    def Open_Document_Clicked(self, sender, event):
        self.openfiledialog = OpenFileDialog()
        self.openfiledialog.InitialDirectory = "%s/samples" % uiaqa_path
        self.openfiledialog.RestoreDirectory = True

        if(self.openfiledialog.ShowDialog() == DialogResult.OK):
            self.stream = System.IO.StreamReader(self.openfiledialog.OpenFile())
            self.textbox.Text = self.stream.ReadToEnd()
            self.stream.Close()


form = MenuStripOpenFileDialogApp()
Application.Run(form)


