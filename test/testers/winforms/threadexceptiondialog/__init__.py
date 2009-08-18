# vim: set tabstop=4 shiftwidth=4 expandtab
##############################################################################
# Written by:  Ray Wang <rawang@novell.com>
# Date:        02/10/2009
# Description: Application wrapper for threadexceptiondialog.py
#              be called by ../threadexceptiondialog_basic_ops.py
##############################################################################

"""Application wrapper for threadexceptiondialog.py"""

from strongwind import *
from os.path import exists
from sys import path

def launchThreadExceptionDialog(exe=None):
    """ 
    Launch threadexceptiondialog with accessibility enabled and return a threadexceptiondialog object.
    Log an error and return None if something goes wrong
    """

    if exe is None:
        # make sure we can find the sample application
        harness_dir = path[0]
        i = harness_dir.rfind("/")
        j = harness_dir[:i].rfind("/")
        uiaqa_path = harness_dir[:j]
        exe = '%s/samples/winforms/threadexceptiondialog.py' % uiaqa_path
        if not exists(exe):
          raise IOError, "Could not find file %s" % exe

    args = [exe]

    (app, subproc) = cache.launchApplication(args=args, name='ipy', wait=config.LONG_DELAY)

    threadexceptiondialog = ThreadExceptionDialog(app, subproc)

    cache.addApplication(threadexceptiondialog)

    threadexceptiondialog.threadExceptionDialogFrame.app = threadexceptiondialog

    return threadexceptiondialog

class ThreadExceptionDialog(accessibles.Application):
    """class to represent the application"""

    def __init__(self, accessible, subproc=None): 
        """Get a reference to the threadexceptiondialog window"""

        super(ThreadExceptionDialog, self).__init__(accessible, subproc)
        self.findFrame(re.compile('^ThreadExceptionDialog'), logName='Thread Exception Dialog')
