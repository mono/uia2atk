# vim: set tabstop=4 shiftwidth=4 expandtab
##############################################################################
# Written by:  Ray Wang <rawang@novell.com>
# Date:        01/08/2008
# Description: Application wrapper for toolstripsplitbutton.py
#              be called by ../toolstripsplitbutton_basic_ops.py
##############################################################################

"""Application wrapper for toolstripsplitbutton.py"""

from strongwind import *

class ToolStripSplitButtonFrame(accessibles.Frame):
    """the profile of the toolstripsplitbutton sample"""

    FONT_SIZE_LABEL = 'The current font size is 10'
    CLICK_LABEL = '0 clicks'
    PUSH_BUTTON = '10'
    MENUITEM_10 = '10'
    MENUITEM_12 = '12'
    MENUITEM_14 = '14'

    def __init__(self, accessible):
        super(ToolStripSplitButtonFrame, self).__init__(accessible)
        self.font_size_label = self.findLabel(self.FONT_SIZE_LABEL)
        self.click_label = self.findLabel(self.CLICK_LABEL)
        self.toolbar = self.findToolBar(None)
        self.push_button = self.findPushButton(self.PUSH_BUTTON)
        self.toggle_button = self.findToggleButton(None)
        self.menuitem_10 = self.findMenuItem(self.MENUITEM_10, checkShowing=False)
        self.menuitem_12 = self.findMenuItem(self.MENUITEM_12, checkShowing=False)
        self.menuitem_14 = self.findMenuItem(self.MENUITEM_14, checkShowing=False)

    def click(self, button):
        procedurelogger.action("click %s" % button)
        button.click()

    def assertImage(self, accessible, expected_width, expected_height):
        procedurelogger.action('assert the image size of "%s"' % accessible)
        actual_width, actual_height = \
                             accessible._accessible.queryImage().getImageSize()
        procedurelogger.expectedResult('"%s" image size is %s x %s' % \
                                 (accessible, expected_width, expected_height))

        assert actual_width == expected_width, "%s (%s), %s (%s)" % \
                                            ("expected width",
                                              expected_width,
                                             "does not match the actual width",
                                              actual_width)
        assert actual_height == expected_height, "%s (%s), %s (%s)" % \
                                           ("expected height",
                                            expected_height,
                                            "does not match the actual height",
                                            actual_height)

    def assertText(self, accessible, expected_text):
        """make sure accessible's text is expected"""
        procedurelogger.action("check %s's text" % accessible)
        procedurelogger.expectedResult('%s\'s text is "%s"' % \
                                            (accessible, expected_text))
        assert accessible.text == expected_text, \
                               'actual text is "%s", expected text is "%s"' % \
                               (accessible.text, expected_text)

    def selectChild(self, accessible, index):
        """assert Selection implementation"""
        procedurelogger.action('select index %s in "%s"' % \
                                        (index, accessible))
        accessible.selectChild(index)

    def assertDifferentAbsolutePositions(self, accessible1, accessible2):
        '''
        Assert that accessible1 and accessible2 have different absolute
        positions.
        '''
        # get the (x,y) position of accessible1
        position1 = accessible1._accessible.queryComponent().getPosition(0) 
        # get the (x,y) position of accessible1
        position2 = accessible2._accessible.queryComponent().getPosition(0) 
        assert position1 != position2, \
                        "%s and %s should not have the same position %s" % \
                        (accessible1, accessible2, position1)

    def quit(self):
        self.altF4()
