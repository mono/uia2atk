
##############################################################################
# Written by:  Cachen Chen <cachen@novell.com>
# Date:        07/22/2008
# Description: button.py wrapper script
#              Used by the button-*.py tests
##############################################################################$

import sys
import os
import actions
import states

from strongwind import *
from button import *


# class to represent the main window.
class ButtonFrame(accessibles.Frame):

    # constants
    # the available widgets on the window
    BUTTON_ONE = "button1"
    BUTTON_TWO = "button2"
    BUTTON_TREE = "button3"

    def __init__(self, accessible):
        super(ButtonFrame, self).__init__(accessible)
        self.button1 = self.findPushButton(self.BUTTON_ONE)
        self.button2 = self.findPushButton(self.BUTTON_TWO)
        self.button3 = self.findPushButton(self.BUTTON_TREE)

    #diff Button's inital actions list with expectant list in actions.py
    def actionsCheck(self, accessible):
        procedurelogger.action('diff %s\'s actions list' % accessible)
        ca = accessible._accessible.queryAction()
        initallist = []
        for lists in range(ca.nActions):
            initallist.append(ca.getName(lists))

        procedurelogger.expectedResult('%s\'s inital actions \"%s\" live up to our expectation' % (accessible,initallist))
        def resultMatches():
            return sorted(initallist) == sorted(actions.Button.actions)
        assert retryUntilTrue(resultMatches), "%s != %s" % \
                       (sorted(initallist), sorted(actions.Button.actions))

    #check Button's all expectant states
    def statesCheck(self, accessible):
        procedurelogger.action('Check %s\'s states' % accessible)
        if accessible == self.button3:
            self.assertInsensitiveButtonStates(accessible)    
        else:
            self.assertSensitiveButtonStates(accessible)    

    def assertInsensitiveButtonStates(self, accessible):
        # create a list of all states for button except "sensitive"
        invalid_states = ["sensitive", "enabled"] 
        expected_states = \
                  [s for s in states.Button.states if s not in invalid_states]

        procedurelogger.expectedResult('States:  %s' % expected_states)

        # get a list of all actual states for accessible
        actual_states = accessible._accessible.getState().getStates()
        # need to convert the numbers retreived above into their associated
        # strings
        actual_states = [pyatspi.stateToString(s) for s in actual_states]

        # assert there are no elements in expected_states that are not
        # in actual_states
        missing_states = set(expected_states).difference(set(actual_states))

        # assert there are no elements in actual_states that are not
        # in expected_states
        extra_states = set(actual_states).difference(set(expected_states))

        is_same = len(missing_states) == 0 and len(extra_states) == 0
        assert is_same, "\n  %s: %s\n  %s: %s" %\
                                             ("Missing actual states: ",
                                               missing_states,
                                              "Extraneous actual states: ",
                                               extra_states) 
        
    def assertSensitiveButtonStates(self, accessible):
        # create a list of all states for button except "sensitive"
        expected_states = [s for s in states.Button.states]

        procedurelogger.expectedResult('States:  %s' % expected_states)

        # get a list of all actual states for accessible
        actual_states = accessible._accessible.getState().getStates()
        # need to convert the numbers retreived above into their associated
        # strings
        actual_states = [pyatspi.stateToString(s) for s in actual_states]

        # assert there are no elements in expected_states that are not
        # in actual_states and vice versa
        missing_states = set(expected_states).difference(set(actual_states))
        extra_states = set(actual_states).difference(set(expected_states))

        is_same = len(missing_states) == 0 and len(extra_states) == 0
        assert is_same, "\n  %s: %s\n  %s: %s" %\
                                             ("Missing actual states: ",
                                               missing_states,
                                              "Extraneous actual states: ",
                                               extra_states) 

    #give 'click' action
    def click(self,button):
        button.click()

    #check the Label text after click button2
    def assertLabel(self, labelText):
        procedurelogger.expectedResult('Label text has been changed to \"%s\"' % labelText)
        self.findLabel(labelText)

    #rise message frame window after click button1
    def assertMessage(self):
        self.message = self.app.findFrame('message')

        self.message.findPushButton('OK').click()
    
    #close application main window after running test
    def quit(self):
        self.altF4()
