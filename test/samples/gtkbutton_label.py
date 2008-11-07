#!/usr/bin/python
 
import pygtk
import gtk
 
class MainWindow:
 
    # destroy the window (when False is returned)
    def delete_event(self, widget, event, data=None):
        gtk.main_quit()
        return False
 
    def __init__(self):

        #tv = TreeView()
        #cb = CheckButton()

        # Create new window
        self.window = gtk.Window(gtk.WINDOW_TOPLEVEL)
        self.window.set_title("Choose Wisely")
 
        # connect the event to close the window
        # this is given by the window manager, usually by the "close" option,
        # or on the titlebar 
        self.window.connect("delete_event", self.delete_event)
 
        # Set the border width of the window.
        self.window.set_border_width(80)

        # create a label
        self.label = gtk.Label()
        self.label.set_markup("<span color=\"red\">This is a label</span>"); 

        # create a horizontal box (HBox) to organize widgets
        # we will pack two buttons in this box.
        self.vbox = gtk.VBox(False, 0)

        # create a horizontal box (HBox) to organize widgets
        # we will pack two buttons in this box.
        self.hbox = gtk.HBox(False, 0)
 
        # Put the vbox into the main window.
        self.window.add(self.vbox)

        # Creates a new button with the label "Button 1".
        self.button1 = gtk.Button("Button 1")
        self.button1.modify_bg(gtk.STATE_NORMAL,gtk.gdk.color_parse("red"))
 
        # Now when the button is clicked, we call the open_dialog method
        # with a pointer to "button 1" as its argument
        self.button1.connect("clicked", self.button_one_clicked)
 
        # Instead of add(), we pack this button into the invisible
        # box, which has been packed into the window.
        self.hbox.pack_start(self.button1, True, True, 0)

        # pack the label into the vbox
        self.vbox.pack_start(self.label, True, True, 0)

        # pack the hbox into the vbox
        self.vbox.pack_start(self.hbox, True, True, 0)
     
        # Always remember this step, this tells GTK that our preparation for
        # this button is complete, and it can now be displayed.
        self.button1.show()
 
        # Do these same steps again to create a second button
        self.button2 = gtk.Button("Button 2")
 
        self.button2.connect("clicked", self.button_two_clicked)
 
        # put the 2nd button in the HBox
        self.hbox.pack_start(self.button2, True, True, 0)
 
        # show the second button
        self.button2.show()

        # Creates a new button with the label "Button 3".
        self.button3 = gtk.Button("Button 3")
 
        # Now when the button is clicked, we call the open_dialog method
        # with a pointer to "button 3" as its argument
        self.button3.connect("clicked", self.button_three_clicked)

	# disable button three
	self.button3.set_sensitive(False)

        #creates an insensitive label
        self.insensitive_label = gtk.Label("I'm so insensitive")
        self.insensitive_label.set_sensitive(False)
 
        # Instead of add(), we pack this button into the invisible
        # box, which has been packed into the window.
        self.vbox.pack_start(self.button3, True, True, 0)

        #pack insensitive label
        self.vbox.pack_start(self.insensitive_label, True, True, 0)

        # Always remember this step, this tells GTK that our preparation for
        # this button is complete, and it can now be displayed.
        self.button3.show()

        # show the HBox
        self.hbox.show()

        # show the VBox
        self.vbox.show()

        # show the label
        self.label.show()

        # show the insensitive label
        self.insensitive_label.show()

        # finally, show the window
        self.window.show()

    def button_one_clicked(self, widget):
        self.label.set_text("button one was clicked last")

    def button_two_clicked(self, widget):
        self.label.set_text("button two was clicked last")

    def button_three_clicked(self, widget):
        self.label.set_text("button three was clicked last")

    def main(self):
        gtk.main()

if __name__ == "__main__":
    m = MainWindow()
    m.main()
