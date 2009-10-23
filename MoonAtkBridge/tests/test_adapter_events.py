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
# Copyright (c) 2009 Novell, Inc. (http://www.novell.com)
#
# Authors:
#      Brad Taylor <brad@getcoded.net>
#

from moonlight import *
from strongwind import *
from strongwind.events import EventListener

import pyatspi
from time import sleep
from os.path import abspath

class AdapterEvents(TestCase):
    """
    Exercises the events emitted the Adapter class
    """
    @classmethod
    def setup_class(cls):
        cls.app = launchAddress(abspath('assets/AdapterEventsTest/AdapterEventsTest.html'))
        cls.textbox = cls.app.slControl.findText('Hello World!')
        cls.textbox_text = cls.textbox._accessible.queryText()

    @classmethod
    def teardown_class(cls):
        cls.app.kill()

    def test_aaa_focus_events(self):
        """
        Verify that focus events are properly raised.
        """
        listener = EventListener()
        size_button = self.app.slControl.findPushButton('Toggle Size')

        # TODO: grabFocus() doesn't currently grab app-wide focus, so we have
        # to mouseClick also.
        self.textbox.mouseClick()
        self.textbox.grabFocus()

        sleep(config.MEDIUM_DELAY)

        self.assertTrue(self.textbox.focused)
        self.assertFalse(size_button.focused)

        # test gain and lost focus
        with listener.listenTo([self.textbox, size_button]):
            sleep(config.LONG_DELAY)

            size_button.grabFocus()

            sleep(config.LONG_DELAY)
            self.assertFalse(self.textbox.focused)
            self.assertTrue(size_button.focused)

        assert listener.containsEvent(self.textbox,
                                      'object:state-changed:focused',
                                      qty=1)
        assert listener.containsEvent(size_button,
                                      'object:state-changed:focused',
                                      qty=1)
        assert listener.containsEvent(size_button, 'focus:', qty=1)

    def test_accessible_name_event(self):
        """
        Verify that accessible-name events are being properly raised.
        """
        def backspace():
            procedurelogger.action('Press Backspace.', self.textbox)
            self.textbox.grabFocus()
            pyatspi.Registry.generateKeyboardEvent(0xFF08, None, pyatspi.KEY_SYM)

        listener = EventListener()

        self.assertEqual(self.textbox.name, 'Hello World!')

        # TODO: grabFocus() doesn't currently grab app-wide focus, so we have
        # to mouseClick also.
        self.textbox.grabFocus()
        self.textbox.mouseClick()

        assert self.textbox.focused

        with listener.listenTo(self.textbox):
            self.textbox.typeText('1')

            sleep(config.SHORT_DELAY)
            self.assertEqual(self.textbox.name, 'Hello World!1')

        assert listener.containsEvent(self.textbox,
                                      'object:property-change:accessible-name',
                                      qty=1)

        with listener.listenTo(self.textbox):
            self.textbox.typeText('23')
            sleep(config.MEDIUM_DELAY)
            self.assertEqual(self.textbox.name, 'Hello World!123')

            backspace()
            sleep(config.MEDIUM_DELAY)
            self.assertEqual(self.textbox.name, 'Hello World!12')

            backspace()
            sleep(config.MEDIUM_DELAY)
            self.assertEqual(self.textbox.name, 'Hello World!1')

            backspace()
            sleep(config.MEDIUM_DELAY)
            self.assertEqual(self.textbox.name, 'Hello World!')

        assert listener.containsEvent(self.textbox,
                                      'object:property-change:accessible-name',
                                      qty=5)

    def test_bounds_changed_event(self):
        """
        Verify that the bounds_changed event is being properly raised.
        """
        def height(control):
            c = control._accessible.queryComponent()
            return c.getExtents(pyatspi.WINDOW_COORDS).height

        size_button = self.app.slControl.findPushButton('Toggle Size')

        self.assertEqual(height(self.textbox), 100)

        listener = EventListener()
        with listener.listenTo(self.textbox):
            size_button.click()

            sleep(config.MEDIUM_DELAY)
            self.assertEqual(height(self.textbox), 200)

        assert listener.containsEvent(self.textbox,
                                      'object:bounds-changed',
                                      qty=1)

        with listener.listenTo(self.textbox):
            size_button.click()

            sleep(config.MEDIUM_DELAY)
            self.assertEqual(height(self.textbox), 100)

        assert listener.containsEvent(self.textbox,
                                      'object:bounds-changed',
                                      qty=1)

    def test_sensitivity_event(self):
        """
        Verify that the state-changed:sensitive event is being properly raised.
        """
        sensitivity_button = self.app.slControl.findPushButton('Toggle Sensitivity')

        self.assertTrue(self.textbox.sensitive)

        listener = EventListener()
        with listener.listenTo(self.textbox):
            sensitivity_button.click()

            sleep(config.SHORT_DELAY)
            self.assertFalse(self.textbox.sensitive)

        assert listener.containsEvent(self.textbox,
                                      'object:state-changed:sensitive',
                                      qty=1)

        with listener.listenTo(self.textbox):
            sensitivity_button.click ()

            sleep(config.SHORT_DELAY)
            self.assertTrue(self.textbox.sensitive)

        assert listener.containsEvent(self.textbox,
                                      'object:state-changed:sensitive',
                                      qty=1)
