#!/usr/bin/env python

# example gtkstatusbar.py

import pygtk
pygtk.require('2.0')
import gtk
import gobject

class Statusbar:
    def delete_event(self, widget, event, data=None):
        gtk.main_quit()
        return False

    def __init__(self):
        self.window = gtk.Window(gtk.WINDOW_TOPLEVEL)
        self.window.set_title("Statusbar")
        self.window.connect("delete_event", self.delete_event)
        self.window.resize(300, 100)

        vbox = gtk.VBox()
        vbox.set_spacing(6)
        
        vbox.pack_start(gtk.Label("gtkstatusbar example"), True, True, 0)

        statusbar = gtk.Statusbar()
        store = gtk.ListStore(gobject.TYPE_STRING)
        combo = gtk.ComboBox(store)
        cell = gtk.CellRendererText()
        combo.pack_start(cell, True)
        combo.add_attribute(cell, 'text', 0)
        combo.insert_text(0, "Apple")
        combo.insert_text(1, "Banana")
        combo.insert_text(2, "Cherry")
        statusbar.add(combo)
        statusbar.push(0, "This is some text in the statusbar")
        statusbar.add(gtk.ToolButton(gtk.STOCK_ABOUT))
        statusbar.add(gtk.ToolButton(gtk.STOCK_OPEN))
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
