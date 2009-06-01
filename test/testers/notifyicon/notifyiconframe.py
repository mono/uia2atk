
##############################################################################
# Written by:  Cachen Chen <cachen@novell.com>
# Date:        09/16/2008
# Description: notifyicon.py wrapper script
#              Used by the notifyicon-*.py tests
##############################################################################$

import sys
import os
import actions
import states

from strongwind import *
from notifyicon import *


# class to represent the main window.
class NotifyIconFrame(accessibles.Frame):

    # constants
    # the available widgets on the window
    BUTTON_ONE = "notifyicon"
    BUTTON_TWO = "balloon"

    def __init__(self, accessible):
        super(NotifyIconFrame, self).__init__(accessible)
        self.notifyicon_button = self.findPushButton(self.BUTTON_ONE)
        self.balloon_button = self.findPushButton(self.BUTTON_TWO)
        # find the gnome-panel application and the "Panel Notification Area",
        # which has the actual NotifyIcon icon as a child 
        gnome_panel = \
              cache._desktop.findApplication('gnome-panel', checkShowing=False)
        # Promote the gnome_panel object to an Application accessible and add
        # the new object to the cache.  This needs to be done so that
        # Strongwind can log actions on its children without crashing.
        self.gnome_panel = Application(gnome_panel)
        cache.addApplication(self.gnome_panel)
        # find the Panel Notification Area, which has the actual panel
        # notification icons as descendants (but not direct descendants)
        self.pna = self.gnome_panel.findEmbedded('Panel Notification Area')
        # Find the original notification area accessibles.  This list can
        # then be compared a list returned from a later call to
        # findAllNotificationAreaAccessibles to find the new notification area
        # accessible (using set.difference for example).
        self.original_notification_area_accessibles = \
                                      self.findAllNotificationAreaAccessibles()

    def findNotifyIcon(self):
        '''
        This method calls findAllNotificationAreaAccessibles and compares the
        list that is returned to the original_notification_area_accessibles
        list to find a single notification area icon that has been placed in
        the panel since the original_notification_area_accessibles list was
        created in __init__.  In short, the intention of this method is to find
        a new notification icon that has been placed in the panel since the
        test began.
        '''
        procedurelogger.action('Search for the NotifyIcon icon')
        procedurelogger.expectedResult('The NotifyIcon icon is found')
        # find the current notification area accessibles
        new_notification_area_accessibles = \
                                      self.findAllNotificationAreaAccessibles()
        new_set = \
            set([acc._accessible for acc in new_notification_area_accessibles])
        original_set = set([acc._accessible for acc in \
                                  self.original_notification_area_accessibles])
        # Get the difference between the accessibles in the set of original
        # notification accessibles and the new notification area accessibles.
        # The difference should be the new notification icon that was placed
        # on the panel during this test.
        difference = list(new_set.difference(original_set))
        NUMNEW = 1
        # Make sure we only see one new notification area accessible.  This
        # assert could fail it some other icon happens to pop up during the
        # test, but we can't really tell exactly which notification area
        # accessible is which, so this is the best we can do (we think).
        assert len(difference) == NUMNEW, \
                "Expected %s new notification area accessibles, found %s" %\
                (NUMNEW, len(difference))
        # We want the Strongwind accessible object, not just the raw
        # accessible, so here we figure out which strongwind accsesible
        # matches the actual raw accessible
        for accessible in new_notification_area_accessibles:
            if accessible._accessible is difference[0]:
                self.notify_icon = accessible
                return
        # If we can't find the Strongwind accessible object that matches then
        # something went wrong.  This shouldn't happen.
        assert False, "Oops!  I lost the new accessible"

    def findAllNotificationAreaAccessibles(self):
        '''
        This method finds all of the panel children of the Panel Notification
        Area.  If the NotifyIcon icon is being displayed, one of these panels
        should be the accessible for that icon.  Therefore, the NotifyIcon
        icon accessible can be found by comparing a list of the accessibles
        returned by this method before and after the NotifyIcon icon is
        displayed
        '''
        filler = self.pna.findFiller(None)
        filler_panels = filler.findAllPanels(None)
        return filler_panels

    def findNotifyIconContextMenu(self):
        '''This method finds the context menu accessible of the NotifyIcon'''
        # this method needs to be implemented when BUG507281 is fixed.
        pass


    def findBalloonAccessibles(self):
        """
        Find all widgets from balloon alert
        """
        self.balloon_alert = self.app.findAlert("Hello")
        self.label = self.balloon_alert.findLabel("I'm NotifyIcon")
        self.icon = self.balloon_alert.findIcon(None)
 
    # close application main window after running test
    def quit(self):
        self.altF4()
