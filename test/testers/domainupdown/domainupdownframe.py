# vim: set tabstop=4 shiftwidth=4 expandtab
##############################################################################
# Written by:  Ray Wang <rawang@novell.com>
# Date:        12/07/2008
# Description: Application wrapper for domainupdown.py
#              be called by ../domainupdown_basic_ops.py
##############################################################################$

"""Application wrapper for domainupdown.py"""

from strongwind import *
import pdb

# class to represent the main window.
class DomainUpDownFrame(accessibles.Frame):
    

    def __init__(self, accessible):
        cities = ("Austin", "Beijing", "Cambridge", "Madrid", "Provo", "San Diego")
        super(DomainUpDownFrame, self).__init__(accessible)
        self.domainupdown = self.findAllSpinButtons(None)
        # editable
        self.editable_domainupdown = self.domainupdown[0]
        self.editable_domainupdown.listitem = \
            [self.editable_domainupdown.findListItem(x, checkShowing=False) for x in cities]

        # uneditable
        self.uneditable_domainupdown = self.domainupdown[1]
        self.uneditable_domainupdown.listitem = \
            [self.uneditable_domainupdown.findListItem(x, checkShowing=False) for x in cities]

    # enter Text Value for EditableText
    def inputText(self, accessible, text):
        procedurelogger.action('set %s text to "%s"' % (accessible, text))
        if accessible == self.editable_domainupdown:
            accessible.text = text
        elif accessible == self.uneditable_domainupdown:
            try:
                accessible.text = text
            except NotImplementedError:
                pass

    # assert domainupdown's Text value
    def assertText(self, accessible, text):
        procedurelogger.expectedResult('%s text is "%s"' % \
                                                (accessible, accessible.text))
        assert accessible.text == text, '%s text is not match with "%s"' % \
                                                (accessible, accessible.text)

    #close application window
    def quit(self):
        self.altF4()
