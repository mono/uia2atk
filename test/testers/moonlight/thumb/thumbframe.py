
##############################################################################
# Written by:  Neville Gao  <nevillegao@gmail.com>
# Date:        2009/09/22
# Description: thumb.py wrapper script
#              Used by the thumb-*.py tests
##############################################################################

from strongwind import *
from thumb import *

# class to represent the main window.
class ThumbFrame(accessibles.Frame):

    def __init__(self, accessible):
        super(ThumbFrame, self).__init__(accessible)
        self.frame = self.findDocumentFrame('ThumbSample')
        self.filler = self.frame.findFiller('Silverlight Control')
        self.thumb = self.filler.findPushButton('')
        self.label = self.filler.findLabel('Size: 100, 100')
