# vim: set tabstop=4 shiftwidth=4 expandtab
##############################################################################
# Written by:  Cachen Chen <cachen@novell.com>
# Date:        11/10/2008
# Description: Application wrapper for combobox_simple
#              be called by ../combobox_simple_ops.py
##############################################################################

"""Application wrapper for combobox_simple.py"""

from strongwind import *

class ComboBoxSimpleFrame(accessibles.Frame):
    """the profile of the combobox_simple sample"""

    def __init__(self, accessible):
        super(ComboBoxSimpleFrame, self).__init__(accessible)

        self.combobox = self.findComboBox(None)
        self.textbox = self.findText(None)
        self.tree = self.findTreeTable(None)
        self.tablecell = dict([(x, self.findTableCell(str(x))) for x in range(10)])

        print "####################"
        print len(self.tree)
        print len(self.combobox)
        print "####################"

    def click(self,accessible):
        accessible.click()

    def inputText(self, accessible, text):
        try:
            accessible.text = text
        except NotImplementedError:
            pass

    def assertText(self, accessible, text):
        procedurelogger.expectedResult('%s\'s text is %s' % \
                                            (accessible, accessible.text))
        assert accessible.text == text, "%s is not equal to %s" % \
                                            (accessible.text, text)
    
    def assertTable(self, accessible, row=None, col=None):
        """assert Table implementation"""

        procedurelogger.action('check "%s" Table implemetation' % accessible)
        itable = accessible._accessible.queryTable()
        procedurelogger.expectedResult('"%s" have %s Rows and %s Columns' % \
                                                        (accessible, row, col))
        assert itable.nRows == row and itable.nColumns == col, \
                                        "Not match Rows %s and Columns %s" % \
                                        (itable.nRows, itable.nColumns)

    def selectChild(self, accessible, index):
        procedurelogger.action('select item%s in "%s"' % (index, accessible))
        accessible.selectChild(index)

    def clearSelection(self, accessible):
        procedurelogger.action('clear selection in "%s"' % (accessible))
        accessible.clearSelection()

    def quit(self):
        self.altF4()
