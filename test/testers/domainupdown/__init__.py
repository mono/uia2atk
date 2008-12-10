# vim: set tabstop=4 shiftwidth=4 expandtab
##############################################################################
# Written by:  Ray Wang <rawang@novell.com>
# Date:        12/07/2008
# Description: Application wrapper for domainupdown.py
#              be called by ../domainupdown_basic_ops.py
##############################################################################$

"""Application wrapper for domainupdown"""

from strongwind import *
from os.path import exists
from sys import path

def launchDomainUpDown(exe=None):
    """
    Launch domainupdown with accessibility enabled and return a domainupdown object.  
    Log an error and return None if something goes wrong
    """

    if exe is None:
        # make sure we can find the sample application
        harness_dir = path[0]
        i = harness_dir.rfind("/")
        uiaqa_path = harness_dir[:i]
        exe = '%s/samples/domainupdown.py' % uiaqa_path
        if not exists(exe):
          raise IOError, "Could not find file %s" % exe

    args = [exe]

    (app, subproc) = cache.launchApplication(args=args, name='ipy', wait=config.LONG_DELAY)

    domainupdown = DomainUpDown(app, subproc)

    cache.addApplication(domainupdown)

    domainupdown.domainUpDownFrame.app = domainupdown

    return domainupdown

# class to represent the application
class DomainUpDown(accessibles.Application):

    def __init__(self, accessible, subproc=None):
        """Get a reference to the domainupdown window"""
        super(DomainUpDown, self).__init__(accessible, subproc)

        self.findFrame(re.compile('^DomainUpDown'), logName='Domain Up Down')
