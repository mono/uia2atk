
##############################################################################
# Written by:  Neville Gao  <nevillegao@gmail.com>
# Date:        2009/09/22
# Description: repeatbutton.py wrapper script
#              Used by the repeatbutton-*.py tests
##############################################################################

import pyatspi
from strongwind import *
from repeatbutton import *

# class to represent the main window.
class RepeatButtonFrame(accessibles.Frame):

    def __init__(self, accessible):
        super(RepeatButtonFrame, self).__init__(accessible)
        self.frame = self.findDocumentFrame('RepeatButtonSample')
        self.button = self.frame.findButton('Show the time')
        self.label = self.frame.findLabel('Not clicked yet.')

    def press(self, accessible, time):
        procedurelogger.action('Check %s\'s actions' % accessible)

        extents = self.button.queryComponent().getExtents(pyatspi.DESKTOP_COORDS)
        x = extents.x + (extents.width / 2)
        y = extents.y + (extents.height / 2)

        pyatspi.Registry.generateMouseEvent(x, y, 'b1p')
        sleep(time)
        pyatspi.Registry.generateMouseEvent(x, y, 'b1r')
