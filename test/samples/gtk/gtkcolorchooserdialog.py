#!/usr/bin/env python

# example gtkcolorselectiondialog.py

import pygtk
pygtk.require('2.0')
import gtk

class ColorSelectionDialog:
    def delete_event(self, widget, event, data=None):
        gtk.main_quit()
        return False

    def response(self, widget, response_id, data=None):
        gtk.main_quit()
        return False

    def __init__(self):
        self.window = gtk.ColorSelectionDialog("Color Selection Dialog")
        self.window.connect("delete_event", self.delete_event)
        self.window.connect("response", self.response)
        self.window.show()

def main():
    gtk.main()
    return 0       

if __name__ == "__main__":
    ColorSelectionDialog()
    main()
