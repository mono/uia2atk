
##############################################################################
# Written by:  Neville Gao <nevillegao@gmail.com>
# Date:        02/16/2009
# Description: pagesetupdialog.py wrapper script
#              Used by the pagesetupdialog-*.py tests
##############################################################################

from strongwind import *
from helpers import *


class PageSetupDialogFrame(accessibles.Frame):

    MARGINS_TEXTS_NUM = 4
    PAPER_COMBOBOXS_NUM = 2
    PRINTER_LABELS_NUM = 9

    def __init__(self, accessible):
        super(PageSetupDialogFrame, self).__init__(accessible)
        self.click_button = self.findPushButton('Click me')

    def findPageSetupDialogAccessibles(self):
        procedurelogger.action('Searching for all widgets in PageSetupDialog')
        procedurelogger.expectedResult('All widgets in PageSetupDialog should show up')

        # Page Setup Dialog
        self.main_dialog = self.app.findDialog('Page Setup')
        # Preview panel
        self.area_panel = self.main_dialog.findPanel(None)
        # Buttons
        self.main_ok_button = self.main_dialog.findPushButton('OK')
        self.main_cancel_button = self.main_dialog.findPushButton('Cancel')
        self.printer_button = self.main_dialog.findPushButton('Printer...')
        # Margins panel
        self.margins_panel = self.main_dialog.findPanel(re.compile('^Margins'))
        self.margins_texts = self.margins_panel.findAllTexts(None)
        assert len(self.margins_texts) == self.MARGINS_TEXTS_NUM, \
                                  "actual number of text:%s, expected:%s" % \
                            (len(self.margins_texts), self.MARGINS_TEXTS_NUM)
        self.bottom_text = self.margins_panel.findAllTexts(None)[0]
        self.right_text = self.margins_panel.findAllTexts(None)[1]
        self.top_text = self.margins_panel.findAllTexts(None)[2]
        self.left_text = self.margins_panel.findAllTexts(None)[3]
        self.bottom_label = self.margins_panel.findLabel('Bottom:')
        self.right_label = self.margins_panel.findLabel('Right:')
        self.top_label = self.margins_panel.findLabel('Top:')
        self.left_label = self.margins_panel.findLabel('Left:')
        # Orientation panel
        self.orientation_panel = self.main_dialog.findPanel('Orientation')
        self.landscape_radio = \
                            self.orientation_panel.findRadioButton('Landscape')
        self.portrait_radio = self.orientation_panel.findRadioButton('Portrait')
        # Paper panel
        self.paper_panel = self.main_dialog.findPanel('Paper')
        self.paper_comboboxs = self.paper_panel.findAllComboBoxs(None)
        assert len(self.paper_comboboxs) == self.PAPER_COMBOBOXS_NUM, \
                                "actual number of combobox:%s, expected:%s" % \
                           (len(self.paper_comboboxs), self.PAPER_COMBOBOXS_NUM)
        self.source_combobox = self.paper_panel.findAllComboBoxs(None)[0]
        self.size_combobox = self.paper_panel.findAllComboBoxs(None)[1]
        self.source_label = self.paper_panel.findLabel('Source:')
        self.size_label = self.paper_panel.findLabel('Size:')

        # Size combobox menuitems
        self.size_menu = self.size_combobox.findMenu(None, checkShowing=False)
        # BUG520542: missing vscrollbar
        #self.size_vscrollbar = \
        #             self.size_menu.findScrollBar(None, checkShowing=False)
        self.size_text = self.size_combobox.findText(None)
        # we are not sure the exactly menu_item because of the difference 
        # printer, but we sure the length of size_menu_item > 0
        self.size_menu_items = \
                      self.size_menu.findAllMenuItems(None, checkShowing=False)
        assert len(self.size_menu_items) > 0, \
                          "actual number of size menu item:%s, expected:%s" % \
                       (len(self.size_menu_items), self.SIZE_MENUITEMS_NUM)

        # Source combobox menuitems
        self.source_menu = \
                      self.source_combobox.findMenu(None, checkShowing=False)
        self.source_text = self.source_combobox.findText(None)
        # we are not sure the exactly menu_item because of the difference 
        # printer, but we sure the length of source_menu_item > 0
        self.source_menu_items = \
                     self.source_menu.findAllMenuItems(None, checkShowing=False)
        assert len(self.source_menu_items) > 0, \
                        "actual number of source menu item:%s, expected:%s" % \
                       (len(self.source_menu_items), self.SOURCE_MENUITEMS_NUM)

    def pageSetupDialogAccessiblesAction(self):
        """Check actions for some accessibles in Page Setup Dialog"""
        actionsCheck(self.main_ok_button, "Button")
        actionsCheck(self.main_cancel_button, "Button")
        actionsCheck(self.printer_button, "Button")

        actionsCheck(self.portrait_radio, "RadioButton")
        actionsCheck(self.landscape_radio, "RadioButton")

        actionsCheck(self.size_combobox, "ComboBox")
        actionsCheck(self.source_combobox, "ComboBox")

    def pageSetupDialogAccessiblesStates(self):
        """
        Check default states for accessibles in Page Setup Dialog
        """
        statesCheck(self.main_dialog, "Dialog", 
                                           add_states=["active", "modal"], 
                                           invalid_states=["resizable"])
        
        # this panel can be focused so it should have focusable state
        statesCheck(self.area_panel, "Panel", add_states=["focusable"])

        statesCheck(self.main_ok_button, "Button")
        statesCheck(self.main_cancel_button, "Button")
        statesCheck(self.printer_button, "Button")

        statesCheck(self.margins_panel, "Panel")
        statesCheck(self.bottom_text, "TextBox")
        statesCheck(self.top_text, "TextBox")
        statesCheck(self.left_text, "TextBox")
        statesCheck(self.right_text, "TextBox")
        statesCheck(self.bottom_label, "Label")
        statesCheck(self.top_label, "Label")
        statesCheck(self.left_label, "Label")
        statesCheck(self.right_label, "Label")

        statesCheck(self.orientation_panel, "Panel")
        statesCheck(self.portrait_radio, "RadioButton", add_states=["checked"])
        statesCheck(self.landscape_radio, "RadioButton")

        statesCheck(self.paper_panel, "Panel")
        statesCheck(self.source_combobox, "ComboBox")
        statesCheck(self.size_combobox, "ComboBox")
        statesCheck(self.source_label, "Label")
        statesCheck(self.size_label, "Label")
        statesCheck(self.size_menu, "Menu", 
                                          invalid_states=["visible", "showing"])
        # selection is called from combobox, in this sample menu_item[0] is 
        # selected, so menu as the child of combobox with index0 also have 
        # selected state
        statesCheck(self.source_menu, "Menu", add_states=["selected"], 
                                          invalid_states=["visible", "showing"])

        # Text as descendant of combobox can be selected, so it should have 
        # selectable states. default focus on size_text
        statesCheck(self.size_text, "TextBox", 
                                          add_states=["selectable", "focused"])
        statesCheck(self.source_text, "TextBox", add_states=["selectable"])

    def findConfigrePageDialogAccessibles(self):
        procedurelogger.action('Searching for all widgets in ConfigurePageDialog')
        procedurelogger.expectedResult('All widgets in ConfigurePageDialog should show up')

        # ConfigurePageDialog
        self.configure_dialog = self.app.findDialog('Configure page')
        # Buttons
        self.configure_ok_button = self.configure_dialog.findPushButton('OK')
        self.configure_cancel_button = \
                                self.configure_dialog.findPushButton('Cancel')
        self.network_button = self.configure_dialog.findPushButton('Network...')
        # Printer panel
        self.printer_panel = self.configure_dialog.findPanel('Printer')
        self.printer_labels = self.printer_panel.findAllLabels(None)
        assert len(self.printer_labels) == self.PRINTER_LABELS_NUM, \
                                  "actual number of label:%s, expected:%s" % \
                            (len(self.printer_labels), self.PRINTER_LABELS_NUM)
        self.properties_button = \
                              self.printer_panel.findPushButton('Properties...')
        self.comment_content_label = self.printer_panel.findAllLabels(None)[0]
        self.where_content_label = self.printer_panel.findAllLabels(None)[1]
        self.comment_label = self.printer_panel.findLabel('Comment:')
        self.where_label = self.printer_panel.findLabel('Where:')
        self.type_label = self.printer_panel.findLabel('Type:')
        self.status_content_label = self.printer_panel.findAllLabels(None)[6]
        self.status_label = self.printer_panel.findLabel('Status:')
        self.name_label = self.printer_panel.findLabel('Name:')
        self.name_combobox = self.printer_panel.findComboBox(None)
        self.name_text = self.name_combobox.findText(None)

    def configurePageDialogAccessiblesActions(self):
        """
        Check actions for some accessibles in Configre Page Dialog
        """
        for button in self.configure_dialog.findAllPushButtons(None):
            actionsCheck(button, "Button")

        actionsCheck(self.name_combobox, "ComboBox")

    def configurePageDialogAccessiblesStates(self):
        """
        Check default states for accessibles in Configre Page Dialog
        """
        statesCheck(self.configure_dialog, "Dialog", 
                                           add_states=["active", "modal"], 
                                           invalid_states=["resizable"])
        # default focus on Ok button
        statesCheck(self.configure_ok_button, "Button", add_states=["focused"])
        statesCheck(self.configure_cancel_button, "Button")
        statesCheck(self.network_button, "Button")
        statesCheck(self.printer_panel, "Panel")
        statesCheck(self.properties_button, "Button")
        statesCheck(self.name_combobox, "ComboBox")
        statesCheck(self.name_text, "TextBox", add_states=["selectable"])
        for label in self.printer_labels:
            statesCheck(label, "Label")

    def changeText(self, accessible, expected_text):
        """
        Insert expected_text to accessible to make sure the text is changed
        """
        old_text = accessible.text
        procedurelogger.action('change text %s to %s for %s' % \
                                        (old_text, expected_text, accessible))
        accessible._accessible.queryEditableText().setTextContents(expected_text)
        sleep(config.SHORT_DELAY)

        procedurelogger.expectedResult('new text of %s is %s' % \
                                                 (accessible, expected_text))
        assert accessible.text == expected_text, \
                "actual text:%s, expected:%s" % (accessible.text, expected_text)

    def quit(self):
        self.altF4()
