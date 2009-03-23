# vim: set tabstop=4 shiftwidth=4 expandtab
##############################################################################
# Written by:  Brian G. Merrell <bgmerrell@novell.com>
# Date:        12/03/2008
# Description: helpprovider.py wrapper script
#              Used by the helpprovider-*.py tests
##############################################################################

'''Application wrapper for helpprovider.py'''

from strongwind import *
from helpers import *
import sys

# class to represent the main window.
class HelpProviderFrame(accessibles.Frame):

    STREET_TIP = "Enter the street address in this text box."
    CITY_TIP = "Enter the city here."
    STATE_TIP = "Enter the state in this text box."
    ZIP_TIP = "Enter the zip code here."

    def __init__(self, accessible):
        super(HelpProviderFrame, self).__init__(accessible)
        self.text_boxes = self.findAllTexts(None)
        try:
            self.street_text_box = self.text_boxes[3]
            self.city_text_box = self.text_boxes[2]
            self.state_text_box = self.text_boxes[1]
            self.zip_text_box = self.text_boxes[0]
        except IndexError, e:
            print "Could not find all the expected text boxes"
            print e
            sys.exit(1)


    def assert_tooltip_appeared(self, message):
        procedurelogger.action("Verify that a tooltip appears and that it has the correct message.  Also verify that no other tooltip accessibles are found")
        procedurelogger.expectedResult("Tooltip appears and reads: \"%s\"" % message)  
        # verify that we can only find one tooltip
        tooltips = self.app.findAllToolTips(None)
        assert len(tooltips) == 1, "Only one tooltip accessible should exist"
        # verify that the tooltip has the message we expect
        tooltip = tooltips[0]
        assert tooltip.name == message, \
                               "The tooltip does not have the expected message"
        # check the state of the tooltip just for fun
        statesCheck(tooltip, "ToolTip")

    def assert_descriptions(self):
        # Make sure that  the accessible description for each text box matches
        # the tooltip message for that text box.  This could be done from
        # assert_tooltip_appeared, but this allows a lot of tests to run even
        # if this assertion fails
        for text_box in self.text_boxes:
            procedurelogger.action("Click in %s" % text_box)
            text_box.mouseClick()
            self.keyCombo("F1")
            sleep(config.SHORT_DELAY)
            procedurelogger.expectedResult("A tooltip appears for %s" % \
                                           text_box)
            tooltip = self.app.findAllToolTips(None)[0]
            #BUG487859, COMMENTING OUT TEST BECAUSE BUG IS AN ENHANCEMENT
            #procedurelogger.action("Verify that the accessible description for the text box matches the text box's tooltip message.")
            #procedurelogger.expectedResult("The accessible description \"%s\" matches the tooltip message \"%s\"" % (text_box.description, tooltip.name))
            #assert text_box.description == tooltip.name
            #END BUG487859

    # close sample application after running the test
    def quit(self):
        self.altF4()
