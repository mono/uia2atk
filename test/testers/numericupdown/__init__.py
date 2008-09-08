
##############################################################################
# Written by:  Cachen Chen <cachen@novell.com>
# Date:        09/08/2008
# Description: Application wrapper for numericupdown.py
#              Used by the numericupdown-*.py tests
##############################################################################$

'Application wrapper for numericupdown'

from strongwind import *

from os.path import exists
from sys import path

def launchNumericUpDown(exe=None):
    'Launch numericupdown with accessibility enabled and return a numericupdown object.  Log an error and return None if something goes wrong'

    if exe is None:
        # make sure we can find the sample application
        harness_dir = path[0]
        i = harness_dir.rfind("/")
        uiaqa_path = harness_dir[:i]
        exe = '%s/samples/numericupdown.py' % uiaqa_path
        if not exists(exe):
          raise IOError, "Could not find file %s" % exe
  
    args = [exe]

    (app, subproc) = cache.launchApplication(args=args, name='ipy', wait=config.LONG_DELAY)

    numericupdown = NumericUpDown(app, subproc)

    cache.addApplication(numericupdown)

    numericupdown.numericUpDownFrame.app = numericupdown

    return numericupdown

# class to represent the application
class NumericUpDown(accessibles.Application):
    #checkShowing=False
    def __init__(self, accessible, subproc=None): 
        'Get a reference to the numericupdown window'
        super(NumericUpDown, self).__init__(accessible, subproc)
        
        self.findFrame(re.compile('^NumericUpDown Example'), logName='Numeric Up Down')

