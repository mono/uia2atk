
##############################################################################
# Written by:  Neville Gao <nevillegao@gmail.com>
# Date:        02/18/2009
# Description: contextmenustrip.py wrapper script
#              Used by the contextmenustrip-*.py tests
##############################################################################

from strongwind import *


class ContextMenuStripFrame(accessibles.Frame):
    def __init__(self, accessible):
        super(ContextMenuStripFrame, self).__init__(accessible)
        self.label = self.findLabel(re.compile('^Right'))

    def assertWidgets(self):
        procedurelogger.action('Searching for all widgets in ContextMenuStrip')
        procedurelogger.expectedResult('All widgets in ContextMenuStrip should show up')

        # ContextMenuStrip
	self.label = self.findLabel ('Right Click on me to see ContextMenuStrip');
	# Simulate a mouse click to make the menu appear
	self.label.mouseClick (3)
        self.context_menu_strip = self.app.findWindow(None, checkShowing=False)
        # Menu items
	self.menu = self.context_menu_strip.findMenu (None);
        self.item1 = self.menu.findMenu ('Apple')
        self.item1a = self.item1.findMenuItem ('Macintosh', checkShowing = False)
        self.item1b = self.item1.findMenuItem ('Delicious', checkShowing = False)
        self.item2 = self.menu.findMenuItem ('Banana')
        self.item3 = self.menu.findMenuItem ('Watermelon')
        self.item4 = self.menu.findMenuItem ('Orange')
        self.item5 = self.menu.findMenuItem ('Peach')

    def quit(self):
        self.altF4()
