
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
        procedurelogger.action("how many %s items on toolbar" % children_type)

        if children_type == "push button":
            count = self.toolbar.findAllPushButtons(None)
            expected_num = self.PUSHBUTTON_NUM
            assert len(count) == expected_num, \
             "actual %s, expected %s push button" % (len(count), expected_num)
        elif children_type == "label":
            count = self.toolbar.findAllLabels(None)
            expected_num = self.LABEL_NUM
            assert len(count) == expected_num,  \
                    "actual %s, expected %s label" % (len(count), expected_num)
        elif children_type == "separator":
            count = self.toolbar.findAllSeparators(None)
            expected_num = self.SEPARATOR_NUM
            assert len(count) == expected_num, \
                "actual %s, expected %s separator" % (len(count), expected_num)
        elif children_type == "combo box":
            count = self.toolbar.findAllComboBoxs(None)
            expected_num = self.COMBOBOX_NUM
            assert len(count) == expected_num, \
                  "actual %s, expected %s combobox" % (len(count), expected_num)
        elif children_type == "menu":
            count = self.toolbar.findAllMenus(None)
            expected_num = self.MENU_NUM
            assert len(count) == expected_num, \
                  "actual %s, expected %s menu" % (len(count), expected_num)
        elif children_type == "menu item":
            count = self.toolbar.findAllMenuItems(None)
            expected_num = self.MENUITEM_NUM
            assert len(count) == expected_num, \
                  "actual %s, expected %s menu" % (len(count), expected_num)
        else:
            print "no %s in this sample" % children_type

        procedurelogger.expectedResult("%s %s on toolbar" % \
                                                  (expected_num, children_type))
    
    # close main window
    def quit(self):
        self.altF4()
