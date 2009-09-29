
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

    def __init__(self, accessible):
        super(SliderFrame, self).__init__(accessible)
        self.frame = self.findDocumentFrame('SliderSample')
        # two labels to show slider's value
        self.label1 = self.frame.findLabel(self.LABEL_ONE)
        self.label2 = self.frame.findLabel(self.LABEL_TWO)
        # one checkbox for IsDirectionReversed setting
        self.checkbox = self.frame.findCheckBox(self.CHECKBOX)
        # there are two sliders
        self.sliders = self.frame.findAllSliders("")
        assert len(self.sliders) == self.SLIDER_NUM, \
                "actual number of slider is:%s, expected is:%s" % \
                 (len(self.sliders), self.SLIDER_NUM)
        # find horizontal slider and thumb
        self.horizontal_slider = self.sliders[0]
        self.horizontal_thumb = self.horizontal_slider.findPushButton(None)
        # find vertical slider and thumb
        self.vertical_slider = self.sliders[1]
        self.vertical_thumb = self.vertical_slider.findPushButton(None)

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



