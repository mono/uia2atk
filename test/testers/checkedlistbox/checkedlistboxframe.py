
##############################################################################
# Written by:  Cachen Chen <cachen@novell.com>
# Date:        08/21/2008
# Description: checkedlistbox.py wrapper script
#              Used by the checkedlistbox-*.py tests
##############################################################################

import sys
import os
import actions
import states

from strongwind import *
from checkedlistbox import *


# class to represent the main window.
class CheckedListBoxFrame(accessibles.Frame):

    # constants
    # the available widgets on the window
    LABEL1 = ""
    LISTITEM0 = "0"

    def __init__(self, accessible):
        super(CheckedListBoxFrame, self).__init__(accessible)
        self.label1 = self.findLabel(self.LABEL1)
        self.listbox = self.findList(None)
        self.listitem0 = self.findListItem(self.LISTITEM0)

    #diff listitem's inital actions list of CheckedListBox with expectant list in actions.py
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

    #check Checkedlistbox's all expectant states
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
        procedurelogger.expectedResult('Item \"%s\" is %s' % (itemname, 'selected'))

        def resultMatches():
            return self.findLabel("Item %s : Checked" % itemname)
	
        assert retryUntilTrue(resultMatches)

    #check the state after click listitem
    def assertChecked(self, itemname):
        'Raise exception if the accessible does not match the given result'   
        procedurelogger.expectedResult('Item \"%s\" is %s' % (itemname, 'checked'))
        accessible = self.findListItem(itemname)

        def resultMatches():
            return accessible.checked
	
        assert retryUntilTrue(resultMatches)

    def assertUnchecked(self, itemname):
        'Raise exception if the accessible does not match the given result'   
        procedurelogger.expectedResult('Item \"%s\" is %s.' % (itemname, "unchecked"))
        accessible = self.findListItem(itemname)

        def resultMatches():
            return not accessible.checked
	
        assert retryUntilTrue(resultMatches)
        
    
    #close application main window after running test
    def quit(self):
        self.altF4()
