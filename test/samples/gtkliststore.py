#!/usr/bin/env python
import gtk
import gobject

COLUMN_NUMBER = 0
COLUMN_STRING = 1

data = [
    [ 1, 'first row' ],
    [ 2, 'second row' ],
    [ 3, 'third row' ],
    [ 4, 'fourth row' ],
    [ 5, 'fifth row' ],
    [ 6, 'sixth row' ],
    [ 7, 'seventh row' ],
    [ 8, 'eigth row' ],
    [ 9, 'ninth row' ],
    [ 10, 'tenth row' ],
    [ 1, 'first row' ],
    [ 2, 'second row' ],
    [ 3, 'third row' ],
    [ 4, 'fourth row' ],
    [ 5, 'fifth row' ]
    ]

def list_selections(param):
    print param

def selection_cb(selection):
    print selection
    selection.selected_foreach(list_selections)

def main():
    win = gtk.Window()
    win.set_title("Main Window")
    win.connect("destroy", lambda win: gtk.main_quit())

    vbox = gtk.VBox()
    win.add(vbox)
    vbox.show()

    label = gtk.Label("This is a label")
    vbox.pack_start(label, expand = False)
    label.show()

    sw = gtk.ScrolledWindow(None, None)
    sw.set_policy(gtk.POLICY_NEVER, gtk.POLICY_AUTOMATIC)
    sw.set_shadow_type(gtk.SHADOW_ETCHED_IN)
    vbox.pack_start(sw, expand = True)

    ls = gtk.ListStore(gobject.TYPE_UINT, gobject.TYPE_STRING)
    for item in data:
        iter = ls.append()
        ls.set(iter, COLUMN_NUMBER, item[0],
                 COLUMN_STRING, item[1])
    tv = gtk.TreeView(ls)
    tv.set_rules_hint(True)
    tv.set_search_column(COLUMN_STRING)
    selection = tv.get_selection()
    selection.set_mode(gtk.SELECTION_MULTIPLE)
    selection.connect("changed", selection_cb)
    sw.add(tv)
    tv.show()

    renderer = gtk.CellRendererText()
    col = gtk.TreeViewColumn('Number', renderer, text=COLUMN_NUMBER)
    col.set_sort_column_id(COLUMN_NUMBER)
    tv.append_column(col)
    
    renderer = gtk.CellRendererText()
    col = gtk.TreeViewColumn('String', renderer, text=COLUMN_STRING)
    col.set_sort_column_id(COLUMN_STRING)
    tv.append_column(col)
    
    win.set_default_size (200,300)
    sw.show()
    win.show_all()
    gtk.main()
    
if __name__ == '__main__':
    main()
			
