#!/usr/bin/env python

# example gtkmenubar.py

import pygtk
pygtk.require('2.0')
import gtk

class MenuBar:
    def delete_event(self, widget, event, data=None):
        gtk.main_quit()
        return False

    def set_menu(self):

        # items in File 
        file_menu = gtk.Menu()
        open_item = gtk.MenuItem("_Open")
        save_item = gtk.MenuItem("_Save")
        quit_item = gtk.MenuItem("_Quit")
        file_menu.append(open_item)
        file_menu.append(save_item)
        file_menu.append(quit_item)
        # "File" entry on menubar
        file_item = gtk.MenuItem("_File")
        file_item.set_submenu(file_menu)

        # items in Help
        help_menu = gtk.Menu()
        about_item = gtk.MenuItem("_About")
        help_menu.append(about_item)
        # "Help" entry on menubar
        help_item = gtk.MenuItem("_Help")
        help_item.set_submenu(help_menu)

        # menubar
        self.menubar = gtk.MenuBar()
        self.menubar.append(file_item)
        self.menubar.append(help_item)
        self.menubar.show_all()

    def __init__(self):

        self.set_menu()

        # create window
        self.window = gtk.Window(gtk.WINDOW_TOPLEVEL)
        self.window.connect("delete_event", self.delete_event)
        self.window.set_title("Menu Bar")
        self.window.set_border_width(0)
        self.window.resize(200, 10)
        self.window.add(self.menubar)
        self.window.show()


def main():
    gtk.main()
    return 0       

if __name__ == "__main__":
    MenuBar()
    main()
