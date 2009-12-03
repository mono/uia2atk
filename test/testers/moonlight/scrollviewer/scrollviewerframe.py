
##############################################################################
# Written by:  Calen Chen  <cachen@novell.com>
# Date:        10/20/2009
# Description: scrollviewer.py wrapper script
#              Used by the scrollviewer-*.py tests
##############################################################################

from strongwind import *
from scrollviewer import *

# class to represent the main window.
class ScrollViewerFrame(accessibles.Frame):

    BUTTON1 = "Hidden Vertical"
    BUTTON2 = "Hidden Horizontal"
    import re
    BUTTON3 = "^Ad"
    #BUTTON3 = "Adjust Viewer"
    LABEL = "Scrolling is enabled when it is necessary."

    SCROLLBARS_NUM = 2

    def __init__(self, accessible):
        super(ScrollViewerFrame, self).__init__(accessible)
        self.frame = self.findDocumentFrame("ScrollViewerSample")
        self.filler = self.findFiller("Silverlight Control")
        # 3 buttons for hidden or showing scrollbars
        self.hidden_vertical = self.filler.findPushButton(self.BUTTON1)
        self.hidden_horizontal = self.filler.findPushButton(self.BUTTON2)
        self.viewer_button = self.filler.findPushButton(re.compile(self.BUTTON3))
        # scrollviewer is a panel
        self.scroll_viewer = self.filler.findPanel("")
        # 1 label and 2 scrollbar in scrollviewer
        self.label = self.scroll_viewer.findLabel(self.LABEL)
        self.findScrollBars()

    def findScrollBars(self):
        self.scrollbars = self.scroll_viewer.findAllScrollBars(None)
        assert len(self.scrollbars) == self.SCROLLBARS_NUM, \
                           "actual scrollbar is %s, expected %s" % \
                           (len(self.scrollbars), self.SCROLLBARS_NUM)
        # variable vscrollbar and hscrollbar
        for scroll_bar in self.scrollbars:
            if scroll_bar.vertical:
                self.vscrollbar = scroll_bar
            else:
                self.hscrollbar = scroll_bar

    def valueScrollBar(self, scrollbar, new_value=None):
        """Set new_value for scrollbar"""
        procedurelogger.action('set %s value to "%s"' % (scrollbar, new_value))
        scrollbar.value = new_value

    def assertScrollbar(self, scrollbar, expected_value=None):
        """Make sure scrollbar's value is expected"""
        self.maximumValue = scrollbar._accessible.queryValue().maximumValue
        self.minimumValue = scrollbar._accessible.queryValue().minimumValue

        if 0 <= expected_value <= self.maximumValue:
            procedurelogger.expectedResult('the %s\'s current value is "%s"' % (scrollbar, expected_value))
            assert scrollbar.value == expected_value, \
                                     "current value is %s:" % \
                                     scrollbar.value
        else:
            if expected_value > self.maximumValue:
                procedurelogger.expectedResult('value "%s" out of run %s' % (expected_value, self.maximumValue))
            elif expected_value < self.minimumValue:
                procedurelogger.expectedResult('value "%s" out of run %s' % (expected_value, self. minimumValue))
            assert not scrollbar.value == expected_value, \
                                     "current value is %s:" % \
                                     scrollbar.value

    def assertHiddenScrollBar(self, scroll_bar, is_hidden=False):
        """
        Make sure the scroll_bar is not accessible after it is hidden
        """
        if is_hidden:
            procedurelogger.expectedResult("%s is showing" % scroll_bar)
            self.findScrollBars()
        else:
            procedurelogger.expectedResult("%s is hidden" % scroll_bar)
            try:
                self.findScrollBars()
            except AssertionError:
                return
            assert False, "%s shouldn't be showing" % scroll_bar
