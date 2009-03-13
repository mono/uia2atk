# vim: set tabstop=4 shiftwidth=4 expandtab
##############################################################################
# Written by:  Brian G. Merrell <bgmerrell@novell.com>
# Date:        03/12/2009
# Description: Application wrapper for combobox_stylechanges.py
##############################################################################

"""Application wrapper for combobox_stylechanges.py"""

from strongwind import *
import time

class ComboBoxStyleChangesFrame(accessibles.Frame):

    def __init__(self, accessible):
        super(ComboBoxStyleChangesFrame, self).__init__(accessible)

    def quit(self):
        self.altF4()
