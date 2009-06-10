##############################################################################
# Written by:  Cachen Chen <cachen@novell.com>
# Date:        02/04/2009
# Description: toolbar.py wrapper script
#              Used by the toolbar-*.py tests
##############################################################################

import sys
import os
import actions
import states

from strongwind import *
from toolbar import *
from helpers import *

# class to represent the main window.
class ToolBarFrame(accessibles.Frame):

    PUSHBUTTON_NUM = 4
    LABEL_NUM = 1
    SEPARATOR_NUM = 1
    COMBOBOX_NUM = 1
    MENU_NUM = 1
    MENUITEM_NUM = 10

    def __init__(self, accessible):
        super(ToolBarFrame, self).__init__(accessible)
        self.toolbar = self.findToolBar(None)

    def assertChildrenNumber(self, children_type, expected_num=None):
        """
        search for toolbar's children by find children_type, ensure the number
        of that kind of children_type are expected
        """
        procedurelogger.action("check how many %s items are on the toolbar" % \
                                                                 children_type)
        procedurelogger.expectedResult("%s %s on toolbar" % \
                                                 (expected_num, children_type))
    
        if children_type == "push button":
            self.pushbuttons = self.toolbar.findAllPushButtons(None)
            expected_num = self.PUSHBUTTON_NUM
            assert len(self.pushbuttons) == expected_num, \
                                       "actual %s, expected %s push button" % \
                                       (len(self.pushbuttons), expected_num)
        elif children_type == "label":
            self.labels = self.toolbar.findAllLabels(None)
            expected_num = self.LABEL_NUM
            assert len(self.labels) == expected_num,  \
                                             "actual %s, expected %s label" % \
                                              (len(self.labels), expected_num)
        elif children_type == "separator":
            self.separators = self.toolbar.findAllSeparators(None)
            expected_num = self.SEPARATOR_NUM
            assert len(self.separators) == expected_num, \
                                         "actual %s, expected %s separator" % \
                                         (len(self.separators), expected_num)
        elif children_type == "combo box":
            self.comboboxes = self.toolbar.findAllComboBoxes(None)
            expected_num = self.COMBOBOX_NUM
            assert len(self.comboboxes) == expected_num, \
                                          "actual %s, expected %s combobox" % \
                                          (len(self.comboboxes), expected_num)
        elif children_type == "menu":
            self.menus = self.toolbar.findAllMenus(None, checkShowing=False)
            expected_num = self.MENU_NUM
            assert len(self.menus) == expected_num, \
                                            "actual %s, expected %s menu" % \
                                            (len(self.menus), expected_num)
        elif children_type == "menu item":
            self.menuitems = \
                        self.toolbar.findAllMenuItems(None, checkShowing=False)
            expected_num = self.MENUITEM_NUM
            assert len(self.menuitems) == expected_num, \
                                            "actual %s, expected %s menu" % \
                                            (len(self.menuitems), expected_num)
        else:
            raise ValueError, "no %s in this sample" % children_type


    def checkComboBox(self):

        # combobox Actions
        actionsCheck(self.comboboxes[0], "ComboBox")

        # combobox States, should default focus on combobox
        statesCheck(self.comboboxes[0], "ComboBox", \
                                            add_states=["focused"])

        # doing click action to drop down menu 
        self.comboboxes[0].click(log=True)
        sleep(config.SHORT_DELAY)
        procedurelogger.expectedResult("drop_down menu is showing up")
        assert self.menus[0].showing, "combobox drop_down menu doesn't showing"

        # doing click action again to close menu
        self.comboboxes[0].click(log=True)
        sleep(config.SHORT_DELAY)
        procedurelogger.expectedResult("drop_down menu is closed")
        assert not self.menus[0].showing, \
                                        "combobox drop_down menu still showing"      
    def checkComboBoxMenuItem(self):

        # combobox Actions and States
        for menuitem in self.menuitems:
            actionsCheck(menuitem, "MenuItem")
            statesCheck(menuitem, "MenuItem")

        # doing click action to raise focused and selected states
        self.menuitems[0].click(log=True)
        sleep(config.SHORT_DELAY)
        procedurelogger.expectedResult("combobox and page label's name is changed")
        assert self.combobox[0].name == "0"
        assert self.toolbar.findLabel("page:0")

        for menuitem in self.menuitems:
            if menuitem is self.menuitems[0]:
                statesCheck(self.menuitems[0], "MenuItem", 
                                       add_states["focused", "selected"])
            else:
                statesCheck(self.menuitems[0], "MenuItem")
    
    # close main window
    def quit(self):
        self.altF4()
