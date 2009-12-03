
##############################################################################
# Written by:  Neville Gao  <nevillegao@gmail.com>
# Date:        2009/09/22
#              Application wrapper for Moonlight scrollbar
#              Used by the scrollbar-*.py tests
##############################################################################

'Application wrapper for Moonlight ScrollBar'

from strongwind import *

from os.path import exists, dirname
from sys import path

init_dir = path[0]
uiaqa_path = dirname(dirname(init_dir))
# Variable the path of Firefox to run the application, Please install
# Firefox3.5.1 first which is accessible by accerciser
firefox_path = '/usr/local/bin/firefox'

def launchScrollBar(exe=None):
    '''Launch Moonlight ScrollBar with accessibility enabled and return a
     ScrollBar object.  Log an error and return None if something goes wrong'''

    if exe is None:
        # make sure we can find the sample applications
        exe = '%s/samples/moonlight/ScrollBar/ScrollBarSample.html' % uiaqa_path
        if not exists(exe):
          raise IOError, "Could not find file %s" % exe

    args = [firefox_path, exe]

    (app, subproc) = cache.launchApplication(args=args, name="Minefield", \
                                                        wait=config.LONG_DELAY)

    scrollbar = ScrollBar(app, subproc)

    cache.addApplication(scrollbar)

    scrollbar.scrollBarFrame.app = scrollbar

    return scrollbar

# class to represent the application
class ScrollBar(accessibles.Application):
    #checkShowing=False
    def __init__(self, accessible, subproc=None):
        'Get a reference to the ScrollBar window'
        super(ScrollBar, self).__init__(accessible, subproc)

        self.findFrame(re.compile('^ScrollBarSample'), logName='Scroll Bar')
