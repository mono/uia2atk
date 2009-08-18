##############################################################################
# Written by:  Cachen Chen <cachen@novell.com>
# Date:        02/23/2009
# Description: colordialog.py wrapper script
#              Used by the colordialog-*.py tests
##############################################################################$

import sys
import os
import actions
import states

from strongwind import *
from colordialog import *
from sys import path
from helpers import *
from states import *

# class to represent the main window.
class ColorDialogFrame(accessibles.Frame):

    # constants
    # the available widgets on the window
    BUTTON1 = "ColorDialog Button"
    LABEL = "Examples for: ColorDialog"
    NUMPANELS = 5
    NUMTEXTS = 6
    NUMCOLORBUTTONS = 64

    def __init__(self, accessible):
        super(ColorDialogFrame, self).__init__(accessible)
        self.color_dialog_button = self.findPushButton(self.BUTTON1)
        self.main_label= self.findLabel(self.LABEL)
        self.main_text = self.findText(None)

    def openColorDialog(self):
        '''
        Opens the color dialog by clicking on the color_dialog_button and 
        then finds the dialog accessible for the color dialog
        '''
        self.color_dialog_button.click()
        # A longer wait here, because there are a lot of accessibles that
        # might need time to load
        sleep(config.MEDIUM_DELAY)

    def findAllColorDialogAccessibles(self):
        """
        Use strongwind find* method to search for all widgets in Color dialog. 
        """
        # find the 4 push buttons in the dialog, the "Add To Custom Colors"
        # button isn't showing initially
        procedurelogger.action('Search for all widgets from "Color" Dialog')

        # find the "Color" dialog, first of all
        self.color_dialog = self.app.findDialog("Color")

        self.ok_button = self.color_dialog.findPushButton("OK")
        self.cancel_button = self.color_dialog.findPushButton("Cancel")
        self.def_custom_colors_button = \
                     self.color_dialog.findPushButton("Define Custom Colors >>")
        self.add_color_button = \
                        self.color_dialog.findPushButton("Add To Custom Colors",
                                                        checkShowing=False)

        # There should be five panels.
        self.panels = self.color_dialog.findAllPanels(None, checkShowing=False)
        assert len(self.panels) == 5, \
                        "There should be exactly %s panels, found %s" % \
                        (self.NUMPANELS, len(self.panels))
        self.colors_panel = self.panels[0]
        self.custom_colors_panel = self.panels[1]
        self.color_shader_panel = self.panels[2]
        self.slider_panel = self.panels[3]
        self.color_panel = self.panels[4]

        # There are NUMCOLORBUTTONS SmallColorControls showing as PushButton
        self.small_color_buttons = self.panels[0].findAllPushButtons(None)
        assert len(self.small_color_buttons) == self.NUMCOLORBUTTONS, \
                           "Expected exactly %s buttons, found %s" % \
                           (self.NUMCOLORBUTTONS, len(self.small_color_buttons))

        # Find "Base Colors" and "User Colors" labels from default Color
        # dialog
        self.base_colors_label = self.color_dialog.findLabel("Base Colors:")
        self.user_colors_label = self.color_dialog.findLabel("User Colors:")

        # Find the other labels that show on the custom colors dialog
        self.color_label = \
                        self.color_dialog.findLabel("Color", checkShowing=False)
        self.hue_label = self.color_dialog.findLabel("Hue:", checkShowing=False)
        self.sat_label = self.color_dialog.findLabel("Sat:", checkShowing=False)
        self.bri_label = self.color_dialog.findLabel("Bri:", checkShowing=False)
        self.red_label = self.color_dialog.findLabel("Red:", checkShowing=False)
        self.green_label = \
                       self.color_dialog.findLabel("Green:", checkShowing=False)
        self.blue_label = \
                        self.color_dialog.findLabel("Blue:", checkShowing=False)

        # there are NUMTEXTS text accessibles in the Color dialog
        self.texts = self.color_dialog.findAllTexts(None, checkShowing=False)
        assert len(self.texts) == self.NUMTEXTS, \
                               "Expected exactly %s texts, found %s" % \
                               (self.NUMTEXTS, len(self.texts))
        self.hue_text = self.texts[0]
        self.sat_text = self.texts[1]
        self.bri_text = self.texts[2]
        self.red_text = self.texts[3]
        self.breen_text = self.texts[4]
        self.blue_text = self.texts[5]

    def checkColorDialogStates(self, is_custom_showing):
        '''
        This method checks the states of some predetermined accessibles.  The
        expected states are those states before the "Define Custom Colors"
        button is pressed
        '''

        # Check the states of the "Color" dialog, first of all
        statesCheck(self.color_dialog,
                    "Dialog",
                    add_states=["active", "modal"], 
                    invalid_states=["resizable"])

        # BUG484217 Color panel has extra focusable state
        # statesCheck(self.colors_panel, "Panel")

        # Now check the states of the "Color" dialog's children

        # Check the states of the two showing labels 
        statesCheck(self.base_colors_label, "Label")
        statesCheck(self.user_colors_label, "Label")

        # Checking the states of a few of the SmallColorControl push button
        # accessibles should be good enough
        statesCheck(self.small_color_buttons[0], "Button")
        statesCheck(self.small_color_buttons[1], "Button")
        statesCheck(self.small_color_buttons[-1], "Button")
        statesCheck(self.small_color_buttons[-2], "Button")

        # Check the states of the three buttons that should be showing
        statesCheck(self.cancel_button, "Button")
        # if the "Define Custom Colors" button has been pressed, it becomes
        # insensitive.  The OK button also isn't focused anymore.
        if is_custom_showing:
            statesCheck(self.ok_button, "Button")
            statesCheck(self.def_custom_colors_button,
                        "Button",
                        invalid_states=[FOCUSABLE, ENABLED, SENSITIVE])
        else:
            statesCheck(self.def_custom_colors_button, "Button")
            statesCheck(self.ok_button, "Button", add_states=[FOCUSED])

        # Now check the states of things that may or may not be showing
        # depending on whether the "Define Custom Colors" button was
        # clicked yet.  States may differ depending on the boolean value of
        # is_custom_showing.
        if is_custom_showing:
            invalid_states = []
        else:
            invalid_states = [VISIBLE, SHOWING]

        statesCheck(self.add_color_button,
                    "Button",
                    invalid_states)

        statesCheck(self.custom_colors_panel,
                    "Panel",
                    invalid_states)
        statesCheck(self.color_shader_panel,
                    "Panel",
                    invalid_states)
        statesCheck(self.slider_panel,
                    "Panel",
                    invalid_states)
        statesCheck(self.color_panel,
                    "Panel",
                    invalid_states)

        # Checking a couple of the labels should be fine
        statesCheck(self.color_label,
                    "Label",
                    invalid_states)
        statesCheck(self.blue_label,
                    "Label",
                    invalid_states)

        # Checking a couple of the texts should be fine
        statesCheck(self.hue_text,
                    "Text",
                    invalid_states)
        statesCheck(self.blue_text,
                    "Text",
                    invalid_states)

    def assertSmallColorText(self):
        """
        Check Text for all SmallColorControls under Base Colors and User 
        Colors, Make sure SmallColorControls show None as Text value
        """
        procedurelogger.action("Check the text of SmallColorControls")
        procedurelogger.expectedResult("The text of all SmallColorControls should be blank")

        for small_color_button in self.small_color_buttons:
            actual_text = small_color_button.text
            assert actual_text == "", 'Text was "%s", expected blank ("")' % \
                                      (actual_text)
            
    def selectColorAndAssertLabelChange(self,
                                        small_color_button_index,
                                        expected_text):
        """
        Click a SmallColorControl and then click OK button.  The label text on
        the main application frame should then match the expected_text
        parameter
        """
        procedurelogger.action('Select a color and then make sure the ColorDialog control frame label changes to the correct text')

        # open the ColorDialog and find the accessibles
        self.color_dialog_button.click(log=True)
        sleep(config.MEDIUM_DELAY)
        self.findAllColorDialogAccessibles()

        # click on the SmallColorControl and then click OK
        self.small_color_buttons[small_color_button_index].click(log=True)
        sleep(config.SHORT_DELAY)
        self.ok_button.click(log=True)
        sleep(config.MEDIUM_DELAY)

        #Expected result
        procedurelogger.expectedResult('Label reads "%s"' % expected_text)
        actual_text = self.main_label.text
        assert actual_text == expected_text, \
                                'Label text was "%s", expected "%s"' %  \
                                (actual_text, expected_text)

    # assert the size of an image in the button
    def assertImageSize(self, button, expected_width=-1, expected_height=-1):
        """
        Check Image Size for button
        """
        procedurelogger.action("Assert %s's image size" % button)
        actual_width, actual_height = button.imageSize

        procedurelogger.expectedResult('"%s" image size is %s x %s' % \
                                     (button, expected_width, expected_height))

        assert actual_width == expected_width, "%s (%s), %s (%s)" %\
                                            ("expected width",
                                              expected_width,
                                             "does not match actual width",
                                              actual_width)
        assert actual_width == expected_width, "%s (%s), %s (%s)" %\
                                            ("expected height",
                                              expected_height,
                                             "does not match actual height",
                                              actual_height)

    def assertComponentSize(self, accessible, expected_width, expected_height):
        """
        Ensure that the accessible has the size we expect
        """
        procedurelogger.action('Ensure that the %s has the size we expect' % \
                               accessible)
        procedurelogger.expectedResult('Size is %sx%s' % \
                                             (expected_width, expected_height))
        actual_width, actual_height = \
                              accessible._accessible.queryComponent().getSize()
        assert actual_width == expected_width, \
                "Width was %s, expected %s" % (actual_width, expected_width)
        assert actual_height == expected_height, \
                "Height was %s, expected %s" % (actual_height, expected_height)

    # close application main window after running test
    def quit(self):
        self.altF4()
