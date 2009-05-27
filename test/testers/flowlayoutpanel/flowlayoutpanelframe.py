# vim: set tabstop=4 shiftwidth=4 expandtab
##############################################################################
# Written by:  Ray Wang <rawang@novell.com>
# Date:        02/02/2009
# Description: Application wrapper for flowlayoutpanel.py
#              be called by ../flowlayoutpanel_basic_ops.py
##############################################################################

"""Application wrapper for flowlayoutpanel.py"""

from strongwind import *

class FlowLayoutPanelFrame(accessibles.Frame):
    """the profile of the flowlayoutpanel sample"""

    def __init__(self, accessible):
        super(FlowLayoutPanelFrame, self).__init__(accessible)
        self.panels = self.findAllPanels(None)
        self.labels = self.findAllLabels(None)
        self.buttons = self.findAllPushButtons(None)

        PNL_NUM = 4
        LBL_NUM = 4
        BTN_NUM = 4

        assert len(self.panels) == PNL_NUM, \
            "The number of panels is wrong! (found %s, expected %s)" % \
                                                  (len(self.panels), PNL_NUM)
        assert len(self.labels) == LBL_NUM, \
            "The number of labels is wrong! (found %s, expected %s)" % \
                                                  (len(self.labels), LBL_NUM)
        assert len(self.buttons) == BTN_NUM, \
            "The number of buttons is wrong! (found %s, expected %s)" % \
                                                  (len(self.buttons), BTN_NUM)

        # give the buttons some intuitive names
        self.button1 = self.buttons[0]
        self.button2 = self.buttons[1]
        self.button3 = self.buttons[2]
        self.button4 = self.buttons[3]

        # give the labels some intuitive names
        self.label1 = self.labels[0]
        self.label2 = self.labels[1]
        self.label3 = self.labels[2]
        self.label4 = self.labels[3]

        # give the panels some intuitive names
        self.panel1 = self.panels[0]
        self.panel2 = self.panels[1]
        self.panel3 = self.panels[2]
        self.panel4 = self.panels[3]
    
    def assertText(self, accessible, expected_text):
        """Assert that accessible's text is equal to the expected text"""
        procedurelogger.action('Check the text of "%s"' % accessible)
        procedurelogger.expectedResult('The text is "%s"' % expected_text)
        actual_text = accessible.text
        assert actual_text == expected_text, \
              'Text was "%s", expected "%s"' % (actual_text, expected_text)

    def quit(self):
        self.altF4()
