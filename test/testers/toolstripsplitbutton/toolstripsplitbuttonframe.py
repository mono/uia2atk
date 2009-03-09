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

    LABEL = 'The current font size is 10'
    PUSH_BUTTON = '10'
    MENUITEM_10 = '10'
    MENUITEM_12 = '12'
    MENUITEM_14 = '14'

    def __init__(self, accessible):
        super(ToolStripSplitButtonFrame, self).__init__(accessible)
        self.label = self.findLabel(self.LABEL)
        self.toolbar = self.findToolBar(None)
        self.push_button = self.findPushButton(self.PUSH_BUTTON)
        self.toggle_button = self.findToggleButton(None)
        self.menuitem_10 = self.findMenuItem(self.MENUITEM_10, checkShowing=False)
        self.menuitem_12 = self.findMenuItem(self.MENUITEM_12, checkShowing=False)
        self.menuitem_14 = self.findMenuItem(self.MENUITEM_14, checkShowing=False)

    def click(self, button):
        procedurelogger.action("click %s" % button)
        button.click()

    def assertImage(self, accessible, width=None, height=None):
        procedurelogger.action("assert %s's image size" % accessible)
        size = accessible._accessible.queryImage().getImageSize()
        procedurelogger.expectedResult('"%s" image size is %s x %s' %
                                                  (accessible, width, height))

        assert width == size[0], "%s (%s), %s (%s)" % \
                                            ("expected width",
                                              width,
                                             "does not match the actual width",
                                              size[0])
        assert height == size[1], "%s (%s), %s (%s)" % \
                                            ("expected height",
                                              height,
                                             "does not match the actual height",
                                              size[1]) 

    def assertText(self, accessible, text=None):
        """assert text is equal to the input"""

        procedurelogger.action('Assert the text of %s' % accessible)
        procedurelogger.expectedResult('%s text is "%s"' % \
                                                (accessible, accessible.text))
        assert accessible.text == text, '%s is not match with "%s"' % \
                                                (accessible, accessible.text)

    def selectChild(self, accessible, index):
        """assert Selection implementation"""
        procedurelogger.action('select index %s in "%s"' % \
                                        (index, accessible))
        accessible.selectChild(index)

    def inputText(self, accessible, text):
        try:
            procedurelogger.action('input %s to %s' % (text, accessible))
            accessible.text = text
        except NotImplementedError:
            pass

    def assertComponent(self):
        self.menuitems = self.findAllMenuItems(None)
        assert len(self.menuitems) == 3, \
                        "the number of menu items is not correct"

    def quit(self):
        self.altF4()
