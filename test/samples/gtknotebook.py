#!/usr/bin/env python

# example gtknotebook.py

import pygtk
pygtk.require('2.0')
import gtk

class Notebook:
    def delete_event(self, widget, event, data=None):
        gtk.main_quit()
        return False

    def __init__(self):
        self.window = gtk.Window(gtk.WINDOW_TOPLEVEL)
        self.window.set_title("Notebook")
        self.window.connect("delete_event", self.delete_event)
        self.window.set_border_width(12)
        self.window.resize(400, 400)

        notebook = gtk.Notebook()
        notebook.append_page(gtk.Button("Contents 1"), gtk.Label("Tab 1"))
        notebook.append_page(gtk.Label("Contents 2"), gtk.Label("Tab 2"))
        self.window.add(notebook)

        notebook.show_all()
        self.window.show()

def main():
    gtk.main()
    return 0       

if __name__ == "__main__":
    Notebook()
    main()
