#!/usr/bin/env python

# example gtkimage.py

import pygtk
pygtk.require('2.0')
import gtk

class Image:
    def delete_event(self, widget, event, data=None):
        gtk.main_quit()
        return False

    def clicked(self, widget):
        self.switch_image()

    def switch_image(self):
        self.current_image = (self.current_image + 1) % 2
        self.image.set_from_file(self.images[self.current_image])

    def __init__(self):
        self.window = gtk.Window(gtk.WINDOW_TOPLEVEL)
        self.window.set_title("Image")
        self.window.connect("delete_event", self.delete_event)
        self.window.set_border_width(12)

        vbox = gtk.VBox()
        vbox.set_spacing(6)

        self.current_image = 0
        self.images = ("universe.jpg", "desktop-blue_soccer.jpg")

        self.image = gtk.Image()
        self.switch_image()
        vbox.pack_start(self.image, True, True, 0)

        button = gtk.Button("Swap Image")
        button.connect("clicked", self.clicked)
        vbox.pack_start(button, False, False, 0)

        self.window.add(vbox)

        vbox.show_all()
        self.window.show()

def main():
    gtk.main()
    return 0       

if __name__ == "__main__":
    Image()
    main()
