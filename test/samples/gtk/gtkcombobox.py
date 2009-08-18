#!/usr/bin/env python

# example gtkcombobox.py

import pygtk
pygtk.require('2.0')
import gtk
import gobject

class ComboBox:
    def delete_event(self, widget, event, data=None):
        gtk.main_quit()
        return False

    def __init__(self):
        self.window = gtk.Window(gtk.WINDOW_TOPLEVEL)
        self.window.set_title("Combo Box")
        self.window.connect("delete_event", self.delete_event)
        self.window.set_border_width(12)

        # create a horizontal box (VBox) to organize widgets
        # we will pack two buttons in this box.
        self.vbox = gtk.VBox(False, 0)

        store = gtk.ListStore(gobject.TYPE_STRING)
        combo = gtk.ComboBox(store)
        cell = gtk.CellRendererText()
        combo.pack_start(cell, True)
        combo.add_attribute(cell, 'text', 0)
        combo.insert_text(0, "Apple")
        combo.insert_text(1, "Banana")
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

        self.button = gtk.Button("ButtoN")

        self.vbox.pack_start(self.button, True, True, 0)
        self.vbox.pack_start(combo, True, True, 0)
        self.window.add(self.vbox)
        self.button.show()
	self.vbox.show()

        combo.show()
        self.window.show()

def main():
    gtk.main()
    return 0       

if __name__ == "__main__":
    ComboBox()
    main()
