#!/usr/bin/env ipy

import sys
import clr

clr.AddReference('System.Windows.Forms')
clr.AddReference('System.Drawing')

from System.Drawing import *
from System.Windows.Forms import *

harness_dir = sys.path[0]
i = harness_dir.rfind("/")
uiaqa_path = harness_dir[:i]

class Buttons(Form):
    """Buttons control class"""

    def __init__(self):
        """Buttons class init function."""

        self.Text = "Buttons"
        self.Height = 228
        self.Width = 445

        # set up Button1 control
        self.button1 = Button()
        self.button1.Text = "Button 1"
        self.button1.Location = Point(80,80)
        self.button1.Size = Size(67, 50)
        self.button1.Click += self.button_click

        #set up Button2 control:
        self.button2 = Button()
        self.button2.Text = "Button 2"
        self.button2.Location = Point(147, 80)
        self.button2.Size = Size(67, 50)
        self.button2.Click += self.button_click

        #set up Enabled Button4 control:
        self.button3 = Button()
        self.button3.Text = "openSUSE"
        self.button3.Location = Point(214, 80)
        self.button3.Size = Size(134, 50)
        self.button3.Image = Image.FromFile("%s/winforms/opensuse60x38.gif" % uiaqa_path)
        self.button3.Click += self.button_click

        # add controls
        self.Controls.Add(self.button1)
        self.Controls.Add(self.button2)
        self.Controls.Add(self.button3)
    
    def button_click(self, sender, event):
        MessageBox.Show("", "Message Dialog")
        #self.form = Form()
        #self.form.Text = "Message Dialog"
        #self.form.Show()

form = Buttons()
Application.Run(form)
