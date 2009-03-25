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

    def __init__(self, accessible):
        super(ComboBoxStyleChangesFrame, self).__init__(accessible)
        self.dropdownbutton = self.findPushButton(self.DROPDOWNBUTTON)
        self.dropdownlistbutton = self.findPushButton(self.DROPDOWNLISTBUTTON)
        self.simplebutton = self.findPushButton(self.SIMPLEBUTTON)

    # create some find<style> methods.  the test writer must call the
    # respective find<style> method before testing that style and after
    # testing any other style.
    def startDropDownStyle(self):
        self.dropdownbutton.click()
        sleep(config.SHORT_DELAY)
        self.label1 = self.findLabel(self.LABEL1)
        self.combobox = self.findComboBox(None)
        self.textbox = self.findText(None)

    def startDropDownListStyle(self):
        self.dropdownlistbutton.click()
        sleep(config.SHORT_DELAY)
        self.label1 = self.findLabel(self.LABEL1)
        self.combobox = self.findComboBox(None)
        self.menu = self.findMenu("", checkShowing=False)

    def startSimpleStyle(self):
        self.simplebutton.click()
        sleep(config.SHORT_DELAY)
        self.combobox = self.findComboBox(None)
        self.textbox = self.findText(None)
        self.menu = self.findMenu("", checkShowing=False)

    def assertOnlyOneComboBoxExists(self):
        procedurelogger.action('Make sure only one combo box accessible exists')
        procedurelogger.expectedResult('Only one combo box accessible exists')
        n_comboboxes = self.findAllComboBoxes(None)
        assert len(n_comboboxes) == 1,\
               "Only one combo box accessible should exist at a time"

    def findDropDownComboBoxChildren():
        '''
        Find only the immediate children of the combo box and assert that
        they are what we expect for the DropDown style
        '''
        procedurelogger.action('Make sure only one menu accessible exists')
        procedurelogger.expectedResult('Only one menu accessible exists')
        menus = self.combobox.findAllMenus(None)
        assert len(menus) == 1,\
               "Only one menu accessible should exist for the combo box"
        procedurelogger.action('Make sure only one text accessible exists')
        procedurelogger.expectedResult('Only one text accessible exists')
        texts = self.combobox.findAllTexts(None)
        assert len(texts) == 1,\
               "Only one text accessible should exist for the combo box"
        
    def assertLabelText(self, acc, expected_text):
        procedurelogger.action('Ensure %s contains the expected text' % acc)
        actual_text = acc.text
        procedurelogger.expectedResult('Actual text "%s" matches expected text "%s"' % (actual_text, expected_text))
        assert actual_text == expected_text,\
               'Actual text "%s" does not match the expected text "%s"' % \
               (actual_text, expected_text)

    def quit(self):
        self.altF4()
