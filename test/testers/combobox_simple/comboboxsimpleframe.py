# vim: set tabstop=4 shiftwidth=4 expandtab
##############################################################################
# Written by:  Cachen Chen <cachen@novell.com>
#              Ray Wang <rawang@novell.com>
# Date:        11/10/2008
# Description: Application wrapper for combobox_simple
#              be called by ../combobox_simple_ops.py
##############################################################################

"""Application wrapper for combobox_simple.py"""

from strongwind import *

class ComboBoxSimpleFrame(accessibles.Frame):
    """the profile of the combobox_simple sample"""
    LABEL = "You select "

    def __init__(self, accessible):
        super(ComboBoxSimpleFrame, self).__init__(accessible)
        self.label = self.findLabel(self.LABEL)
        self.combobox = self.findComboBox(None)
        self.textbox = self.findText(None)
        self.treetable = self.findTreeTable(None)
        self.tablecell = dict([(x, self.findTableCell(str(x))) for x in range(10)])

    def click(self,accessible):
        """'click' action"""
        procedurelogger.action("click %s" % accessible)
        accessible.click()

    def editableTextIsUnimplemented(self, accessible):
        """make sure EditableText of accessible is unimplemented"""
        procedurelogger.action("check %s's text" % accessible)

        procedurelogger.expectedResult("%s's text is uneditable" % accessible)
        try:
            accessible._accessible.queryEditableText()
        except NotImplementedError:
            return
        assert False, "EditableText shouldn't be implemented"

    def assertLabel(self, expected_label):
        """make sure label's text is expected"""
        procedurelogger.expectedResult("Lable's text is %s" % expected_label)
        assert self.label.text == expected_label, \
                                  "actual label is %s, expected label is %s" % \
                                               (self.label.text, expected_label)

    def assertText(self, accessible, expected_text, actionlog=None):
        """make sure accessible's text is expected"""
        if actionlog is True:
            procedurelogger.action("check %s's text" % accessible)

        procedurelogger.expectedResult('%s\'s text is %s' % \
                                            (accessible, expected_text))
        assert accessible.text == expected_text, \
                                    "actual text is %s, expected text is %s" % \
                                            (accessible.text, expected_text)
    
    def assertTable(self, accessible, row=None, col=None):
        """
        assert Table implementation, make sure row and col's number are expected

        """
        procedurelogger.action('check "%s" Table implemetation' % accessible)
        itable = accessible._accessible.queryTable()
        procedurelogger.expectedResult('"%s" have %s Rows and %s Columns' % \
                                                        (accessible, row, col))
        assert itable.nRows == row and itable.nColumns == col, \
                                        "Not match Rows %s and Columns %s" % \
                                        (itable.nRows, itable.nColumns)

    def selectChild(self, accessible, index):
        """assert Selection implementation"""
        procedurelogger.action('select item%s in "%s"' % (index, accessible))
        accessible.selectChild(index)

    def clearSelection(self, accessible):
        """assert ClearSelection implementation"""
        procedurelogger.action('clear selection in "%s"' % (accessible))
        accessible.clearSelection()

    def quit(self):
        self.altF4()
