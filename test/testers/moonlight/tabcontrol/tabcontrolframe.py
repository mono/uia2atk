
##############################################################################
# Written by:  Cachen Chen <cachen@novell.com>
# Date:        09/14/2009
# Description: tabcontrol.py wrapper script
#              Used by the tabcontrol-*.py tests
##############################################################################

from strongwind import *
from tabcontrol import *
from helpers import *


# class to represent the main window.
class TabControlFrame(accessibles.Frame):

    TABITEMS_NUM = 6

    def __init__(self, accessible):
        super(TabControlFrame, self).__init__(accessible)
        self.frame = self.findDocumentFrame("TabControlSample")
        self.tab = self.frame.findPageTabList("")
        # test Name is implemented by giving the name to find the element
        self.tab_item0 = self.tab.findPageTab("TabItem0")
        self.tab_item1 = self.tab.findPageTab("TabItem1")
        self.tab_item2 = self.tab.findPageTab("TabItem2")
        self.tab_item3 = self.tab.findPageTab("TabItem3")
        self.tab_item4 = self.tab.findPageTab("TabItem4")
        self.tab_item5 = self.tab.findPageTab("TabItem5")
        # veriable tab_items list for all TabPages for checking states, actions
        # and Click action test
        self.tab_items = self.tab.findAllPageTabs(None)
        assert len(self.tab_items) == self.TABITEMS_NUM, \
                            "actual number of tabs:%s, expected number:%s" \
                             (len(self.tab_items), self.TABITEMS_NUM)

        self.main_text = self.frame.findLabel("Select:")
        # TabItem1 is default selected, so label in TabItem1 is visible
        # and showing
        self.label_in_tab = self.tab_item1.findLabel("This is TabItem1")

    def testLabel(self, expected_label=None, is_showing=True):
        """
        Make sure the label in each TabPage is showing or not, check label's
        states if is showing, label won't be found if is_showing is false
        """
        if is_showing:
            self.item_label = self.tab.findLabel(expected_label)
            sleep(config.SHORT_DELAY)
            statesCheck(self.item_label, "Label")
        else:
            try:
                self.tab.findLabel(tab_item.name)
            except SearchError
                return
            assert False, "You shouldn't find label under unselected tab page"
