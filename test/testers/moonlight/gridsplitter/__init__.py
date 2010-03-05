
##############################################################################
# Written by:  Calen Chen  <cachen@gmail.com>
# Date:        2009/09/29
#              Application wrapper for Moonlight gridsplitter
#              Used by the gridsplitter-*.py tests
##############################################################################

'Application wrapper for Moonlight gridsplitter'

from strongwind import *

from os.path import exists, dirname
from sys import path

init_dir = path[0]
uiaqa_path = dirname(dirname(init_dir))
# Variable the path of Firefox to run the application, Please install
# Firefox3.5.1 first which is accessible by accerciser
firefox_path = '/usr/bin/firefox'

def launchGridSplitter(exe=None):
    '''Launch Moonlight slider with accessibility enabled and return a
     slider object.  Log an error and return None if something goes wrong'''

    if exe is None:
        # make sure we can find the sample applications
        ## If BUG555165 fixed please change TestPage.html to
        ## GridSplitterSample.html
        exe = '%s/samples/moonlight/GridSplitter/GridSplitterSample.html' % uiaqa_path
        if not exists(exe):
          raise IOError, "Could not find file %s" % exe

    args = [firefox_path, exe]

    (app, subproc) = cache.launchApplication(args=args, name="Minefield", \
                                                        wait=config.LONG_DELAY)

    gridsplitter = GridSplitter(app, subproc)

    cache.addApplication(gridsplitter)

    gridsplitter.gridSplitterFrame.app = gridsplitter

    return gridsplitter

# class to represent the application
class GridSplitter(accessibles.Application):
    #checkShowing=False
    def __init__(self, accessible, subproc=None):
        'Get a reference to the GridSplitter window'
        super(GridSplitter, self).__init__(accessible, subproc)

        self.findFrame(re.compile('^GridSplitterSample'), logName='Grid Splitter')
