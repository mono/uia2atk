#!/usr/bin/python
 
# example helloworld2.py
 
import pygtk
#pygtk.require('2.0')
import gtk
 
class HelloWorld2:
 
    # callback that opens a message dialog
    def open_dialog(self, widget, data=None):
        self.dialog = gtk.MessageDialog()
        self.dialog.show()
        self.dialog.set_title("MessageDialog")
        self.dialog.set_border_width(50)
        
 
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

        self.button2.show()
        self.box1.show()
        self.window.show()

    def main(self):
        gtk.main()

if __name__ == "__main__":
    hello = HelloWorld2()
    hello.main()
