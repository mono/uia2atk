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

    def __init__(self, accessible):
        super(TrackBarFrame, self).__init__(accessible)
        self.trackbars = self.findAllSliders(None)
        self.labels = self.findAllLabels(None)

        assert len(self.labels) == 2, "The number of labels is wrong!"
        assert len(self.trackbars) == 2, "The number of trackbars is wrong!"

        self.trackbar_ver = self.trackbars[0]
        self.trackbar_hor = self.trackbars[1]
        self.label_ver = self.labels[0]
        self.label_hor = self.labels[1]

    def inputText(self, accessible, text=None):
        procedurelogger.action('set %s text to "%s"' % (accessible, text))
        try:
            accessible.text = text
        except NotImplementedError:
            pass

    def inputValue(self, accessible, value=None):
        procedurelogger.action('set %s value to "%s"' % (accessible, value))
        accessible.value = value

    def assertValue(self, accessible, value=None):
        procedurelogger.action('Assert the value of %s' % accessible)
        procedurelogger.expectedResult('%s value is %d' % \
                                                (accessible, accessible.value))
        assert accessible.value == value, "%s value is not match with %d" % \
                                                (accessible, accessible.value)
    
    def assertText(self, accessible, text=None):
        """assert text is equal to the input"""

        procedurelogger.action('Assert the text of %s' % accessible)
        procedurelogger.expectedResult('%s text is "%s"' % \
                                                (accessible, accessible.text))
        assert accessible.text == text, '%s is not match with "%s"' % \
                                                (accessible, accessible.text)

    def quit(self):
        self.altF4()
