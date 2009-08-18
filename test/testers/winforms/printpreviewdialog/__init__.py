
##############################################################################
# Written by:  Cachen Chen <cachen@novell.com>
# Date:        01/16/2009
#              Application wrapper for printpreviewdialog.py
#              Used by the printpreviewdialog-*.py tests
##############################################################################$

'Application wrapper for printpreviewdialog'

from strongwind import *

from os.path import exists
from sys import path

def launchPrintPreviewDialog(exe=None):
    'Launch printpreviewdialog with accessibility enabled and return a printpreviewdialog object.  Log an error and return None if something goes wrong'

    if exe is None:
        # make sure we can find the sample application
        harness_dir = path[0]
        i = harness_dir.rfind("/")
        uiaqa_path = harness_dir[:i]
        exe = '%s/samples/winforms/printpreviewdialog.py' % uiaqa_path
        if not exists(exe):
          raise IOError, "Could not find file %s" % exe
  
    args = [exe]

    (app, subproc) = cache.launchApplication(args=args, name='ipy', wait=config.LONG_DELAY)

    printpreviewdialog = PrintPreviewDialog(app, subproc)

    cache.addApplication(printpreviewdialog)

    printpreviewdialog.printPreviewDialogFrame.app = printpreviewdialog

    return printpreviewdialog

# class to represent the application
class PrintPreviewDialog(accessibles.Application):
    #checkShowing=False
    def __init__(self, accessible, subproc=None): 
        'Get a reference to the printpreviewdialog window'
        super(PrintPreviewDialog, self).__init__(accessible, subproc)
        
        self.findFrame(re.compile('^PrintPreviewDialog control'), logName='Print Preview Dialog')

