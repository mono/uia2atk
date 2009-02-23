
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
        self.context_menu_strip = self.app.findWindow(None, checkShowing=False)
        # Menu items
        self.orig_item = self.context_menu_strip.findMenuItem('Item 1')
        self.radio_item = self.context_menu_strip.findMenuItem('Item 2')
        self.check_item = self.context_menu_strip.findMenuItem('Item 3')
        self.exit_item = self.context_menu_strip.findMenuItem('Exit')

    def quit(self):
        self.altF4()
