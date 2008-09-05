#!/usr/bin/env python

# example gtkcalendar.py

import pygtk
pygtk.require('2.0')
import gtk

class Calendar:
    def delete_event(self, widget, event, data=None):
        gtk.main_quit()
        return False

    def __init__(self):
        self.window = gtk.Window(gtk.WINDOW_TOPLEVEL)
        self.window.set_title("Calendar")
        self.window.connect("delete_event", self.delete_event)
        self.window.set_border_width(12)

        calendar = gtk.Calendar()
        self.window.add(calendar)

        calendar.show_all()
        self.window.show()

def main():
    gtk.main()
    return 0       

if __name__ == "__main__":
    Calendar()
    main()
