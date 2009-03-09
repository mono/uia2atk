# vim: set tabstop=4 shiftwidth=4 expandtab
##############################################################################
# Written by:  Ray Wang <rawang@novell.com>
# Date:        12/10/2008
# Description: Application wrapper for toolstripcombobox.py
#              be called by ../toolstripcombobox_basic_ops.py
##############################################################################

"""Application wrapper for toolstripcombobox.py"""

from strongwind import *

class ToolStripComboBoxFrame(accessibles.Frame):
    """the profile of the toolstripcombobox sample"""

    MENU = '8'
    MENUITEM_6 = '6'
    MENUITEM_8 = '8'
    MENUITEM_10 = '10'
    MENUITEM_12 = '12'
    MENUITEM_14 = '14'

    def __init__(self, accessible):
        super(ToolStripComboBoxFrame, self).__init__(accessible)
        self.label = self.findLabel("Please Select one Font Size from the ComboxBox")
        self.toolbar = self.findToolBar(None)
        self.toolstripcombobox = self.findComboBox(None)

    def press(self, accessible):
        accessible.press()
        self.menu = self.findMenu(None)
        self.menuitem_6 = self.findMenuItem(self.MENUITEM_6)
        self.menuitem_8 = self.findMenuItem(self.MENUITEM_8)
        self.menuitem_10 = self.findMenuItem(self.MENUITEM_10)
        self.menuitem_12 = self.findMenuItem(self.MENUITEM_12)
        self.menuitem_14 = self.findMenuItem(self.MENUITEM_14)

    def inputText(self, accessible, text):
        try:
            procedurelogger.action('input %s to %s' % (text, accessible))
            accessible.text = text
        except NotImplementedError:
            pass

    def assertText(self, accessible, text=None):
        """assert text is equal to the input"""

        procedurelogger.action('Assert the text of %s' % accessible)
        procedurelogger.expectedResult('%s text is "%s"' % \
                                                (accessible, accessible.text))
        assert accessible.text == text, '%s text is not match with "%s"' % \
                                                (accessible, accessible.text)

    def selectChild(self, accessible, index):
        """assert Selection implementation"""
        procedurelogger.action('select index %s in "%s"' % \
                                        (index, accessible))
        accessible.selectChild(index)

    def quit(self):
        self.altF4()
