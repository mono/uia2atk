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

class ExpandCollapseEvents(TestCase):
    """
    Exercises the events emitted by the ExpandCollapseInvoke class using a
    ComboBox.
    """
    @classmethod
    def setup_class(cls):
        cls.app = launchAddress(abspath('assets/ExpandCollapseEventsTest/ExpandCollapseEventsTest.html'))
        cls.combobox = cls.app.slControl.findAllComboBoxes('')[0]
        cls.combobox_action = cls.combobox._accessible.queryAction()

    @classmethod
    def teardown_class(cls):
        cls.app.kill()

    def test_visible_data_changed_event(self):
        self.assertTrue(self.combobox.expandable)
        self.assertFalse(self.combobox.expanded)

        self.assertEqual(self.combobox_action.nActions, 1)
        self.assertEqual(self.combobox_action.getName(0), 'expand or collapse')

        listener = EventListener()
        with listener.listenTo(self.combobox):
            self.combobox_action.doAction(0)
            self.assertTrue(self.combobox.expanded)

        assert listener.containsEvent(self.combobox,
                                      'object:visible-data-changed',
                                      qty=1)

        assert listener.containsEvent(self.combobox,
                                      'object:state-changed:expanded',
                                      qty=1)

        with listener.listenTo(self.combobox):
            self.combobox_action.doAction(0)
            self.assertFalse(self.combobox.expanded)

        assert listener.containsEvent(self.combobox,
                                      'object:visible-data-changed',
                                      qty=1)

        assert listener.containsEvent(self.combobox,
                                      'object:state-changed:expanded',
                                      qty=1)
