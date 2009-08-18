
##############################################################################
# Written by:  Cachen Chen <cachen@novell.com>
# Date:        2/26/2008
# Description: Application wrapper for datagrid.py
#              Used by the datagrid-*.py tests
##############################################################################$

'Application wrapper for datagrid'

from strongwind import *

from os.path import exists
from sys import path

def launchDataGrid(exe=None):
    'Launch datagrid mode with accessibility enabled and return a datagrid object. \
     Log an error and return None if something goes wrong'

    if exe is None:
        # make sure we can find the sample application
        harness_dir = path[0]
        i = harness_dir.rfind("/")
        uiaqa_path = harness_dir[:i]
        exe = '%s/samples/datagrid.py' % uiaqa_path
        if not exists(exe):
          raise IOError, "Could not find file %s" % exe
  
    args = [exe]

    (app, subproc) = cache.launchApplication(args=args, name='ipy', wait=config.LONG_DELAY)

    datagrid = DataGrid(app, subproc)

    cache.addApplication(datagrid)

    datagrid.dataGridFrame.app = datagrid

    return datagrid

# class to represent the application
class DataGrid(accessibles.Application):
    #checkShowing=False
    def __init__(self, accessible, subproc=None): 
        'Get a reference to the DataGrid window'
        super(DataGrid, self).__init__(accessible, subproc)
        
        self.findFrame(re.compile('^DataGrid control'), logName='Data Grid')

