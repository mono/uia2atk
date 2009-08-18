
##############################################################################
# Written by:  Neville Gao <nevillegao@gmail.com>
# Date:        02/16/2009
# Description: Application wrapper for pagesetupdialog.py
#              Used by the pagesetupdialog-*.py tests
##############################################################################

'Application wrapper for pagesetupdialog'

from strongwind import *

from os.path import exists
from sys import path


def launchPageSetupDialog(exe=None):
    'Launch PageSetupDialog with accessibility enabled and return a PageSetupDialog object. Log an error and return None if something goes wrong'

    if exe is None:
        # make sure we can find the sample application
        harness_dir = path[0]
        i = harness_dir.rfind("/")
        j = harness_dir[:i].rfind("/")
        uiaqa_path = harness_dir[:j]
        exe = '%s/samples/winforms/pagesetupdialog.py' % uiaqa_path
        if not exists(exe):
          raise IOError, "Could not find file %s" % exe

    args = [exe]

    (app, subproc) = cache.launchApplication(args=args, name='ipy', wait=config.LONG_DELAY)

    pagesetupdialog = PageSetupDialog(app, subproc)

    cache.addApplication(pagesetupdialog)

    pagesetupdialog.pageSetupDialogFrame.app = pagesetupdialog

    return pagesetupdialog


# class to represent the application
class PageSetupDialog(accessibles.Application):
    def __init__(self, accessible, subproc=None):
        super(PageSetupDialog, self).__init__(accessible, subproc)
        self.findFrame('PageSetupDialog control', logName='Page Setup Dialog')
