
##############################################################################
# Written by:  Cachen Chen <cachen@novell.com>
# Date:        02/23/2009
#              Application wrapper for colordialog.py
#              Used by the colordialog-*.py tests
##############################################################################$

'Application wrapper for colordialog'

from strongwind import *

from os.path import exists
from sys import path

def launchColorDialog(exe=None):
    'Launch colordialog with accessibility enabled and return a colordialog object.  Log an error and return None if something goes wrong'

    if exe is None:
        # make sure we can find the sample application
        harness_dir = path[0]
        i = harness_dir.rfind("/")
        uiaqa_path = harness_dir[:i]
        exe = '%s/samples/winforms/colordialog.py' % uiaqa_path
        if not exists(exe):
          raise IOError, "Could not find file %s" % exe
  
    args = [exe]

    (app, subproc) = cache.launchApplication(args=args, name='ipy', wait=config.LONG_DELAY)

    colordialog = ColorDialog(app, subproc)

    cache.addApplication(colordialog)

    colordialog.colorDialogFrame.app = colordialog

    return colordialog

# class to represent the application
class ColorDialog(accessibles.Application):
    #checkShowing=False
    def __init__(self, accessible, subproc=None): 
        'Get a reference to the colordialog window'
        super(ColorDialog, self).__init__(accessible, subproc)
        
        self.findFrame(re.compile('^ColorDialog control'), logName='Color Dialog')

