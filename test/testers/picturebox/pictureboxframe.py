##############################################################################
# Written by:  Cachen Chen <cachen@novell.com>
# Date:        08/14/2008
# Description: picturebox.py wrapper script
#              Used by the picturebox-*.py tests
##############################################################################$

import sys
import os
import actions
import states

from strongwind import *
from picturebox import *

# class to represent the main window.
class PictureBoxFrame(accessibles.Frame):

    # constants
    # the available widgets on the window
    BUTTON_ONE = "Toggle"

    def __init__(self, accessible):
        super(PictureBoxFrame, self).__init__(accessible)
        self.button1 = self.findPushButton(self.BUTTON_ONE)

    #diff PictureBox's inital actions list with expectant list in actions.py
    def actionsCheck(self, accessible):
        procedurelogger.action('diff %s\'s actions list' % accessible)
        ca = accessible._accessible.queryAction()

        actual_actions = []
        for i in range(ca.nActions):
            actual_actions.append(ca.getName(i))

        procedurelogger.expectedResult('%s\'s actual actions "%s" live up to our expectation' % (accessible, actual_actions))
        def resultMatches():
            return sorted(actual_actions) == sorted(actions.Button.actions)
        assert retryUntilTrue(resultMatches), "%s != %s" % \
                       (sorted(actual_actions), sorted(actions.Button.actions))

    #check PictureBox's all expectant states
    def statesCheck(self):
        procedurelogger.action('check %s\'s all states' % self)

        procedurelogger.expectedResult('%s\'s all states can be found' % self)
        
        for s in states.PictureBox.states:
            state = getattr(self, s)
            assert state, "Expected state: %s" % (s)
        		
    #give 'click' action
    def click(self,button):
        #procedurelogger.action('Click the %s.' % button)
        #button._doAction("click")
        button.click()

    #check the picture after click button
    def assertPicture(self, picture=None):
        def resultMatches():
            if picture == 1:
                procedurelogger.expectedResult('picture has been changed to "%s"' % 'desktop-blue_soccer.jpg')
                return self.findLabel("You are watching %s/samples/desktop-blue_soccer.jpg" % uiaqa_path)
            if picture == 2:
                procedurelogger.expectedResult('picture has been changed to "%s"' % 'universe.jpg')
                return self.findLabel("You are watching %s/samples/universe.jpg" % uiaqa_path)
        assert retryUntilTrue(resultMatches), "Expected picture: %s" % picture
    
    #close application main window after running test
    def quit(self):
        self.altF4()
