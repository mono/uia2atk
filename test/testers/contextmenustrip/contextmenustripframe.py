
##############################################################################
# Written by:  Neville Gao <nevillegao@gmail.com>
# Date:        02/18/2009
# Description: contextmenustrip.py wrapper script
#              Used by the contextmenustrip-*.py tests
##############################################################################

from strongwind import *


class ContextMenuStripFrame(accessibles.Frame):
    def __init__(self, accessible):
        super(ContextMenuStripFrame, self).__init__(accessible)
        self.label = self.findLabel(re.compile('^Right'))

    def assertWidgets(self):
        """assert all the controls is out there"""

        # ContextMenuStrip
        self.context_menu_strip = self.app.findWindow(None, checkShowing=False)

        procedurelogger.action('Searching for all widgets in ContextMenuStrip')
        procedurelogger.expectedResult('All widgets in ContextMenuStrip should show up')

        # Menu items
        self.menu = self.context_menu_strip.findMenu (None);
        self.item1 = self.menu.findMenu ('Apple')
        self.item1a = self.item1.findMenuItem ('Macintosh', checkShowing = False)
        self.item1b = self.item1.findMenuItem ('Delicious', checkShowing = False)
        self.item2 = self.menu.findMenuItem ('Banana')
        self.item3 = self.menu.findMenuItem ('Watermelon')
        self.item4 = self.menu.findMenuItem ('Orange')
        self.item5 = self.menu.findMenuItem ('Peach')

    def assertImage(self, accessible, width=None, height=None):
        """assert that the image size is equal to the actual one"""

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

    def assertText(self, accessible, expected_text):
        """assert that the actual text is equal to the expected text"""
        
        procedurelogger.action('Assert that the text of %s is what we expect' % accessible)
        actual_text = accessible.text
        procedurelogger.expectedResult('%s text is "%s"' % \
                                                (accessible, expected_text))
        assert actual_text == expected_text, \
                  'Text was "%s", expected "%s"' % (actual_text, expected_text)

    def quit(self):
        self.altF4()
