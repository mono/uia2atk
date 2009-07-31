############################################################################## 
# Written by:  Cachen Chen <cachen@novell.com> 
#              Felicia Mu <fxmu@novell.com> 
# Date:        07/16/2009
# Description: printpreviewdialog.py wrapper script
#              Used by the printpreviewdialog-*.py tests
##############################################################################$

import sys
import os
import actions
import states

from helpers import *
from strongwind import *
from printpreviewdialog import *


# class to represent the main window.
class PrintPreviewDialogFrame(accessibles.Frame):

    # constants
    # the available widgets on the window
    BUTTON = "PrintPreviewDialog"
    
    NUM_DIALOG_PANELS = 7
    NUM_DIALOG_PUSH_BUTTONS = 8
    NUM_DIALOG_SEPARATORS = 2
    NUM_ZOOM_MENU_ITEMS = 9

    def __init__(self, accessible):
        super(PrintPreviewDialogFrame, self).__init__(accessible)

        # find the launched button
        self.start_button =  self.findPushButton(self.BUTTON)

    def findLaunchedDialogAccessibles(self):
        """
        Find all the accessibles of the PrintPreviewDialog
        """

        # find the actual PrintPreviewDialog dialog
        self.dialog = self.app.findDialog("PrintPreviewDialog")

        # find the panel that has the actual print preview
        self.print_preview_panel = self.dialog.findPanel("")

        # find the toolbar
        self.toolbar = self.dialog.findToolBar(None)

        # find push buttons under the toolbar
        self.pushbuttons = self.toolbar.findAllPushButtons(None)
        self.close_button = self.toolbar.findPushButton("Close")

        # find all the panels under toolbar
        self.panels = self.toolbar.findAllPanels(None)

        # find toggle button under the toolbar
        self.zoom_toggle = self.toolbar.findToggleButton(None)

        # find spin button under the toolbar
        self.spinbutton = self.toolbar.findSpinButton(None)

        # find separators under the toolbar
        self.separators = self.toolbar.findAllSeparators(None)

        # find filler under the toolbar
        self.filler  = self.toolbar.findFiller(None)

        # find label under the toolbar
        self.label  = self.toolbar.findLabel(None)

        # BUG508593 - PrintPreviewDialog: ToolTips are not accessible   
        # find all the tooltips
        # for button in self.pushbuttons:
        #    if button.name != "Close":
        #       button.mouseMove()
        #       sleep(config.SHORT_DELAY) 
        #       tooltip = self.app.findToolTip(None)

    def findPringtingFrameAccessibles(self):
        """
        Find the launched priting frame
        """
        printing_frame_unfound = False
        try:
            printing_frame = pyatspi.findDescendant(self.app._accessible,
                               lambda x: x.name == 'Printing' and \
                                                    x.getRoleName() == 'frame')
        except SearchError:
            printing_frame_unfound = True
        if printing_frame_unfound == False:
            assert False, 'The "Printing" frame accessible should not exist'


    def assertNumOfAccessibles(self, accessible, num):
        """
        check the number of the accessibles more than one
        """
        procedurelogger.action("assert the num of the %s" % accessible[1])
        procedurelogger.expectedResult("expected number of %s is %s" % (accessible[1], num))
        
        assert len(accessible) == num, \
                            "Found %s ' quatity %s , expected %s" % \
                           (accessible, len(accessible), num)

    def findZoomDropDownAccessibles(self):
        """
        find all the accessibles on the menu
        """
        procedurelogger.action("find all the accessibles on the drop down menu")
        self.zoom_window = self.app.findWindow(None)
        self.zoom_menu = self.zoom_window.findMenu(None)
        self.zoom_menu_items = self.zoom_menu.findAllMenuItems(None)

    def assignNumericUpdownValue(self, value):
        '''
        Use the Value interface to set the current value of the NumericUpDown
        accessible.
        '''
        procedurelogger.action('set %s value to page numericupdown' % (value))
        self.spinbutton.value = value

    def assertNumericUpdownValue(self, expected_value):
        '''
        Ensure that the current value of the NumericUpDown accessible matches
        the expected_value.
        '''
        actual_value = self.spinbutton.value
        procedurelogger.action("Ensure that page spin's value is what we expect")
        procedurelogger.expectedResult('%s value is %d' % \
                                        (self.spinbutton, expected_value))
        assert actual_value == expected_value, \
                                "%s's value is %d, expected %d" % \
                                 (self.spinbutton, actual_value, expected_value)

    def quit(self):
        """
        close application main window after running test
        """
        self.altF4()
