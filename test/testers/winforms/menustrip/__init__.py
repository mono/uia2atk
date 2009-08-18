# vim: set tabstop=4 shiftwidth=4 expandtab
##############################################################################
# Written by:  Ray Wang <rawang@novell.com>
# Date:        11/19/2008
# Description: Application wrapper for menustrip.py
#              be called by ../menustrip_basic_ops.py
##############################################################################

"""Application wrapper for menustrip.py"""

from strongwind import *
from os.path import exists
from sys import path

def launchMenuStrip(exe=None):
    """ 
    Launch menustrip with accessibility enabled and return a menustrip object.
    Log an error and return None if something goes wrong
    """

    if exe is None:
        # make sure we can find the sample application
        harness_dir = path[0]
        i = harness_dir.rfind("/")
        uiaqa_path = harness_dir[:i]
        exe = '%s/samples/winforms/menustrip.py' % uiaqa_path
        if not exists(exe):
          raise IOError, "Could not find file %s" % exe

    args = [exe]

    (app, subproc) = cache.launchApplication(args=args, name='ipy', wait=config.LONG_DELAY)

    menustrip = MenuStrip(app, subproc)

    cache.addApplication(menustrip)

    menustrip.menuStripFrame.app = menustrip

    return menustrip

class MenuStrip(accessibles.Application):
    """class to represent the application"""

    def __init__(self, accessible, subproc=None): 
        """Get a reference to the menustirp window"""

        super(MenuStrip, self).__init__(accessible, subproc)

        self.findFrame(re.compile('^MenuStrip'), logName='Menu Strip')
