# vim: set tabstop=4 shiftwidth=4 expandtab
##############################################################################
# Written by:  Ray Wang <rawang@novell.com>
# Date:        02/17/2008
# Description: Application wrapper for mainmenu.py
#              be called by ../mainmenu_basic_ops.py
##############################################################################

"""Application wrapper for mainmenu.py"""

from strongwind import *
from os.path import exists
from sys import path

def launchMainMenu(exe=None):
    """ 
    Launch mainmenu with accessibility enabled and return a mainmenu object.
    Log an error and return None if something goes wrong
    """

    if exe is None:
        # make sure we can find the sample application
        harness_dir = path[0]
        i = harness_dir.rfind("/")
        j = harness_dir[:i].rfind("/")
        uiaqa_path = harness_dir[:j]
        exe = '%s/samples/winforms/mainmenu.py' % uiaqa_path
        if not exists(exe):
          raise IOError, "Could not find file %s" % exe

    args = [exe]

    (app, subproc) = cache.launchApplication(args=args, name='ipy', wait=config.LONG_DELAY)

    mainmenu = MainMenu(app, subproc)

    cache.addApplication(mainmenu)

    mainmenu.mainMenuFrame.app = mainmenu

    return mainmenu

class MainMenu(accessibles.Application):
    """class to represent the application"""

    def __init__(self, accessible, subproc=None): 
        """Get a reference to the menustirp window"""
        super(MainMenu, self).__init__(accessible, subproc)
        self.findFrame(re.compile('^MainMenu'), logName='Main Menu')
