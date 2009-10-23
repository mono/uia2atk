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
        cls.addAFooButton = self.app.slControl.findPushButton('Add A Foo')

    @classmethod
    def teardown_class(cls):
        cls.app.kill()

    def test_visible_data_changed_event(self):
        t = self.textbox_text
        self.assertEqual(t.getText(0, -1), 'Foo');

        listener = EventListener()
        with listener.listenTo(self.textbox):
            addAFooButton.click()

            sleep(config.MEDIUM_DELAY)
            self.assertEqual(t.getText(0, -1), 'FooFoo\n')

        assert listener.containsEvent(self.textbox,
                                      'object:visible-data-changed',
                                      qty=1)
