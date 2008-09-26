#!/usr/bin/env python

# example gtktable.py

import pygtk
pygtk.require('2.0')
import gtk

class Table:
    def delete_event(self, widget, event, data=None):
        gtk.main_quit()
        return False

    def __init__(self):
        self.window = gtk.Window(gtk.WINDOW_TOPLEVEL)
        self.window.set_title("Table")
        self.window.connect("delete_event", self.delete_event)
        self.window.set_border_width(12)
        self.window.resize(200, 200)

        table = gtk.Table(2, 2)
        table.attach(gtk.Label("Foo:"), 0, 1, 0, 1)
        table.attach(gtk.Entry(), 1, 2, 0, 1)
        table.attach(gtk.Label("Bar:"), 0, 1, 1, 2)
        table.attach(gtk.Entry(), 1, 2, 1, 2)
        self.window.add(table)

        table.show_all()
        self.window.show()

def main():
    gtk.main()
    return 0       

if __name__ == "__main__":
    Table()
    main()
