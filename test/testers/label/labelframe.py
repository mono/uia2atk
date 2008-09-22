
##############################################################################
# Written by:  Cachen Chen <cachen@novell.com>
# Date:        08/07/2008
# Description: label.py wrapper script
#              Used by the label-*.py tests
##############################################################################

import sys
import os
import actions
import states

from strongwind import *
from label import *


# class to represent the main window.
class LabelFrame(accessibles.Frame):

    # constants
    # the available widgets on the window
    BUTTON = "button2"
    LABEL = ""

    def __init__(self, accessible):
        super(LabelFrame, self).__init__(accessible)
        self.button = self.findPushButton(self.BUTTON)
        self.label = self.findLabel(None)

    #check label's all expectant states
    def statesCheck(self, accessible, control,
                                invalid_states=[], add_states=[]):
        """Check the states of an accessible using the default states
        of the accessible (specified by control class in states.py) as
        the default expected states.
       
        Keyword arguments:
        accessible -- the accessible whose states will be checked
        control -- the class name of the control whose states we want to check
        invalid_states -- a list of states that should be removed from the
        list of default expected states
        add_states -- a list of states that should be added to the list of
        default expected states

        """
        procedurelogger.action('Check %s\'s states' % accessible)
        # create a list of all states for button except "sensitive"
        states_list = states.__getattribute__(control).states
        expected_states = \
                  [s for s in states_list if s not in invalid_states]
        expected_states = set(expected_states).union(set(add_states))

        procedurelogger.expectedResult('States:  %s' % expected_states)

        # get a list of all actual states for accessible
        actual_states = accessible._accessible.getState().getStates()
        # need to convert the numbers retrieved above into their associated
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

    #give 'click' action
    def click(self,button):
        button.click()

    #check the Label text after click button2
    def assertLabel(self, labelText):
        procedurelogger.expectedResult('Label text has been changed to "%s"' % labelText)
        self.findLabel(labelText)

    #check label's text value
    def assertText(self, textValue):
        procedurelogger.expectedResult('Label\'s text value shows in accerciser is "%s"' % textValue)
        def resultMatches():
            return self.label.text == textValue
        assert retryUntilTrue(resultMatches)
    
    #close application main window after running test
    def quit(self):
        self.altF4()
