##############################################################################
# Written by:  Andres G. Aragoneses <aaragoneses@novell.com>
# Date:        02/24/2009
# Description: propertygridframe.py wrapper script
#              Used by the propertygrid-*.py tests
##############################################################################$

import sys
import os
import actions
import states

from strongwind import *
from datagridview import *

# class to represent the main window.
class PropertyGridFrame(accessibles.Frame):

        # constants
        # the available widgets on the frame
        CELL_A11Y = "Accessibility"
        CELL_ACCESSIBLE_DESC = "AccessibleDescription"
        CELL_VISIBLE = "Visible"

        def __init__(self, accessible):
                super(PropertyGridFrame, self).__init__(accessible)
                self.toolbar = self.findToolBar(None)
                self.treetable = self.findTreeTable(None)
                self.propertygrid = self.findPanel("Property Grid")
                self.subpanels = self.propertygrid.findAllPanels(None)

                #maybe this check is useless because the first panel just holds the treetable
                #so change it to "1" if this changes in the future:
                assert len(self.subpanels) == 2

                self.textpanel = self.subpanels [1]

                #enable the line above when bug 479113 is fixed:
                #self.a11ycell = self.findTableCell(self.CELL_A11Y, checkShowing=False)

                #check for relations of accessibledesccell when bug 479142 is fixed
                self.accessibledesccell = self.findTableCell(self.CELL_ACCESSIBLE_DESC, checkShowing=False)
                self.visible = self.findTableCell(self.CELL_VISIBLE, checkShowing=False)

                self.tablecells = self.findAllTableCells(None)
                self.togglebuttons = self.toolbar.findAllToggleButtons(None)

                self.categorizedbutton = self.togglebuttons[1]
                self.alphabeticbutton = self.togglebuttons[2]

        #close application main window after running test
        def quit(self):
                self.altF4()

