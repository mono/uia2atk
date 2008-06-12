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
        self.Text = "Simple TextBox Example"
        self.Height = 500
        self.Width = 350
        self.FormBorderStyle = FormBorderStyle.Fixed3D

        self.label = Label()
        self.label.Text = "there is nothing"
        self.label.Location = Point(25,25)
        self.label.Height = 60
        self.label.Width = 150
    
        self.textbox1 = TextBox()
        self.textbox1.Text = "the default text"
        self.textbox1.Location = Point(25,100)
        self.textbox1.Width = 150
        self.textbox1.Height = 60

##some others properties of TextBox
        self.textbox1.Multiline = True
        self.textbox1.ScrollBars = ScrollBars.Vertical #method:Horizontal, Both, None, Vertical
        self.textbox1.AcceptsReturn = True #can use RETURN keyboard
        self.textbox1.AcceptsTab = True #can use TAB keyboard
        self.textbox1.WordWrap = True #auto wrap to the next line

        self.button1 = Button()
        self.button1.Text = "Accept"
        self.button1.ForeColor = Color.Red
        self.button1.Location = Point(75,180)
        self.button1.Click += self.accept

        self.button2 = Button()
        self.button2.Text = "Reset"
        self.button2.BackColor = Color.Green
        self.button2.Location = Point(155,180)
        self.button2.Click += self.reset

##create a password entry textbox to display asterisks instead of the text typed
        self.label1 = Label()
        self.label1.Text = "your passwd is:"
        self.label1.Location = Point(25,220)
        self.label1.Size = Size(150,20)

        self.textbox2 = TextBox()
        self.textbox2.Text = ""
        self.textbox2.Location = Point(25,260)
        self.textbox2.Size = Size(150,20)
        self.textbox2.UseSystemPasswordChar = True

        self.button3 = Button()
        self.button3.Text = "Add Password"
        self.button3.Location = Point(75,280)
        self.button3.Click += self.passwdSet

        self.AcceptButton = self.button1 #define press button1 can accept the text
        self.CancelButton = self.button2 #define press button1 will never accept the text
        self.AcceptButton = self.button3

        self.Controls.Add(self.label)
        self.Controls.Add(self.textbox1)
        self.Controls.Add(self.button1)
        self.Controls.Add(self.button2)
        self.Controls.Add(self.label1)
        self.Controls.Add(self.textbox2)
        self.Controls.Add(self.button3)

    def accept(self, sender, event):
        self.label.Text = self.textbox1.Text

    def reset(self, sender, event):
        self.label.Text = "nothing so far"
        self.textbox1.Text = "the default text"
    
    def passwdSet(self, sender, event):
        self.label1.Text = "your passwd is: %s" % self.textbox2.Text

form = TextBoxButtonApp()
Application.Run(form)
