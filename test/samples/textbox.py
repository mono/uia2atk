#!/usr/bin/env ipy

##############################################################################
# Written by:  Calen Chen <cachen@novell.com>
# Date:        03/11/2008
# Description: This is a test application sample for winforms control:
#              TextBox
#              Button
##############################################################################

import clr

clr.AddReference('System.Windows.Forms')
clr.AddReference('System.Drawing')

from System.Drawing import *
from System.Windows.Forms import *


class TextBoxButtonApp(Form):

    def __init__(self):
        self.Text = "TextBox Control"

        self.label1 = Label()
        self.label1.Text = "Normal TextBox"
        self.label1.Dock = DockStyle.Top
    
        self.textbox1 = TextBox()
        self.textbox1.AcceptsTab = True
        self.textbox1.AcceptsReturn = True
        self.textbox1.Dock = DockStyle.Top
        self.textbox1.Name = "self.textbox1"
        self.textbox1.TextChanged += self.textbox1_enter

        # create a password entry textbox to display asterisks 
        # instead of the text typed
        self.label2 = Label()
        self.label2.Text = "Multi-Line TextBox"
        self.label2.Dock = DockStyle.Top
        self.label2.Height = 80

        self.textbox2 = TextBox()
        self.textbox2.Dock = DockStyle.Top
        self.textbox2.Height = 100
        self.textbox2.Multiline = True
        self.textbox2.ScrollBars = ScrollBars.Both
        self.textbox2.AcceptsTab = True
        self.textbox2.AcceptsReturn = True
        self.textbox2.WordWrap = False
        self.textbox2.TextChanged += self.textbox2_enter

        self.label3 = Label()
        self.label3.Text = "Password TextBox"
        self.label3.Dock = DockStyle.Top

        self.textbox3 = TextBox()
        self.textbox3.Dock = DockStyle.Top
        self.textbox3.UseSystemPasswordChar = True
        self.textbox3.TextChanged += self.textbox3_enter

        self.Controls.Add(self.textbox3)
        self.Controls.Add(self.label3)
        self.Controls.Add(self.textbox2)
        self.Controls.Add(self.label2)
        self.Controls.Add(self.textbox1)
        self.Controls.Add(self.label1)

    def textbox1_enter(self, sender, event):
        self.label1.Text = self.textbox1.Text

    def textbox2_enter(self, sender, event):
        self.label2.Text = self.textbox2.Text

    def textbox3_enter(self, sender, event):
        self.label3.Text = self.textbox3.Text

form = TextBoxButtonApp()
Application.Run(form)
