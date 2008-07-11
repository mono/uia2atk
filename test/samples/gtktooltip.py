#!/usr/bin/env python

# example tooltip.py

import pygtk
pygtk.require('2.0')
import gtk

# Create an Arrow widget with the specified parameters
# and pack it into a button
def create_arrow_button(arrow_type, shadow_type):
    button = gtk.Button()
    arrow = gtk.Arrow(arrow_type, shadow_type)
    button.add(arrow)
    button.show()
    arrow.show()
    return button

class Tooltips:
    def __init__(self):
        # Create a new window
        window = gtk.Window(gtk.WINDOW_TOPLEVEL)

        window.set_title("Tooltips")

        # It's a good idea to do this for all windows.
        window.connect("destroy", lambda w: gtk.main_quit())

        # Sets the border width of the window.
        window.set_border_width(10)

        # Create a box to hold the arrows/buttons
        box = gtk.HBox(False, 0)
        box.set_border_width(2)
        window.add(box)

        # create a tooltips object
        self.tooltips = gtk.Tooltips()

        # Pack and show all our widgets
        box.show()

        button = create_arrow_button(gtk.ARROW_UP, gtk.SHADOW_IN)
        box.pack_start(button, False, False, 3)
        self.tooltips.set_tip(button, "SHADOW_IN")

        button = create_arrow_button(gtk.ARROW_DOWN, gtk.SHADOW_OUT)
        box.pack_start(button, False, False, 3)
        self.tooltips.set_tip(button, "SHADOW_OUT")
  
        button = create_arrow_button(gtk.ARROW_LEFT, gtk.SHADOW_ETCHED_IN)
        box.pack_start(button, False, False, 3)
        self.tooltips.set_tip(button, "SHADOW_ETCHED_IN")
  
        button = create_arrow_button(gtk.ARROW_RIGHT, gtk.SHADOW_ETCHED_OUT)
        box.pack_start(button, False, False, 3)
        self.tooltips.set_tip(button, "SHADOW_ETCHED_OUT")

        window.show()

def main():
    gtk.main()
    return 0

if __name__ == "__main__":
    tt = Tooltips()
    main()
