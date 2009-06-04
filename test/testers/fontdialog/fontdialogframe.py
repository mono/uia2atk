
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
    NUMTEXTS = 3
    NUMLABELS = 4
    NUMBUTTONS = 4
    NUMTREETABLES = 3
    NUMMENUITEMS = 25 
    NUMCOLORS = 16
    NUMSCRIPTS = 25

    def __init__(self, accessible):
        super(FontDialogFrame, self).__init__(accessible)
        self.fontdialog_button = self.findPushButton(self.BUTTON1)

    # find all of the accessibles on the font dialog
    def findAllFontDialogAccessibles(self):
        procedurelogger.action("Find all of the Font dialog accessibles")
        procedurelogger.expectedResult("All accessibles are found")
        # Font Dialog is appeared
        self.fontdialog = self.app.findDialog("Font")
        # there are NUMLABELS labels in dialog
        self.font_label = self.fontdialog.findLabel("Font:")
        self.fontstyle_label = self.fontdialog.findLabel("Font Style:")
        self.size_label = self.fontdialog.findLabel("Size:")
        self.script_label = self.fontdialog.findLabel("Script:")
        # there are NUMBUTTON push buttons in dialog
        self.ok_button = self.fontdialog.findPushButton("OK")
        self.cancel_button = self.fontdialog.findPushButton("Cancel")
        self.apply_button = self.fontdialog.findPushButton("Apply")
        self.help_button = self.fontdialog.findPushButton("Help")
        # there are NUMTEXTS text accessibles in dialog
        self.texts = self.fontdialog.findAllTexts(None)
        # check the length of self.texts before indexing it
        assert len(self.texts) == self.NUMTEXTS, \
                            'There were %s "text" accessibles, expected %s' % \
                                               (len(self.texts), self.NUMTEXTS)
        self.size_text = self.texts[0]
        self.fontstyle_text = self.texts[1]
        self.font_text = self.texts[2]
        # there are 3 ListBoxs in dialog
        self.treetables = self.fontdialog.findAllTreeTables(None)
        assert len(self.treetables) == self.NUMTREETABLES, \
                           'Found %s "tree table" accessibles, expected %s' % \
                                     (len(self.treetables), self.NUMTREETABLES)
        self.size_treetable = self.treetables[0]
        self.fontstyle_treetable = self.treetables[1]
        self.font_treetable = self.treetables[2]
        # each treetable have some TableCells
        self.size_table_cells = \
              self.size_treetable.findAllTableCells(None, checkShowing = False)
        self.fontstyle_table_cells = \
              self.fontstyle_treetable.findAllTableCells(None,
                                                         checkShowing = False)
        self.font_table_cells = \
              self.font_treetable.findAllTableCells(None, checkShowing = False)

        # different numbers of Font with the different machine
        assert len(self.size_table_cells) > 0, \
                                  "%s size table cells found, expected > 0" % \
                                  len(self.size_table_cells)
        assert len(self.fontstyle_table_cells) > 0, \
                            "%s font style table cells found, expected > 0" % \
                            len(self.fontstyle_table_cells)
        assert len(self.font_table_cells) > 0, \
                                  "%s font table cells found, expected > 0" % \
                                  len(self.font_table_cells)

        # there are 2 GroupBox controls
        self.effects_groupbox = self.fontdialog.findPanel("Effects")
        self.example_groupbox = self.fontdialog.findPanel("Example")
        # find all of the accessible children of the "Effects" GroupBox
        self.strikethrough_checkbox = \
                            self.effects_groupbox.findCheckBox("Strikethrough")
        self.underlined_checkbox = \
                               self.effects_groupbox.findCheckBox("Underlined")
        self.color_combobox = self.effects_groupbox.findComboBox(None)
        self.color_combobox_menu = \
                       self.color_combobox.findMenu(None, checkShowing = False)
        self.color_combobox_menuitems = \
               self.color_combobox.findAllMenuItems(None, checkShowing = False)
        assert len(self.color_combobox_menuitems) == self.NUMCOLORS, \
                         "Found %s color menu items, expected %s" % \
                         (len(self.color_combobox_menuitems), self.NUMCOLORS)
        # find the accessible child of the "Example" GroupBox
        self.example_panel = self.example_groupbox.findPanel(None)

        # find single ComboBox that is a direct descendant of the font dialog
        self.script_combobox = self.fontdialog.findComboBox(None)
        self.script_menuitems = \
              self.script_combobox.findAllMenuItems(None, checkShowing = False)
        assert len(self.script_menuitems) == self.NUMSCRIPTS, \
                                  "Found %s script menu items, expected %s" % \
                                  (len(self.script_menuitems), self.NUMSCRIPTS)
     
    def checkDefaultFontDialogStates(self):
        # check the default states of the color combo box
        self.colorComboBoxStatesCheck()
        # check the states of a label
        statesCheck(self.fontstyle_label, "Label")
        # check the states of a button
        statesCheck(self.apply_button, "Button")
        # check the states of the Font Style text accessible, which is focused
        # when the font dialog opens
        statesCheck(self.fontstyle_text, "Text", add_states=["focused"])
        # make sure none of the other texts are focused
        statesCheck(self.font_text, "Text")
        statesCheck(self.size_text, "Text")
        # ensure that the "regular" font style is selected by default
        statesCheck(self.fontstyle_table_cells[0],
                    "TableCell",
                    add_states=["selected"])
        statesCheck(self.font_table_cells[0], "TableCell", 
                                                       add_states=["selected"])
        statesCheck(self.size_table_cells[2], "TableCell", 
                                                       add_states=["selected"])
        # BUG506744: combobox menuitems are missing focusable state
        #statesCheck(self.color_combobox_menuitems[0], "MenuItem",
        #                                    add_states=["focused", "selected"])
        #statesCheck(self.script_menuitems[0], "MenuItem",
        #                                    add_states=["focused", "selected"]) 
        # TODO: There are lot more states we could check here

    # color ComboBox and sub children test
    def colorComboBoxStatesCheck(self, selected_color_menuitem=None):
        """
        Check the states of the collapsed color combo box (and its children) of
        the FontDialog.  The selected_color_menuitem parameter is the
        accessible of the menu item that has been selected.  If no menu item
        has been selected, the method will assume that the first menu item
        (i.e., "Black") is selected
        """
        # ensure that Menu and MenuItems are not showing except the selected
        # menu item.  color_menuitems[0] selected by default when the font
        # dialog opens
        if selected_color_menuitem is None:
            try:
                selected_color_menuitem = self.color_combobox_menuitems[0]
            except AttributeError:
                print "The color combo box menu items have not yet been found"
                raise
        statesCheck(self.color_combobox, "ComboBox")
        statesCheck(self.color_combobox_menu,
                    "Menu",
                    invalid_states=["visible", "showing"])
        for menuitem in self.color_combobox_menuitems:
            add_states = []
            invalid_states = []
            if menuitem is selected_color_menuitem:
                add_states=["selected"]
            else:
                invalid_states=["showing"]
            # Bug 506694 - Default selected index of ComboBox erroneously has
            # "focused" state
            # Bug 503725 - Menu item loses "focusable" state when that item
            # becomes focused
            #statesCheck(menuitem, "MenuItem", invalid_states, add_states)

    def colorComboBoxNameAndTextTest(self):
        # make sure actual texts of all menuitem are equal to expected texts
        expected_texts = [
            "Black",
            "Maroon",
            "Green",
            "Olive",
            "Navy", 
            "Purple",
            "Cyan",
            "Gray",
            "Silver",
            "Red",
            "Lime",
            "Yellow",
            "Blue",
            "Fuchsia",
            "Aqua", 
            "White"]

        procedurelogger.action("Ensure that the accessibles in the combo box have the names that we expect")
        procedurelogger.expectedResult("The following are valid names: %s" % expected_texts)
        # BUG506726 Comment #10: mono implement wrong color name
        #for expected_text in expected_texts:
        #    self.effects_groupbox.findMenuItem(expected_text,
        #                                       checkShowing=False)

        procedurelogger.action("Ensure that the accessibles in the combo box have the texts that we expect")
        procedurelogger.expectedResult("The list of texts should be: %s" % expected_texts)
        actual_texts = \
                 [menuitem.text for menuitem in self.color_combobox_menuitems]
        # BUG506726 - Duplicate "Aquamarine" color in FontDialog color drop
        # down combo box.
        # there should be no duplicates in the list of actual texts
        #assert expected_texts == actual_texts, \
        #    'Actual list was %s, expected %s' % (actual_texts, expected_texts)

        # perform 'press' action for color combobox and make sure the states
        # change
        procedurelogger.action("press color combobox")
        procedurelogger.expectedResult("menu and some menuitems are showing and visible")
        self.color_combobox.press()
        sleep(config.SHORT_DELAY)
        # the menu should be open (i.e., it should have the default states
        # and menu items that were previously not showing should be showing
        # (i.e., they should also have default states)
        statesCheck(self.color_combobox_menu, "Menu")
        # Bug 506744 - Drop-down CombBox menu items receive "focused" state,
        # but do not have "focusable" state
        #statesCheck(self.color_combobox_menuitems[1], "MenuItem")
        #statesCheck(self.color_combobox_menuitems[2], "MenuItem")
        self.color_combobox.press()
        sleep(config.SHORT_DELAY)

    def colorComboBoxMenuItemActionTest(self):
        """
        check 'click' action for all MenuItems of colorComboBox to make sure the
        clicked item is selected and focused
        """
        for menuitem in self.color_combobox_menuitems:
            menuitem.click(log=True)
            # Bug 506744: missing "focusable" state
            #statesCheck(menuitem, "MenuItem", add_states=["selected", "focused"])
             

    # close application main window after running test
    def quit(self):
        self.altF4()
