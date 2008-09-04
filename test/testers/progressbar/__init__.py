
##############################################################################
# Written by:  Cachen Chen <cachen@novell.com>
# Date:        09/04/2008
# Description: Application wrapper for progressbar.py
#              Used by the progressbar-*.py tests
##############################################################################$

'Application wrapper for progressbar'

from strongwind import *

from os.path import exists
from sys import path

def launchProgressBar(exe=None):
    'Launch progressbar with accessibility enabled and return a progressbar object.  Log an error and return None if something goes wrong'

    if exe is None:
        # make sure we can find the sample application
        harness_dir = path[0]
        i = harness_dir.rfind("/")
        uiaqa_path = harness_dir[:i]
        exe = '%s/samples/progressbar.py' % uiaqa_path
        if not exists(exe):
          raise IOError, "Could not find file %s" % exe
  
    args = [exe]

    (app, subproc) = cache.launchApplication(args=args, name='ipy', wait=config.LONG_DELAY)

    progressbar = ProgressBar(app, subproc)

    cache.addApplication(progressbar)

    progressbar.progressBarFrame.app = progressbar

    return progressbar

# class to represent the application
class ProgressBar(accessibles.Application):
    #checkShowing=False
    def __init__(self, accessible, subproc=None): 
        'Get a reference to the progressbar window'
        super(ProgressBar, self).__init__(accessible, subproc)
        
        self.findFrame(re.compile('^ProgressBar control'), logName='Progress Bar')

