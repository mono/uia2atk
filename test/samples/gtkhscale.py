#!/usr/bin/env python

# example gtkhscale.py

import pygtk
pygtk.require('2.0')
import gtk

class HScale:
    def delete_event(self, widget, event, data=None):
        gtk.main_quit()
        return False

    def value_changed(self, range, data=None):
        self.label.set_text("Value: " + str(self.hscale.get_adjustment().value))

    def __init__(self):
        self.window = gtk.Window(gtk.WINDOW_TOPLEVEL)
        self.window.set_title("HScale")
        self.window.connect("delete_event", self.delete_event)
        self.window.set_border_width(12)
        self.window.resize(400, 100)

        vbox = gtk.VBox()
        vbox.set_spacing(6)
        self.window.add(vbox)
        
        self.label = gtk.Label("Value: 0.0")
        vbox.pack_start(self.label, True, True, 0)

        self.hscale = gtk.HScale(gtk.Adjustment(0, 0, 119, 10, 20, 50))
        self.hscale.connect("value_changed", self.value_changed)
        vbox.pack_start(self.hscale, False, False, 0)

        vbox.show_all()
        self.window.show()

def main():
    gtk.main()
    return 0       

if __name__ == "__main__":
    HScale()
    main()
