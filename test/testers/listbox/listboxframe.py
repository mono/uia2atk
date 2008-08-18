
##############################################################################
# Written by:  Cachen Chen <cachen@novell.com>
# Date:        08/19/2008
# Description: listbox.py wrapper script
#              Used by the listbox-*.py tests
##############################################################################

import sys
import os
import actions
import states

from strongwind import *
from listbox import *


# class to represent the main window.
class ListBoxFrame(accessibles.Frame):

    # constants
    # the available widgets on the window
    LABEL1 = "You select "
    LISTITEM0 = "0"

    def __init__(self, accessible):
        super(ListBoxFrame, self).__init__(accessible)
        self.label1 = self.findLabel(self.LABEL1)
        self.listbox = self.findList(None)
        self.listitem0 = self.findListItem(self.LISTITEM0)

    #diff listitem's inital actions list of ListBox with expectant list in actions.py
    def actionsCheck(self, accessible):
        procedurelogger.action('diff %s\'s actions list' % accessible)
        ca = accessible._accessible.queryAction()
        initallists = ()
        for lists in range(ca.nActions):
            initallists = (ca.getName(lists))

        procedurelogger.expectedResult('%s\'s inital actions \"%s\" live up to\
	our expectation' % (accessible,initallists))
        def resultMatches():
            return sorted(initallists) == sorted(actions.ListItem.actions)
        assert retryUntilTrue(resultMatches)

    #check listbox's all expectant states
    def statesCheck_box(self, accessible):
        procedurelogger.action('check %s\'s all states' % accessible)

        procedurelogger.expectedResult('%s\'s all states can be found' % accessible)
        for a in states.ListBox.states:
            cmd = "state = accessible." + a
            exec(cmd)

            if state == False:
                print "ERROR: %s can't be checked" % cmd
            else:
                pass

    #check listitem's all expectant states
    def statesCheck_item(self, accessible):
        procedurelogger.action('check %s\'s all states' % accessible)

        procedurelogger.expectedResult('%s\'s all states can be found' % accessible)
        for a in states.ListItem.states:
            cmd = "state = accessible." + a
            exec(cmd)

            if state == False:
                print "ERROR: %s can't be checked" % cmd
            else:
                pass

    #give 'click' action
    def click(self,itemname):
        listitem = self.findListItem(itemname)
        #procedurelogger.action('Click the %s.' % itemname)
        #listitem._doAction('click')
        listitem.click()

    #check the state after click listitem
    def assertItemSelected(self, itemname):
        'Raise exception if the accessible does not match the given result'   
        procedurelogger.expectedResult('item \"%s\" is %s' % (itemname, 'select'))

        def resultMatches():
            return self.findLabel("You select %s" % itemname)
	
        assert retryUntilTrue(resultMatches)
    
    #close application main window after running test
    def quit(self):
        self.altF4()
