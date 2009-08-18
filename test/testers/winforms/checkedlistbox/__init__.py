
##############################################################################
# Written by:  Cachen Chen <cachen@novell.com>
# Date:        08/21/2008
# Description: Application wrapper for checkedlistbox.py
#              Used by the checkedlistbox-*.py tests
##############################################################################$

'Application wrapper for checkedlistbox'

from strongwind import *

from os.path import exists
from sys import path

def launchCheckedListBox(exe=None):
    'Launch checkedlistbox with accessibility enabled and return a checkedlistbox object.  Log an error and return None if something goes wrong'

    if exe is None:
        # make sure we can find the sample application
        harness_dir = path[0]
        i = harness_dir.rfind("/")
        uiaqa_path = harness_dir[:i]
        exe = '%s/samples/winforms/checkedlistbox.py' % uiaqa_path
        if not exists(exe):
          raise IOError, "Could not find file %s" % exe
  
    args = [exe]

    (app, subproc) = cache.launchApplication(args=args, name='ipy', wait=config.LONG_DELAY)

    checkedlistbox = CheckedListBox(app, subproc)

    cache.addApplication(checkedlistbox)

    checkedlistbox.checkedListBoxFrame.app = checkedlistbox

    return checkedlistbox

# class to represent the application
class CheckedListBox(accessibles.Application):
    #checkShowing=False
    def __init__(self, accessible, subproc=None): 
        'Get a reference to the checkedlistbox window'
        super(CheckedListBox, self).__init__(accessible, subproc)
        
        self.findFrame(re.compile('^CheckedListBox control'), logName='Checked List Box')

