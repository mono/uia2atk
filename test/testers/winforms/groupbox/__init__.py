
##############################################################################
# Written by:  Cachen Chen <cachen@novell.com>
# Date:        08/19/2008
#              Application wrapper for groupbox.py
#              Used by the groupbox-*.py tests
##############################################################################$

'Application wrapper for groupbox'

from strongwind import *

from os.path import exists
from sys import path

def launchGroupBox(exe=None):
    'Launch groupbox with accessibility enabled and return a groupbox object.  Log an error and return None if something goes wrong'

    if exe is None:
        # make sure we can find the sample application
        harness_dir = path[0]
        i = harness_dir.rfind("/")
        uiaqa_path = harness_dir[:i]
        exe = '%s/samples/groupbox.py' % uiaqa_path
        if not exists(exe):
          raise IOError, "Could not find file %s" % exe
  
    args = [exe]

    (app, subproc) = cache.launchApplication(args=args, name='ipy', wait=config.LONG_DELAY)

    groupbox = GroupBox(app, subproc)

    cache.addApplication(groupbox)

    groupbox.groupBoxFrame.app = groupbox

    return groupbox

# class to represent the application
class GroupBox(accessibles.Application):
    #checkShowing=False
    def __init__(self, accessible, subproc=None): 
        'Get a reference to the groupbox window'
        super(GroupBox, self).__init__(accessible, subproc)
        
        self.findFrame(re.compile('^GroupBox with Button'), logName='Group Box')

