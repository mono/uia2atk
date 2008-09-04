#!/usr/bin/env python

# example gtkhpaned.py

import pygtk
pygtk.require('2.0')
import gtk

class HPaned:
    def delete_event(self, widget, event, data=None):
        gtk.main_quit()
        return False

    def __init__(self):
        self.window = gtk.Window(gtk.WINDOW_TOPLEVEL)
        self.window.set_title("HPaned")
        self.window.connect("delete_event", self.delete_event)
        self.window.set_border_width(12)
        self.window.resize(200, 200)

        hpaned = gtk.HPaned()
        hpaned.add1(gtk.Label("Child 1"))
        hpaned.add2(gtk.Label("Child 2"))
        self.window.add(hpaned)

        hpaned.show_all()
        self.window.show()

def main():
    gtk.main()
    return 0       

if __name__ == "__main__":
    HPaned()
    main()
