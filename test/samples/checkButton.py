#!/usr/bin/env python

# example checkbutton.py

import pygtk
pygtk.require('2.0')
import gtk

class CheckButton:
    # Our callback.
    # The data passed to this method is printed to stdout
    def callback(self, widget, data=None):
        print "%s was toggled %s" % (data, ("OFF", "ON")[widget.get_active()])

    # This callback quits the program
    def delete_event(self, widget, event, data=None):
        gtk.main_quit()
        return False

    def __init__(self):
        # Create a new window
        self.window = gtk.Window(gtk.WINDOW_TOPLEVEL)

        # Set the window title
        self.window.set_title("Check Button")

        # Set a handler for delete_event that immediately
        # exits GTK.
        self.window.connect("delete_event", self.delete_event)

        # Sets the border width of the window.
        self.window.set_border_width(20)

        # Create a vertical box
        vbox = gtk.VBox(True, 2)

        # Put the vbox in the main window
        self.window.add(vbox)

        # Create first button
        button = gtk.CheckButton("check button 1")

        # When the button is toggled, we call the "callback" method
        # with a pointer to "button" as its argument
        button.connect("toggled", self.callback, "check button 1")


        # Insert button 1
        vbox.pack_start(button, True, True, 2)

        button.show()

        # Create second button

        button = gtk.CheckButton("check button 2")

        # When the button is toggled, we call the "callback" method
        # with a pointer to "button 2" as its argument
        button.connect("toggled", self.callback, "check button 2")
        # Insert button 2
        vbox.pack_start(button, True, True, 2)

        button.show()

        # Create "Quit" button
        button = gtk.Button("Quit")

        # When the button is clicked, we call the mainquit function
        # and the program exits
        button.connect("clicked", lambda wid: gtk.main_quit())

        # Insert the quit button
        vbox.pack_start(button, True, True, 2)

        button.show()
        vbox.show()
        self.window.show()

def main():
    gtk.main()
    return 0       

if __name__ == "__main__":
    CheckButton()
    main()
