#!/usr/bin/env python

# example gtkframe.py

import pygtk
pygtk.require('2.0')
import gtk

class Frame:
    def delete_event(self, widget, event, data=None):
        gtk.main_quit()
        return False

    def __init__(self):
        self.window = gtk.Window(gtk.WINDOW_TOPLEVEL)
        self.window.set_title("Frame")
        self.window.connect("delete_event", self.delete_event)
        self.window.set_border_width(12)
        self.window.resize(200, 200)

        frame = gtk.Frame("Frame")
        frame.add(gtk.Label("This is a label"))
        self.window.add(frame)

        frame.show_all()
        self.window.show()

def main():
    gtk.main()
    return 0       

if __name__ == "__main__":
    Frame()
    main()
