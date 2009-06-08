##############################################################################
# Written by:  Cachen Chen <cachen@novell.com>
# Date:        01/07/2009
# Description: tabpage.py wrapper script
#              Used by the tabpage-*.py tests
##############################################################################

import sys
import os
import actions
import states

from strongwind import *
from tabpage import *

# class to represent the main window.
class TabPageFrame(accessibles.Frame):

    # constants
    # the available widgets on the window
    TAB0 = "Tab 0"
    TAB1 = "Tab 1"
    TAB2 = "Tab 2"
    TAB3 = "Tab 3"
    LABEL0 = "I'm in tab page 0"
    LABEL1 = "I'm in tab page 1"
    LABEL2 = "I'm in tab page 2"
    LABEL3 = "I'm in tab page 3"
    BUTTON = "Button"
    NUM_TAB_PAGES = 7

    def __init__(self, accessible):
        super(TabPageFrame, self).__init__(accessible)
        self.tab_pages = self.findAllPageTabs(None, checkShowing=False)
        assert len(self.tab_pages) == self.NUM_TAB_PAGES, \
                                      "Found %s tab pages, expected %s" % \
                                      (len(self.tab_pages), self.NUM_TAB_PAGES)
        # give intuitive names to the tab pages we'll test individually
        self.tab_page_0 = self.tab_pages[0]
        self.tab_page_1 = self.tab_pages[1]
        self.tab_page_2 = self.tab_pages[2]
        self.tab_page_3 = self.tab_pages[3]
        # assert the names of tab pages 0-3
        assert self.tab_page_0.name == self.TAB0, \
                                            'Name was "%s", expected "%s"' % \
                                            (self.tab_page_0.name, self.TAB0)
        assert self.tab_page_1.name == self.TAB1, \
                                            'Name was "%s", expected "%s"' % \
                                            (self.tab_page_1.name, self.TAB1)
        assert self.tab_page_2.name == self.TAB2, \
                                            'Name was "%s", expected "%s"' % \
                                            (self.tab_page_2.name, self.TAB2)
        assert self.tab_page_3.name == self.TAB3, \
                                            'Name was "%s", expected "%s"' % \
                                            (self.tab_page_3.name, self.TAB3)
        # Tab Page 0 is selected by default, so we'll find its accessibles here
        self.findTabPageAccessibles(0)
        # and also check the child count of all the tab pages
        self.assertTabPageChildCount(self.tab_page_0, 2)

    # enter Text Value for EditableText
    def enterTextValue(self, textbox, values):
        procedurelogger.action('in %s enter %s' % (textbox, values))
        textbox.text = values

    # assert Text implementation for TabPage
    def assertText(self, accessible, textvalue):
        procedurelogger.action("check TabPage Text Value")
        procedurelogger.expectedResult('text of %s is %s' % \
                                                        (accessible,textvalue))
        assert accessible.text == textvalue
   
    def assertLabelText(self, expected_text):
        """
        Ensure that the current label has the expected text
        """
        procedurelogger.action('Check the text of "%s"' % self.label)
        procedurelogger.expectedResult('The text should be "%s"' % \
                                                                 expected_text)
        actual_text = self.label.text 
        assert actual_text == expected_text, 'Text was "%s", expected "%s"' % \
                                             (actual_text, expected_text)
    
    def findTabPageAccessibles(self, tab_page_number):
        '''
        Find the expected accessibles on the tab_page specified by
        tab_page_number.
        '''
        if tab_page_number == 0:
            self.label = self.tab_page_0.findLabel(self.LABEL0)
            self.button = self.tab_page_0.findPushButton(self.BUTTON)
        elif tab_page_number == 1:
            self.label = self.tab_page_1.findLabel(self.LABEL1)
            self.textbox = self.tab_page_1.findText(None)
        elif tab_page_number == 2:
            self.label = self.tab_page_2.findLabel(self.LABEL2)
            self.checkbox = self.tab_page_2.findCheckBox(None)
        elif tab_page_number == 3:
            self.label = self.tab_page_3.findLabel(self.LABEL3)
            self.radiobutton = self.tab_page_3.findRadioButton(None)

    def assertTabPageChildCount(self,
                                selected_tab_page,
                                num_expected_children):
        '''
        Ensure that the tab pages have the number of children we expect.  The
        selected tab page should have num_expected_children and the others
        should have 0.
        '''
        procedurelogger.action('All tab pages have the expected number of children')
        for tab_page in self.tab_pages:
            num_actual_children = tab_page.childCount
            if tab_page._accessible is selected_tab_page._accessible:
                procedurelogger.expectedResult('"%s" has %s children' % \
                                             (tab_page, num_expected_children))
                assert num_actual_children == num_expected_children, \
                                   '%s child(ren) found, expected %s' % \
                                   (num_actual_children, num_expected_children)
            else:
                procedurelogger.expectedResult('"%s" has 0 children' % \
                                                                    (tab_page))
                assert num_actual_children == 0, \
                                   '%s child(ren) found, expected 0' % \
                                                          (num_actual_children)

    # close application main window after running test
    def quit(self):
        self.altF4()
