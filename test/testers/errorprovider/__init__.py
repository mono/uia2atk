# vim: set tabstop=4 shiftwidth=4 expandtab
##############################################################################
# Written by:  Brian G. Merrell <bgmerrell@novell.com>
# Date:        12/03/2008
# Description: Application wrapper for errorprovider.py
#              Used by the errorprovider-*.py tests
##############################################################################$

'''Application wrapper for errorprovider.py'''

from strongwind import *
from os.path import exists
from sys import path

def launchErrorProvider(exe=None):
    '''
    Launch errorprovider.py with accessibility enabled and return a errorprovider
    object.  Log an error and return None if something goes wrong
    '''

    if exe is None:
        # make sure we can find the sample application
        harness_dir = path[0]
        i = harness_dir.rfind("/")
        uiaqa_path = harness_dir[:i]
        exe = '%s/samples/errorprovider.py' % uiaqa_path
        if not exists(exe):
          raise IOError, "Could not find file %s" % exe
  
    args = [exe]

    (app, subproc) = cache.launchApplication(args=args, name='ipy', wait=config.LONG_DELAY)

    errorprovider = ErrorProvider(app, subproc)

    cache.addApplication(errorprovider)

    errorprovider.errorProviderFrame.app = errorprovider

    return errorprovider

# class to represent the application
class ErrorProvider(accessibles.Application):

    def __init__(self, accessible, subproc=None): 
        '''Get a reference to the errorprovider window'''
        super(ErrorProvider, self).__init__(accessible, subproc)
        
        self.findFrame(re.compile('^ErrorProvider'), logName='Error Provider')
