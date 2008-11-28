
##############################################################################
# Written by:  Cachen Chen <cachen@novell.com>
# Date:        08/06/2008
# Description: Application wrapper for scrollbar.py
#              Used by the scrollbar-*.py tests
##############################################################################$

'Application wrapper for scrollbar'

from strongwind import *

from os.path import exists
from sys import path

def launchScrollBar(exe=None):
    'Launch ScrollBar with accessibility enabled and return a scrollbar object.  Log an error and return None if something goes wrong'

    if exe is None:
        # make sure we can find the sample application
        harness_dir = path[0]
        i = harness_dir.rfind("/")
        uiaqa_path = harness_dir[:i]
        if uiaqa_path is None:
          raise IOError, "When launching an application you must provide the "\
                         "full path or set the\nUIAQA_HOME environment "\
                         "variable."

        exe = '%s/samples/scrollbar.py' % uiaqa_path
   
    if not os.path.exists(exe):
      raise IOError, "%s does not exist" % exe
  
    args = [exe]

    (app, subproc) = cache.launchApplication(args=args, name='ipy', wait=config.LONG_DELAY)

    scrollbar = ScrollBar(app, subproc)

    cache.addApplication(scrollbar)

    scrollbar.scrollBarFrame.app = scrollbar

    return scrollbar

# class to represent the application
class ScrollBar(accessibles.Application):
    #checkShowing=False
    def __init__(self, accessible, subproc=None): 
        'Get a reference to the scrollBar window'
        super(ScrollBar, self).__init__(accessible, subproc)
        
        self.findFrame(re.compile('^ScrollBar control'), logName='Scroll Bar')

