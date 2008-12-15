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
        self.toolstripcombobox = self.findComboBox(None)

    def press(self, accessible):
        accessible.press()
        self.toolstripcombobox_menu = self.findMenu(str(self.MENU))
        #self.toolstripcombobox_menuitem_6 = self.findMenuItem(self.MENUITEM_6)
        #self.toolstripcombobox_menuitem_8 = self.findMenuItem(self.MENUITEM_8)
        #self.toolstripcombobox_menuitem_10 = self.findMenuItem(self.MENUITEM_10)
        #self.toolstripcombobox_menuitem_12 = self.findMenuItem(self.MENUITEM_12)
        #self.toolstripcombobox_menuitem_14 = self.findMenuItem(self.MENUITEM_14)

    def assertText(self, accessible, text=None):
        """assert text is equal to the input"""

        procedurelogger.action('Assert the text of %s' % accessible)
        procedurelogger.expectedResult('%s text is "%s"' % \
                                                (accessible, accessible.text))
        assert accessible.text == text, '%s text is not match with "%s"' % \
                                                (accessible, accessible.text)

    def assertSelectionChild(self, accessible, childIndex):
        """assert Selection implementation"""
        procedurelogger.action('select childIndex %s in "%s"' % \
                                        (childIndex, accessible))
        accessible.selectChild(childIndex)

    def quit(self):
        self.altF4()
