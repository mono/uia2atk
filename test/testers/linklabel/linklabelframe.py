
##############################################################################
# Written by:  Cachen Chen <cachen@novell.com>
# Date:        10/22/2008
# Description: linklabel.py wrapper script
#              Used by the linklabel-*.py tests
##############################################################################

import sys
import os
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

    # give 'jump' action for linklabel and make sure the appropriate
    # application is opened (and to the appropriate URL if applicable")
    def openLink(self, accessible, app_name, linknum, uri, is_url):
        # get the accessible with the jump action
        ihypertext = accessible._accessible.queryHypertext()
        link = ihypertext.getLink(linknum)
        obj = link.getObject(0)
        iaction = obj.queryAction()
        
        # make sure the action interface only has one action and that it is
        # jump
        procedurelogger.action("%s %s" %\
                               ("Check the number of actions associated with",
                                accessible))
        procedurelogger.expectedResult("%s has one associated action" % \
								    accessible)
        assert iaction.nActions == 1, \
                               "Only one action should exist for the LinkLabel"
        actionName = iaction.getName(0)
        procedurelogger.action("Check the name of the action associated with" \
                                                                  % accessible)
        procedurelogger.expectedResult("Action name is \"jump\"")
        assert actionName == "jump", \
                                "Action name for LinkLabel should be \"jump\""

        # if the link is a url:
        #   make sure firefox opens to the link
        # if it's not a link:
        #   make sure the proper application opens
        if is_url:
            # if firefox is already open get a list of urls currently open
            firefox_app = cache._desktop.findApplication("Firefox")
            before_urls = []
            before_frames = firefox_app.findAllFrames(None)
            if firefox_app is not None:
                locations = firefox_app.findAllEntries("Location")
                for location in locations:
                    before_urls.append(location.text)

            # execute the jump action, this should open another firefox
            # application to the URL provided
            iaction.doAction(0)
            # give firefox plenty of time to start
            sleep(10)

            after_frames = firefox_app.findAllFrames(None)

            # only one new frame should have been opened
            assert len(after_frames) == len(before_frames) + 1, \
                                       "Only one frame should have been opened"

            # find the urls after jumping
            after_urls = []
            if firefox_app is not None:
                locations = firefox_app.findAllEntries("Location")
                for location in locations:
                    after_urls.append(location.text)

            # remove all the previous urls from the list of after_urls
            for url in before_urls:
                after_urls.remove(url)
            # what's left should be the new url
            assert len(after_urls) == 1, "Only one url should have been opened"
            new_url = after_urls[0]

            # now make sure Firefox opened to the correct link
            procedurelogger.action("%s" % \
                               "Ensure the correct link was opened in Firefox")
            procedurelogger.expectedResult("%s was opened" % uri)
            assert uri == new_url, "%s should have been opened instead of %s"\
                                                               % (uri, new_url)
            # now close firefox by finding one of the frames, finding its
            # extents, clicking on the title bar to select the firefox window
            # and then press Alt+F4 (what a pain!)

            # it looks like the the most recent frame is always found last
            opened_frame = after_frames[len(after_frames)-1]
            icomponent = opened_frame._accessible.queryComponent()
            bbox = icomponent.getExtents(pyatspi.DESKTOP_COORDS)
            x = bbox.x + (bbox.width / 2)
            y = bbox.y
            pyatspi.Registry.generateMouseEvent(x, y - 1, 'b1c')
            sys.exit(33)
            self.altF4(assertClosed=False)
            sleep(config.SHORT_DELAY)
        else:
            before_calc_apps = cache._desktop.findAllApplications("gcalctool")
            print before_calc_apps




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


class NewAppFrame(accessibles.Frame):

    def __init__(self, accessible):
        super(NewAppFrame, self).__init__(accessible)

    def quit(self):
        self.altF4


