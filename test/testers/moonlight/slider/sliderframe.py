
##############################################################################
# Written by:  Calen Chen  <cachen@gmail.com>
# Date:        2009/09/23
# Description: slider.py wrapper script
#              Used by the slider-*.py tests
##############################################################################

from strongwind import *
from slider import *

# class to represent the main window.
class SliderFrame(accessibles.Frame):
    # Variables
    LABEL_ONE = "Horizontal Slider Value: 0"
    LABEL_TWO = "Vertical Slider Value: 15"
    CHECKBOX = "IsDirectionReversed"
    SLIDER_NUM = 2
    BUTTON_NUM = 3

    def __init__(self, accessible):
        super(SliderFrame, self).__init__(accessible)
        self.frame = self.findDocumentFrame('SliderSample')
        self.filler = self.frame.findFiller('Silverlight Control')
        # two labels to show slider's value
        self.label1 = self.filler.findLabel(self.LABEL_ONE)
        self.label2 = self.filler.findLabel(self.LABEL_TWO)
        # one checkbox for IsDirectionReversed setting
        self.checkbox = self.filler.findCheckBox(self.CHECKBOX)
        # there are two sliders
        self.sliders = self.filler.findAllSliders("")
        assert len(self.sliders) == self.SLIDER_NUM, \
                "actual number of slider is:%s, expected is:%s" % \
                 (len(self.sliders), self.SLIDER_NUM)

        # find horizontal slider and buttons
        self.horizontal_slider = self.sliders[0]
        self.hs_buttons = self.horizontal_slider.findAllPushButtons(None)
        # BUG560711: extraneous push buttons
        #assert len(self.hs_buttons) == self.BUTTON_NUM, \
        #        "actual number of button is:%s, expected is:%s" % \
        #         (len(self.hs_buttons), self.BUTTON_NUM)

        self.horizontal_thumb = self.hs_buttons[1]

        # find vertical slider and buttons
        self.vertical_slider = self.sliders[1]
        self.vs_buttons = self.vertical_slider.findAllPushButtons(None)
        # BUG560711: extraneous push buttons
        #assert len(self.vs_buttons) == self.BUTTON_NUM, \
        #        "actual number of button is:%s, expected is:%s" % \
        #         (len(self.vs_buttons), self.BUTTON_NUM)
        self.vertical_thumb = self.vs_buttons[1]

    def assertValue(self, accessible, expected_value):
        """
        Make sure the value of accessible is expected
        """
        procedurelogger.expectedResult("update %s's value to %s" % 
                                   (accessible,expected_value))

        assert  accessible.value == expected_value,\
                "actual value is %s, expected value is %s" % \
                (accessible.value, expected_value)

    def setValue(self, accessible, expected_value):
        """
        Set accessible's value
        """
        procedurelogger.action("set %s's value to %s" % 
                                       (accessible,expected_value))

        accessible.value = expected_value

    def assertMaximumValue(self, accessible, expected_value):
        """
        check the accessible's maximum value
        """
        procedurelogger.action("Ensure that %s's maximum value is what we expect" % accessible)
        procedurelogger.expectedResult("%s's maximum value is %s" % \
                                                (accessible, expected_value))
        self.maximumValue = \
                        accessible._accessible.queryValue().maximumValue
        assert self.maximumValue == expected_value, \
                                        "Maximum value is %s, expected %s" % \
                                        (self.maximumValue, expected_value)

    def assertMinimumValue(self, accessible, expected_value):
        """
        check the accessible's minimum value
        """
        procedurelogger.action("Ensure that %s's minimum value is what we expect" % accessible)
        procedurelogger.expectedResult("%s's minimum value is %s" % \
                                                (accessible, expected_value))
        self.minimumValue = \
                        accessible._accessible.queryValue().minimumValue
        assert self.minimumValue == expected_value, \
                                        "Minimum value is %s, expected %s" % \
                                        (self.minimumValue, expected_value)
