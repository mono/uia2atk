
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

# class to represent the main window.
class ColorDialogFrame(accessibles.Frame):

    # constants
    # the available widgets on the window
    BUTTON1 = "ColorDialog Button"
    LABEL = "Examples for: ColorDialog"

    def __init__(self, accessible):
        super(ColorDialogFrame, self).__init__(accessible)
        self.colordialog_button = self.findPushButton(self.BUTTON1)
        self.main_label= self.findLabel(self.LABEL)
        self.main_text = self.findText(None)

    def click(self, button):
        """
        wrap strongwind click action to be used by click button to invoke 
        color dialog

        """

        button.click()

        # click button will invoke dialog page
        self.colordialog = self.app.findDialog("Color")

    def AssertWidgets(self):
        """
        Use strongwind Find* method to search for all widgets in Color Dialog, 
        Make sure all widgets are showing

        """
        procedurelogger.action("search for all widgets from ColorDialog windows")

        # there are 4 push buttons in dialog
        procedurelogger.expectedResult("4 PushButtons are showing up")
        self.ok_button = self.colordialog.findPushButton("OK")
        self.cancel_button = self.colordialog.findPushButton("Cancel")
        self.definecolor_button = self.colordialog.findPushButton("Define Custom Colors >>")
        self.addcolor_button = self.colordialog.findPushButton("Add To Custom Colors", checkShowing=False)
        # click definecolor_button
        self.click(self.definecolor_button)
        sleep(config.SHORT_DELAY)
        # there are 1 panel for SmallColorControls and 4 panels for custom colors
        procedurelogger.expectedResult("5 Panels are showing up")      
        self.panels = self.colordialog.findAllPanels(None)
        assert len(self.panels) == 5, "%s not match 4" % len(self.panels)
        # there are 64 SmallColorControls showing as PushButton
        procedurelogger.expectedResult("64 SmallColorControls are showing up as PushButton")
        self.smallcolor_buttons = self.panels[0].findAllPushButtons(None)
        assert len(self.smallcolor_buttons) == 64, "%s not match 64" % len(self.smallcolor_buttons)
        # there are 9 Labels in dialog
        procedurelogger.expectedResult("9 Labels are showing up")
        self.base_label = self.colordialog.findLabel("Base Colors:")
        self.user_label = self.colordialog.findLabel("User Colors:")
        self.color_label = self.colordialog.findLabel("Color")
        self.hue_label = self.colordialog.findLabel("Hue:")
        self.sat_label = self.colordialog.findLabel("Sat:")
        self.bri_label = self.colordialog.findLabel("Bri:")
        self.Red_label = self.colordialog.findLabel("Red:")
        self.Green_label = self.colordialog.findLabel("Green:")
        self.Blue_label = self.colordialog.findLabel("Blue:")
        # there are 6 TextBoxs in dialog
        procedurelogger.expectedResult("6 TextBoxs are showing up")
        self.texts = self.colordialog.findAllTexts(None)
        self.hue_text = self.texts[0]
        self.sat_text = self.texts[1]
        self.bri_text = self.texts[2]
        self.red_text = self.texts[3]
        self.breen_text = self.texts[4]
        self.blue_text = self.texts[5]

    def assertSmallColorText(self):
        """
        Check Text for all SmallColorControls under Base Colors and User 
        Colors, Make sure SmallColorControls show None as Text value

        """
        procedurelogger.action("test the text of SmallColorControls")

        for i in range(64):
            procedurelogger.expectedResult("the text of %s are None" % self.smallcolor_buttons[i])
            assert self.smallcolor_buttons[i].text == "", "%s not match None" \
                                         % self.smallcolor_buttons[i].text

    def assertSmallColorName(self, accessible=None, colorname=None):
        """
        Check Name for all SmallColorControls under Base Colors and User 
        Colors, Make sure SmallColorControls show its color name as its Name 

        """
        procedurelogger.action("test the name of SmallColorControls")

        procedurelogger.expectedResult("the name of SmallColorControl are not None")
        for i in range(64):
            names = self.smallcolor_buttons[i].name
            assert names != "", "name is None"

        if colorname != None:
            procedurelogger.expectedResult("the name of SmallColorControl is %s" % colorname)
            assert accessible.name == colorname, "%s is not match %s" % \
                                                  (accessible.name, colorname)

    def clickSmallColorToChangeLabel(self, smallcolorbutton):
        """
        Click SmallColorControl and click OK button to select one color, Label 
        will shows which color to be selected

        """
        #Action
        smallcolorbutton.click()
        sleep(config.SHORT_DELAY)
        self.ok_button.click()
        sleep(config.SHORT_DELAY)

        #Expected result
        procedurelogger.expectedResult('lable shows "%s" being selected' % smallcolorbutton.name)

        assert self.main_label.text == "Color [%s]" % smallcolorbutton.name, \
                                         "%s not match %s" % (self.main_label.text, smallcolorbutton.name)

    # assert the size of an image in the button
    def assertImageSize(self, button, width=-1, height=-1):
        """
        Check Image Size for buttons

        """
        procedurelogger.action("assert %s's image size" % button)
        size = button.imageSize

        procedurelogger.expectedResult('"%s" image size is %s x %s' %
                                                  (button, width, height))

        assert width == size[0], "%s (%s), %s (%s)" %\
                                            ("expected width",
                                              width,
                                             "does not match actual width",
                                              size[0])
        assert height == size[1], "%s (%s), %s (%s)" %\
                                            ("expected height",
                                              height,
                                             "does not match actual height",
                                              size[1]) 



    # close application main window after running test
    def quit(self):
        self.altF4()
