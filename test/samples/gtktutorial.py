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
 
        # create a horizontal box (HBox) to organize widgets
        # we will pack two buttons in this box.
        self.box1 = gtk.HBox(False, 0)
 
        # Put the box into the main window.
        self.window.add(self.box1)
         
        # Creates a new button with the label "Button 1".
        self.button1 = gtk.Button("Button 1")
 
        # Now when the button is clicked, we call the open_dialog method
        # with a pointer to "button 1" as its argument
        self.button1.connect("clicked", TreeView)
 
        # Instead of add(), we pack this button into the invisible
        # box, which has been packed into the window.
        self.box1.pack_start(self.button1, True, True, 0)
 
        # Always remember this step, this tells GTK that our preparation for
        # this button is complete, and it can now be displayed.
        self.button1.show()
 
        # Do these same steps again to create a second button
        self.button2 = gtk.Button("Button 2")
 
        self.button2.connect("clicked", CheckButton)
 
        # put the 2nd button in the HBox
        self.box1.pack_start(self.button2, True, True, 0)
 
        # show the second button
        self.button2.show()

        # show the HBox
        self.box1.show()

        # finally, show the window
        self.window.show()

    def main(self):
        gtk.main()

class TreeView:

    def delete_event(self, widget, event, data=None):
        self.window.destroy()

    def __init__(self, widget, data=None):
        self.window = gtk.Window(gtk.WINDOW_TOPLEVEL)
        self.window.set_title("Sample Tree View")
        self.window.set_size_request(300, 600)
        self.window.connect("delete_event", self.delete_event)

        # create a TreeStore with one string column to use as the model
        self.treestore = gtk.TreeStore(str)

        # we'll add some data now - 4 rows with 3 child rows each
        for parent in range(4):
            piter = self.treestore.append(None, ['parent %i' % parent])
            for child in range(3):
                self.treestore.append(piter, ['child %i of parent %i' %
                                              (child, parent)])

        # create the TreeView using treestore
        self.treeview = gtk.TreeView(self.treestore)

        # create the TreeViewColumn to display the data
        self.tvcolumn = gtk.TreeViewColumn('Column 0')

        # add tvcolumn to treeview
        self.treeview.append_column(self.tvcolumn)

        # create a CellRendererText to render the data
        self.cell = gtk.CellRendererText()

        # add the cell to the tvcolumn and allow it to expand
        self.tvcolumn.pack_start(self.cell, True)

        # set the cell "text" attribute to column 0 - retrieve text
        # from that column in treestore
        self.tvcolumn.add_attribute(self.cell, 'text', 0)

        # make it searchable
        self.treeview.set_search_column(0)

        # Allow sorting on the column
        self.tvcolumn.set_sort_column_id(0)

        self.window.add(self.treeview)
        self.window.show_all()


class CheckButton:

    def delete_event(self, widget, event, data=None):
        self.window.destroy()

    def __init__(self, widget, data=None):
        # Create a new window
        self.window = gtk.Window(gtk.WINDOW_TOPLEVEL)

        # Set the window title
        self.window.set_title("Sample Check Button")

        # Set a handler for delete_event that immediately
        # exits GTK.
        self.window.connect("delete_event", self.delete_event)

        # Sets the border width of the window.
        self.window.set_border_width(80)

        # Create a vertical box
        vbox = gtk.VBox(True, 2)

        # Put the vbox in the main window
        self.window.add(vbox)

        # Create first button
        button = gtk.CheckButton("check button 1")

        # Insert button 1
        vbox.pack_start(button, True, True, 2)

        button.show()

        # Create second button
        button = gtk.CheckButton("check button 2")

        # Insert button 2
        vbox.pack_start(button, True, True, 2)

        button.show()

        # Create "Quit" button
        button = gtk.Button("Close")

        # When the button is clicked, we call the mainquit function
        # and the program exits
        button.connect("clicked", lambda destroy: self.window.destroy()) 

        # Insert the quit button
        vbox.pack_start(button, True, True, 2)

        button.show()
        vbox.show()
        self.window.show()

if __name__ == "__main__":
    m = MainWindow()
    m.main()
