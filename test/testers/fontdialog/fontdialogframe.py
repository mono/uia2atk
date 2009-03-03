
##############################################################################
# Written by:  Cachen Chen <cachen@novell.com>
# Date:        02/20/2009
# Description: fontdialog.py wrapper script
#              Used by the fontdialog-*.py tests
##############################################################################$

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

    #do click action
    def click(self, button):
        button.click()

        #click button will invoke dialog page
        self.fontdialog = self.app.findDialog("Font")

    #assert if all widgets on Font dialog are showing
    def AssertWidgets(self):
        procedurelogger.action("search for all widgets from FontDialog windows")

        #there are 4 Labels in dialog
        procedurelogger.expectedResult("4 Labels are showing up")
        self.font_label = self.fontdialog.findLabel("Font:")
        self.fontstyle_label = self.fontdialog.findLabel("Font Style:")
        self.size_label = self.fontdialog.findLabel("Size:")
        self.script_label = self.fontdialog.findLabel("Script:")
        #there are 4 push buttons in dialog
        procedurelogger.expectedResult("4 PushButtons are showing up")
        self.ok_button = self.fontdialog.findPushButton("OK")
        self.cancel_button = self.fontdialog.findPushButton("Cancel")
        self.apply_button = self.fontdialog.findPushButton("Apply")
        self.help_button = self.fontdialog.findPushButton("Help")
        #there are 3 TextBoxs in dialog
        procedurelogger.expectedResult("3 TextBoxs are showing up")
        self.texts = self.fontdialog.findAllTexts(None)
        self.size_text = self.texts[0]
        self.fontstyle_text = self.texts[1]
        self.font_text = self.texts[2]
        #there are 3 ListBoxs in dialog
        procedurelogger.expectedResult("3 ListBoxs are showing up")
        self.treetables = self.fontdialog.findAllTreeTables(None)
        self.size_treetable = self.treetables[0]
        self.fontstyle_treetable = self.treetables[1]
        self.font_treetable = self.treetables[2]
        #each treetable have some TableCells
        procedurelogger.expectedResult("18 tablecell under size ListBox, \
                                       and 4 tablecell under fontstyle ListBox, \
                                       and 75 tablecell under font ListBox")
        self.size_tablecell = self.size_treetable.findAllTableCells(None, checkShowing = False)
        self.fontstyle_tablecell =self.fontstyle_treetable.findAllTableCells(None, checkShowing = False)
        self.font_tablecell = self.font_treetable.findAllTableCells(None, checkShowing = False)

        assert len(self.size_tablecell) == 18 and  \
               len(self.fontstyle_tablecell) == 4 and \
               len(self.font_tablecell) == 75, "missing TableCell"

        #there are 2 GroupBox
        self.effects_groupbox = self.fontdialog.findPanel("Effects")
        self.example_groupbox = self.fontdialog.findPanel("Example")
        #there are 2 CheckBox in Effects groupbox 1 panel in Example groupbox
        self.strikethrough_checkbox = self.effects_groupbox.findCheckBox("Strikethrough")
        self.underlined_checkbox = self.effects_groupbox.findCheckBox("Underlined")
        self.example_panel = self.example_groupbox.findPanel(None)
        #there is 1 script ComboBox in dialog
        self.script_combobox = self.fontdialog.findComboBox(None)
        self.script_menuitems = self.script_combobox.findAllMenuItems(None, checkShowing = False)

        assert len(self.script_menuitems) == 25, "missing MenuItem"

    #Text test for MenuItem under color ComboBox
    def textTest(self, menuitem, textname):
        procedurelogger.action("test the text of %s under color ComboBox" % menuitem)

        procedurelogger.expectedResult("the text of %s is %s" % (menuitem, textname)) 
        assert menuitem.text == textname, "%s not match %s" % (menuitem.text, textname)

    #color ComboBox and sub children test
    def colorComboBoxTest(self):
        #search for color combobox and sub children
        procedurelogger.action("search for color combobox and sub children in Effects GroupBox")

        procedurelogger.expectedResult("color combobox and sub children are showing up")
        self.color_combobox = self.effects_groupbox.findComboBox(None)
        self.color_menu = self.color_combobox.findMenu(None, checkShowing=False)
        self.color_menuitems = self.color_combobox.findAllMenuItems(None, checkShowing=False)

        assert len(self.color_menuitems) == 16, "missing menuitem"

        #check states
        statesCheck(self.color_combobox, "ComboBox")
        statesCheck(self.color_menu, "Menu", add_states=["selected"],\
                                           invalid_states=["visible", "showing"])
        statesCheck(self.color_menuitems[0], "MenuItem", add_states=["focused", "selected"])

        #test menuitems' text
        self.textTest(self.color_menuitems[0], "Black")
        self.textTest(self.color_menuitems[10], "Yellow-Green")
        self.textTest(self.color_menuitems[15], "White")

        #test press action for color combobox
        procedurelogger.action("press color combobox")
        self.color_combobox.press()
        sleep(config.SHORT_DELAY)

        procedurelogger.expectedResult("menu and menuitems are showing")
        assert self.color_menu.showing == True and \
               self.color_menuitems[0].showing == True

        #test click action for color menuitem
        procedurelogger.action("click Red color menuitem")
        self.color_menuitems[9].click()
        sleep(config.SHORT_DELAY)

        procedurelogger.expectedResult("Red color menuitem is selected and showing")
        assert self.color_menuitems[9].showing == True      
   
    #close application main window after running test
    def quit(self):
        self.altF4()
