
##############################################################################
# Written by:  Neville Gao <nevillegao@gmail.com>
# Date:        02/18/2009
# Description: contextmenu_menuitem.py wrapper script
#              Used by the contextmenu-*.py tests
##############################################################################

from strongwind import *


class ContextMenuFrame(accessibles.Frame):
    def __init__(self, accessible):
        super(ContextMenuFrame, self).__init__(accessible)
        self.label = self.findLabel(re.compile('^Right'))

    def click(self, button):
        button.click()

    def mClick(self, widget):
        widget.mouseClick(button=3)

    def assertWidgets(self):
        procedurelogger.action('Searching for all widgets in PageSetupDialog')
        procedurelogger.expectedResult('All widgets in PageSetupDialog should show up')

        # ContextMenu
        self.context_menu = self.app.findWindow(None, checkShowing=False)
        # Menu items
        self.orig_item = self.context_menu.findMenuItem('Item 1')
        self.radio_item = self.context_menu.findMenuItem('Item 2')
        self.check_item = self.context_menu.findMenuItem('Item 3')
        self.exit_item = self.context_menu.findMenuItem('Exit')

    def quit(self):
        self.altF4()
