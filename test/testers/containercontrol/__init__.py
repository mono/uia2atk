# vim: set tabstop=4 shiftwidth=4 expandtab
##############################################################################
# Written by:  Ray Wang <rawang@novell.com>
# Date:        02/09/2009
# Description: Application wrapper for containercontrol.py
#              be called by ../containercontrol_basic_ops.py
##############################################################################

"""Application wrapper for containercontrol.py"""

from strongwind import *
from os.path import exists
from sys import path

def launchContainerControl(exe=None):
    """ 
    Launch containercontrol with accessibility enabled and return a containercontrol object.
    Log an error and return None if something goes wrong
    """

    if exe is None:
        # make sure we can find the sample application
        harness_dir = path[0]
        i = harness_dir.rfind("/")
        uiaqa_path = harness_dir[:i]
        exe = '%s/samples/containercontrol.py' % uiaqa_path
        if not exists(exe):
          raise IOError, "Could not find file %s" % exe

    args = [exe]

    (app, subproc) = cache.launchApplication(args=args, name='ipy', wait=config.LONG_DELAY)

    containercontrol = ContainerControl(app, subproc)

    cache.addApplication(containercontrol)

    containercontrol.containerControlFrame.app = containercontrol

    return containercontrol

class ContainerControl(accessibles.Application):
    """class to represent the application"""

    def __init__(self, accessible, subproc=None): 
        """Get a reference to the containercontrol window"""

        super(ContainerControl, self).__init__(accessible, subproc)
        self.findFrame(re.compile('^ContainerControl'), logName='Container Control')
