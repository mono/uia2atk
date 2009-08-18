#!/usr/bin/env python

# example gtkfontchooser.py

import pygtk
pygtk.require('2.0')
import gtk

class FontChooser:

    def __init__(self):
        fontchooser = gtk.FontSelectionDialog("my title")
        fontchooser.show_all()

def main():
    gtk.main()
    return 0

if __name__ == "__main__":
    FontChooser()
    main()
