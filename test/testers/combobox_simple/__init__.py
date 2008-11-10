
##############################################################################
# Written by:  Cachen Chen <cachen@novell.com>
# Date:        11/10/2008
# Description: Application wrapper for combobox_simple.py
#              Used by the combobox_simple-*.py tests
##############################################################################$

'Application wrapper for combobox_simple'

from strongwind import *

from os.path import exists
from sys import path

def launchComboBox(exe=None):
    'Launch combobox_simple with accessibility enabled and return a \
    combobox_simple object.  Log an error and return None if something \
    goes wrong'

    if exe is None:
        # make sure we can find the sample application
        harness_dir = path[0]
        i = harness_dir.rfind("/")
        uiaqa_path = harness_dir[:i]
        exe = '%s/samples/combobox_simple.py' % uiaqa_path
        if not exists(exe):
          raise IOError, "Could not find file %s" % exe
  
    args = [exe]

    (app, subproc) = cache.launchApplication(args=args, name='ipy', wait=config.LONG_DELAY)

    combobox = ComboBox(app, subproc)

    cache.addApplication(combobox)

    combobox.comboBoxFrame.app = combobox

    return combobox

# class to represent the application
class ComboBox(accessibles.Application):
    #checkShowing=False
    def __init__(self, accessible, subproc=None): 
        'Get a reference to the combobox window'
        super(ComboBox, self).__init__(accessible, subproc)
        
        self.findFrame(re.compile('^ComboBox control'), logName='Combo Box')

