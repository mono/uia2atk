
##############################################################################
# Written by:  Calen Chen  <cachen@novell.com>
# Date:        10/20/2009
#              Application wrapper for Moonlight scrollviewer
#              Used by the scrollviewer-*.py tests
##############################################################################

'Application wrapper for Moonlight ScrollViewer'

from strongwind import *

from os.path import exists, dirname
from sys import path

init_dir = path[0]
uiaqa_path = dirname(dirname(init_dir))
# Variable the path of Firefox to run the application, Please install
# Firefox3.5.1 first which is accessible by accerciser
firefox_path = '/usr/bin/firefox'

def launchScrollViewer(exe=None):
    '''Launch Moonlight ScrollViewer with accessibility enabled and return a
     ScrollViewer object.  Log an error and return None if something goes wrong'''

    if exe is None:
        # make sure we can find the sample applications
        exe = '%s/samples/moonlight/ScrollViewer/ScrollViewerSample.html' % uiaqa_path
        if not exists(exe):
          raise IOError, "Could not find file %s" % exe

    args = [firefox_path, exe]

    (app, subproc) = cache.launchApplication(args=args, name="Minefield", \
                                                        wait=config.LONG_DELAY)

    scrollviewer = ScrollViewer(app, subproc)

    cache.addApplication(scrollbar)

    scrollviewer.scrollViewerFrame.app = scrollviewer

    return scrollviewer

# class to represent the application
class ScrollViewer(accessibles.Application):
    #checkShowing=False
    def __init__(self, accessible, subproc=None):
        'Get a reference to the ScrollViewer window'
        super(ScrollViewer, self).__init__(accessible, subproc)

        self.findFrame(re.compile('^ScrollViewerSample'), logName='Scroll Viewer')
