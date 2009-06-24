# vim: set tabstop=4 shiftwidth=4 expandtab
##############################################################################
# Written by:  Brian G. Merrell <bgmerrell@novell.com>
# Date:        03/12/2009
# Description: Application wrapper for combobox_stylechanges.py
##############################################################################

"""Application wrapper for combobox_stylechanges.py"""

from strongwind import *
import time

class ComboBoxStyleChangesFrame(accessibles.Frame):

    # constants
    # the available widgets on the window
    LABEL1 = "You select 1"
    DROPDOWNBUTTON = "DropDown"
    DROPDOWNLISTBUTTON = "DropDownList"
    SIMPLEBUTTON = "Simple"
    X10BUTTON = "Toggle x10"

    def __init__(self, accessible):
        super(ComboBoxStyleChangesFrame, self).__init__(accessible)
        self.dropdownbutton = self.findPushButton(self.DROPDOWNBUTTON)
        self.dropdownlistbutton = self.findPushButton(self.DROPDOWNLISTBUTTON)
        self.simplebutton = self.findPushButton(self.SIMPLEBUTTON)
        self.x10button = self.findPushButton(self.X10BUTTON)

    # create some find<style> methods.  the test writer must call the
    # respective find<style> method before testing that style and after
    # testing any other style.
    def startDropDownStyle(self):
        self.dropdownbutton.click()
        sleep(config.SHORT_DELAY)
        self.label1 = self.findLabel(self.LABEL1)
        self.findSingleComboBox()
        self.findSingleMenuChild()
        self.findSingleTextChild()

    def startDropDownListStyle(self):
        self.dropdownlistbutton.click()
        sleep(config.SHORT_DELAY)
        self.combobox = self.findSingleComboBox()
        self.label1 = self.findLabel(self.LABEL1)
        self.findSingleComboBox()
        self.findSingleMenuChild()
        self.assertNoTextChild()

    def startSimpleStyle(self):
        self.simplebutton.click()
        sleep(config.SHORT_DELAY)
        self.findSingleComboBox()
        self.findSingleMenuChild()
        self.findSingleTextChild()

    def findSingleComboBox(self):
        '''
        Return the combo box accessible, raise an error is more than one
        combo box accessible exists
        '''
        procedurelogger.action('Make sure only one combo box accessible exists')
        procedurelogger.expectedResult('Only one combo box accessible exists')
        comboboxes = self.findAllComboBoxes(None)
        assert len(comboboxes) == 1,\
               "Only one combo box accessible should exist at a time"
        self.combobox = comboboxes[0]

    def findSingleMenuChild(self):
        '''
        Find a menu that is a child of the combo box accessible and make
        sure there is only one menu.
        '''
        procedurelogger.action('Make sure only one menu accessible exists')
        procedurelogger.expectedResult('Only one menu accessible exists')
        menus = self.combobox.findAllMenus(None, checkShowing=False)
        assert len(menus) == 1,\
               "Only one menu accessible should exist for the combo box"
        self.menu = menus[0]

    def findSingleTextChild(self):
        '''
        Find a "text" accessible that is a child of the combo box accessible
        and make sure there is only one "text" accessible.
        '''
        procedurelogger.action('Make sure only one text accessible exists')
        procedurelogger.expectedResult('Only one text accessible exists')
        texts = self.combobox.findAllTexts(None)
        assert len(texts) == 1,\
               "Only one text accessible should exist for the combo box"
        self.text_accessible = texts[0]

    def assertNoTextChild(self):
        '''
        Ensure that the combo box accessible has no child that is a "text"
        accessible
        '''
        procedurelogger.action('Check to see if %s has a child that is a "text" accessible' % self.combobox)
        procedurelogger.expectedResult('No such accessible exists')
        text = self.combobox.findAllTexts(None, checkShowing=False)
        assert len(text) == 0, \
               '%s should not have a "text" accessible' % self.combobox
        
        
    def assertLabelText(self, acc, expected_text):
        procedurelogger.action('Ensure %s contains the expected text' % acc)
        actual_text = acc.text
        procedurelogger.expectedResult('Actual text "%s" matches expected text "%s"' % (actual_text, expected_text))
        assert actual_text == expected_text,\
               'Actual text "%s" does not match the expected text "%s"' % \
               (actual_text, expected_text)

    def assertComboBoxItems(self, is_x10=False, is_simple_style=False):
        '''
        Ensure that only the correct combo box items are present and that they
        are the only such items in existence
        '''
        procedurelogger.action('Ensure only the correct combo box items exist')
        procedurelogger.expectedResult('Only the %s items exist' %\
                                    ("0-9 x10" if is_x10 else "default (0-9)"))
        if is_simple_style:
            items = self.menu.findAllTableCells(None, checkShowing=False)
        else:
            items = self.menu.findAllMenuItems(None, checkShowing=False)
        item_names = [item.name for item in items]
        n_items = len(items)
        assert n_items == 10,\
               "Exactly 10 items should exist, not %d" % n_items
        if is_x10:
            default_item_names = \
                                     [str(n*10) for n in [0,1,2,3,4,5,6,7,8,9]]
        else:
            default_item_names = [str(n) for n in [0,1,2,3,4,5,6,7,8,9]]
        assert item_names == default_item_names,\
               "Actual items %s, differs from default items %s" %\
               (item_names, default_item_names)

    def quit(self):
        self.altF4()
