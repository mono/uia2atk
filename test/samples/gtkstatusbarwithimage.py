#!/usr/bin/env python

# example gtkstatusbar.py

import pygtk
pygtk.require('2.0')
import gtk

class Statusbar:
    def delete_event(self, widget, event, data=None):
        gtk.main_quit()
        return False

    def __init__(self):
        self.window = gtk.Window(gtk.WINDOW_TOPLEVEL)
        self.window.set_title("Statusbar")
        self.window.connect("delete_event", self.delete_event)
        self.window.resize(300, 50)

        vbox = gtk.VBox()
        vbox.set_spacing(6)
        
        vbox.pack_start(gtk.Label(""), True, True, 0)

        pixbuf = gtk.gdk.pixbuf_new_from_file('icons/novell.ico')
        novell_icon = gtk.Image()
        novell_icon.set_from_pixbuf(pixbuf)

        statusbar = gtk.Statusbar()
        statusbar.push(0, "This is some text in the statusbar")
        statusbar.pack_start(novell_icon)
        vbox.pack_start(statusbar, False, False, 0)

        self.window.add(vbox)

        vbox.show_all()
        self.window.show()

def main():
    gtk.main()
    return 0       

if __name__ == "__main__":
    Statusbar()
    main()
