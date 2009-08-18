
##############################################################################
# Written by:  Cachen Chen <cachen@novell.com>
# Date:        01/13/2009
#              Application wrapper for printpreviewcontrol.py
#              Used by the printpreviewcontrol-*.py tests
##############################################################################$

'Application wrapper for printpreviewcontrol'

from strongwind import *

from os.path import exists
from sys import path

def launchPrintPreviewControl(exe=None):
    'Launch printpreviewcontrol with accessibility enabled and return a printpreviewcontrol object.  Log an error and return None if something goes wrong'

    if exe is None:
        # make sure we can find the sample application
        harness_dir = path[0]
        i = harness_dir.rfind("/")
        j = harness_dir[:i].rfind("/")
        uiaqa_path = harness_dir[:j]
        exe = '%s/samples/winforms/printpreviewcontrol.py' % uiaqa_path
        if not exists(exe):
          raise IOError, "Could not find file %s" % exe
  
    args = [exe]

    (app, subproc) = cache.launchApplication(args=args, name='ipy', wait=config.LONG_DELAY)

    printpreviewcontrol = PrintPreviewControl(app, subproc)

    cache.addApplication(printpreviewcontrol)

    printpreviewcontrol.printPreviewControlFrame.app = printpreviewcontrol

    return printpreviewcontrol

# class to represent the application
class PrintPreviewControl(accessibles.Application):
    #checkShowing=False
    def __init__(self, accessible, subproc=None): 
        'Get a reference to the PrintPreviewControl window'
        super(PrintPreviewControl, self).__init__(accessible, subproc)
        
        self.findFrame(re.compile('^PrintPreviewControl control'), logName='Print Preview Control')

