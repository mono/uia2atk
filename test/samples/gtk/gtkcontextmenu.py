#!/usr/bin/env python

# example gtkcontextmenu.py

import pygtk
pygtk.require('2.0')
import gtk
import gobject

class ContextMenu:
    def delete_event(self, widget, event, data=None):
        gtk.main_quit()
        return False

    def close(self):
        self.menu.popdown()

    def after_timeout(self):
        self.menu.get_accessible().add_selection (1)

    def open_apple(self, widget):
        self.button.set_label("You selected apple")

    def open_banana(self, widget):
        self.button.set_label("You selected banana")

    def open_cherry(self, widget):
        self.button.set_label("You selected cherry")

    def button_click(self, widget, data=None):
        self.menu = gtk.Menu()
        item = gtk.MenuItem("Apple")
        item.connect("activate", self.open_apple)
        self.menu.append (item)
        
        item = gtk.MenuItem("Banana")
        item.connect("activate", self.open_banana)
        self.menu.append (item)
        
        item = gtk.MenuItem("Cherry")
        item.connect("activate", self.open_cherry)
        self.menu.append (item)
        
        self.menu.popup(None, None, None, 1, gtk.get_current_event_time())
	self.menu.show_all()

	gobject.timeout_add (2000, self.after_timeout)
        gobject.timeout_add (10000, self.close)

    def __init__(self):
        self.window = gtk.Window(gtk.WINDOW_TOPLEVEL)
        self.window.set_title("Context Menu")
        self.window.connect("delete_event", self.delete_event)
        self.window.set_border_width(12)
        self.window.resize(200, 200)

        self.button = gtk.Button("Click to show context menu")
        self.button.connect("clicked", self.button_click)
        self.window.add(self.button)

        self.button.show_all()
        self.window.show()

def main():
    gtk.main()
    return 0       

if __name__ == "__main__":
    ContextMenu()
    main()
