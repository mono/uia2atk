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

from os.path import abspath
import pyatspi

class RangeValueSlider(TestCase):
    """
    Exercises the basic functionality of the RangeValue class using a Slider
    control
    """
    @classmethod
    def setup_class(cls):
        cls.app = launchAddress(abspath('assets/RangeValueTest/RangeValueTest.html'))
        cls.slider = cls.app.slControl.findAllSliders('')[0]
        cls.slider_value = cls.slider._accessible.queryValue()

    @classmethod
    def teardown_class(cls):
        cls.app.kill()

    def test_minimum_value(self):
        self.assertEqual(self.slider_value.minimumValue, 0)

    def test_maximum_value(self):
        self.assertEqual(self.slider_value.maximumValue, 10)

    def test_minimum_increment(self):
        self.assertEqual(self.slider_value.minimumIncrement, 0)

    def test_current_value(self):
        self.slider_value.currentValue = 0
        self.assertEqual(self.slider_value.currentValue, 0)

        self.slider_value.currentValue = 1
        self.assertEqual(self.slider_value.currentValue, 1)

        self.slider_value.currentValue = 5
        self.assertEqual(self.slider_value.currentValue, 5)

        self.slider_value.currentValue = -1
        self.assertEqual(self.slider_value.currentValue, 5)

        self.slider_value.currentValue = 10
        self.assertEqual(self.slider_value.currentValue, 10)

        self.slider_value.currentValue = 100
        self.assertEqual(self.slider_value.currentValue, 10)

    def test_accessible_value_event(self):
        self.slider_value.currentValue = 0
        self.assertEqual(self.slider_value.currentValue, 0)

        listener = EventListener()
        with listener.listenTo(self.slider):
            self.slider_value.currentValue = 5
            self.assertEqual(self.slider_value.currentValue, 5)

        assert listener.containsEvent(self.slider,
                                      'object:property-change:accessible-value',
                                      qty=1)
