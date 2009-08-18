# vim: set tabstop=4 shiftwidth=4 expandtab
##############################################################################
# Written by:  Sandy Armstrong <saarmstrong@novell.com>
# Date:        02/23/2009
#              Application wrapper for MonthCalendar.py
#              Used by the monthcalendar-*.py tests
##############################################################################$

'Application wrapper for monthcalendar'

from strongwind import *

from os.path import exists
from sys import path

def launchMonthCalendar(exe=None):
    'Launch monthcalendar with accessibility enabled and return a monthcalendar object.  Log an error and return None if something goes wrong'

    if exe is None:
        # make sure we can find the sample application
        harness_dir = path[0]
        i = harness_dir.rfind("/")
        j = harness_dir[:i].rfind("/")
        uiaqa_path = harness_dir[:j]
        exe = '%s/samples/winforms/monthcalendar.py' % uiaqa_path
        if not exists(exe):
          raise IOError, "Could not find file %s" % exe
  
    args = [exe]

    (app, subproc) = cache.launchApplication(args=args, name='ipy', wait=config.LONG_DELAY)

    monthcalendar = MonthCalendar(app, subproc)

    cache.addApplication(monthcalendar)

    monthcalendar.monthCalendarFrame.app = monthcalendar

    return monthcalendar

# class to represent the application
class MonthCalendar(accessibles.Application):
    #checkShowing=False
    def __init__(self, accessible, subproc=None): 
        'Get a reference to the MonthCalendar window'
        super(MonthCalendar, self).__init__(accessible, subproc)
        
        self.findFrame(re.compile('^MonthCalendar control'), logName='Month Calendar')

