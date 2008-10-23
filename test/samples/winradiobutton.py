#!/usr/bin/env ipy

# The docstring below is used in the generated log file

import clr

clr.AddReference('System.Windows.Forms')
clr.AddReference('System.Drawing')

from System.Drawing import *
from System.Windows.Forms import *


class RadioButtonApp(Form):

    def __init__(self):
        self.Text = "Radio Button"
        self.Width = 169
        self.Height = 171
        self.BorderStyle = BorderStyle.FixedSingle

        self.radiobutton1 = RadioButton()
        self.radiobutton1.Text = "Apple"
        self.radiobutton1.TabIndex = 0
        self.radiobutton1.Location = Point(22,25)

        self.radiobutton2 = RadioButton()
        self.radiobutton2.Text = "Banana"
        self.radiobutton2.TabIndex = 1
        self.radiobutton2.Location = Point(22,60)

        self.radiobutton3 = RadioButton()
        self.radiobutton3.Text = "Cherry"
        self.radiobutton3.TabIndex = 2
        self.radiobutton3.Location = Point(22,95)

        self.Controls.Add(self.radiobutton3)
        self.Controls.Add(self.radiobutton2)
        self.Controls.Add(self.radiobutton1)

Application.EnableVisualStyles()
form = RadioButtonApp()
Application.Run(form)
