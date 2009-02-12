# vim: set tabstop=4 shiftwidth=4 expandtab
##############################################################################
# Written by:  Ray Wang <rawang@novell.com>
# Date:        02/10/2009
# Description: Application wrapper for threadexceptiondialog.py
#              be called by ../threadexceptiondialog_basic_ops.py
##############################################################################

"""Application wrapper for threadexceptiondialog.py"""

from strongwind import *

class ThreadExceptionDialogFrame(accessibles.Frame):
    """the profile of the threadexceptiondialog sample"""

    def __init__(self, accessible):
        super(ThreadExceptionDialogFrame, self).__init__(accessible)
        self.raise_button = self.findPushButton("Raise an Exception")

    def click(self, accessible):

        procedurelogger.action("click %s" % accessible)
        accessible.click()

    def show_dialog(self, accessible):

        procedurelogger.action("click %s" % accessible)
        accessible.click()

        # start to find children
        self.dialog = self.app.findDialog(None)
    
        # assign controls
        
        DESCRIPTION = re.compile("^An unhandled exception has occurred in you application.")
        ERROR_TYPE = "Division by zero"
        self.description_label = self.dialog.findLabel(DESCRIPTION)
        self.errortype_label = self.dialog.findLabel(ERROR_TYPE)

        self.detail_button = self.dialog.findPushButton("Show &Details")
        self.ignore_button = self.dialog.findPushButton("&Ignore")
        self.abort_button = self.dialog.findPushButton("&Abort")

        # BUG474254     
        #self.scrollbars = self.dialog.findAllScrollBars(None)
        #self.scrollbar_ver = self.scrollbars[0]
        #self.scrollbar_hor = self.scrollbars[1]

    def show_textbox(self, accessible):
        """click detain_button in order to show its textbox"""
        procedurelogger.action("click %s" % accessible)
        accessible.click()

        if accessible is self.detail_button:
            ERROR_TITLE = "Exception details"
            self.textbox = self.dialog.findText(None)
            self.errortitle_label = self.dialog.findLabel(ERROR_TITLE)

    def hide_textbox(self, accessible):
        """click detain_button in order to hide its textbox"""
        procedurelogger.action("click %s" % accessible)
        accessible.click()

        if accessible is self.detail_button:
            ERROR_TITLE = "Exception details"
            try:
                self.textbox = self.dialog.findText(None)
                self.errortitle_label = self.dialog.findLabel(ERROR_TITLE)
            except SearchError:
                pass # expected

    def inputText(self, accessible, text):
        procedurelogger.action('set %s text to "%s"' % (accessible, text))
        try:
            accessible.text = text
        except NotImplementedError:
            pass

    def assertText(self, accessible, text=None):
        """assert text is equal to the input"""

        procedurelogger.action('Assert the text of %s' % accessible)
        procedurelogger.expectedResult('%s text is "%s"' % \
                                                (accessible, accessible.text))
        assert accessible.text == text, '%s is not match with "%s"' % \
                                                (accessible, accessible.text)

    def quit(self):
        self.altF4()
