
##############################################################################
# Written by:  Cachen Chen <cachen@novell.com>
# Date:        10/12/2009
#              Application wrapper for Moonlight hyperlinkbutton
#              Used by the hyperlinkbutton-*.py tests
##############################################################################

'Application wrapper for Moonlight HyperlinkButton'

from strongwind import *

from os.path import exists, dirname
from sys import path

init_dir = path[0]
uiaqa_path = dirname(dirname(init_dir))
# Variable the path of Firefox to run the application, Please install
# Firefox3.5.1 first which is accessible by accerciser
firefox_path = '/usr/bin/firefox'

def launchHyperlinkButton(exe=None):
    '''Launch Moonlight HyperlinkButton with accessibility enabled and return a
     HyperlinkButton object.  Log an error and return None if something goes wrong'''

    if exe is None:
        # make sure we can find the sample applications
        exe = '%s/samples/moonlight/HyperlinkButton/TestPage.html' % uiaqa_path
        if not exists(exe):
          raise IOError, "Could not find file %s" % exe

    args = [firefox_path, exe]

    (app, subproc) = cache.launchApplication(args=args, name="Firefox", \
                                                        wait=config.LONG_DELAY)

    hyperlinkbutton = HyperlinkButton(app, subproc)

    cache.addApplication(hyperlinkbutton)

    hyperlinkbutton.hyperlinkButtonFrame.app = hyperlinkbutton

    return hyperlinkbutton

# class to represent the application
class HyperlinkButton(accessibles.Application):
    #checkShowing=False
    def __init__(self, accessible, subproc=None):
        'Get a reference to the HyperlinkButton window'
        super(HyperlinkButton, self).__init__(accessible, subproc)

        self.findFrame(re.compile('^HyperlinkButtonSample'), logName='Hyperlink Button')

