
##############################################################################
# Written by:  Mario Carrion <mcarrion@novell.com>
# Date:        02/23/2009
#              Application wrapper for datagridview.py
#              Used by the datagridview-*.py tests
##############################################################################$

'Application wrapper for DataGridView'

from strongwind import *

from os.path import exists
from sys import path

def launchDataGridView(exe=None):
    'Launch datagridview with accessibility enabled and return a datagridview object.  Log an error and return None if something goes wrong'

    if exe is None:
        # make sure we can find the sample application
        harness_dir = path[0]
        i = harness_dir.rfind("/")
        uiaqa_path = harness_dir[:i]
        exe = '%s/samples/datagridview.py' % uiaqa_path
        if not exists(exe):
          raise IOError, "Could not find file %s" % exe
  
    args = [exe]

    (app, subproc) = cache.launchApplication(args=args, name='ipy', wait=config.LONG_DELAY)

    datagridview = DataGridView(app, subproc)

    cache.addApplication(datagridview)

    datagridview.dataGridViewFrame.app = datagridview

    return datagridview

# class to represent the application
class DataGridView(accessibles.Application):
    #checkShowing=False
    def __init__(self, accessible, subproc=None): 
        'Get a reference to the DataGridView window'
        super(DataGridView, self).__init__(accessible, subproc)
        
        self.findFrame(re.compile('^DataGridView control'), logName='Data Grid View')

