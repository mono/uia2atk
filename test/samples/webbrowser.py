#!/usr/bin/env ipy

##############################################################################
# Written by:  Ray Wang <rawang@novell.com>
# Date:        06/24/2008
# Description: the sample for winforms control:
#              WebBrowser
##############################################################################

# The docstring below is used in the generated log file
"""
Test accessibility of "WebBrowser" control
"""

# imports
import clr
clr.AddReference('System.Windows.Forms')
from System.Windows.Forms import Application, DockStyle, Form, WebBrowser, TextBox

class WebBrowserSample(Form):
    """WebBrowser control class"""

    def __init__(self):
        """WebBrowserSample class init function."""

        # setup title
        self.Text = "WebBrowser control"
        self.Width = 1024
        self.Height = 800
        url = 'www.google.com'

        # setup webbrowser
        self.webbrowser = WebBrowser()
        self.webbrowser.Navigate(url)
        self.webbrowser.Dock = DockStyle.Fill
        self.webbrowser.Navigated += self.navigated

        # setup Location bar
        self.location = TextBox()
        self.location.Text = url
        self.location.Dock = DockStyle.Top
        self.location.KeyPress += self.key_press

        # add controls
        self.Controls.Add(self.webbrowser)
        self.Controls.Add(self.location)

    def key_press(self, sender, event):
        if event.KeyChar == u'\r':
            self.webbrowser.Navigate(self.location.Text.strip())

    def navigated(self, sender, event):
        self.location.Text = event.Url.ToString()


# run application
form = WebBrowserSample()
Application.EnableVisualStyles()
Application.Run(form)
