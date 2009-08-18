
##############################################################################
# Written by:  Neville Gao <nevillegao@gmail.com>
# Date:        02/18/2009
# Description: contextmenu_menuitem.py wrapper script
#              Used by the contextmenu-*.py tests
##############################################################################

from strongwind import *
from helpers import *

class ContextMenuFrame(accessibles.Frame):
    def __init__(self, accessible):
        super(ContextMenuFrame, self).__init__(accessible)
        self.label = self.findLabel(re.compile('^Right'))

    def assertWidgets(self):
        # ContextMenu
        self.context_menu = self.app.findWindow(None, checkShowing=False)

        procedurelogger.action('Searching for all widgets in ContextMenu')
        procedurelogger.expectedResult('All widgets in ContextMenu should show up')

        # Menu items
        self.menu = self.context_menu.findMenu(None)
        self.normal_item = self.context_menu.findMenuItem('Item 1')
        self.radio_item = self.context_menu.findMenuItem('Item 2')
        self.check_item = self.context_menu.findMenuItem('Item 3')
        self.exit_item = self.context_menu.findMenuItem('Exit')

    def selectChildAndCheckStates(self, accessible, childIndex, add_states=[], 
                                    invalid_states=[]):
        """
        Select the child at childIndex and then assert that the appropriate
        accessible was indeed selected.
        """
        procedurelogger.action('Select child at index %s in "%s"' % \
                                                (childIndex, accessible))
        procedurelogger.expectedResult('Child at index %s is selected and has the appropriate states' % childIndex)

        accessible.selectChild(childIndex)
        sleep(config.SHORT_DELAY)
        statesCheck(accessible.getChildAtIndex(childIndex), "MenuItem", invalid_states, add_states)

    def quit(self):
        self.altF4()
