##############################################################################$
# Written by:  Brian G. Merrell <bgmerrell@novell.com>$
# Date:        May 23 2008$
# Description: gtkcheckbutton.py wrapper script
#              Used by the gtkcheckbutton-*.py tests
##############################################################################$

from strongwind import *

# class to represent the main window.
class GtkCheckButtonFrame(accessibles.Frame):

    # constants
    # the available widgets on the window
    CHECK_BUTTON_ONE = "check button 1"
    CHECK_BUTTON_TWO = "check button 2"
    BUTTON_QUIT = "Quit"

    # available results for the check boxes
    RESULT_UNCHECKED = "unchecked"
    RESULT_CHECKED = "checked"
    # end constants

    def __init__(self, accessible):
        super(GtkCheckButtonFrame, self).__init__(accessible)
        self.checkbox1 = self.findCheckBox(self.CHECK_BUTTON_ONE)
        self.checkbox2 = self.findCheckBox(self.CHECK_BUTTON_TWO)

    def assertChecked(self, accessible):
        'Raise exception if the accessible does not match the given result'   
        procedurelogger.expectedResult('%s is %s.' % (accessible, self.RESULT_CHECKED))
        def resultMatches():
            return accessible.checked
	
        assert retryUntilTrue(resultMatches)

    def assertUnchecked(self, accessible):
        'Raise exception if the accessible does not match the given result'   
        procedurelogger.expectedResult('%s is %s.' % (accessible, self.RESULT_UNCHECKED))

        def resultMatches():
            return not accessible.checked
	
        assert retryUntilTrue(resultMatches)

    def quit(self):
        'Quit checkbutton'

        # click the quit button
        self.app.findPushButton(self.BUTTON_QUIT).click()

        self.assertClosed()

    def assertClosed(self):
        super(GtkCheckButtonFrame, self).assertClosed()

        # if the checkbutton window closes, the entire app should close.  
        # assert that this is true 
        self.app.assertClosed()
