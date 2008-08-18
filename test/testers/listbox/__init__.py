
##############################################################################
# Written by:  Cachen Chen <cachen@novell.com>
# Date:        08/18/2008
# Description: Application wrapper for listbox.py
#              Used by the listbox-*.py tests
##############################################################################$

'Application wrapper for listbox'

from strongwind import *

from os.path import exists
from sys import path

def launchListBox(exe=None):
    'Launch listbox with accessibility enabled and return a listbox object.  Log an error and return None if something goes wrong'

    if exe is None:
        # make sure we can find the sample application
        harness_dir = path[0]
        i = harness_dir.rfind("/")
        uiaqa_path = harness_dir[:i]
        exe = '%s/samples/listbox.py' % uiaqa_path
        if not exists(exe):
          raise IOError, "Could not find file %s" % exe
  
    args = [exe]

    (app, subproc) = cache.launchApplication(args=args, name='ipy')

    listbox = ListBox(app, subproc)

    cache.addApplication(listbox)

    listbox.listBoxFrame.app = listbox

    return listbox

# class to represent the application
class ListBox(accessibles.Application):
    #checkShowing=False
    def __init__(self, accessible, subproc=None): 
        'Get a reference to the listbox window'
        super(ListBox, self).__init__(accessible, subproc)
        
        self.findFrame(re.compile('^ListBox control'), logName='List Box')

