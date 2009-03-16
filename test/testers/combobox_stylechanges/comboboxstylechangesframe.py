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

    def quit(self):
        self.altF4()
