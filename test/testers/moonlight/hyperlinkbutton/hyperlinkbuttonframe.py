
##############################################################################
# Written by:  Cachen Chen <cachen@novell.com>
# Date:        10/12/2009
# Description: hyperlinkbutton.py wrapper script
#              Used by the hyperlinkbutton-*.py tests
##############################################################################

# imports
from strongwind import *
from hyperlinkbutton import *


# class to represent the main window.
class HyperlinkButtonFrame(accessibles.Frame):

    # constants
    # the available widgets on the window
    LINK_ONE = "Click here\r\nto open OpenSUSE in New Window"
    LINK_TWO = "Click here to open OpenSUSE in Parent Window"
    LINK_URL = "http://www.opensuse.org/en/"

    def __init__(self, accessible):
        super(HyperlinkButtonFrame, self).__init__(accessible)
        self.frame = self.findDocumentFrame("HyperlinkButtonSample")
        self.filler = self.frame.findFiller("Silverlight Control")
        # 2 hyperlink button
        self.hyperlink1 = self.filler.findPushButton(self.LINK_ONE)
        self.hyperlink2 = self.filler.findPushButton(self.LINK_TWO)

    # assert that the accessible contains the expected number of links
    def assertNLinks(self, accessible, expected_linknum):
        """
        Make sure the accessible's link number is expected
        """
        procedurelogger.action("Check the number of links in %s" % accessible)
        procedurelogger.expectedResult("%s has %d link(s)" % \
                                                (accessible, expected_linknum))
        actual_linknum = accessible._accessible.queryHypertext().getNLinks()
        assert actual_linknum == expected_linknum,\
                            "%s should have %d link(s) instead of %d" \
                               % (accessible, expected_linknum, actual_linknum)

    # URI: http://en.wikipedia.org/wiki/Uniform_Resource_Identifier
    def assertURI(self, accessible, linknum=1, expected_uri=None):
        """
        Make sure each link points to the expected uri
        """
        expected_uri = self.LINK_URL

        procedurelogger.action("Verify link %d of %s" % (linknum, accessible))
        procedurelogger.expectedResult("URI is %s" % expected_uri)
        actual_uri = accessible._accessible.queryHypertext().getLink(linknum).getURI(0)
        assert expected_uri == actual_uri,\
                "link %d of %s should have uri \"%s\" instead of \"%s\"" %\
                                (linknum, accessible, expected_uri, actual_uri)

    def openURL(self, accessible, linknum=1):
        # get the accessible with the jump action
        iaction = None

        iaction = accessible._accessible.queryAction()

        # make sure the action interface only has one action and that it is jump
        procedurelogger.action("%s %s" %\
                                ("Check the number of actions associated with",
                                                                   accessible))
        procedurelogger.expectedResult("%s has one associated action" % \
                                                                    accessible)
        assert iaction.nActions == 1,\
                               "Only one action should exist for the LinkLabel"
        actionName = iaction.getName(0)

        # now we can just call iaction.doAction(0) to perform the jump action
        procedurelogger.action('Do "jump" action for %s' % accessible)
        iaction.doAction(0)
        sleep(20)
        # invoke hyperlink1 will open the URL in a new window, so there are
        # 2 document frames with different name
        if accessible is self.hyperlink1:
            procedurelogger.expectedResult('"%s" is invoked in new window' % \
                                                                self.LINK_URL)
            # new page is blocked by the firefox
            firefox_alert = self.findAlert(None)
            firefox_preferences = firefox_alert.findPushButton("Preferences")
            firefox_preferences.press()
            sleep(config.SHORT_DELAY)
            url_link = self.findMenuItem("Show 'http://www.opensuse.org/en/'")
            url_link.click()
            sleep(config.SHORT_DELAY)

            documentframes = self.findAllDocumentFrames(None, checkShowing=False)
            assert len(documentframes) == 2, \
                            "2 documentframe are expected, the actual is %s" %\
                                          len(documentframes)

            procedurelogger.action("Check the name of the %s" % \
                                                             documentframes[0])
            procedurelogger.expectedResult("the name should be %s" % \
                                                      ("HyperlinkButtonSample"))
            assert documentframes[0].name == "HyperlinkButtonSample", \
                         "actual name is %s, expected name is %s" % \
                          (documentframes[0].name, "HyperlinkButtonSample")

            procedurelogger.action("Check the name of the %s" % \
                                                             documentframes[1])
            procedurelogger.expectedResult("the name should be %s" % \
                                                        ("openSUSE.org"))
            assert documentframes[1].name == "openSUSE.org", \
                         "actual name is %s, expected name is %s" % \
                          (documentframes[1].name, "openSUSE.org")

        # invoke hyperlink2 will open the URL in the parent window, because
        # the URL opened with hyperlink1 is not closed, so there are 2
        # document frames with the same name
        if accessible is self.hyperlink2:
            procedurelogger.expectedResult('"%s" is invoked in parent window' % \
                                                                self.LINK_URL)
            documentframes = self.findAllDocumentFrames(None, checkShowing=False)
            assert len(documentframes) == 2, \
                            "2 documentframe are expected, the actual is %s" %\
                                          len(documentframes)

            procedurelogger.action("Check the name of the %s" % \
                                                             documentframes[0])
            procedurelogger.expectedResult("the name should be %s" % \
                                                      ("openSUSE.org"))
            assert documentframes[0].name == "openSUSE.org", \
                         "actual name is %s, expected name is %s" % \
                          (documentframes[0].name, "openSUSE.org")

            procedurelogger.action("Check the name of the %s" % \
                                                             documentframes[1])
            procedurelogger.expectedResult("the name should be %s" % \
                                                        ("openSUSE.org"))
            assert documentframes[1].name == "openSUSE.org", \
                         "actual name is %s, expected name is %s" % \
                          (documentframes[1].name, "openSUSE.org")
