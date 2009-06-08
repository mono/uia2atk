
##############################################################################
# Written by:  Cachen Chen <cachen@novell.com>
# Date:        01/16/2008
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
    NUM_TOOLBAR_PANELS = 6
    NUM_TOOLBAR_SEPARATORS = 2
    NUM_TOOLBAR_FILLERS = 1 
    NUM_ZOOM_MENU_OPTIONS = 9

    def __init__(self, accessible):
        super(PrintPreviewDialogFrame, self).__init__(accessible)
        self.button = self.findPushButton(self.BUTTON)

    def findAllPrintPreviewDialogAccessibles(self):
        """
        Find all the accessibles of the PrintPreviewDialog
        """

        # BUG465958 - PrintPreviewControl: printing window is hided that is
        # inconsistant with Vista does 
        #printing_frame_unfound = False
        #try:
        #    printing_frame = pyatspi.findDescendant(self.app._accessible,
        #                       lambda x: x.name == 'Printing' and \
        #                                            x.getRoleName() == 'frame')
        #except SearchError:
        #    printing_frame_unfound = True
        #if printing_frame_unfound == False:
        #    assert False, 'The "Printing" frame accessible should not exist'

        # find the actual PrintPreviewDialog dialog
        self.dialog = self.app.findDialog("PrintPreviewDialog")

        # find the panel that has the actual print preview
        self.print_preview_panel = self.dialog.findPanel("")

        # find the toolbar, which has a bunch of children
        self.toolbar = self.dialog.findToolBar(None)

        # find the toolbar's children
        self.toolbar_panels = self.toolbar.findAllPanels(None)
        assert len(self.toolbar_panels) == self.NUM_TOOLBAR_PANELS, \
                            "Toolbar had %s panel children, expected %s" % \
                            (len(self.toolbar_panels), self.NUM_TOOLBAR_PANELS)
        self.toolbar_separators = self.toolbar.findAllSeparators(None)
        assert len(self.toolbar_separators) == self.NUM_TOOLBAR_SEPARATORS, \
                    "Toolbar had %s separator children, expected %s" % \
                    (len(self.toolbar_separators), self.NUM_TOOLBAR_SEPARATORS)
        self.toolbar_fillers = self.toolbar.findAllFillers(None)
        assert len(self.toolbar_fillers) == self.NUM_TOOLBAR_FILLERS, \
                       "Toolbar had %s filler children, expected %s" % \
                       (len(self.toolbar_fillers), self.NUM_TOOLBAR_FILLERS)
        self.toolbar_filler = self.toolbar_fillers[0]

        # assign each of the toolbar panels individually
        self.print_panel = self.toolbar_panels[0]
        self.one_page_panel = self.toolbar_panels[1]
        self.two_pages_panel = self.toolbar_panels[2]
        self.three_pages_panel = self.toolbar_panels[3]
        self.four_pages_panel = self.toolbar_panels[4]
        self.six_pages_panel = self.toolbar_panels[5]

        # assign each of the toolbar filler children
        self.zoom_push_button = self.toolbar_filler.findPushButton("")
        self.zoom_toggle_button = self.toolbar_filler.findToggleButton("")

        # assign each of the toolbar panels children
        self.print_button = self.print_panel.findPushButton("")
        self.one_page_button = self.one_page_panel.findPushButton("")
        self.two_pages_button = self.two_pages_panel.findPushButton("")
        self.three_pages_button = self.three_pages_panel.findPushButton("")
        self.four_pages_button = self.four_pages_panel.findPushButton("")
        self.six_pages_button = self.six_pages_panel.findPushButton("")

        # some controls of the PrintPreviewDialog are not accessible.  We
        # should test these controls once the following bugs are resolved:
        #BUG508567 PrintPreviewDialog: NumericUpDown control is not accessible
        #BUG508566 PrintPreviewDialog: "Page" label is not accessible
        #BUG508565 PrintPreviewDialog: "Close" button is not accessible

    def checkAllDefaultStates(self):
        '''Check all the default states of the accessibles'''

        # some controls of the PrintPreviewDialog are not accessible.  We
        # should check the states of these controls once these bugs are fixed:
        #BUG508567 PrintPreviewDialog: NumericUpDown control is not accessible
        #BUG508566 PrintPreviewDialog: "Page" label is not accessible
        #BUG508565 PrintPreviewDialog: "Close" button is not accessible
        # Additionally, I think the NumericUpDown control should be "focused"
        # by default.  See BUGXXX.

        statesCheck(self.dialog, "Dialog", add_states=["modal"])
        statesCheck(self.print_preview_panel,
                    "Panel",
                    add_states=["focusable"])
        statesCheck(self.toolbar, "ToolBar")
        for panel in self.toolbar_panels:
            statesCheck(panel, "Panel")
        for separator in self.toolbar_separators:
            statesCheck(separator, "ToolStripSeparator") 

        # check each of the toolbar Button children
        statesCheck(self.zoom_push_button, "Button")
        statesCheck(self.zoom_toggle_button, "ToggleButton")
        statesCheck(self.print_button, "Button")
        statesCheck(self.one_page_button, "Button")
        statesCheck(self.two_pages_button, "Button")
        statesCheck(self.three_pages_button, "Button")
        statesCheck(self.four_pages_button, "Button")
        statesCheck(self.six_pages_button, "Button")

    def checkAllActions(self):
        actionsCheck(self.zoom_push_button, "Button")
        actionsCheck(self.zoom_toggle_button, "ToggleButton")
        actionsCheck(self.print_button, "Button")
        actionsCheck(self.one_page_button, "Button")
        actionsCheck(self.two_pages_button, "Button")
        actionsCheck(self.three_pages_button, "Button")
        actionsCheck(self.four_pages_button, "Button")
        actionsCheck(self.six_pages_button, "Button")
        # Need to add an actionsCheck for the "Close" button once BUG508565 is
        # fixed.  Bug 508565 PrintPreviewDialog: "Close" button is not
        # accessible.

    def clickAllDefaultButtons(self):
        '''
        Clicks all the accessible buttons available by default on the
        PrintPreviewDialog, except for the "Close" button, which should be
        tested separately.
        '''
        self.zoom_push_button.click(log=True)
        sleep(config.SHORT_DELAY)
        self.zoom_toggle_button.click(log=True)
        sleep(config.SHORT_DELAY)
        self.print_button.click(log=True)
        sleep(config.SHORT_DELAY)
        self.one_page_button.click(log=True)
        sleep(config.SHORT_DELAY)
        self.two_pages_button.click(log=True)
        sleep(config.SHORT_DELAY)
        self.three_pages_button.click(log=True)
        sleep(config.SHORT_DELAY)
        self.four_pages_button.click(log=True)
        sleep(config.SHORT_DELAY)
        self.six_pages_button.click(log=True)
        sleep(config.SHORT_DELAY)
    
    def findZoomDropDownAccessibles(self):
        '''
        Find all of the accessibles that appear after the zoom toggle button
        has been clicked.
        '''
        # there is a window that contains a menu, which has all of the other
        # accessibles
        self.zoom_window = self.app.findWindow(None) 
        self.zoom_menu = self.zoom_window.findMenu(None)
        self.zoom_menu_items = self.zoom_menu.findAllMenuItems(None)
        assert len(self.zoom_menu_items) == self.NUM_ZOOM_MENU_OPTIONS, \
                           "Found %s zoom menu options, expected %s" % \
                           (len(self.zoom_menu_items), NUM_ZOOM_MENU_OPTIONS)
        assert self.zoom_menu_items[0].name == "Auto", \
                                    'Menu item name was %s, expected %s' % \
                                    (self.zoom_menu_items[0].name, "Auto")
        assert self.zoom_menu_items[1].name == "500%", \
                                    'Menu item name was %s, expected %s' % \
                                    (self.zoom_menu_items[1].name, "500%")
        assert self.zoom_menu_items[2].name == "200%", \
                                    'Menu item name was %s, expected %s' % \
                                    (self.zoom_menu_items[2].name, "200%")
        assert self.zoom_menu_items[3].name == "150%", \
                                    'Menu item name was %s, expected %s' % \
                                    (self.zoom_menu_items[3].name, "150%")
        assert self.zoom_menu_items[4].name == "100%", \
                                    'Menu item name was %s, expected %s' % \
                                    (self.zoom_menu_items[4].name, "100%")
        assert self.zoom_menu_items[5].name == "75%", \
                                    'Menu item name was %s, expected %s' % \
                                    (self.zoom_menu_items[5].name, "75%")
        assert self.zoom_menu_items[6].name == "50%", \
                                    'Menu item name was %s, expected %s' % \
                                    (self.zoom_menu_items[6].name, "50%")
        assert self.zoom_menu_items[7].name == "25%", \
                                    'Menu item name was %s, expected %s' % \
                                    (self.zoom_menu_items[7].name, "25%")
        assert self.zoom_menu_items[8].name == "10%", \
                                    'Menu item name was %s, expected %s' % \
                                    (self.zoom_menu_items[8].name, "10%")

    def checkZoomDropDownDefaultStates(self):
        '''
        Check the default states of all the accessibles from the zoom drop
        down menu
        '''
        statesCheck(self.zoom_window,
                    "Form",
                    invalid_states=["resizable"],
                    add_states=["active"])
        ''' here, need to log bugs for this failure'''
        # BUG509165 PrintPreviewDialog: zoom "menu" accessible has extraneous
        # "focused" state
        #statesCheck(self.zoom_menu, "Menu", invalid_states=["selectable"])
        self.checkZoomMenuItems()

    def checkZoomMenuItems(self, checked_menu_item=None):
        '''
        Check the states of the zoom menu items.  If no checked_menu_item is
        passed, it is assumed that the first menu item is checked.
        '''
        if checked_menu_item is None:
            checked_menu_item = self.zoom_menu_items[0]
        for menu_item in self.zoom_menu_items:
            if menu_item is checked_menu_item:
                statesCheck(menu_item, "MenuItem", add_states=["checked"])
            else:
                # Bug 509276 - PrintPreviewDialog: All zoom menu items have the
                # "checked" state
                #statesCheck(menu_item, "MenuItem")
                pass # delete me when uncommenting the statesCheck

    def checkZoomDropDownActions(self):
        '''
        Check the actions of all the accessibles from the zoom drop down menu
        '''
        for menu_item in self.zoom_menu_items:
            actionsCheck(menu_item, "MenuItem")

    def assignNumericUpdownValue(self, value): 
        '''
        Use the Value interface to set the current value of the NumericUpDown
        accessible.
        '''
        #BUG508567 PrintPreviewDialog: NumericUpDown control is not accessible
        pass

    def assertNumericUpdownValue(self, expected_value): 
        '''
        Ensure that the current value of the NumericUpDown accessible matches
        the expected_value.
        '''
        #BUG508567 PrintPreviewDialog: NumericUpDown control is not accessible
        pass

    # close application main window after running test
    def quit(self):
        self.altF4()
