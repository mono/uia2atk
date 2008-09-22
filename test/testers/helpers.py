##############################################################################
# Written by:  Brian G. Merrell <bgmerrell@novell.com>
# Date:        09/22/2008
# Description: A file where generic functions that help with testing can be
#              placed.  This file should be used to avoid duplicating the same
#              function over and over in the application wrappers.
#              
##############################################################################

# The docstring below  is used in the generated log file
"""
A file where generic functions that help with testing can be placed.  This file
should be used to avoid duplicating the same function over and over in the
application wrappers.
"""

import states
from strongwind import *

def statesCheck(accessible, control, invalid_states=[], add_states=[]):
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


