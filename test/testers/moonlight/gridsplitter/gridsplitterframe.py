
##############################################################################
# Written by:  Calen Chen  <cachen@gmail.com>
# Date:        2009/09/29
# Description: gridsplitter.py wrapper script
#              Used by the gridsplitter-*.py tests
##############################################################################

from strongwind import *
from gridsplitter import *

# class to represent the main window.
class GridSplitterFrame(accessibles.Frame):
    # Variables
    LABEL1 = "One!"
    LABEL2 = "Two!"
    LABEL3 = "Three!"
    LABEL4 = "Four!"
    THUMB_NUM = 2

    def __init__(self, accessible):
        super(GridSplitterFrame, self).__init__(accessible)
        self.frame = self.findDocumentFrame("GridSplitterSample")
        self.filler = self.frame.findFiller("Silverlight Control")
        # 4 labels in different grid
        self.label1 = self.filler.findLabel(self.LABEL1)
        self.label2 = self.filler.findLabel(self.LABEL2)
        self.label3 = self.filler.findLabel(self.LABEL3)
        self.label4 = self.filler.findLabel(self.LABEL4)
        # 2 thumbs with PushButton role name, gridsplitter is a Thumb 
        # controltype
        self.thumbs = self.filler.findAllPushButtons("")
        assert len(self.thumbs) == self.THUMB_NUM, \
                "actual number of thumb is:%s, expected is:%s" % \
                 (len(self.thumbs), self.THUMB_NUM)
        self.vertical_thumb = self.thumbs[0]
        self.horizontal_thumb = self.thumbs[1]

    def assertAction(self, accessible):
        """
        Action shouldn't be implemented even though the accessible's
        a PushButton
        """
        procedurelogger.action("check %s's Action implementation" % accessible) 
        procedurelogger.expectedResult("Action is not implemented") 
        try:
            accessible._accessible.queryAction()
        except NotImplementedError:
            return
        assert False, "Action shouldn't be implemented"
        
    def changePosition(self, accessible, key):
        """
        Make sure accessible's position is changed
        """
        # get the old position before press key to move splitter
        old_position = accessible._getAccessibleCenter()
        # press key for accessible
        accessible.keyCombo(key)
        sleep(config.SHORT_DELAY)
        # get new position
        new_position = accessible._getAccessibleCenter()

        procedurelogger.expectedResult("%s's position is changed to %s from %s" % \
                                   (accessible, new_position, old_position))
        assert new_position != old_position, "%s position should be moved" % \
                                               accessible
