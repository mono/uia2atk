#!/usr/bin/python
import gtk

class MainWindow(object):
    # close the window and quit
    def delete_event(self, widget, event, data=None):
        gtk.main_quit()
        return False

    def __init__(self,title="Tree View",width=300,height=600):
        # Create a new window
        self.window = gtk.Window(gtk.WINDOW_TOPLEVEL)
        self.window.set_title(title)
        self.window.set_size_request(width, height)
        self.window.connect("delete_event", self.delete_event)

    def get_widget(self):
        pass

    def main(self):
        widget=self.get_widget()
        if widget:
            sw = gtk.ScrolledWindow()
            sw.add(widget)
            self.window.add(sw)
            self.window.show_all()
            gtk.main()

class BasicTreeview(MainWindow):
    def get_widget(self):
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

        # Allow drag and drop reordering of rows
        self.treeview.set_reorderable(True)
        return  self.treeview

if __name__=="__main__":
    basictv=BasicTreeview()
    basictv.main()
