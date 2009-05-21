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

    MENUITEM_6 = '6'
    MENUITEM_8 = '8'
    MENUITEM_10 = '10'
    MENUITEM_12 = '12'
    MENUITEM_14 = '14'
    LABEL_TEXT = 'Please Select one Font Size from the ComboBox'

    def __init__(self, accessible):
        super(ToolStripComboBoxFrame, self).__init__(accessible)
        self.label = self.findLabel(self.LABEL_TEXT)
        self.toolbar = self.findToolBar(None)
        self.toolstripcombobox = self.findComboBox(None)
        self.menu = self.findMenu(None, checkShowing=False)
        self.menuitem_6 = self.findMenuItem(self.MENUITEM_6, checkShowing=False)
        self.menuitem_8 = self.findMenuItem(self.MENUITEM_8, checkShowing=False)
        self.menuitem_10 = self.findMenuItem(self.MENUITEM_10, checkShowing=False)
        self.menuitem_12 = self.findMenuItem(self.MENUITEM_12, checkShowing=False)
        self.menuitem_14 = self.findMenuItem(self.MENUITEM_14, checkShowing=False)

    def click(self, accessible):
        """wrap strongwind click action to add log"""
        procedurelogger.action('click %s' % accessible)
        accessible.click()

    def press(self, accessible):
        """wrap strongwind press action to add log"""
        procedurelogger.action('press %s' % accessible)
        accessible.press()

    def editableTextIsNotImplemented(self, accessible):
        """assert accessible's EditableText is not implemented"""
        procedurelogger.action('check EditableText for %s' % accessible)

        procedurelogger.expectedResult('EditableText is not implemented for %s' % accessible)
        try:
            accessible._accessible.queryEditableText()
        except NotImplementedError:
            return
        assert False, "EditableText shouldn't be implemented"

    def assertText(self, accessible, expected_text=None):
        """assert text is equal to the expected_text"""
        procedurelogger.expectedResult('%s\'s text is "%s"' % \
                                                (accessible, expected_text))
        assert accessible.text == expected_text, \
                           'the actual text is %s, the expected text is %s' % \
                                               (accessible.text, expected_text)

    def selectChild(self, accessible, index):
        """assert Selection implementation"""
        procedurelogger.action('select index %s in "%s"' % (index, accessible))
        accessible.selectChild(index)

    def quit(self):
        self.altF4()
