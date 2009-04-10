#!/usr/bin/env python

# example gtkspinbutton.py

import pygtk
pygtk.require('2.0')
import gtk

class SpinButton:
    def delete_event(self, widget, event, data=None):
        gtk.main_quit()
        return False

    def __init__(self):
        self.window = gtk.Window(gtk.WINDOW_TOPLEVEL)
        self.window.set_title("Spin Button")
        self.window.connect("delete_event", self.delete_event)
        self.window.set_border_width(12)

        self.vbox = gtk.VBox(False, 0)

        spinbutton_editable = \
                       gtk.SpinButton(gtk.Adjustment(0, -100, 119, 10, 20, 50))
        editable_label = gtk.Label("Editable:")
        editable_label.set_alignment(0, 0)
        spinbutton_uneditable = \
                         gtk.SpinButton(gtk.Adjustment(0, -100, 119, 1, 1, 50))
        spinbutton_uneditable.set_editable(False)
        uneditable_label = gtk.Label("Not Editable:")
        uneditable_label.set_alignment(0, 0)

        # put the spin buttons in the vbox
        self.vbox.pack_start(editable_label, False, False, 0)
        self.vbox.pack_start(spinbutton_editable, True, True, 10)
        self.vbox.pack_start(gtk.VBox(), True, True, 10)
        self.vbox.pack_start(uneditable_label, False, False, 0)
        self.vbox.pack_start(spinbutton_uneditable, True, True, 10)

        self.window.add(self.vbox)

        self.window.show_all()

def main():
    gtk.main()
    return 0       

if __name__ == "__main__":
    SpinButton()
    main()
