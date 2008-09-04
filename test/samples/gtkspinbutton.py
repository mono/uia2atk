#!/usr/bin/env python

# example gtkspinbutton.py

import pygtk
pygtk.require('2.0')
import gtk

class SpinButton:
    def delete_event(self, widget, event, data=None):
        gtk.main_quit()
        return False

    def __init__(self):
        self.window = gtk.Window(gtk.WINDOW_TOPLEVEL)
        self.window.set_title("Spin Button")
        self.window.connect("delete_event", self.delete_event)
        self.window.set_border_width(12)

        spinbutton = gtk.SpinButton(gtk.Adjustment(0, 0, 119, 10, 20, 50))
        self.window.add(spinbutton)

        spinbutton.show()
        self.window.show()

def main():
    gtk.main()
    return 0       

if __name__ == "__main__":
    SpinButton()
    main()
