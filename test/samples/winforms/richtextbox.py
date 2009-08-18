#!/usr/bin/env ipy

##############################################################################
# Written by:  Calen Chen <cachen@novell.com>
# Date:        03/11/2008
# Description: This is a test application sample for winforms control:
#              RichTextBox

#note: need create a file name "richtextbox_text.txt" with "Text" word first on the current directory .
##############################################################################



import clr
import sys
import os

clr.AddReference('System.Windows.Forms')
clr.AddReference('System.Drawing')
clr.AddReference('System')

from System.Windows.Forms import *
from System.Drawing import *
from System import *

SAMPLES_DIR = os.path.dirname(sys.argv[0])

class RichTextBoxApp(Form):

    def newPanel(self, x, y):
        panel = Panel()
        panel.Width = 300
        panel.Height = 200
        panel.Location = Point(x, y)
        panel.BorderStyle = BorderStyle.Fixed3D
        return panel

    def __init__(self):
        self.Text = "Simple RichTextBox Example"
        self.Width = 400
        self.Height = 500
        self.FormBorderStyle = FormBorderStyle.Fixed3D

        self.mainLabel1 = Label()
        self.mainLabel1.Text = "Examples for: RichTextBox"
        self.mainLabel1.Location = Point(10,10)
        self.mainLabel1.AutoSize = True
        self.Controls.Add(self.mainLabel1)

        self.richtextboxPanel1 = self.newPanel(10, 45)
        self.richtextbox1 = RichTextBox()
        self.richtextbox1.Width = 200
        self.richtextbox1.Height = 200
        self.richtextbox1.Dock = DockStyle.Fill
        self.richtextbox1.LoadFile(os.path.join(SAMPLES_DIR,
                                                "richtextbox_text.txt"))
        self.richtextbox1.SelectAll()
        self.richtextbox1.SelectionColor = Color.Red

        if(self.richtextbox1.SelectionFont != 0):
            currentFont = self.richtextbox1.SelectionFont
            newFontStyle = FontStyle()
            print "currentFont: ",currentFont
            if(self.richtextbox1.SelectionFont.Bold == False):
                newFont = FontStyle.Bold
                print "newFont: ",newFont
            else:
                newFont = FontStyle.Regular
            self.richtextbox1.SelectionFont = Font(currentFont.FontFamily,currentFont.Size,newFont)

        self.richtextboxPanel2 = self.newPanel(10, 250)
        self.richtextbox2 = RichTextBox()
        self.richtextbox2.Location = Point(10, 90)
        self.richtextbox2.Width = 200
        self.richtextbox2.Height = 100
        self.richtextbox2.Dock = DockStyle.Fill
        self.richtextbox2.LoadFile(os.path.join(SAMPLES_DIR,
                                                "richtextbox_text.txt"))
        self.richtextbox2.SelectAll()
        self.richtextbox2.Text = "This is some text."

        self.Controls.Add(self.richtextboxPanel1)
        self.Controls.Add(self.richtextboxPanel2)
        self.richtextboxPanel1.Controls.Add(self.richtextbox1)
        self.richtextboxPanel2.Controls.Add(self.richtextbox2)


form = RichTextBoxApp()
Application.Run(form)

