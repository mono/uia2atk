#!/usr/bin/env python

# example gtkvscrollbar.py

import pygtk
pygtk.require('2.0')
import gtk

class VScrollbar:
    def delete_event(self, widget, event, data=None):
        gtk.main_quit()
        return False

    def value_changed(self, range, data=None):
        self.label.set_text("Value: " + str(self.vscrollbar.get_adjustment().value))

    def __init__(self):
        self.window = gtk.Window(gtk.WINDOW_TOPLEVEL)
        self.window.set_title("VScrollbar")
        self.window.connect("delete_event", self.delete_event)
        self.window.set_border_width(12)
        self.window.resize(200, 400)

        hbox = gtk.HBox()
        hbox.set_spacing(6)
        self.window.add(hbox)
        
        self.label = gtk.Label("Value: 0.0")
        hbox.pack_start(self.label, True, True, 0)

        self.vscrollbar = gtk.VScrollbar(gtk.Adjustment(0, 0, 119, 10, 20, 50))
        self.vscrollbar.connect("value_changed", self.value_changed)
        hbox.pack_start(self.vscrollbar, False, False, 0)

        hbox.show_all()
        self.window.show()

def main():
    gtk.main()
    return 0       

if __name__ == "__main__":
    VScrollbar()
    main()
