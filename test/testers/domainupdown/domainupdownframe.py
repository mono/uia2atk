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
from helpers import *

# class to represent the main window.
class DomainUpDownFrame(accessibles.Frame):
    

    def __init__(self, accessible):
        cities = ("Austin", "Beijing", "Cambridge", "Madrid", "Provo", "San Diego")
        super(DomainUpDownFrame, self).__init__(accessible)
        self.domainupdown = self.findAllSpinButtons(None)
        # editable
        self.editable_domainupdown = self.domainupdown[0]
        self.editable_domainupdown.listitems = \
            [self.editable_domainupdown.findListItem(x, checkShowing=False) for x in cities]

        # uneditable
        self.uneditable_domainupdown = self.domainupdown[1]
        self.uneditable_domainupdown.listitems = \
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
    def assertText(self, accessible, expected_text):
        procedurelogger.expectedResult('%s text is "%s"' % \
                                                (accessible, accessible.text))
        actual_text = accessible.text
        assert  actual_text == expected_text, \
                                   '%s text is "%s", expected "%s"' % \
                                   (accessible, actual_text, expected_text)

    def selectChild(self, accessible, index):
        """assert Selection implementation"""
        
        procedurelogger.action('select index %s in %s' % (index, accessible))
        accessible.selectChild(index)

    def checkAllStates(self, domainupdown_items, focused_item):
        '''
        Check the states of all the list items of domainupdown_items.  The
        focused_item should have +selected +focused states.  All other
        list items should have -showing -visible states
        '''
        for item in domainupdown_items:
            if item == focused_item:
                #BUG482285
                #statesCheck(item, "ListItem", add_states=["selected", "focused"])
                pass # delete this line when BUG482285 is fixed
            else:
                #BUG482285
                #statesCheck(item, "ListItem", invalid_states=["showing", "visible"])
                pass # delete this line when BUG482285 is fixed
        
    #close application window
    def quit(self):
        self.altF4()
