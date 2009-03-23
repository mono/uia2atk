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
        self.menuitem_file = self.findMenu("File")
        self.menuitem_file_new = self.findMenu("New", checkShowing=False)
        self.menuitem_file_new_doc = self.findMenuItem("Document", checkShowing=False)
        self.menuitem_file_open = self.findMenuItem("Open", checkShowing=False)
        self.menuitem_file_exit = self.findMenuItem("Exit", checkShowing=False)

        self.menuitem_edit = self.findMenu("Edit")
        self.menuitem_edit_undo = self.findMenuItem("Undo", checkShowing=False)
        self.menuitem_edit_redo = self.findMenuItem("Redo", checkShowing=False)

        self.menuitem_help = self.findMenu("Help")
        self.menuitem_help_about = self.findMenuItem("About", checkShowing=False)

        self.label = self.findLabel(None)

    def click(self, accessible):
        procedurelogger.action("click %s" % accessible)
        accessible.click()

    def inputText(self, accessible, text=None):
        procedurelogger.action('set %s text to "%s"' % (accessible, text))
        try:
            accessible.text = text
        except NotImplementedError:
            pass

    def assertText(self, accessible, text=None):
        """assert text is equal to the input"""

        procedurelogger.expectedResult('%s text is "%s"' % \
                                                (accessible, accessible.text))
        assert accessible.text == text, '%s is not match with "%s"' % \
                                                (accessible, accessible.text)

    def selectChild(self, accessible, index):
        """assert Selection implementation"""
        procedurelogger.action('select index %s in "%s"' % (index, accessible))

        accessible.selectChild(index)

    # close sample application after running the test
    def quit(self):
        self.altF4()
