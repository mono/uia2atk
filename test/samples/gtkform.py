#!/usr/bin/python
 
# example gtkbutton.py
 
import pygtk
import gtk
 
class FormSample:
 
    # another callback
    def delete_event(self, widget, event, data=None):
        gtk.main_quit()
        return False
 
    def __init__(self):
        # Create a new window
        self.window = gtk.Window(gtk.WINDOW_TOPLEVEL)

        # This is a new call, which just sets the title of our
        # new window to "Hello Buttons!"
        self.window.set_title("Form")
 
        # Here we just set a handler for delete_event that immediately
        # exits GTK.
        self.window.connect("delete_event", self.delete_event)
 
        # Sets the border width of the window.
        self.window.set_border_width(80)

        self.window.show()

    def main(self):
        gtk.main()

if __name__ == "__main__":
    form = FormSample()
    form.main()
