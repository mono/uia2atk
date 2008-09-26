#!/usr/bin/env python

# example gtkcontextmenu.py

import pygtk
pygtk.require('2.0')
import gtk

class ContextMenu:
    def delete_event(self, widget, event, data=None):
        gtk.main_quit()
        return False

    def button_click(self, widget, data=None):
        menu = gtk.Menu()
        item = gtk.MenuItem("Apple")
        menu.append (item)
        
        item = gtk.MenuItem("Banana")
        menu.append (item)
        
        item = gtk.MenuItem("Cherry")
        menu.append (item)
        
        menu.popup(None, None, None, 1, gtk.get_current_event_time())
	menu.show_all()

    def __init__(self):
        self.window = gtk.Window(gtk.WINDOW_TOPLEVEL)
        self.window.set_title("Context Menu")
        self.window.connect("delete_event", self.delete_event)
        self.window.set_border_width(12)
        self.window.resize(200, 200)

        button = gtk.Button("Click to show context menu")
        button.connect("clicked", self.button_click)
        self.window.add(button)

        button.show_all()
        self.window.show()

def main():
    gtk.main()
    return 0       

if __name__ == "__main__":
    ContextMenu()
    main()
