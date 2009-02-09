#!/usr/bin/env python

# example gtkmenubar.py

import pygtk
pygtk.require('2.0')
import gtk
import gobject

class MenuBar:
    def delete_event(self, widget, event, data=None):
        gtk.main_quit()
        return False

    def after_timeout(self):
        self.file_item.get_accessible().add_selection (3)

    def open_activate(self, widget):
        self.label.set_text("You selected Open")

    def quit_activate(self, widget):
        self.label.set_text("You selected Quit")

    def file_activate(self, widget):
        self.label.set_text("You selected File")
        gobject.timeout_add (3000, self.after_timeout)

    def set_menu(self):

        # items in File 
        file_menu = gtk.Menu()
        open_item = gtk.MenuItem("_Open")
        open_recent_item = gtk.MenuItem("Open _Recent")
        save_item = gtk.MenuItem("_Save")
        quit_item = gtk.MenuItem("_Quit")
        open_recent_item_menu = gtk.Menu()
        open_recent_item_menu.append(gtk.MenuItem("foo"))
        open_recent_item_menu.append(gtk.MenuItem("bar"))
        open_recent_item.set_submenu(open_recent_item_menu)

        file_menu.append(open_item)
        file_menu.append(open_recent_item)
        file_menu.append(save_item)
        file_menu.append(quit_item)
        open_item.connect("activate", self.open_activate)
        quit_item.connect("activate", self.quit_activate)

        # "File" entry on menubar
        self.file_item = gtk.MenuItem("_File")
        self.file_item.set_submenu(file_menu)
        self.file_item.connect("activate", self.file_activate)

        # items in Help
        help_menu = gtk.Menu()
        about_item = gtk.MenuItem("_About")
        help_menu.append(about_item)
        # "Help" entry on menubar
        help_item = gtk.MenuItem("_Help")
        help_item.set_submenu(help_menu)

        # menubar
        self.menubar = gtk.MenuBar()
        self.menubar.append(self.file_item)
        self.menubar.append(help_item)
        self.menubar.show_all()

    def set_label(self):
        self.label = gtk.Label("You Select:")

    def __init__(self):

        self.set_menu()
        self.set_label()

        # create window
        self.window = gtk.Window(gtk.WINDOW_TOPLEVEL)
        self.window.connect("delete_event", self.delete_event)
        self.window.set_title("Menu Bar")
        self.window.set_border_width(0)
        self.window.resize(200, 100)
        self.box1 = gtk.VBox(False, 0)
        self.hbox = gtk.HBox(False, 0)
        self.window.add(self.box1)
        self.hbox.pack_start(self.menubar, True, True, 0)
        self.menubar.show()
        self.box1.pack_start(self.hbox, True, True, 0)
        self.hbox.show()
        self.box1.pack_start(self.label, True, True, 0)
        self.label.show()
        #self.window.add(self.menubar)
        self.box1.show()
        self.window.show()


def main():
    gtk.main()
    return 0       

if __name__ == "__main__":
    MenuBar()
    main()
