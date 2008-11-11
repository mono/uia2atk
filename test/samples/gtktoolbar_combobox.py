#!/usr/bin/env python

# example gtktoolbar.py

import pygtk
pygtk.require('2.0')
import gtk
import gobject

class Toolbar:
    def delete_event(self, widget, event, data=None):
        gtk.main_quit()
        return False

    def __init__(self):
        self.window = gtk.Window(gtk.WINDOW_TOPLEVEL)
        self.window.set_title("Toolbar")
        self.window.connect("delete_event", self.delete_event)
        self.window.set_border_width(12)
        self.window.resize(200, 50)

        toolbar = gtk.Toolbar()
        store = gtk.ListStore(gobject.TYPE_STRING)
        combo = gtk.ComboBox(store)
        cell = gtk.CellRendererText()
        combo.pack_start(cell, True)
        combo.add_attribute(cell, 'text', 0)
        combo.insert_text(0, "Apple")
        combo.insert_text(1, "Banana")
        combo.insert_text(2, "Cherry")
        toolbar.add(combo)
        #toolbar.insert(gtk.ToolButton(gtk.STOCK_ABOUT), 0)
        #toolbar.insert(gtk.ToolButton(gtk.STOCK_OPEN), 1)
        self.window.add(toolbar)

        toolbar.show_all()
        self.window.show()

def main():
    gtk.main()
    return 0       

if __name__ == "__main__":
    Toolbar()
    main()
