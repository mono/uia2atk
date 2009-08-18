#!/usr/bin/env python

# example gtkcomboboxentry.py

import pygtk
pygtk.require('2.0')
import gtk
import gobject

class ComboBoxEntry:
    def delete_event(self, widget, event, data=None):
        gtk.main_quit()
        return False

    def __init__(self):
        self.window = gtk.Window(gtk.WINDOW_TOPLEVEL)
        self.window.set_title("Combo Box")
        self.window.connect("delete_event", self.delete_event)
        self.window.set_border_width(12)

        store = gtk.ListStore(gobject.TYPE_STRING)
        combo = gtk.ComboBoxEntry(store)
        combo.insert_text(0, "Apples")
        combo.insert_text(1, "Bananas")
        combo.insert_text(2, "Cherry")
        combo.insert_text(3, "Durian")
        combo.insert_text(4, "Fig")
        combo.insert_text(5, "Grapefruit")
        combo.insert_text(6, "Jakfruit")
        combo.insert_text(7, "Kiwi")
        combo.insert_text(8, "Lemon")
        combo.insert_text(9, "Mango")
        combo.insert_text(10, "Orange")
        combo.insert_text(11, "Papaya")
        self.window.add(combo)

        combo.show()
        self.window.show()

def main():
    gtk.main()
    return 0       

if __name__ == "__main__":
    ComboBoxEntry()
    main()
