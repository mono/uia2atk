
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

class States(object):

    def __init__(self, states, control_name):
        self.states = list(states)
        self.control_name = control_name

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
        self.button1_states = States(states.Button.states, "Button")
        self.button2_states = States(states.Button.states, "Button")
        invalid_states = ["sensitive","enabled"]
        default_states = \
                  [s for s in states.Button.states if s not in invalid_states]
        self.button3_states = States(default_states, "Button")
        self.states_dict = { self.button1 : self.button1_states,
                             self.button2 : self.button2_states,
                             self.button3 : self.button3_states }

 
    #diff Button's inital actions list with expectant list in actions.py
    def actionsCheck(self, accessible):
        procedurelogger.action('Check %s\'s actions' % accessible)

        # get list of expected actions 
        expected_actions = actions.Button.actions

        # get list of actual actions of the accessible
        qa = accessible._accessible.queryAction()
        actual_actions = [qa.getName(i) for i in range(qa.nActions)]

        procedurelogger.expectedResult('Actions: %s' % actual_actions)

        # get a list of actual states that are missing or extraneous
        missing_actions = set(expected_actions).difference(set(actual_actions))
        extra_actions = set(actual_actions).difference(set(expected_actions))

        # if missing_actions and extra_actions are empty, the test case passes
        # otherwise, throw an exception
        is_same = len(missing_actions) == 0 and len(extra_actions) == 0
        assert is_same, "\n  %s: %s\n  %s: %s" %\
                                             ("Missing actual actions: ",
                                               missing_actions,
                                              "Extraneous actual actions: ",
                                               extra_actions) 

    #check Button's all expectant states
    def statesCheck(self):
        """Check the states of an accessible using states_dict to retrieve
        the expected states
        """
        procedurelogger.action('Check all states for all accessibles')
        # create a list of all states for button except "sensitive"


        # get a list of all actual states for accessible
        accessibles = [accessible for accessible in self.states_dict]
        actual_states_dict = {}
        for accessible in accessibles:
            actual_states_dict[accessible] = \
                                  accessible._accessible.getState().getStates()
        for accessible in actual_states_dict:
            for i in range(len(actual_states_dict[accessible])):
                actual_states_dict[accessible][i] = \
                        pyatspi.stateToString(actual_states_dict[accessible][i])
                
        procedurelogger.expectedResult('All states are accurate')

        for accessible in accessibles:
            actual_states = actual_states_dict[accessible]
            expected_states = self.states_dict[accessible].states

            # assert there are no elements in expected_states that are not
            # in actual_states
            missing_states = set(expected_states).difference(set(actual_states))

            # assert there are no elements in actual_states that are not
            # in expected_states
            extra_states = set(actual_states).difference(set(expected_states))

            is_same = len(missing_states) == 0 and len(extra_states) == 0
            assert is_same, "Error for accessible:  %s\n  %s: %s\n  %s: %s" %\
                                                  (accessible,
                                                  "Missing actual states: ",
                                                   missing_states,
                                                  "Extraneous actual states: ",
                                                   extra_states) 
            
    def buttonClick(self, button):
        """ give 'click' action for a button """
        button.click()
        # remove the expected focused state from any other states
        [self.states_dict[a].remove(states.FOCUSED) for a in self.states_dict] 
        self.states_dict[button].states.append(states.FOCUSED)


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
