
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
    LINK3 = "gedit:/usr/bin/gedit"

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

    #give 'jump' action for linklabel
    def openLink(self,linklabel):
        linklabel.jump()

    #close new application without log
    def close(self):
        self.keyCombo('<alt>F4', log=False)

    #assert if can invoke the link
    def assertLinkable(self, url):

        def resultMatches():
            if url == "Firefox":
                return launchNewApp(url)

            elif url == "gcalctool":
                return launchNewApp(url)

            elif url == "gedit111":
                return not pyatspi.findDescendant(cache._desktop, lambda x: x.name == url, True)

        assert retryUntilTrue(resultMatches)

    #close application main window after running test
    def quit(self):
        self.altF4()


class NewAppFrame(accessibles.Frame):

    def __init__(self, accessible):
        super(NewAppFrame, self).__init__(accessible)

    def quit(self):
        self.altF4


