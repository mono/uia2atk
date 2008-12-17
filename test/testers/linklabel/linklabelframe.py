
##############################################################################
# Written by:  Cachen Chen <cachen@novell.com>
# Date:        10/22/2008
# Description: linklabel.py wrapper script
#              Used by the linklabel-*.py tests
##############################################################################

import sys
import os
import re
import actions
import states
import pyatspi

from strongwind import *
from linklabel import *


# class to represent the main window.
class LinkLabelFrame(accessibles.Frame):

    # constants
    # the available widgets on the window
    LINK1 = re.compile('^openSUSE')
    LINK2 = "calculator:/usr/bin/gcalctool"
    LINK3 = "gmail:www.gmail.com"

    desktop = pyatspi.Registry.getDesktop(0)

    def __init__(self, accessible):
        super(LinkLabelFrame, self).__init__(accessible)
        self.link1 = self.findLabel(self.LINK1)
        self.link2 = self.findLabel(self.LINK2)
        self.link3 = self.findLabel(self.LINK3)

    #assert that the accessible contains the expected number of links
    def assertNLinks(self, accessible, expected_linknum):
        procedurelogger.action("Check the number of links in %s" % accessible)
        procedurelogger.expectedResult("%s has %d link(s)" % \
                                                (accessible, expected_linknum))

        ihypertext = accessible._accessible.queryHypertext()
        actual_linknum = ihypertext.getNLinks()
        assert actual_linknum == expected_linknum,\
                            "%s should have %d link(s) instead of %d" \
                               % (accessible, expected_linknum, actual_linknum) 

    def _getOpenURL(self, firefox_app):
        # return a list of all the URLs current open in firefox
        open_urls = []
        if firefox_app is not None:
            locations = firefox_app.findAllEntries("Location")
            for location in locations:
                open_urls.append(location.text)
        assert len(open_urls) == 1, "There should only be one URL open"
        return open_urls[0] 

    # Give 'jump' action for linklabel and make sure the appropriate
    # application is opened (and to the appropriate URL if applicable").
    # For mouse_click to be True, the center of the accessible must be a link
    # or the mouse click will be issued on a non-link and invalidate the test
    def openLink(self, accessible, app_name, linknum, uri, is_url, mouse_click=False):
        # get the action interface if we're not using a mouseClick
        iaction = None
        if not mouse_click:
            # get the accessible with the jump action
            ihypertext = accessible._accessible.queryHypertext()
            num_links = ihypertext.getNLinks()
            assert linknum < num_links, \
                        'Invalid linknum (%d), %s only have %d links' % \
                                                    (linknum, accessible, numlinks)
            link = ihypertext.getLink(linknum)
            obj = link.getObject(0)
            is_enabled = pyatspi.STATE_ENABLED in obj.getState().getStates()
            iaction = obj.queryAction()

            # make sure the action interface only has one action and that it is
            # jump
            procedurelogger.action("%s %s" %\
                                ("Check the number of actions associated with",
                                                                   accessible))
            procedurelogger.expectedResult("%s has one associated action" % \
                                                                    accessible)
            assert iaction.nActions == 1,\
                               "Only one action should exist for the LinkLabel"
            actionName = iaction.getName(0)
            procedurelogger.action("%s %s" % \
                  ("Check the name of the action associated with", accessible))
            procedurelogger.expectedResult("Action name is \"jump\"")
            assert actionName == "jump", \
                                 "Action name for LinkLabel should be \"jump\""

        # now we can just call iaction.doAction(0) to perform the jump action

        # if the link is a url:
        #   make sure firefox opens to the link
        # if it's not a link:
        #   make sure the proper application opens
        if is_url and is_enabled:
            # TODO: enhance this test so that it will handle the case
            # of a firefox app already being open

            firefox_app = pyatspi.findDescendant(self.desktop, \
                          lambda x: x.getRoleName() == 'application' and \
                          x.name == 'Firefox') 

            assert firefox_app is None, "Firefox should not be open"

            # execute the jump action, this should open firefox
            # application to the URL provided
            if mouse_click:
                accessible.mouseClick()
            else:
                iaction.doAction(0)
            # give firefox plenty of time to start
            sleep(20)

            firefox_app = cache._desktop.findApplication("Firefox")

            assert firefox_app is not None, "Firefox should have opened"

            frames = firefox_app.findAllFrames(None)

            # only one new frame should have been opened
            assert len(frames) == 1, "Only one frame should have been opened"
   
            frame = frames[0]

            # find the urls after jumping
            url = self._getOpenURL(firefox_app)

            # now make sure Firefox opened to the correct link
            procedurelogger.action("%s" % \
                               "Ensure the correct link was opened in Firefox")
            procedurelogger.expectedResult("%s was opened" % uri)
            assert uri == url, \
                    '"%s" should have been opened instead of "%s"' % (uri, url)
            # now close firefox by finding one of the frames, finding its
            # extents, clicking on the title bar to select the firefox window
            # and then press Alt+F4 (what a pain!)

            # it looks like the the most recent frame is always found last
            icomponent = frame._accessible.queryComponent()
            bbox = icomponent.getExtents(pyatspi.DESKTOP_COORDS)
            x = bbox.x + (bbox.width / 2)
            y = bbox.y
            pyatspi.Registry.generateMouseEvent(x, y - 1, 'b1c')
            self.altF4(assertClosed=False)
            procedurelogger.expectedResult("Firefox should be closed")
            sleep(config.SHORT_DELAY)
        elif is_url and not is_enabled:
            # if the URI is a URL but the link should be disabled we should
            # make sure we can still try to jump, but nothing should happen

            # execute the jump action, this should open another firefox
            # application to the URL provided

            firefox_app = pyatspi.findDescendant(self.desktop, \
                          lambda x: x.getRoleName() == 'application' and \
                          x.name == 'Firefox')

            assert firefox_app is None, "Firefox should not be open" 

            if mouse_click:
                accessible.mouseClick()
            else:
                iaction.doAction(0)

            # give firefox plenty of time to start (in case it does try to 
            # start)
            sleep(10)

            procedurelogger.action("%s" % \
                               "Ensure that Firefox was not opened")
            firefox_app = pyatspi.findDescendant(self.desktop, \
                        lambda x: x.getRoleName() == 'application' and \
                        x.name == 'Firefox')           
 
            procedurelogger.expectedResult("Firefox was not opened")
            assert firefox_app is None, \
                            "no \"Firefox\" application should have been found"
        elif not is_url:
            # TODO: enhance this test so that it will handle the case
            # of a gcalctool app already being open

            procedurelogger.action("Check if gcalctool is open")
            procedurelogger.expectedResult("gcalctool should not be open")
            calc_app = pyatspi.findDescendant(self.desktop, \
                       lambda x: x.getRoleName() == 'application' and \
                       x.name == 'gcalctool')
            assert calc_app is None, "No gcalctool should be open"

            # jump, this should open a gcalctool app
            if mouse_click:
                accessible.mouseClick()
            else:
                iaction.doAction(0)

            # give gcalctool plenty of time to open
            sleep(20)

            procedurelogger.action("Check if gcalctool is open")
            procedurelogger.expectedResult("gcalctool should be open")
            calc_app = pyatspi.findDescendant(self.desktop, \
                       lambda x: x.getRoleName() == 'application' and \
                       x.name == 'gcalctool')

            assert calc_app is not None, "One gcalctool should be open"
            
            # close the opened gcalctool (it becomes the active window, unlike
            # firefox, so this is a lot easier)
            self.altF4(assertClosed=False)
            procedurelogger.expectedResult("gcalctool should be closed")
            sleep(config.MEDIUM_DELAY)
        else:
            raise NotImplementedError, \
                                "Applcation wrapper does not support this call"

    # make sure each link points to the expected uri
    # URI: http://en.wikipedia.org/wiki/Uniform_Resource_Identifier
    def assertURI(self, accessible, linknum, expected_uri):
        procedurelogger.action("Verify link %d of %s" % (linknum, accessible))
        procedurelogger.expectedResult("URI is %s" % expected_uri)
        ihypertext = accessible._accessible.queryHypertext()
        actual_uri = ihypertext.getLink(linknum).getURI(0)
        assert expected_uri == actual_uri,\
                "link %d of %s should have uri \"%s\" instead of \"%s\"" %\
                                (linknum, accessible, expected_uri, actual_uri)

    def quit(self):
        self.altF4()
