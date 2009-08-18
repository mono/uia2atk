
##############################################################################
# Written by:  Cachen Chen <cachen@novell.com>
# Date:        02/06/2009
#              Application wrapper for toolbarbutton.py
#              Used by the toolbarbutton-*.py tests
##############################################################################

'Application wrapper for toolbarbutton'

from strongwind import *

from os.path import exists
from sys import path

def launchToolBar(exe=None):
    'Launch form with accessibility enabled and return a toolbarbutton object.  \
     Log an error and return None if something goes wrong'

    if exe is None:
        # make sure we can find the sample application
        harness_dir = path[0]
        i = harness_dir.rfind("/")
        j = harness_dir[:i].rfind("/")
        uiaqa_path = harness_dir[:j]
        exe = '%s/samples/winforms/toolbarbutton.py' % uiaqa_path
        if not exists(exe):
          raise IOError, "Could not find file %s" % exe
  
    args = [exe]

    (app, subproc) = cache.launchApplication(args=args, name='ipy', wait=config.LONG_DELAY)

    toolbarbutton = ToolBarButton(app, subproc)

    cache.addApplication(toolbarbutton)

    toolbarbutton.toolBarButtonFrame.app = toolbarbutton

    return toolbarbutton

# class to represent the application
class ToolBarButton(accessibles.Application):

    def __init__(self, accessible, subproc=None): 
        'Get a reference to the toolbarbutton window'
        super(ToolBarButton, self).__init__(accessible, subproc)
        
        self.findFrame(re.compile('^ToolBarButton control'), logName='Tool Bar Button')

