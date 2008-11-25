#!/usr/bin/env python
import gtk

def main():
    win = gtk.Window()
    win.set_title("Main Window")
    win.connect("destroy", lambda win: gtk.main_quit())

    vbox = gtk.VBox()
    win.add(vbox)
    vbox.show()

    label = gtk.Label("This is a label")
    vbox.pack_start(label, False)
    label.show()

    ls = gtk.List()
    ls.set_name("bbb")
    selection = ls.get_selection()
    ls.show()
    vbox.pack_start(ls, True)
    
    win.set_default_size (200,300)
    win.show_all()
    gtk.main()
    
if __name__ == '__main__':
    main()
			
