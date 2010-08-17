
##############################################################################
# Written by:  Cachen Chen <cachen@novell.com>
# Date:        09/25/2009
# Description: combobox.py wrapper script
#              Used by the combobox-*.py tests
##############################################################################

# imports

from strongwind import *
from helpers import *
from combobox import *


# class to represent the main window.
class ComboBoxFrame(accessibles.Frame):

    # constants
    # the available widgets on the window
    LABEL1 = "ComboBox 1:"
    LABEL2 = "ComboBox 2:"
    LABEL3 = "Selected: Honda 12500"
    BUTTON1 = "Add Item"
    BUTTON2 = "Delete Item"
    BUTTON3 = "Reset Item"

    COMBOBOX_NUM = 3
    BOX1_ITEM_NUM = 10
    BOX2_ITEM_NUM = 7
    BOX3_ITEM_NUM = 3


    def __init__(self, accessible):
        super(ComboBoxFrame, self).__init__(accessible)
        self.frame = self.findDocumentFrame("ComboBoxSample")
        self.filler = self.frame.findFiller("Silverlight Control")
        self.label1 = self.filler.findLabel(self.LABEL1)
        self.label2 = self.filler.findLabel(self.LABEL2)
        self.label = self.filler.findLabel(self.LABEL3)

        self.add_button = self.filler.findPushButton(self.BUTTON1)
        self.del_button = self.filler.findPushButton(self.BUTTON2)
        self.reset_button = self.filler.findPushButton(self.BUTTON3)

        self.text1 = self.filler.findText(None)

        # find and variable combobox
        self.comboboxs = self.filler.findAllComboBoxs(None)
        self.assertNumber(self.comboboxs, self.COMBOBOX_NUM, log=False)
        self.combobox1 = self.comboboxs[0]
        self.combobox2 = self.comboboxs[1]
        self.combobox3 = self.comboboxs[2]

        # find listitems in combobox1
        self.box1_listitems = self.combobox1.findAllListItems(None, checkShowing=False)
        self.assertNumber(self.box1_listitems, self.BOX1_ITEM_NUM, log=False)

        # find listitems in combobox2
        self.box2_listitems = self.combobox2.findAllListItems(None, checkShowing=False)
        self.assertNumber(self.box2_listitems, self.BOX2_ITEM_NUM, log=False)

        # find checkbox and image in combobox2
        #BUG631348
        #self.box2_checkbox = self.box2_listitems[3].findCheckBox(None)
        #self.box2_image = self.box2_listitems[5].findImage(None)

        # find listitems in combobox3
        #BUG629820
        #self.box3_listitems = self.combobox3.findAllListItems(None, checkShowing=False)
        #self.assertNumber(self.box3_listitems, self.BOX3_ITEM_NUM, log=False)

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
        for item in items_list:
            if item.name == focused_item:
                statesCheck(item, "ListItem", invalid_states=["focusable"])
            else:
                statesCheck(item, "ListItem", invalid_states=["focusable"])
