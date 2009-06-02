# vim: set tabstop=4 shiftwidth=4 expandtab
##############################################################################
# Written by:  Ray Wang <rawang@novell.com>
# Date:        02/05/2009
# Description: Application wrapper for trackbar.py
#              be called by ../trackbar_basic_ops.py
##############################################################################

"""Application wrapper for trackbar.py"""

from strongwind import *

class TrackBarFrame(accessibles.Frame):
    """the profile of the trackbar sample"""

    NUMLABELS = 2
    NUMTRACKBARS = 2

    def __init__(self, accessible):
        super(TrackBarFrame, self).__init__(accessible)

        # The findAllSliders method may need to change when BUG504433 is fixed
        self.trackbars = self.findAllSliders("")
        self.labels = self.findAllLabels(None)

        assert len(self.labels) == self.NUMLABELS, \
                "The number of labels is wrong! (found %s, expected %s)" % \
                (len(self.labels), self.NUMLABELS)
        assert len(self.trackbars) == self.NUMTRACKBARS, \
                "The number of trackbars is wrong! (found %s, expected %s)" % \
                (len(self.trackbars), self.NUMTRACKBARS)

        self.trackbar_ver = self.trackbars[0]
        self.trackbar_hor = self.trackbars[1]

        self.label_ver = self.labels[0]
        self.label_hor = self.labels[1]
        
    def assignText(self, accessible, text):
        procedurelogger.action('set %s text to "%s"' % (accessible, text))
        try:
            accessible.text = text
        except NotImplementedError:
            pass

    def assignValue(self, accessible, value):
        procedurelogger.action('set %s\'s value to "%s"' % (accessible, value))
        accessible.value = value

    def assertValue(self, accessible, expected_value):
        procedurelogger.action('Assert that the value of %s is what we expect' % accessible)
        actual_value = accessible.value
        procedurelogger.expectedResult('%s value is %s' % \
                                                (accessible, expected_value))
        assert actual_value == expected_value, \
                   'Value was %s, expected %s' % (actual_value, expected_value)
    
    def assertText(self, accessible, expected_text):
        """assert that the actual text is equal to the expected text"""

        procedurelogger.action('Assert that the text of %s is what we expect' % accessible)
        actual_text = accessible.text
        procedurelogger.expectedResult('%s text is "%s"' % \
                                                (accessible, expected_text))
        assert actual_text == expected_text, \
                  'Text was "%s", expected "%s"' % (actual_text, expected_text)

    def quit(self):
        self.altF4()
