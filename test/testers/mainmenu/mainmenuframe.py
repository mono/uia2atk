# vim: set tabstop=4 shiftwidth=4 expandtab
##############################################################################
# Written by:  Ray Wang <rawang@novell.com>
# Date:        02/17/2008
# Description: Application wrapper for mainmenu.py
#              be called by ../mainmenu_basic_ops.py
##############################################################################

"""Application wrapper for mainmenu.py"""

from strongwind import *

class MainMenuFrame(accessibles.Frame):
    """the profile of the mainmenu sample"""

    def __init__(self, accessible):
        super(MainMenuFrame, self).__init__(accessible)
        self.mainmenu = self.findMenuBar(None)
        self.menuitem_file = self.findMenuItem("&File")
        self.menuitem_file_new = self.findMenuItem("&New")
        self.menuitem_file_new_doc = self.findMenuItem("&Document")
        self.menuitem_file_open = self.findMenuItem("&Open")
        self.menuitem_file_exit = self.findMenuItem("E&xit")

        self.menuitem_edit = self.findMenuItem("&Edit")
        self.menuitem_edit_undo = self.findMenuItem("&Undo")
        self.menuitem_edit_redo = self.findMenuItem("&Redo")

        self.menuitem_help = self.findMenuItem("&Help")
        self.menuitem_help_about = self.findMenuItem("&About")

        self.label = self.findLabel(None)

    def assertText(self, accessible, text=None):
        """assert text is equal to the input"""

        procedurelogger.expectedResult('%s text is "%s"' % \
                                                (accessible, accessible.text))
        assert accessible.text == text, '%s is not match with "%s"' % \
                                                (accessible, accessible.text)

    def assertSelectChild(self, accessible, index):
        """assert Selection implementation"""
        procedurelogger.action('select index %s in "%s"' % (index, accessible))

        accessible.selectChild(index)

    # close sample application after running the test
    def quit(self):
        self.altF4()
