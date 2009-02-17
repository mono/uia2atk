
##############################################################################
# Written by:  Neville Gao <nevillegao@gmail.com>
# Date:        02/16/2009
# Description: pagesetupdialog.py wrapper script
#              Used by the pagesetupdialog-*.py tests
##############################################################################

from strongwind import *


class PageSetupDialogFrame(accessibles.Frame):
    def __init__(self, accessible):
        super(PageSetupDialogFrame, self).__init__(accessible)
        self.click_button = self.findPushButton('Click me')

    def click(self, button):
        button.click()

    def assertPageSetupDialog(self, button=None):
        procedurelogger.action('Searching for all widgets in PageSetupDialog diglog')
        procedurelogger.expectedResult('All widgets in PageSetupDialog should show up')

        # PageSetupDialog
        self.main_dialog = self.app.findDialog('Page Setup')
        # Preview panel
        self.area_panel = self.main_dialog.findPanel(None)
        # Buttons
        self.main_ok_button = self.main_dialog.findPushButton('OK')
        self.main_cancel_button = self.main_dialog.findPushButton('Cancel')
        self.printer_button = self.main_dialog.findPushButton('&Printer...')
        # Margins panel
        self.margins_panel = self.main_dialog.findPanel(re.compile('^Margins'))
        self.bottom_text = self.margins_panel.findAllTexts(None)[0]
        self.right_text = self.margins_panel.findAllTexts(None)[1]
        self.top_text = self.margins_panel.findAllTexts(None)[2]
        self.left_text = self.margins_panel.findAllTexts(None)[3]
        self.bottom_label = self.margins_panel.findLabel('&Bottom:')
        self.right_label = self.margins_panel.findLabel('&Right:')
        self.top_label = self.margins_panel.findLabel('&Top:')
        self.left_label = self.margins_panel.findLabel('&Left:')
        # Orientation panel
        self.orientation_panel = self.main_dialog.findPanel('Orientation')
        self.landscape_radio = self.orientation_panel.findRadioButton('L&andscape')
        self.portrait_radio = self.orientation_panel.findRadioButton('P&ortrait')
        # Paper panel
        self.paper_panel = self.main_dialog.findPanel('Paper')
        self.source_combobox = self.paper_panel.findAllComboBoxs(None)[0]
        self.size_combobox = self.paper_panel.findAllComboBoxs(None)[1]
        self.source_label = self.paper_panel.findLabel('&Source:')
        self.size_label = self.paper_panel.findLabel('Si&ze:')

    def assertConfigrePageDialog(self, button=None):
        procedurelogger.action('Searching for all widgets in ConfigurePageDialog')
        procedurelogger.expectedResult('All widgets in ConfigurePageDialog should show up')

        # ConfigurePageDialog
        self.configure_dialog = self.app.findDialog('Configure page')
        # Buttons
        self.configure_ok_button = self.configure_dialog.findPushButton('OK')
        self.configure_cancel_button = self.configure_dialog.findPushButton('Cancel')
        self.network_button = self.configure_dialog.findPushButton('Network...')
        # Printer panel
        self.printer_panel = self.configure_dialog.findPanel('Printer')
        self.properties_button = self.printer_panel.findPushButton('Properties...')
        self.comment_content_label = self.printer_panel.findAllLabels(None)[0]
        self.where_content_label = self.printer_panel.findAllLabels(None)[1]
        self.comment_label = self.printer_panel.findLabel('Comment:')
        self.where_label = self.printer_panel.findLabel('Where:')
        self.type_content_label = self.printer_panel.findLabel(re.compile('^Bej'))
        self.type_label = self.printer_panel.findLabel('Type:')
        self.status_content_label = self.printer_panel.findAllLabels(None)[6]
        self.status_label = self.printer_panel.findLabel('Status:')
        self.name_label = self.printer_panel.findLabel('Name:')
        self.name_combobox = self.printer_panel.findAllComboBoxes(None)[0]

    def quit(self):
        self.altF4()
