
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

from strongwind import *
from linklabel import *


# class to represent the main window.
class LinkLabelFrame(accessibles.Frame):

    # constants
    # the available widgets on the window
    LINK1 = re.compile('^openSUSE:www.opensuse.org')
    LINK2 = "calculator:/usr/bin/gcalctool"
    LINK3 = "gmail:gmail.novell.com"

    def __init__(self, accessible):
        super(LinkLabelFrame, self).__init__(accessible)
        self.link1 = self.findLabel(self.LINK1)
        self.link2 = self.findLabel(self.LINK2)
        self.link3 = self.findLabel(self.LINK3)

    #assert if Hypertext is implemented by using getNLinks()
    def showLink(self, accessible, url, linknum=1):
        procedurelogger.action("search for %s to calculate link number" % accessible)
        iaction = accessible._accessible.queryHypertext()
     
        procedurelogger.expectedResult('got %s link in label %s' % (linknum, accessible))
        assert iaction.getNLinks() == linknum, "missing %s" % url

    #give 'jump' action for linklabel, and implement Hypertext and Hyperlink
    def openLink(self, accessible, hyperlink=None, index=0):
        
        iaction = accessible._accessible.queryHypertext()
        hyperlink = iaction.getLink(index)

        linkurl = hyperlink.getURI(0)

        linklabel = hyperlink.getObject(0).queryAction()

        procedurelogger.action('do "jump" action for %s' % linkurl)
        linklabel.doAction(0)

    #assert if can invoke the link
    def assertLinkable(self, url):

        #def resultMatches():
        if url == "Firefox":
            assert launchNewApp(url)

        elif url == "gcalctool":
            assert launchNewApp(url)

        elif url == "gmail":
            procedurelogger.expectedResult("%s is a disable link, you can't invoke it" % url)

            application = pyatspi.findDescendant(cache._desktop, lambda x: x.name == "Firefox", True)
            assert not application

    #close application main window after running test
    def quit(self):
        self.altF4()


class NewAppFrame(accessibles.Frame):

    def __init__(self, accessible):
        super(NewAppFrame, self).__init__(accessible)

    def quit(self):
        self.altF4


