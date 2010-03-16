#!/usr/bin/env python

# Permission is hereby granted, free of charge, to any person obtaining
# a copy of this software and associated documentation files (the
# "Software"), to deal in the Software without restriction, including
# without limitation the rights to use, copy, modify, merge, publish,
# distribute, sublicense, and/or sell copies of the Software, and to
# permit persons to whom the Software is furnished to do so, subject to
# the following conditions:
#
# The above copyright notice and this permission notice shall be
# included in all copies or substantial portions of the Software.
#
# THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND,
# EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF
# MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND
# NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE
# LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION
# OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION
# WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.
#
# Copyright (c) 2010 Novell, Inc. (http://www.novell.com)
#
# Authors:
#      Mario Carrion <mcarrion@novell.com>
#

from moonlight import *
from strongwind import *

from os.path import abspath
import pyatspi

class DatePicker(TestCase):
    """
    Exercises DatePicker control. DatePicker control implements:
    - UIA.ExpandCollapse pattern, mapped to Action, and
    - UIA.Value pattern, mapped to Value
    """

    @classmethod
    def setup_class(cls):
        cls.app = launchAddress(abspath('assets/DatePickerTest/DatePickerTest.html'))
        cls.datepicker = cls.app.slControl.findAllComboBoxes('')[0]

        cls.action = cls.datepicker._accessible.queryAction()
        cls.ACTIONS_COUNT = 1

        cls.text = cls.datepicker._accessible.queryEditableText()

    @classmethod
    def teardown_class(cls):
        cls.app.kill()

    def test_calendar_basic(self):
        self.assertEqual('', self.datepicker.name)

        self.assertInterfaces(self.datepicker, [
            'accessible', 'component', 'editabletext', 'action', 'text'
        ])

        self.assertStates(self.datepicker, [
            'editable', 'enabled', 'expandable', 'sensitive',
            'showing',  'visible'
        ])

        # Children
        self.assertEqual(3, self.datepicker.childCount)

        # Text
        text = self.datepicker.getChildAtIndex(0)
        self.assertEqual(pyatspi.ROLE_TEXT, text.role)
        self.assertEqual('', text.name)
        self.assertEqual(1, text.childCount)
        self.assertStates(text, [
            'enabled', 'editable', 'focusable', 'sensitive',
            'showing', 'visible'
        ])

        # Button
        button = self.datepicker.getChildAtIndex(1)
        self.assertEqual(pyatspi.ROLE_PUSH_BUTTON, button.role)
        self.assertEqual('Show Calendar', button.name)
        self.assertEqual(0, button.childCount)
        self.assertStates(button, [
            'enabled', 'sensitive', 'showing', 'visible'
        ])

        # Popup
        popup  = self.datepicker.getChildAtIndex(2)
        self.assertEqual(pyatspi.ROLE_FILLER, popup.role)
        self.assertEqual('', popup.name)
        self.assertEqual(1, popup.childCount)
        self.assertStates(popup, [
            'enabled', 'sensitive'
        ])

    # Action Tests

    # NOTE: we can't test 'object:visible-data-changed' because the event is
    # not raised by DatePickerAutomationPeer when the Popup appears/disappears.

    def test_n_actions(self):
        self.assertEqual(self.action.nActions, self.ACTIONS_COUNT)

        for x in xrange(0, self.ACTIONS_COUNT):
            self.assertTrue(self.action.doAction(x)) # Expand

        # It takes a while to show the popup.
        sleep(config.MEDIUM_DELAY)

        # Popup
        popup  = self.datepicker.getChildAtIndex(2)
        self.assertEqual(pyatspi.ROLE_FILLER, popup.role)
        self.assertEqual('', popup.name)
        self.assertEqual(1, popup.childCount)
        self.assertStates(popup, [
            'enabled', 'sensitive', 'showing', 'visible'
        ])

        for x in xrange(0, self.ACTIONS_COUNT):
            self.assertTrue(self.action.doAction(x)) # Collapse

        # It takes a while to hide the popup.
        sleep(config.MEDIUM_DELAY)

        self.assertStates(popup, [
            'enabled', 'sensitive'
        ])

        self.assertFalse(self.action.doAction(self.ACTIONS_COUNT))

    def test_get_description(self):
        # By default everything is empty
        self.assertEqual(self.action.getDescription(0), '')
        self.assertEqual(self.action.getDescription(1), '') # Doesn't exist

    def test_get_name(self):
        self.assertEqual(self.action.getName(0), 'expand or collapse')
        self.assertEqual(self.action.getName(1), '') # Doesn't exist

    # Value Tests

    def test_value(self):
        date = '1/1/2010 2:00:00 AM'
        self.assertEqual('', self.text.getText(0,-1))
        self.text.insertText(0, date, len(date))
        self.assertEqual(date, self.text.getText(0,-1))

        text = self.datepicker.getChildAtIndex(0)
        self.assertEqual(pyatspi.ROLE_TEXT, text.role)
        self.assertEqual(date, text.name)

