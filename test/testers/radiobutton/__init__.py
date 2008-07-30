
##############################################################################
# Written by:  Cachen Chen <cachen@novell.com>
# Date:        07/29/2008
# Description: Application wrapper for radiobutton.py
#              Used by the radiobutton-*.py tests
##############################################################################$

'Application wrapper for radiobutton'

from strongwind import *

import os

def launchRadioButton(exe=None):
    'Launch radiobutton with accessibility enabled and return a radiobutton object.  Log an error and return None if something goes wrong'

    if exe is None:
        # make sure we can find the sample application
        uiaqa_path = os.environ.get("UIAQA_HOME")
        if uiaqa_path is None:
          raise IOError, "When launching an application you must provide the "\
                         "full path or set the\nUIAQA_HOME environment "\
                         "variable."

        exe = '%s/samples/checkbox_radiobutton.py' % uiaqa_path
   
    if not os.path.exists(exe):
      raise IOError, "%s does not exist" % exe
  
    args = [exe]

    (app, subproc) = cache.launchApplication(args=args, name='ipy')

    radiobutton = RadioButton(app, subproc)

    cache.addApplication(radiobutton)

    radiobutton.radioButtonFrame.app = radiobutton

    return radiobutton

# class to represent the application
# because now program can't show winforms applicantion items 'showing' state,
# before runing this test, we should change 'x.showing' to 'not x.showing' in 
# accessibles.py line 180. remember to revert it after program improving
class RadioButton(accessibles.Application):
    #checkShowing=False
    def __init__(self, accessible, subproc=None): 
        'Get a reference to the RadioButton window'
        super(RadioButton, self).__init__(accessible, subproc)
        
        self.findFrame(re.compile('^CheckBox_RadioButton controls'), logName='Radio Button')

