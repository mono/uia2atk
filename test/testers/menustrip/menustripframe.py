# vim: set tabstop=4 shiftwidth=4 expandtab
##############################################################################
# Written by:  Ray Wang <rawang@novell.com>
# Date:        11/19/2008
# Description: Application wrapper for menustrip.py
#              be called by ../menustrip_basic_ops.py
##############################################################################

"""Application wrapper for menustrip.py"""

from strongwind import *

class MenuStripFrame(accessibles.Frame):
    """the profile of the menustrip sample"""

    def __init__(self, accessible):
        super(MenuStripFrame, self).__init__(accessible)
        self.menustrip = self.findMenuBar(None)
        self.menuitem_file = self.findMenu("File")
        self.menuitem_file_new = self.findMenu("New", checkShowing=False)
        self.menuitem_file_new_doc = self.findMenuItem("Document", checkShowing=False)
        self.menuitem_file_open = self.findMenuItem("Open", checkShowing=False)
        self.menuitem_edit = self.findMenu("Edit")
        self.menuitem_edit_copy = self.findMenuItem("Copy", checkShowing=False)
        self.menuitem_edit_paste = self.findMenuItem("Paste", checkShowing=False)
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

    def assertImage(self, accessible, width=None, height=None):
        procedurelogger.action("assert %s's image size" % accessible)
        size = accessible._accessible.queryImage().getImageSize()
        procedurelogger.expectedResult('"%s" image size is %s x %s' %
                                                  (accessible, width, height))

        assert width == size[0], "%s (%s), %s (%s)" % \
                                            ("expected width",
                                              width,
                                             "does not match the actual width",
                                              size[0])
        assert height == size[1], "%s (%s), %s (%s)" % \
                                            ("expected height",
                                              height,
                                             "does not match the actual height",
                                              size[1])

    def clearSelection(self, accessible):
        procedurelogger.action('clear selection in "%s"' % (accessible))
        accessible.clearSelection()

    # close sample application after running the test
    def quit(self):
        self.altF4()
