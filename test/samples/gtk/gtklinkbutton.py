#!/usr/bin/env python

# example gtklinkbutton.py

import pygtk
pygtk.require('2.0')
import gtk
import os

class LinkButton:
    def delete_event(self, widget, event, data=None):
        gtk.main_quit()
        return False

    def __init__(self):
        self.window = gtk.Window(gtk.WINDOW_TOPLEVEL)
        self.window.set_title("Link Button")
        self.window.connect("delete_event", self.delete_event)
        self.window.set_border_width(12)

        link_button = gtk.LinkButton("http://mono-project.com/Accessibility", "Mono Accessibility Website")
        gtk.link_button_set_uri_hook(self.uri_hook)
        self.window.add(link_button)

        link_button.show()
        self.window.show()

    def uri_hook(self, widget, data=None):
        os.popen("gnome-open %s" % widget.get_uri())

def main():
    gtk.main()
    return 0       

if __name__ == "__main__":
    LinkButton()
    main()
