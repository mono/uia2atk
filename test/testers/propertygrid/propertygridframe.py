##############################################################################
# Written by:  Felicia Mu <fxmu@novell.com>
#              Calen Chen <cachen@novell.com>
# Date:        06/25/2009
# Description: propertygridframe.py wrapper script
#              Used by the propertygrid-*.py tests
##############################################################################$

import sys
import os
import actions
import states
import pyatspi

from strongwind import *
from datagridview import *
from helpers import *

# class to represent the main window.
class PropertyGridFrame(accessibles.Frame):

    # constants
    # the available widgets on the frame
    PARENT_CELLS = ["Accessibility", "Appearance", "Font", "Behavior", "Data", 
                   "(DataBindings)", "Focus", "Layout", "Location", "Margin", 
                   "MaximumSize", "MinimumSize", "Size", "Misc"]

    TABLE_CELLS_NUM = 120
    BUTTONS_NUM = 3
    LABELS_NUM = 2
    PANELS_NUM = 6

    def __init__(self, accessible):
        super(PropertyGridFrame, self).__init__(accessible)

        # find the tree table and its childrens
        self.tree_table = self.findTreeTable(None)
        self.all_table_cells = \
                    self.tree_table.findAllTableCells(None, checkShowing=False)
        # BUG519409: some table cells are not accessible
        #assert len(self.all_table_cells) == self.TABLE_CELLS_NUM, \
        #        "actual number of table_cells:%s, expected number is:%s" % 
        #                 (len(self.all_table_cells), self.TABLE_CELLS_NUM)
        self.vscrollbar = self.tree_table.findScrollBar(None)

        # variable parent table cells and child table cells
        self.parent_table_cells = []
        self.child_table_cells = []
        for table_cell in self.all_table_cells:
            if table_cell.name in self.PARENT_CELLS:
                self.parent_table_cells.append(table_cell)
            else:
                self.child_table_cells.append(table_cell)

        # find the toolbar and the controls in it
        self.toolbar = self.findToolBar(None)
        # BUG519507: wrong implemented to push button
        #self.buttons = self.toolbar.findAllToggleButtons(None)
        #assert len(self.buttons) == self.BUTTONS_NUM, \
        #           "actual number of buttons:%s, expected:%s" % 
        #             (len(self.buttons), self.BUTTONS_NUM)
        #self.categorized_button = self.buttons[0]
        #self.alphabetic_button = self.buttons[1]
        #self.property_button = self.buttons[2]
        self.separator_style = self.toolbar.findSeparator(None)

        # find the split pane
        self.split_pane = self.findSplitPane(None)

        # find all the labels
        self.labels = self.findAllLabels(None)
        assert len(self.labels) == self.LABELS_NUM, \
                  "actual number of labels:%s, expected:%s" % \
                   (len(self.labels), self.LABELS_NUM)
        self.help_description_label = self.labels[0]
        self.help_title_label = self.labels[1]

        # find text box
        self.text_box = self.findText(None)
    def refreshTreeTable(self):
        """It's temporary because of BUG519711"""
        self.tree_table = self.findTreeTable(None)
        self.all_table_cells = \
                    self.tree_table.findAllTableCells(None, checkShowing=False)

    def assertText(self, accessible, expected_text):
        """Make sure accessible's text is expected_text"""
        procedurelogger.action('check Text for %s' % accessible)
        procedurelogger.expectedResult("%s's Text is %s" % 
                                                   (accessible, expected_text))
        assert accessible.text == expected_text, \
                    "actual text is %s, expected text is %s" % \
                            (accessible.text, expected_text)

    def testSpitterValue(self, expected_value, treetable_is_enlarged):
        """
        Set value to move the spitter, make sure the current value is expected. 
        """
        tree_table_height_old = \
               self.tree_table._accessible.queryComponent().getSize()[1]

        procedurelogger.action("set splitter's value to %s" % expected_value)
        self.split_pane.value = expected_value
        sleep(config.SHORT_DELAY)

        procedurelogger.expectedResult("current value of splitter is %s" % 
                                                         expected_value)
        current_value = self.split_pane._accessible.queryValue().currentValue
        assert current_value == expected_value, \
               "actual current value is %s, expected current value is %s" % \
                (current_value, expected_value)
        # add test for bug519532: if treetable_is_enlarged that splitter should 
        # be moved to the bottom, otherwise, it should be moved to the top
        tree_table_height_new = \
               self.tree_table._accessible.queryComponent().getSize()[1]
        if treetable_is_enlarged:
            procedurelogger.expectedResult("height of tree_table is enlarged")
            assert tree_table_height_new > tree_table_height_old, \
                        "height of tree table should bigger then the old %s, \
                 but it's %s" % (tree_table_height_old, tree_table_height_new)
        else:
            procedurelogger.expectedResult("height of tree_table is shorten")
            assert tree_table_height_new < tree_table_size_old, \
                        "height of tree table should less then the old %s, \
                 but it's %s" % (tree_table_height_old, tree_table_height_new)


    def assertImageSize(self, accessible, expected_width=0, expected_height=0):
        """
        Make sure accessible's image size is expected 
        """
        procedurelogger.action("assert %s's image size" % accessible)
        actual_width, actual_height = \
                        accessible._accessible.queryImage().getImageSize()

        procedurelogger.expectedResult('"%s" image size is %s x %s' % 
                                (accessible, expected_width, expected_height))

        assert actual_width == expected_width, "%s (%s), %s (%s)" % \
                         ("expecself.popup_toolbar_menuitemsted width",
                          expected_width,
                          "does not match actual width",
                          actual_width)
        assert actual_height == expected_height, "%s (%s), %s (%s)" % \
                         ("expected height",
                          expected_height,
                          "does not match actual height",
                           actual_height) 

    def testExpandOrContract(self, parent_cell, child_cell, is_expand):
        """
        Do "expand or contract" action for accessible which name is parent_cell,
        by make sure if child_cell of the parent_cell can be mouseClick by 
        assert if the text of help_title_label is changed.
        Expand parent_cell that child_cell can be mouseClicked to change label,
        Contract parent_cell that child_cell can't be mouseClicked to change 
        label
        """
        # if BUG520953 is fixed, checkShowing should be removed
        parent_table_cell = self.tree_table.findTableCell(parent_cell, checkShowing=False)
        # if BUG516398 is fixed that doAction(0) should replaced to doAction(1)
        procedurelogger.action("expand or contract %s" % parent_cell)
        parent_table_cell._accessible.queryAction().doAction(0)
        #parent_table_cell.expandorcontract(log=True)
        sleep(config.SHORT_DELAY)

        if is_expand:
            procedurelogger.expectedResult("%s is expaned" % parent_cell)
            self.tree_table.findTableCell(child_cell).mouseClick()
            sleep(config.SHORT_DELAY)
            assert self.help_title_label.text == child_cell, \
                   "actual text of help_title_label:%s, expected:%s" % \
                     (self.help_title_label.text, child_cell)
        else:
            procedurelogger.expectedResult("%s is contracted" % parent_cell)
            try:
                self.tree_table.findTableCell(child_cell)
            except SearchError:
                return
            assert False, "%s couldn't be found because of lack of %s state" %\
                                             (child_cell, "showing")

    def changeText(self, accessible, changed_text):
        """
        Change the text in the editable table cell
        """
        procedurelogger.action("change the text of %s to %s" % \
                                                   (accessible, changed_text))
        accessible._accessible.queryEditableText().setTextContents(changed_text)
        sleep(config.SHORT_DELAY)

    def findContextMenuAccessibles(self):
        """
        Find all the accessibles on ContextMenuStrip
        """
        # mouse button3 click the tree table cell will raise ContextMenu
        self.context_window = self.app.findWindow(None)
        self.context_menu = self.app.findMenu(None)
        self.reset_menu_item = self.context_menu.findMenuItem("Reset")
        self.dash_menu_item = self.context_menu.findMenuItem("-")
        self.description_menu_item = \
                               self.context_menu.findMenuItem("Description")

    def testReset(self, accessible, default_property):
        """
        Test "Reset" function on ContextMenu by change accessible's property, 
        click "Reset" to change the property back to default for accessible
        """
        # first we should get the index of the accessible, then get the object 
        # of the cell that we will reset the property for it, because click the 
        # accessible won't raise ContextMenu
        acc_index = accessible.getIndexInParent()
        reseted_cell = self.tree_table.getChildAtIndex(acc_index - 1)
        reseted_cell.mouseClick(button=3)
        sleep(config.SHORT_DELAY)

        self.findContextMenuAccessibles()

        procedurelogger.action("reset the value of %s " % accessible)
        self.reset_menu_item.click(log=True)
        sleep(config.SHORT_DELAY)

        self.assertText(accessible, default_property)
        statesCheck(self.reset_menu_item, "MenuItem")

    def testDescription(self, is_checked):
        """
        test "Description" function on ContextMenu. uncheck the "Description" 
        menu_item will move the bottom panel
        """
        self.toolbar.mouseClick(button=3)
        sleep(config.SHORT_DELAY)

        self.findContextMenuAccessibles()

        self.description_menu_item.click(log=True)
        sleep(config.SHORT_DELAY)
        self.panels = self.findAllPanels(None)
        sleep(config.SHORT_DELAY)

        if is_checked:
            procedurelogger.expectedResult("Description menu_item is checked, \
                                                  the bottom panel is showing")
            assert len(self.panels) == self.PANELS_NUM, \
                                    "actual find %s panels, expected %s" % \
                                      (len(self.panels), self.PANELS_NUM)
            statesCheck(self.description_menu_item, "MenuItem", 
                                                      add_states=["checked"])
        else:
            procedurelogger.expectedResult("Description menu_item is unchecked,\
                                                  the bottom panel is moved")
            assert len(self.panels) == self.PANELS_NUM - 1, \
                                    "actual find %s panels, expected %s" % \
                                       (len(self.panels), self.PANELS_NUM - 1)
            # BUG514647: still have focused state
            #statesCheck(self.description_menu_item, "MenuItem")

    def selectChild(self, child_index):
        """
        select a child of tree table 
        """
        procedurelogger.action("select the index %s of tree table" % \
                                                           (child_index))
        self.tree_table._accessible.querySelection().selectChild(child_index)
        sleep(config.SHORT_DELAY)

    def quit(self):
        """
        close application main window after running test
        """
        self.altF4()
