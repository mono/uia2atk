#!/usr/bin/python
import gtk
import sys

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
            self.window.add(widget)
            self.window.show_all()
            gtk.main()

class BasicTreeview(MainWindow):

    movie_list = ["The Shawshank Redemption",
                  "The Godfather",
                  "The Godfather II",
                  "The Dark Knight",
                  "Pulp Fiction",
                  "Schindler's List",
                  "Star Wars",
                  "12 Angry Men",
                  "Casablanca",
                  "Goodfellas",
                  "Rear Window",
                  "Cidade de Deus",
                  "Psycho",
                  "Sunset Blvd.",
                  "The Matrix"]
    movie_list.sort()

    def get_widget(self):
       # create a TreeStore with one string column to use as the model
        self.list_store = gtk.ListStore(str)
        for movie in self.movie_list:
            self.list_store.append([movie])

        # create the TreeViewColumn to display the data
        self.list_column = gtk.TreeViewColumn('Movies')

        # create a CellRendererText to render the data
        self.text_renderer = gtk.CellRendererText()

        # add the text_renderer to the tvcolumn and allow it to expand
        self.list_column.pack_start(self.text_renderer, True)

        # set the cell "text" attribute to column 0 - retrieve text
        # from that column in treestore
        self.list_column.set_attributes(self.text_renderer, text = 0)

        # create the TreeView using treestore
        self.treeview = gtk.TreeView(self.list_store)

        # add list_column to treeview
        self.treeview.append_column(self.list_column)

        # allow for multiple selections at once
        self.treeview.get_selection().set_mode(gtk.SELECTION_MULTIPLE)

        # make it searchable
        self.treeview.set_search_column(0)

        # Allow sorting on the column
        self.list_column.set_sort_column_id(0)

        # Allow drag and drop reordering of rows
        # self.treeview.set_reorderable(True)
    
        # Make the headers invisible to better resemble a WinForms ListBox
        self.treeview.set_headers_visible(False)

        return  self.treeview

if __name__=="__main__":
    basictv=BasicTreeview()
    basictv.main()
