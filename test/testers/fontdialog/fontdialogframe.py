
##############################################################################
# Written by:  Cachen Chen <cachen@novell.com>
# Date:        02/20/2009
# Description: fontdialog.py wrapper script
#              Used by the fontdialog-*.py tests
##############################################################################$
"""
The great part of widgets are inherit from other controls except color_ComboBox 
that is internal class, So the major point in this test may focus on 
color_ComboBox.

"""

import sys
import os
import actions
import states

from strongwind import *
from fontdialog import *
from sys import path
from helpers import *

# class to represent the main window.
class FontDialogFrame(accessibles.Frame):

    # constants
    # the available widgets on the window
    BUTTON1 = "Click me"

    def __init__(self, accessible):
        super(FontDialogFrame, self).__init__(accessible)
        self.fontdialog_button = self.findPushButton(self.BUTTON1)

    # do click action
    def click(self, button):
        procedurelogger.action("click %s" % button)
        button.click()

    # assert if all widgets on Font dialog are showing
    def AssertWidgets(self):
        procedurelogger.action("search for all widgets from FontDialog windows")
        # Font Dialog is appeared
        self.fontdialog = self.app.findDialog("Font")
        # there are 4 Labels in dialog
        procedurelogger.expectedResult("4 Labels are showing up")
        self.font_label = self.fontdialog.findLabel("Font:")
        self.fontstyle_label = self.fontdialog.findLabel("Font Style:")
        self.size_label = self.fontdialog.findLabel("Size:")
        self.script_label = self.fontdialog.findLabel("Script:")
        # there are 4 push buttons in dialog
        procedurelogger.expectedResult("4 PushButtons are showing up")
        self.ok_button = self.fontdialog.findPushButton("OK")
        self.cancel_button = self.fontdialog.findPushButton("Cancel")
        self.apply_button = self.fontdialog.findPushButton("Apply")
        self.help_button = self.fontdialog.findPushButton("Help")
        # there are 3 TextBoxs in dialog
        procedurelogger.expectedResult("3 TextBoxs are showing up")
        self.texts = self.fontdialog.findAllTexts(None)
        self.size_text = self.texts[0]
        self.fontstyle_text = self.texts[1]
        self.font_text = self.texts[2]
        # there are 3 ListBoxs in dialog
        procedurelogger.expectedResult("3 ListBoxs are showing up")
        self.treetables = self.fontdialog.findAllTreeTables(None)
        self.size_treetable = self.treetables[0]
        self.fontstyle_treetable = self.treetables[1]
        self.font_treetable = self.treetables[2]
        # each treetable have some TableCells
        self.size_tablecell = self.size_treetable.findAllTableCells(None, checkShowing = False)
        self.fontstyle_tablecell = self.fontstyle_treetable.findAllTableCells(None, checkShowing = False)
        self.font_tablecell = self.font_treetable.findAllTableCells(None, checkShowing = False)

        procedurelogger.expectedResult("%s tablecell under size ListBox, \
                                       and %s tablecell under fontstyle ListBox, \
                                       and %s tablecell under font ListBox" % (len(self.size_tablecell), len(self.fontstyle_tablecell), len(self.font_tablecell)))
        # different numbers of Font with the different machine
        assert len(self.size_tablecell) > 0 and  \
               len(self.fontstyle_tablecell) > 0 and \
               len(self.font_tablecell) > 0, "missing TableCell"

        # there are 2 GroupBox
        self.effects_groupbox = self.fontdialog.findPanel("Effects")
        self.example_groupbox = self.fontdialog.findPanel("Example")
        # there are 2 CheckBox in Effects groupbox, 1 panel in Example groupbox
        self.strikethrough_checkbox = self.effects_groupbox.findCheckBox("Strikethrough")
        self.underlined_checkbox = self.effects_groupbox.findCheckBox("Underlined")
        self.example_panel = self.example_groupbox.findPanel(None)
        # there is 1 script ComboBox in dialog
        self.script_combobox = self.fontdialog.findComboBox(None)
        self.script_menuitems = self.script_combobox.findAllMenuItems(None, checkShowing = False)

        assert len(self.script_menuitems) == 25, "missing MenuItem"

    # color ComboBox and sub children test
    def colorComboBoxTest(self):
        """
        Wrap tests for color ComboBox and its sub children;
        Search for all accessibles; Check states for all accessibles; 
        Check AtkText for MenuItems; Make sure AtkAction can work well

        """
        # search for color combobox and its sub children
        procedurelogger.action("search for color combobox and sub children in Effects GroupBox")

        procedurelogger.expectedResult("color combobox and sub children are showing up")
        self.color_combobox = self.effects_groupbox.findComboBox(None)
        self.color_menu = self.color_combobox.findMenu(None, checkShowing=False)
        self.color_menuitems = self.color_combobox.findAllMenuItems(None, checkShowing=False)

        assert len(self.color_menuitems) == 16, "missing menuitem"

        # check states that Menu and MenuItems are not showing except MenuItem[0]  
        # because its selected by default before press combobox
        statesCheck(self.color_combobox, "ComboBox")
        statesCheck(self.color_menu, "Menu", invalid_states=["visible", "showing"])
        statesCheck(self.color_menuitems[0], "MenuItem", add_states=["focused", "selected"])
        for i in range(1,16):
            statesCheck(self.color_menuitems[i], "MenuItem", invalid_states=["showing"])

        # make sure actual texts of all menuitem are equal to expected texts
        actual_texts = []
        expected_texts = [
		"Black",
		"Dark-Red",
		"Green",
		"Olive-Green",
		"Aquamarine",
		"Crimson",
		"Cyan",
		"Gray",
		"Silver",
		"Red",
		"Yellow-Green",
		"Yellow",
		"Blue",
		"Purple",
		"White"
		]
        for i in range(16):
            actual_texts.append(self.color_menuitems[i].text)
        diff_texts = set(actual_texts).difference(set(expected_texts))
        # assert diff_texts == 0, "%s" % diff_texts

        # pick single menuitem to make sure its text
        #self.textTest(self.color_menuitems[0], "Black")
        #self.textTest(self.color_menuitems[10], "Yellow-Green")
        #self.textTest(self.color_menuitems[15], "White")

        # press action for color combobox, Menu and MenuItem are showing
        procedurelogger.action("press color combobox")
        self.color_combobox.press()
        sleep(config.SHORT_DELAY)

        procedurelogger.expectedResult("menu and some menuitems are showing and visible")
        assert self.color_menu.showing == True and \
		self.color_menu.visible == True and \
		self.color_menuitems[1].showing == True

        # test click action for color MenuItems
        for i in range(16):
            self.clickMenuItem(self.color_menuitems[i])

    def textTest(self, accessible, textname):
        """
        Wrap AtkText test for accessible

        """
        procedurelogger.action("test the text of %s under color ComboBox" % accessible)

        procedurelogger.expectedResult("the text of %s is %s" % (accessible, textname)) 
        assert accessible.text == textname, "%s not match %s" % (accessible.text, textname)
 
    def clickMenuItem(self, accessible):
        """
        Wrap AtkAction test for accessible to do click

        """
        #procedurelogger.action("click %s" % accessible)
        self.click(accessible)
        sleep(config.SHORT_DELAY)

        procedurelogger.expectedResult("%s is selected, focused and showing" % accessible)
        assert accessible.showing == True and \
		accessible.selected == True and \
		accessible.focused == True     
   
    # close application main window after running test
    def quit(self):
        self.altF4()
