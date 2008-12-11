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
    item1 = gtk.ListItem("item1")
    item2 = gtk.ListItem("item2")
    item3 = gtk.ListItem("item3")
    item4 = gtk.ListItem("item4")
    item5 = gtk.ListItem("item5")
    ls.append_items([item1, item2, item3, item4, item5])

    selection = ls.get_selection()
    ls.show()
    vbox.pack_start(ls, True)
    
    win.set_default_size (200,300)
    win.show_all()
    gtk.main()
    
if __name__ == '__main__':
    main()
			
