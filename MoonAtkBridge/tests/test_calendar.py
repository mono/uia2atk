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

class Calendar(TestCase):
    """
    Exercises Calendar control. Calendar control implements:
    - UIA.GridProvider pattern, not mapped
    - UIA.TableProvider pattern, not mapped
    - UIA.MultipleViewProvider pattern, not mapped
    - UIA.Selection pattern, mapped to Selection
    """

    @classmethod
    def setup_class(cls):
        cls.app = launchAddress(abspath('assets/CalendarTest/CalendarTest.html'))

	cls.calendar = cls.app.slControl.findAllCalendars('')[0]
        cls.selection = cls.calendar._accessible.querySelection()
	cls.button1S = cls.app.slControl.findPushButton('Button1S');
	cls.button1U = cls.app.slControl.findPushButton('Button1U');
	cls.button2S = cls.app.slControl.findPushButton('Button2S');
	cls.button2U = cls.app.slControl.findPushButton('Button2U');
	cls.button3S = cls.app.slControl.findPushButton('Button3S');
	cls.button3U = cls.app.slControl.findPushButton('Button3U');
	cls.button4 = cls.app.slControl.findPushButton('Button4');

	# Find the children we will use for selecting and unselecting
	# First we have 3 buttons (prev, month, next)
	# Then, we have 7 labels (week days)
	# Next, 5 March buttons
	# Finally, all the day buttons
	cls.button_3 = cls.calendar.getChildAtIndex(17)
	cls.button_15 = cls.calendar.getChildAtIndex(29)
	cls.button_22 = cls.calendar.getChildAtIndex(36)
	cls.button_23 = cls.calendar.getChildAtIndex(37)

    @classmethod
    def teardown_class(cls):
        cls.app.kill()

    # Basic tests

    def test_calendar_basic(self):
        self.assertEqual('', self.calendar.name)

        self.assertInterfaces(self.calendar, [
            'accessible', 'component', 'selection', 'table'
        ])

        self.assertStates(self.calendar, [
            'enabled', 'sensitive', 'showing', 'visible',
        ])

        # 3 Buttons (prev, month, next)
	# 7 Labels (week days)
	# 42 Days
	# 12 Months
	self.assertEqual(64, self.calendar.childCount)

	# 3 buttons
	name = [ 'previous button', 'April, 1983', 'next button' ]
        for i in xrange(0, 3):
            button = self.calendar.getChildAtIndex(i)
            self.assertEqual(pyatspi.ROLE_PUSH_BUTTON, button.role)
            self.assertEqual(name[i], button.name)
            self.assertEqual(0, button.childCount)

	# 7 week days
	name = [ 'Su', 'Mo', 'Tu', 'We', 'Th', 'Fr', 'Sa' ]
        for i in xrange(3, 10):
            label = self.calendar.getChildAtIndex(i)
            self.assertEqual(pyatspi.ROLE_LABEL, label.role)
            self.assertEqual(name[i-3], label.name)
            self.assertEqual(0, label.childCount)

	# 43 Days
	# March: 27, 28, 29, 30, 31
	day = 27
	for i in xrange(10, 15):
            button = self.calendar.getChildAtIndex(i)
            self.assertEqual(pyatspi.ROLE_PUSH_BUTTON, button.role)
            self.assertEqual(str(day), button.name)
	    day = day + 1
            self.assertEqual(1, button.childCount)
	# April: 1-30
	day = 1
	for i in xrange(15, 45):
            button = self.calendar.getChildAtIndex(i)
            self.assertEqual(pyatspi.ROLE_PUSH_BUTTON, button.role)
            self.assertEqual(str(day), button.name)
	    day = day + 1
            self.assertEqual(1, button.childCount)
	# May: 1-7
	day = 1
	for i in xrange(45, 52):
            button = self.calendar.getChildAtIndex(i)
            self.assertEqual(pyatspi.ROLE_PUSH_BUTTON, button.role)
            self.assertEqual(str(day), button.name)
	    day = day + 1
            self.assertEqual(1, button.childCount)


	# Years
	for i in xrange(52, 64):
            button = self.calendar.getChildAtIndex(i)
            self.assertEqual(pyatspi.ROLE_PUSH_BUTTON, button.role)
	    self.assertEqual('Jan', button.name)
            self.assertEqual(0, button.childCount)

    # Selection Tests

    def test_selection(self):
        # By default everything is unselected
        for x in xrange(0, self.calendar.childCount):
            item = self.calendar.getChildAtIndex(x)
            self.assertFalse(item.selected)
            self.assertFalse(self.selection.isChildSelected(x))

        self.assertTrue(self.selection.selectChild(17)) # button 3 is at index 17
	self.assertTrue(self.button_3.selected)
	self.assertTrue(self.selection.isChildSelected(17))
	self.assertEqual('4/3/1983 12:00:00 AM', self.calendar.name)

        actionU = self.button1U._accessible.queryAction()
        self.assertTrue(actionU.doAction(0))

	self.assertFalse(self.button_3.selected)
	self.assertFalse(self.selection.isChildSelected(17))
	self.assertEqual('', self.calendar.name)

    def test_selection_events(self):
        listener = EventListener()

	# Selecting
	self.assertFalse(self.button_3.selected)
	self.assertFalse(self.selection.isChildSelected(17))
        self.assertEqual('', self.calendar.name)

        with listener.listenTo(self.button_3):
            action1S = self.button1S._accessible.queryAction()
            self.assertTrue(action1S.doAction(0))
        assert listener.containsEvent(self.button_3, 'object:state-changed:selected', 1)
	self.assertTrue(self.button_3.selected)
	self.assertTrue(self.selection.isChildSelected(17))
        self.assertEqual('4/3/1983 12:00:00 AM', self.calendar.name)

	self.assertFalse(self.button_15.selected)
	self.assertFalse(self.selection.isChildSelected(29))

        with listener.listenTo(self.button_15):
            action2S = self.button2S._accessible.queryAction()
            self.assertTrue(action2S.doAction(0))
        assert listener.containsEvent(self.button_15, 'object:state-changed:selected', 1)
	self.assertTrue(self.button_15.selected)
	self.assertTrue(self.selection.isChildSelected(29))
	# When multiselection is enabled it will show the first added date
        self.assertEqual('4/3/1983 12:00:00 AM', self.calendar.name)

	self.assertFalse(self.button_22.selected)
	self.assertFalse(self.selection.isChildSelected(36))

        with listener.listenTo(self.button_22):
            action3S = self.button3S._accessible.queryAction()
            self.assertTrue(action3S.doAction(0))
        assert listener.containsEvent(self.button_22, 'object:state-changed:selected', 1)
	self.assertTrue(self.button_22.selected)
	self.assertTrue(self.selection.isChildSelected(36))
        self.assertEqual('4/3/1983 12:00:00 AM', self.calendar.name)

        # Unselecting

        with listener.listenTo(self.button_3):
            action1U = self.button1U._accessible.queryAction()
            self.assertTrue(action1U.doAction(0))
        assert listener.containsEvent(self.button_3, 'object:state-changed:selected', 1)
	self.assertFalse(self.button_3.selected)
	self.assertFalse(self.selection.isChildSelected(17))
        self.assertEqual('4/15/1983 12:00:00 AM', self.calendar.name)

        with listener.listenTo([self.button_15, self.button_22]):
            action2U = self.button2U._accessible.queryAction()
            self.assertTrue(action2U.doAction(0))
	self.assertFalse(self.button_15.selected)
	self.assertFalse(self.selection.isChildSelected(29))
	# No event raised by 22, because it was selected already
        assert listener.containsEvent(self.button_15, 'object:state-changed:selected', 1)
	self.assertTrue(self.button_22.selected)
	self.assertTrue(self.selection.isChildSelected(36))
        self.assertEqual('4/22/1983 12:00:00 AM', self.calendar.name)

	# Select all items again
	action1S = self.button1S._accessible.queryAction()
	self.assertTrue(action1S.doAction(0))
	self.assertTrue(self.button_3.selected)
	self.assertTrue(self.selection.isChildSelected(17))
        self.assertEqual('4/22/1983 12:00:00 AM', self.calendar.name)

	action2S = self.button2S._accessible.queryAction()
	self.assertTrue(action2S.doAction(0))
	self.assertTrue(self.button_15.selected)
	self.assertTrue(self.selection.isChildSelected(29))
        self.assertEqual('4/22/1983 12:00:00 AM', self.calendar.name)

	self.assertTrue(self.button_22.selected)
	self.assertTrue(self.selection.isChildSelected(36))
        self.assertEqual('4/22/1983 12:00:00 AM', self.calendar.name)

        with listener.listenTo([self.button_3, self.button_15, self.button_22, self.button_23]):
	    action4 = self.button4._accessible.queryAction()
	    self.assertTrue(action4.doAction(0))
        assert listener.containsEvent(self.button_3,  'object:state-changed:selected', 1)
	self.assertFalse(self.button_3.selected)
	self.assertFalse(self.selection.isChildSelected(17))

        assert listener.containsEvent(self.button_15, 'object:state-changed:selected', 1)
	self.assertFalse(self.button_15.selected)
	self.assertFalse(self.selection.isChildSelected(29))

        assert listener.containsEvent(self.button_22, 'object:state-changed:selected', 1)
	self.assertFalse(self.button_22.selected)
	self.assertFalse(self.selection.isChildSelected(36))

        assert listener.containsEvent(self.button_23, 'object:state-changed:selected', 1)
	self.assertTrue(self.button_23.selected)
	self.assertTrue(self.selection.isChildSelected(37))

        self.assertEqual('4/23/1983 12:00:00 AM', self.calendar.name)


