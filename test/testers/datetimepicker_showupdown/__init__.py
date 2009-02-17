# vim: set tabstop=4 shiftwidth=4 expandtab
##############################################################################
# Written by:  Ray Wang <rawang@novell.com>
# Date:        02/16/2008
# Description: Application wrapper for datetimepicker_showupdown.py
#              be called by ../datetimepicker_showupdown_ops.py
##############################################################################

"""Application wrapper for datetimepicker_showupdown.py"""

from strongwind import *
from os.path import exists
from sys import path

def launchDateTimePicker(exe=None):
    """ 
    Launch datetimepicker_showupdown with accessibility enabled and return a datetimepicker_showupdown object.
    Log an error and return None if something goes wrong
    """

    if exe is None:
        # make sure we can find the sample application
        harness_dir = path[0]
        i = harness_dir.rfind("/")
        uiaqa_path = harness_dir[:i]
        exe = '%s/samples/datetimepicker_showupdown.py' % uiaqa_path
        if not exists(exe):
          raise IOError, "Could not find file %s" % exe

    args = [exe]

    (app, subproc) = cache.launchApplication(args=args, name='ipy', wait=config.LONG_DELAY)

    datetimepicker = DateTimePicker(app, subproc)

    cache.addApplication(datetimepicker)

    datetimepicker.dateTimePickerShowUpDownFrame.app = datetimepicker

    return datetimepicker

class DateTimePicker(accessibles.Application):
    """class to represent the application"""

    def __init__(self, accessible, subproc=None): 
        """Get a reference to the datetimepicker_showupdown window"""

        super(DateTimePicker, self).__init__(accessible, subproc)

        self.findFrame(re.compile('^DateTimePicker'), logName='Date Time Picker Show Up Down')
