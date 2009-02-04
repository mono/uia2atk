
##############################################################################
# Written by:  Cachen Chen <cachen@novell.com>
# Date:        02/04/2009
#              Application wrapper for toolbar.py
#              Used by the toolbar-*.py tests
##############################################################################

'Application wrapper for toolbar'

from strongwind import *

from os.path import exists
from sys import path

def launchToolBar(exe=None):
    'Launch form with accessibility enabled and return a toolbar object.  \
     Log an error and return None if something goes wrong'

    if exe is None:
        # make sure we can find the sample application
        harness_dir = path[0]
        i = harness_dir.rfind("/")
        uiaqa_path = harness_dir[:i]
        exe = '%s/samples/toolbar.py' % uiaqa_path
        if not exists(exe):
          raise IOError, "Could not find file %s" % exe
  
    args = [exe]

    (app, subproc) = cache.launchApplication(args=args, name='ipy', wait=config.LONG_DELAY)

    toolbar = ToolBar(app, subproc)

    cache.addApplication(toolbar)

    toolbar.toolBarFrame.app = toolbar

    return toolbar

# class to represent the application
class ToolBar(accessibles.Application):

    def __init__(self, accessible, subproc=None): 
        'Get a reference to the toolbar window'
        super(ToolBar, self).__init__(accessible, subproc)
        
        self.findFrame(re.compile('^ToolBar control'), logName='Tool Bar')

