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

class ProgressBar(TestCase):
    """
    Exercises the ProgressBar control
    """
    @classmethod
    def setup_class(cls):
        cls.app = launchAddress(abspath('assets/ProgressBarTest/ProgressBarTest.html'))
        cls.progressbar = cls.app.slControl.findAllProgressBars('')[0]
        cls.textbox = cls.app.slControl.findAllTexts('')[0]

    @classmethod
    def teardown_class(cls):
        cls.app.kill()

    def test_progressbar_basic(self):
        self.assertEqual('', self.progressbar.name)

        self.assertInterfaces(self.progressbar, [
            'accessible', 'component', 'value',
        ])

        self.assertStates(self.progressbar, [
            'enabled', 'sensitive', 'showing',
            'visible',
        ])

        self.assertEqual(0, self.progressbar.childCount)

    def test_progressbar_value(self):
        value = self.progressbar._accessible.queryValue()

        self.assertEqual(value.minimumValue, 0)
        self.assertEqual(value.maximumValue, 100)
        self.assertEqual(value.minimumIncrement, 0)
        self.assertEqual(value.currentValue, 0)

        self.progressbar.value = 50
        self.assertEqual(self.progressbar.value, 0)

        listener = EventListener()
        with listener.listenTo(self.progressbar):
            self.textbox.text = '50'
            sleep(config.SHORT_DELAY)
            self.assertEqual(self.progressbar.value, 50)

        assert listener.containsEvent(self.progressbar,
                                      'object:property-change:accessible-value',
                                      qty=1)

        with listener.listenTo(self.progressbar):
            self.textbox.text = '100'
            sleep(config.SHORT_DELAY)
            self.assertEqual(self.progressbar.value, 100)

        assert listener.containsEvent(self.progressbar,
                                      'object:property-change:accessible-value',
                                      qty=1)

        with listener.listenTo(self.progressbar):
            self.textbox.text = '1000'
            sleep(config.SHORT_DELAY)
            self.assertEqual(self.progressbar.value, 100)

        assert listener.containsEvent(self.progressbar,
                                      'object:property-change:accessible-value',
                                      qty=0)

    def test_progressbar_setvalue(self):
        listener = EventListener()
        with listener.listenTo(self.progressbar):
            self.progressbar.value = 50
            sleep(config.SHORT_DELAY)
            self.assertNotEqual(self.progressbar.value, 50)

        assert listener.containsEvent(self.progressbar,
                                      'object:property-change:accessible-value',
                                      qty=0)
