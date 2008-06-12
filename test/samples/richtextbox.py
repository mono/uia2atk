#!/usr/bin/env ipy

##############################################################################
# Written by:  Calen Chen <cachen@novell.com>
# Date:        03/11/2008
# Description: This is a test application sample for winforms control:
#              RichTextBox

#note: need create a file name "richtextbox_text.txt" with "Text" word first on the current directory .
##############################################################################



import clr
clr.AddReference('System.Windows.Forms')
clr.AddReference('System.Drawing')
clr.AddReference('System')

from System.Windows.Forms import *
from System.Drawing import *
from System import *


class RichTextBoxApp(Form):

    def newPanel(self, x, y):
        panel = Panel()
        panel.Width = 300
        panel.Height = 150
        panel.Location = Point(x, y)
        panel.BorderStyle = BorderStyle.Fixed3D
        return panel

    def __init__(self):
        self.Text = "Simple RichTextBox Example"
        self.Width = 400
        self.Height = 400
        self.FormBorderStyle = FormBorderStyle.Fixed3D

        self.mainLabel1 = Label()
        self.mainLabel1.Text = "Examples for: RichTextBox"
        self.mainLabel1.Location = Point(10,10)
        self.mainLabel1.AutoSize = True
        self.Controls.Add(self.mainLabel1)

        self.label1 = Label()
        self.label1.Text = ""
        self.label1.Location = Point(10,50)
        self.label1.AutoSize = True
        self.Controls.Add(self.label1)
        
        self.richtextboxPanel = self.newPanel(10, 80)
        self.richtextbox1 = RichTextBox()
        self.richtextbox1.Width = 200
        self.richtextbox1.Height = 200
        self.richtextbox1.Dock = DockStyle.Fill
        self.richtextbox1.LoadFile("richtextbox_text.txt")
        self.richtextbox1.SelectAll()
        self.richtextbox1.SelectionColor = Color.Red

        findtext =self.richtextbox1.Find("Text", RichTextBoxFinds.MatchCase)
        if(findtext >= 0):
            self.richtextbox1.SaveFile("text1.txt",RichTextBoxStreamType.RichText)
            self.label1.Text = "Announce: found \"Text\" word, and saved to text1.txt"
        else:
            self.label1.Text = "Announce: didn't found \"Text\" word"

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

        self.Controls.Add(self.richtextboxPanel)
        self.richtextboxPanel.Controls.Add(self.richtextbox1)


form = RichTextBoxApp()
Application.Run(form)

