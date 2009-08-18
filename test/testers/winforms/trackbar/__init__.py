# vim: set tabstop=4 shiftwidth=4 expandtab
##############################################################################
# Written by:  Ray Wang <rawang@novell.com>
# Date:        02/05/2009
# Description: Application wrapper for trackbar.py
#              be called by ../trackbar_basic_ops.py
##############################################################################

"""Application wrapper for trackbar.py"""

from strongwind import *
from os.path import exists
from sys import path

def launchTrackBar(exe=None):
    """ 
    Launch trackbar with accessibility enabled and return a trackbar object.
    Log an error and return None if something goes wrong
    """

    if exe is None:
        # make sure we can find the sample application
        harness_dir = path[0]
        i = harness_dir.rfind("/")
        j = harness_dir[:i].rfind("/")
        uiaqa_path = harness_dir[:j]
        exe = '%s/samples/winforms/trackbar.py' % uiaqa_path
        if not exists(exe):
          raise IOError, "Could not find file %s" % exe

    args = [exe]

    (app, subproc) = cache.launchApplication(args=args, name='ipy', wait=config.LONG_DELAY)

    trackbar = TrackBar(app, subproc)

    cache.addApplication(trackbar)

    trackbar.trackBarFrame.app = trackbar

    return trackbar

class TrackBar(accessibles.Application):
    """class to represent the application"""

    def __init__(self, accessible, subproc=None): 
        """Get a reference to the trackbar window"""

        super(TrackBar, self).__init__(accessible, subproc)
        self.findFrame(re.compile('^TrackBar'), logName='Track Bar')
