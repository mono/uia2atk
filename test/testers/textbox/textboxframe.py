# vim: set tabstop=4 shiftwidth=4 expandtab
##############################################################################
# Written by:  Ray Wang <rawang@novell.com>
# Date:        02/20/2008
# Description: Application wrapper for textbox.py
#              be called by ../textbox_basic_ops.py
##############################################################################

"""Application wrapper for textbox.py"""

from strongwind import *
import time

class TextBoxFrame(accessibles.Frame):
    """the profile of the textbox sample"""

    LABEL_NORMAL = "Normal TextBox"
    LABEL_MLINE = "Multi-Line TextBox"
    LABEL_PASSWD = "Password TextBox"

    def __init__(self, accessible):
        super(TextBoxFrame, self).__init__(accessible)
        self.label_normal = self.findLabel(self.LABEL_NORMAL)
        self.label_mline = self.findLabel(self.LABEL_MLINE)
        self.label_passwd = self.findLabel(self.LABEL_PASSWD)

        self.textboxes = self.findAllTexts(None)
        assert len(self.textboxes) == 3, "the number of textboxes is incorrect"

        self.textbox_normal = self.textboxes[0]
        self.textbox_mline = self.textboxes[1]
        self.textbox_passwd = self.textboxes[2]

    def click(self, button):
        procedurelogger.action("click %s" % button)
        button.click()

    def assertText(self, accessible, text=None):
        """assert text is equal to the input"""

        procedurelogger.action('Assert the text of %s' % accessible)
        procedurelogger.expectedResult('%s text is "%s"' % \
                                                (accessible, accessible.text))
        assert accessible.text == text, '%s is not match with "%s"' % \
                                                (accessible, accessible.text)

    def assertOffset(self, accessible, offset=None):
        """assert text's offset is equal to the input"""

        procedurelogger.expectedResult('check the offset of "%s"' % accessible)

        assert accessible.caretOffset == offset, '%s is not match with "%s"' % \
                                                (accessible.caretOffset, offset)

    def inputText(self, accessible, text):
        procedurelogger.action('set %s text to "%s"' % (accessible, text))
        try:
            accessible.text = text
        except NotImplementedError:
            pass

    def inputValue(self, accessible, value):
        procedurelogger.action('set %s value to "%s"' % (accessible, value))
        try:
            accessible.value = value
        except NotImplementedError:
            pass

    def assertSelectChild(self, accessible, index):
        """assert Selection implementation"""
        procedurelogger.action('select index %s in "%s"' % \
                                        (index, accessible))
        accessible.selectChild(index)    

    def quit(self):
        self.altF4()
