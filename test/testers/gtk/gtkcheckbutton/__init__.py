# -*- coding: utf-8 -*-

##############################################################################$
# Written by:  Brian G. Merrell <bgmerrell@novell.com>$
# Date:        May 23 2008$
# Description: Application wrapper for gtkcheckbutton.py 
#              Used by the gtkcheckbutton-*.py tests
##############################################################################$

'Application wrapper for checkbutton'

from strongwind import *

from os.path import exists
from sys import path

def launchCheckButton(exe=None):
    'Launch gtkcheckbutton with accessibility enabled and return a CheckButton object.  Log an error and return None if something goes wrong'

    if exe is None:
        # make sure we can find the sample application
        harness_dir = path[0]
        i = harness_dir.rfind("/")
        j = harness_dir[:i].rfind("/")
        uiaqa_path = harness_dir[:j]
        exe = '%s/samples/gtk/gtkcheckbutton.py' % uiaqa_path
        if not exists(exe):
          raise IOError, "Could not find file %s" % exe

    args = [exe]

    (app, subproc) = cache.launchApplication(args=args)

    checkbutton = GtkCheckButton(app, subproc)
    cache.addApplication(checkbutton)

    checkbutton.gtkCheckButtonFrame.app = checkbutton

    return checkbutton

# class to represent the application
class GtkCheckButton(accessibles.Application):
    def __init__(self, accessible, subproc=None):
        'Get a reference to the Check Button window'
        super(GtkCheckButton, self).__init__(accessible, subproc)
        self.findFrame(re.compile('^Check\ Button'), logName='Gtk Check Button')
