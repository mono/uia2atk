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
    NUMTABLECELLS = 10

    def __init__(self, accessible):
        super(ComboBoxSimpleFrame, self).__init__(accessible)
        self.label = self.findLabel(self.LABEL)
        self.combobox = self.findComboBox(None)
        self.textbox = self.findText(None)
        self.treetable = self.findTreeTable(None)
        self.tablecells = \
                [self.findTableCell(str(i)) for i in range(self.NUMTABLECELLS)]

    def click(self,accessible):
        """'click' action"""
        procedurelogger.action("click %s" % accessible)
        accessible.click()

    def editableTextIsUnimplemented(self, accessible):
        """make sure EditableText of accessible is unimplemented"""
        procedurelogger.action("check the implementation of %s's text interface" % accessible)
        procedurelogger.expectedResult("%s's text is uneditable" % accessible)
        try:
            accessible._accessible.queryEditableText()
        except NotImplementedError:
            return
        assert False, "EditableText shouldn't be implemented"

    def assertLabel(self, expected_label):
        """make sure label's text is expected"""
        procedurelogger.action("check %s's text" % self.label)
        procedurelogger.expectedResult('Label\'s text is "%s"' % \
                                       expected_label)
        assert self.label.text == expected_label, \
                             'actual label is "%s", expected label is "%s"' % \
                             (self.label.text, expected_label)

    def assertText(self, accessible, expected_text):
        """make sure accessible's text is expected"""
        procedurelogger.action("check %s's text" % accessible)
        procedurelogger.expectedResult('%s\'s text is "%s"' % \
                                            (accessible, expected_text))
        assert accessible.text == expected_text, \
                               'actual text is "%s", expected text is "%s"' % \
                               (accessible.text, expected_text)
    
    def assertTable(self, accessible, num_rows, num_cols):
        """
        assert Table implementation, make sure row and col's number are expected
        """
        procedurelogger.action('check "%s" Table implemetation' % accessible)
        itable = accessible._accessible.queryTable()
        procedurelogger.expectedResult('"%s" has %s row(s) and %s column(s)' %\
                                              (accessible, num_rows, num_cols))

        assert itable.nRows == num_rows, \
               "%s rows reported, expected %s" % (itable.nRows, num_rows)

        assert itable.nColumns == num_cols, \
               "%s columns reported, expected %s" % (itable.nColumns, num_rows)

    def selectChild(self, accessible, index):
        """assert Selection implementation"""
        procedurelogger.action('select item%s in "%s"' % (index, accessible))
        accessible.selectChild(index)

    def quit(self):
        self.altF4()
