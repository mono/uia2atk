#!/usr/bin/env python
import gtk
import gobject

COLUMN_NUMBER = 0
COLUMN_STRING = 1
COLUMN_BOOL = 2

data = [
    [ 1, 'first row', True ],
    [ 2, 'second row', True ],
    [ 3, 'third row', True ],
    [ 4, 'fourth row', True ],
    [ 5, 'fifth row', True ],
    [ 6, 'sixth row', True ],
    [ 7, 'seventh row', None ],
    [ 8, 'eigth row', None],
    [ 9, 'ninth row', None ],
    [ 10, 'tenth row', None ],
    [ 1, 'first row', False ],
    [ 2, 'second row', False ],
    [ 3, 'third row', False],
    [ 4, 'fourth row', False ],
    [ 5, 'fifth row', False ]
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

    ls = gtk.ListStore(gobject.TYPE_UINT, gobject.TYPE_STRING, gobject.TYPE_BOOLEAN)
    for item in data:
        iter = ls.append()
        ls.set(iter, COLUMN_NUMBER, item[0],
                 COLUMN_STRING, item[1], COLUMN_BOOL, item[2])
    tv = gtk.TreeView(ls)
    tv.set_rules_hint(True)
    sw.add(tv)
    tv.show()

    renderer = gtk.CellRendererText()
    col = gtk.TreeViewColumn('Number', renderer, text=COLUMN_NUMBER)
    col.set_sort_column_id(COLUMN_NUMBER)
    tv.append_column(col)
    
    renderer1 = gtk.CellRendererText()
    renderer1.set_property('editable', True)
    renderer1.connect('edited', col_edited, ls)
    col = gtk.TreeViewColumn('String', renderer1, text=COLUMN_STRING)
    col.set_sort_column_id(COLUMN_STRING)
    tv.append_column(col)

    renderer2 = gtk.CellRendererToggle()
    renderer2.set_property('activatable', True)
    renderer2.connect('toggled', col_toggled, ls)
    col  = gtk.TreeViewColumn('Boolean', renderer2)
    col.add_attribute(renderer2, 'active', 2)
    col.set_sort_column_id(COLUMN_BOOL)
    tv.append_column(col)
    
    win.set_default_size (200,300)
    sw.show()
    win.show_all()
    gtk.main()

def col_edited(cell, path, new_text, ls):
    ls[path][1] = new_text

def col_toggled(cell, path, ls):
    ls[path][2] = not ls[path][2]
    
if __name__ == '__main__':
    main()
			
