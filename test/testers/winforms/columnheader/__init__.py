
##############################################################################
# Written by:  Cachen Chen <cachen@novell.com>
# Date:        01/04/2009
# Description: Application wrapper for columnheader.py
#              Used by the columnheader-*.py tests
##############################################################################$

'Application wrapper for columnheader'

from strongwind import *

from os.path import exists
from sys import path

def launchColumnHeader(exe=None):
    'Launch columnheader detail mode with accessibility enabled and return a\
     columnheader object.Log an error and return None if something goes wrong'

    if exe is None:
        # make sure we can find the sample application
        harness_dir = path[0]
        i = harness_dir.rfind("/")
        uiaqa_path = harness_dir[:i]
        exe = '%s/samples/winforms/columnheader.py' % uiaqa_path
        if not exists(exe):
          raise IOError, "Could not find file %s" % exe
  
    args = [exe]

    (app, subproc) = cache.launchApplication(args=args, name='ipy', wait=config.LONG_DELAY)

    columnheader = ColumnHeader(app, subproc)

    cache.addApplication(columnheader)

    columnheader.columnHeaderFrame.app = columnheader

    return columnheader

# class to represent the application
class ColumnHeader(accessibles.Application):
    #checkShowing=False
    def __init__(self, accessible, subproc=None): 
        'Get a reference to the columnheader window'
        super(ColumnHeader, self).__init__(accessible, subproc)
        
        self.findFrame(re.compile('^ColumnHeader control'), logName='Column Header')

