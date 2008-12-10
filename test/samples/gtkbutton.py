#!/usr/bin/python
 
# example gtkbutton.py
 
import pygtk
import gtk
 
import os
from sys import path
from os.path import exists

harness_dir = path[0]
i = harness_dir.rfind("/")
uiaqa_path = harness_dir[:i]

class ButtonSample:
 
    # callback that opens a message dialog
    def open_dialog(self, widget, data=None):
        self.dialog = gtk.Dialog(title="Message Dialog",
                                 flags=gtk.DIALOG_MODAL)
        self.dialog.add_button("OK", gtk.RESPONSE_CLOSE)
        self.dialog.set_has_separator(False)
        self.dialog.show()
        self.dialog.set_title("Message Dialog")
        self.dialog.set_border_width(50)
        response = self.dialog.run()
        self.dialog.destroy()

    # another callback
    def delete_event(self, widget, event, data=None):
        gtk.main_quit()
        return False
 
    def __init__(self):
        # Create a new window
        self.window = gtk.Window(gtk.WINDOW_TOPLEVEL)
        self.window.show()
        # This is a new call, which just sets the title of our
        # new window to "Hello Buttons!"
        self.window.set_title("Buttons")
 
        # Here we just set a handler for delete_event that immediately
        # exits GTK.
        self.window.connect("delete_event", self.delete_event)
 
        # Sets the border width of the window.
        self.window.set_border_width(80)
 
        # We create a box to pack widgets into.  This is described in detail
        # in the "packing" section. The box is not really visible, it
        # is just used as a tool to arrange widgets.
        self.box1 = gtk.HBox(False, 0)
 
        # Put the box into the main window.
        self.window.add(self.box1)
         
        # Creates a new button with the label "Button 1".
        self.button1 = gtk.Button("Button 1")
        self.tooltips = gtk.Tooltips()
        self.tooltips.set_tip(self.button1, "this is a button")
 
        # Now when the button is clicked, we call the open_dialog method
        # with a pointer to "button 1" as its argument
        self.button1.connect("clicked", self.open_dialog)
 
        # Instead of add(), we pack this button into the invisible
        # box, which has been packed into the window.
        self.box1.pack_start(self.button1, True, True, 0)
 
        # Always remember this step, this tells GTK that our preparation for
        # this button is complete, and it can now be displayed.
        self.button1.show()
 
        # Do these same steps again to create a second button
        self.button2 = gtk.Button("Button 2")
 
        # Call the same callback method with a different argument,
        # passing a pointer to "button 2" instead.
        self.button2.connect("clicked", self.open_dialog)
 
        self.box1.pack_start(self.button2, True, True, 0)
 
        # The order in which we show the buttons is not really important, but I
        # recommend showing the window last, so it all pops up at once.

        self.image = gtk.Image()
        self.image.set_from_file("%s/samples/opensuse60x38.gif" % uiaqa_path)
        self.image.show()
        # a button to contain the image widget
        self.button3 = gtk.Button()
        self.button3.set_label("openSUSE")
        self.button3.set_image(self.image)

        self.box1.pack_start(self.button3, True, True, 0)
        self.button3.connect("clicked", self.open_dialog)
        self.button2.show()
        self.button3.show()
        self.box1.show()
        self.window.show()
	self.window.set_focus(self.button1)
        #button3 with image

    def main(self):
        gtk.main()

if __name__ == "__main__":
    button = ButtonSample()
    button.main()
