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

        label = gtk.Label("First Frame")
        button = gtk.Button("First Button")
        vbox = gtk.VBox()
        vbox.pack_start(label, padding=1)
        vbox.pack_start(button, padding=1)

        frame1 = gtk.Frame("Frame 1")
        frame1.add(vbox)

        label = gtk.Label("Second Frame")
        button = gtk.Button("Second Button")
        vbox = gtk.VBox()
        vbox.pack_start(label, padding=1)
        vbox.pack_start(button, padding=1)

        frame2 = gtk.Frame("Frame 2")
        frame2.add(vbox)

        vbox = gtk.VBox()
        vbox.pack_start(frame1, padding=5)
        vbox.pack_start(frame2, padding=5)

        self.window.add(vbox)
        self.window.show_all()

def main():
    gtk.main()
    return 0       

if __name__ == "__main__":
    Frame()
    main()
