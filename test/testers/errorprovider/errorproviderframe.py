# vim: set tabstop=4 shiftwidth=4 expandtab
##############################################################################
# Written by:  Brian G. Merrell <bgmerrell@novell.com>
# Date:        12/03/2008
# Description: errorprovider.py wrapper script
#              Used by the errorprovider-*.py tests
##############################################################################

'''Application wrapper for errorprovider.py'''

from strongwind import *
from helpers import *

# class to represent the main window.
class ErrorProviderFrame(accessibles.Frame):

    # constants
    NAME_LABEL = "Name:"
    AGE_LABEL = "Age:"
    WEIGHT_LABEL = "Weight:"
    HEIGHT_LABEL = "Height:"
    DEPTH_LABEL = "Depth:"

    def __init__(self, accessible):
        super(ErrorProviderFrame, self).__init__(accessible)
        self.name_label = self.findLabel(self.NAME_LABEL)
        self.age_label = self.findLabel(self.AGE_LABEL)
        self.weight_label = self.findLabel(self.WEIGHT_LABEL)
        self.height_label = self.findLabel(self.HEIGHT_LABEL)
        self.depth_label = self.findLabel(self.DEPTH_LABEL)

    # check all of the error provider panels that are found and assert that
    # we found the number of panels we expected
    def checkErrorProviderStates(self, num_panels):
        procedurelogger.action('Check the states of all of the panels found')
        panels = self.findAllPanels("")
        assert len(panels) == num_panels
        for panel in panels:
            statesCheck(panel, "Panel")

    # the error provider icons are represented as a nameless :( panel, so we
    # will ensure that the correct panel appeared and that its tooltip is what
    # we expect.  The tester which have to keep track of the panel index
    # (num_panel) in the list of panels found by findAllPanels.
    def assertSingleErrorAppeared(self, num_panel, tooltip_message):
        procedurelogger.action("Check if the correct Error Provider icon appears and has the correct tooltip message")
        procedurelogger.expectedResult("Error Provider number %d appears and has the tooltip message: \"%s\"" % (num_panel, tooltip_message))
        panels = self.findAllPanels("")
        panel = panels[num_panel]

        # move the mouse to the error provider and wait for the tooltip
        # to appear
        panel.mouseMove()
        sleep(config.MEDIUM_DELAY)
   
        # find the tooltip and make sure only one is present
        tooltips = self.app.findAllToolTips(None)
        assert len(tooltips) == 1, "Only one tooltip should be active at once"
        # make sure the tooltip has the expected message
        tooltip = tooltips[0]
        assert tooltip.name == tooltip_message,\
                               "The tooltip does not have the expected message"
        # check the state of the tooltip just for fun
        statesCheck(tooltip, "ToolTip")

    def assertNoPanels(self):
        procedurelogger.action("Ensure that there are no panel accessibles")
        procedurelogger.expectedResult("No panels are found")
        panels = None
        try:
            panels = self.findAllPanels(None)
        except errors.SearchError:
            # this is okay
            return
        assert len(panels) == 0, "There should be no panels"

    # close sample application after running the test
    def quit(self):
        self.altF4()
