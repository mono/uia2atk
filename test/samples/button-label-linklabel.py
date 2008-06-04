#!/usr/bin/env ipy
###Button
###Label
import clr
clr.AddReference('System.Windows.Forms')
clr.AddReference('System.Drawing')

from System.Drawing import *
from System.Windows.Forms import *
import System


class ButtonLable(Form):
	def __init__(self):
		self.Text = "Button&Label&LinkLabel Example"
		self.Height = 700
		self.Width = 500
		self.FormBorderStyle = FormBorderStyle.FixedDialog
		self.FormBorderStyle = FormBorderStyle.Fixed3D

##Label control
		self.label = Label()
		self.label.Text = "there is nothing now."
		self.label.Location = Point(30,30)
		self.label.Height = 30
		self.label.Width = 190

		self.count = 0

##Button control
		self.button1 = Button()
		self.button1.Name = "button1"
		self.button1.Text = "button1"
		self.button1.Location = Point(30,70)
		self.button1.Size = Size(300,30)
		self.button1.BackColor = Color.Green
		self.button1.ForeColor = Color.Red
		self.button1.Click += self.b1
		self.button1.Cursor = Cursors.Hand

##LinkLabel control
		self.linklabel1 = LinkLabel()
		self.linklabel1.Location = Point(30,190)
		self.linklabel1.Size = Size(30,120)
		self.linklabel1.AutoSize = True
		self.linklabel1.DisabledLinkColor = Color.Red
		self.linklabel1.VisitedLinkColor = Color.Blue
		self.linklabel1.LinkBehavior = LinkBehavior.HoverUnderline
		self.linklabel1.LinkColor = Color.Navy
		self.linklabel1.TabIndex = 0
		self.linklabel1.TabStop = True
		self.linklabel1.Links[0].Visited = True
		self.linklabel1.Text = "openSUSE:www.opensuse.org  webmail:gmail.novell.com"
		self.linklabel1.Links.Add(9,16,"www.opensuse.org")
		self.linklabel1.Links.Add(35,16,"gmail.novell.com")
		self.linklabel1.LinkClicked += self.linklabel1_LinkClicked
		self.linklabel1.Links[1].Enabled = False
		

		self.Controls.Add(self.label)
		self.Controls.Add(self.button1)
		self.Controls.Add(self.linklabel1)
	
	def b1(self, sender, event):
		self.count += 1
		self.label.Text = "You have clicked b4 %s times" % self.count

	def linklabel1_LinkClicked(self, sender, LinkClicked):
		self.linklabel1.Links[0].Visited = True
		self.linklabel1.Links[1].Visited = True
		target = self.linklabel1.Links[0].LinkData
		if(target.StartsWith("www")):
			System.Diagnostics.Process.Start(target)
		else:
			MessageBox.Show("Item clicked: " + target)

form = ButtonLable()
Application.Run(form)
