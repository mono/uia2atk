#!/usr/bin/env ipy

##############################################################################
# Written by:  Brian G. Merrell <bgmerrell@novell.com>
# Date:        03/11/2009
# Description: A ComboBox sample application that has buttons that can
#              change the style of the ComboBox at runtime.
##############################################################################

# The docstring below is used in the generated log file
"""
Test accessibility of "ComboBox" control that changes style at runtime
"""

# imports
import clr
clr.AddReference('System.Windows.Forms')
clr.AddReference('System.Drawing')
from System.Windows.Forms import Application, Form, ComboBox, Label, DockStyle, ComboBoxStyle, Button
from System.Drawing import Point, Color

class ComboBoxSample(Form):
    """ComboBox control class"""

    def __init__(self):
        """ComboBoxSample class init function."""

        # setup title
        self.Text = "ComboBox control"
        self.Width = 400
        self.Height = 500

        # setup buttons
        self.dropdownlistbutton = Button()
        self.dropdownlistbutton.Text = "DropDownList"
        self.dropdownlistbutton.Width = 110
        self.dropdownlistbutton.Location = Point(5, 5)
        self.dropdownlistbutton.Click += self.change_to_dropdownlist

        self.dropdownbutton = Button()
        self.dropdownbutton.Text = "DropDown"
        self.dropdownbutton.Width = 110
        self.dropdownbutton.Location = Point(135, 5)
        self.dropdownbutton.Click += self.change_to_dropdown
        self.dropdownbutton.BackColor = Color.Lime

        self.simplebutton = Button()
        self.simplebutton.Text = "Simple"
        self.simplebutton.Width = 110
        self.simplebutton.Location = Point(265, 5)
        self.simplebutton.Click += self.change_to_simple

        # setup label
        self.label = Label()
        self.label.Text = "You select " 
        self.label.Location = Point (5, 50)

        # setup combobox
        self.combobox = ComboBox()
        self.combobox.DropDownStyle = ComboBoxStyle.DropDown
        self.combobox.SelectionChangeCommitted += self.select
        self.combobox.TextChanged += self.select
        self.combobox.Width = 390
        self.combobox.Location = Point(0, 80)

        # add items in ComboBox
        for i in range(10):
            self.combobox.Items.Add(str(i))
        self.combobox.SelectedIndex = 1

        # add controls
        self.Controls.Add(self.combobox)
        self.Controls.Add(self.label)
        self.Controls.Add(self.dropdownlistbutton)
        self.Controls.Add(self.dropdownbutton)
        self.Controls.Add(self.simplebutton)

    # ComboBox click event
    def select(self, sender, event):
        """select a item"""
        self.label.Text = "You select " + self.combobox.Text

    def change_back_color(self, button):
        self.simplebutton.BackColor = Button.DefaultBackColor
        self.dropdownbutton.BackColor = Button.DefaultBackColor
        self.dropdownlistbutton.BackColor = Button.DefaultBackColor
        button.BackColor = Color.Lime

    def change_to_dropdownlist(self, sender, event):
        self.combobox.DropDownStyle = ComboBoxStyle.DropDownList
        self.change_back_color(sender)

    def change_to_dropdown(self, sender, event):
        self.combobox.DropDownStyle = ComboBoxStyle.DropDown
        self.change_back_color(sender)

    def change_to_simple(self, sender, event):
        self.combobox.DropDownStyle = ComboBoxStyle.Simple
        self.change_back_color(sender)

# run application
form = ComboBoxSample()
Application.EnableVisualStyles()
Application.Run(form)
