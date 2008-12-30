# vim: set tabstop=4 shiftwidth=4 expandtab
##############################################################################
# Written by:  Cachen Chen <cachen@novell.com>
# Date:        11/10/2008
# Description: Application wrapper for combobox_simple
#              be called by ../combobox_simple
##############################################################################

"""Application wrapper for combobox_simple.py"""

from strongwind import *

class ComboBoxSimpleFrame(accessibles.Frame):
    """the profile of the combobox_simple sample"""

    # constants
    # the available widgets on the window
    LABEL = "You select "

    def __init__(self, accessible):
        super(ComboBoxSimpleFrame, self).__init__(accessible)
        self.label = self.findLabel(self.LABEL)
        self.combobox = self.findComboBox(None)
        self.textbox = self.findText(None)
        self.tree = self.findTreeTable(None)
        self.treecell = dict([(x, self.findTreeCell(str(x))) for x in range(10)])

    def click(self,accessible):
        accessible.click()

    def assertLabel(self, itemname):
        procedurelogger.expectedResult('item "%s" is selected' % itemname)
        def resultMatches():
            return self.findLabel("You select %s" % itemname)
        assert retryUntilTrue(resultMatches)

    def assertText(self, accessible, text):
        procedurelogger.expectedResult('%s\'s text is %s' % \
                                            (accessible, accessible.text))
        assert accessible.text == text, "%s is not equal to %s" % \
                                            (accessible.text, text)

    def assertSelectionChild(self, accessible, index):
        procedurelogger.action('select item%s in "%s"' % (index, accessible))
        accessible.selectChild(index)

    def assertClearSelection(self, accessible):
        procedurelogger.action('clear selection in "%s"' % (accessible))
        accessible.clearSelection()

    def inputText(self, accessible, text):
        procedurelogger.action('input %s in text box' % text)
        self.textbox.typeText(text)

    def enterText(self, text):
        procedurelogger.action('enter %s to text box' % text)
        self.textbox.text == text
    
    def quit(self):
        self.altF4()
