
##############################################################################
# Written by:  Neville Gao  <nevillegao@gmail.com>
# Date:        2009/09/21
#              Application wrapper for Moonlight progressbar
#              Used by the progressbar-*.py tests
##############################################################################

'Application wrapper for Moonlight ProgressBar'

from strongwind import *

from os.path import exists, dirname
from sys import path

init_dir = path[0]
uiaqa_path = dirname(dirname(init_dir))
# Variable the path of Firefox to run the application, Please install
# Firefox3.5.1 first which is accessible by accerciser
firefox_path = '/usr/local/bin/firefox'

def launchProgressBar(exe=None):
    '''Launch Moonlight ProgressBar with accessibility enabled and return a
     ProgressBar object.  Log an error and return None if something goes wrong'''

    if exe is None:
        # make sure we can find the sample applications
        exe = '%s/samples/moonlight/ProgressBar/ProgressBarSample.html' % uiaqa_path
        if not exists(exe):
          raise IOError, "Could not find file %s" % exe

    args = [firefox_path, exe]

    (app, subproc) = cache.launchApplication(args=args, name="Minefield", \
                                                        wait=config.LONG_DELAY)

    progressbar = ProgressBar(app, subproc)

    cache.addApplication(progressbar)

    progressbar.progressBarFrame.app = progressbar

    return progressbar

# class to represent the application
class ProgressBar(accessibles.Application):
    #checkShowing=False
    def __init__(self, accessible, subproc=None):
        'Get a reference to the ProgressBar window'
        super(ProgressBar, self).__init__(accessible, subproc)

        self.findFrame(re.compile('^ProgressBarSample'), logName='Progress Bar')
