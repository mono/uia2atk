#!/usr/bin/env python

import pygtk
pygtk.require('2.0')
import gtk

class GtkContainer():
    def __init__(self):
        # Create the toplevel window
        self.window = gtk.Window(gtk.WINDOW_TOPLEVEL)
        self.window.set_title(self.window.__class__.__name__)
        self.window.set_default_size(200, 200)
        self.window.connect("destroy", lambda x: gtk.main_quit())

        vbox = gtk.VBox()
        self.window.add(vbox)

        label = gtk.Label("A Gtk Label")
        vbox.pack_end(label, False, False, 0)
        
        self.window.show_all()

    def main(self):
        gtk.main()

if __name__ == '__main__':
    object = GtkContainer()
    object.main()
