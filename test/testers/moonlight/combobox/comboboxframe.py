
##############################################################################
# Written by:  Cachen Chen <cachen@novell.com>
# Date:        09/25/2009
# Description: combobox.py wrapper script
#              Used by the combobox-*.py tests
##############################################################################

# imports

from strongwind import *
from combobox import *


# class to represent the main window.
class ComboBoxFrame(accessibles.Frame):

    # constants
    # the available widgets on the window
    TEXT1 = "ComboBox 1:"
    TEXT2 = "ComboBox 2:"
    TEXT3 = "Selected: Honda 12500"
    BUTTON1 = "Add Item"
    BUTTON2 = "Delete Item"
    BUTTON3 = "Reset Item"

    COMBOBOX_NUM = 3
    BOX1_ITEM_NUM = 10
    BOX2_ITEM_NUM = 6
    BOX3_ITEM_NUM = 3


    def __init__(self, accessible):
        super(ComboBoxFrame, self).__init__(accessible)
        self.frame = self.findDocumentFrame("ComboBoxSample")
        self.text1 = self.frame.findLabel(self.TEXT1)
        self.text2 = self.frame.findLabel(self.TEXT2)
        self.text3 = self.frame.findLabel(self.TEXT3)

        self.add_button = self.frame.findPushButton(self.BUTTON1)
        self.del_button = self.frame.findPushButton(self.BUTTON2)
        self.reset_button = self.frame.findPushButton(self.BUTTON3)

        self.text = self.frame.findText(None)

        # find and variable combobox
        self.comboboxs = self.frame.findAllComboBoxs(None)
        self.assertNumber(self.comboboxs, self.COMBOBOX_NUM, log=False)
        self.combobox1 = self.comboboxs[0]
        self.combobox2 = self.comboboxs[1]
        self.combobox3 = self.comboboxs[2]

        # find menu and menuitems in combobox1
        self.box1_menu = self.combobox1.findMenu(None, checkShowing=False)
        self.box1_menuitems = self.box1_menu.findAllMenuItems(None, 
                                                            checkShowing=False)
        self.assertNumber(self.box1_menuitems, self.BOX1_ITEM_NUM, log=False)

        # find menu and menuitems in combobox2
        self.box2_menu = self.combobox2.findMenu(None, checkShowing=False)
        self.box2_menuitems = self.box2_menu.findAllMenuItems(None, 
                                                            checkShowing=False)
        self.assertNumber(self.box2_menuitems, self.BOX2_ITEM_NUM, log=False)

        # find checkbox and image in combobox2
        self.box2_checkbox = self.box2.menuitems[3].findCheckBox(None)
        self.box2_image = self.box2.menuitems[5].findImage(None)

        # find menu and menuitems in combobox3
        self.box3_menu = self.combobox2.findMenu(None, checkShowing=False)
        self.box3_menuitems = self.box3_menu.findAllMenuItems(None, 
                                                            checkShowing=False)
        self.assertNumber(self.box3_menuitems, self.BOX3_ITEM_NUM, log=False)

    def assertNumber(self, accessible, expected_num, log=True):
        """
        Make sure the number of list of accessible is expected
        """
        if log:
            procedurelogger.expectedResult("the number should be %s" % \
                                                                  expected_num)
        else:
            pass
        assert len(accessible) == expected_num, \
                        "actual number is:%s, expected number is:%s" % \
                          (len(accessible), expected_num)

    def assertSelectChild(self, parent, childIndex):
        """
        Select childIndex to test its parent's AtkSelection

        """
        procedurelogger.action('selecte %s childIndex %s' % (parent, childIndex))

        parent.selectChild(childIndex)

    def assertClearSelection(self, accessible):
        """
        Clear selected child from accessible
        """
        procedurelogger.action('clear selection in "%s"' % (accessible))

        accessible.clearSelection()

    def checkAllStates(self, items_list, focused_item):
        """
        Check the states of all the menu items of the menu accessible.  The
        focused_item should have +selected +focused states.  All other menu
        items should have -showing -visible states.
        """
        for item in self.items_list:
            if item is focused_item:
                statesCheck(item,
                            "MenuItem",
                            add_states=["focused", "selected"])
            else:
                statesCheck(item,
                            "MenuItem",
                            invalid_states=["showing"])
