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
        self.label = self.findLabel(None)
        self.menuitem_file = self.findMenu("File")
        self.menuitem_file_new = self.findMenu("New", checkShowing=False)
        self.menuitem_file_new_doc = \
                              self.findMenuItem("Document", checkShowing=False)
        self.menuitem_file_open = self.findMenuItem("Open", checkShowing=False)
        self.menuitem_edit = self.findMenu("Edit")
        self.menuitem_edit_copy = self.findMenuItem("Copy", checkShowing=False)
        self.menuitem_edit_paste = \
                                 self.findMenuItem("Paste", checkShowing=False)

    def assertUneditableText(self, accessible, text):
        procedurelogger.action('set %s text to "%s"' % (accessible, text))
        try:
            # this will call queryEditableText() on the accessible
            accessible.text = text
        except NotImplementedError:
            return
        assert False, '"%s" text should not be editable' % accessible

    def assertText(self, accessible, expected_text):
        """assert that the accessible text is equal to the expected text"""
        procedurelogger.expectedResult('"%s" text is "%s"' % \
                                                (accessible, expected_text))
        assert accessible.text == expected_text, \
                                            'Text was "%s", expected "%s"' % \
                                            (accessible.text, expected_text)

    def selectChild(self, accessible, index):
        """call Strongwind's selectChild(index) on accessible"""
        procedurelogger.action('select index %s in "%s"' % (index, accessible))
        accessible.selectChild(index)

    def assertImage(self, accessible, expected_width, expected_height):
        procedurelogger.action('assert the image size of "%s"' % accessible)
        actual_width, actual_height = \
                             accessible._accessible.queryImage().getImageSize()
        procedurelogger.expectedResult('"%s" image size is %s x %s' % \
                                 (accessible, expected_width, expected_height))

        assert actual_width == expected_width, "%s (%s), %s (%s)" % \
                                            ("expected width",
                                              expected_width,
                                             "does not match the actual width",
                                              actual_width)
        assert actual_height == expected_height, "%s (%s), %s (%s)" % \
                                           ("expected height",
                                            expected_height,
                                            "does not match the actual height",
                                            actual_height)

    # close sample application after running the test
    def quit(self):
        self.altF4()
