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

class ValueEvents(TestCase):
    """
    Exercises the events emitted by the Value class using a TextBox control
    """
    @classmethod
    def setup_class(cls):
        cls.app = launchAddress(abspath('assets/ValueEventsTest/ValueEventsTest.html'))
        cls.textbox = cls.app.slControl.findText('Foo')
        cls.textbox_text = cls.textbox._accessible.queryText()
        cls.textbox_edit = cls.textbox._accessible.queryEditableText()
        cls.addAFooButton = cls.app.slControl.findPushButton('Add A Foo')

    @classmethod
    def teardown_class(cls):
        cls.app.kill()

    def test_text_changed_event(self):
        t = self.textbox_text
        self.assertEqual('Foo', t.getText(0, -1))

        listener = EventListener()
        with listener.listenTo(self.textbox):
            self.addAFooButton.click()

            self.assertEqual('FooFoo\n', t.getText(0, -1))

        assert listener.containsEvent(self.textbox,
                                      'object:text-changed:insert',
                                      qty=1)

        assert listener.containsEvent(self.textbox,
                                      'object:property-change:accessible-name',
                                      qty=1)

        with listener.listenTo(self.textbox):
            self.textbox_edit.insertText(3, 'Bar', 3)

            self.assertEqual('FooBarFoo\n', t.getText(0, -1))

        assert listener.containsEvent(self.textbox,
                                      'object:text-changed:insert',
                                      qty=1)

        assert listener.containsEvent(self.textbox,
                                      'object:property-change:accessible-name',
                                      qty=1)

        with listener.listenTo(self.textbox):
            self.textbox_edit.deleteText(0, 3)

            self.assertEqual(t.getText(0, -1), 'BarFoo\n')

        assert listener.containsEvent(self.textbox,
                                      'object:text-changed:delete',
                                      qty=1)

        assert listener.containsEvent(self.textbox,
                                      'object:property-change:accessible-name',
                                      qty=1)
