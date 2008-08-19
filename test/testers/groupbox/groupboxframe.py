
##############################################################################
# Written by:  Cachen Chen <cachen@novell.com>
# Date:        08/19/2008
# Description: groupbox.py wrapper script
#              Used by the groupbox-*.py tests
##############################################################################$

import sys
import os
import actions
import states

from strongwind import *
from groupbox import *


# class to represent the main window.
class GroupBoxFrame(accessibles.Frame):

    # constants
    # the available widgets on the window
    BUTTON_ONE = "button1"
    BUTTON_TWO = "button2"

    def __init__(self, accessible):
        super(GroupBoxFrame, self).__init__(accessible)
        self.button1 = self.findPushButton(self.BUTTON_ONE)
        self.button2 = self.findPushButton(self.BUTTON_TWO)

    #diff GroupBox's inital actions list with expectant list in actions.py
    def actionsCheck(self, accessible):
        procedurelogger.action('diff %s\'s actions list' % accessible)
        ca = accessible._accessible.queryAction()
        initallists = ()
        for lists in range(ca.nActions):
            initallists = (ca.getName(lists))

        procedurelogger.expectedResult('%s\'s inital actions \"%s\" live up to\
	our expectation' % (accessible,initallists))
        def resultMatches():
            return sorted(initallists) == sorted(actions.Button.actions)
        assert retryUntilTrue(resultMatches)

    #check panel's all expectant states of GroupBox
    def statesCheck(self, accessible=None):
        accessible = self.findPanel(None)
        procedurelogger.action('check %s\'s all states' % accessible)

        procedurelogger.expectedResult('%s\'s all states can be found' % accessible)
        #for a in states.VScrollBar.states:
        #    cmd = "state = accessible." + a
        #   exec(cmd)

        #    if state == False:
        #        print "ERROR: %s can't be checked" % cmd
        #    else:
        #        pass
        #if there is just one state in list, should reset it like:
        cmd = "state = accessible." + states.Panel.states
        exec(cmd)
        
        if state == False:
            print "ERROR: %s can't can't be checked" % cmd
        else:
            pass

    #search groupbox
    def searchGroupBox(self,boxname=None):
        if boxname == 'GroupBox1':
            procedurelogger.action('search for panel of \"%s\"' % boxname)

            sleep(config.SHORT_DELAY)
            procedurelogger.expectedResult('\"%s\" existed' % boxname)
            self.findPanel('GroupBox1')

        elif boxname =='GroupBox2':
            procedurelogger.action('search for panel of \"%s\"' % boxname)

            sleep(config.SHORT_DELAY)
            procedurelogger.expectedResult('\"%s\" existed' % boxname)
            self.findPanel('GroupBox2')

    #give 'click' action
    def click(self,button):
        #procedurelogger.action('Click the %s.' % button)
        button.click()
        #button._doAction('click')

    #check the Label text after click button
    def assertLabel(self, labelText):
        procedurelogger.expectedResult('Label text has been changed to \"%s\"' % labelText)
        self.findLabel(labelText)
    
    #close application main window after running test
    def quit(self):
        self.altF4()
