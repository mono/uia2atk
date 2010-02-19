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

class Slider(TestCase):
    """
    Exercises the Slider control
    """
    @classmethod
    def setup_class(cls):
        cls.app = launchAddress(abspath('assets/SliderTest/SliderTest.html'))
        cls.label = cls.app.slControl.findLabel('0')

        cls.slider = cls.app.slControl.findSlider('')
        cls.slider_value = cls.slider._accessible.queryValue()

    @classmethod
    def teardown_class(cls):
        cls.app.kill()

    def test_slider_basic(self):
        self.assertEqual('', self.slider.name)

        self.assertInterfaces(self.slider, [
            'accessible', 'component', 'value',
        ])

        self.assertStates(self.slider, [
            'enabled', 'sensitive', 'showing', 'visible',
        ])

        self.assertEqual(3, self.slider.childCount)

        for i in xrange(0, 2):
            child = self.slider.getChildAtIndex(i)
            self.assertEqual(pyatspi.ROLE_PUSH_BUTTON, child.role)
            self.assertEqual('', child.name)
            self.assertEqual(0, child.childCount)

        large_decrease = self.slider.getChildAtIndex(0)
        thumb = self.slider.getChildAtIndex(1)
        large_increase = self.slider.getChildAtIndex(2)

        for button in (large_increase, large_decrease):
            self.assertInterfaces(button, [
                'accessible', 'action', 'component',
            ])
            self.assertStates(button, [
                'enabled', 'sensitive', 'showing', 'visible',
            ])

        self.assertInterfaces(thumb, [
            'accessible', 'component',
        ])
        self.assertStates(thumb, [
            'enabled', 'focusable', 'sensitive', 'showing', 'visible',
        ])

    def test_minimum_value(self):
        self.assertEqual(self.slider_value.minimumValue, 0)

    def test_maximum_value(self):
        self.assertEqual(self.slider_value.maximumValue, 100)

    def test_minimum_increment(self):
        self.assertEqual(self.slider_value.minimumIncrement, 0)

    def test_current_value(self):
        self.slider_value.currentValue = 0
        self.assertEqual(self.slider_value.currentValue, 0)
        self.assertEqual(self.label.name, '0')

        listener = EventListener()
        with listener.listenTo(self.slider):
            self.slider_value.currentValue = 100
            self.assertEqual(self.label.name, '100')
            self.assertEqual(self.slider_value.currentValue, 100)

        assert listener.containsEvent(self.slider,
                                      'object:property-change:accessible-value',
                                      qty=1)

        with listener.listenTo(self.slider):
            self.slider_value.currentValue = 100
            self.assertEqual(self.label.name, '100')
            self.assertEqual(self.slider_value.currentValue, 100)

        assert listener.containsEvent(self.slider,
                                      'object:property-change:accessible-value',
                                      qty=0)


        with listener.listenTo(self.slider):
            self.slider_value.currentValue = 10000
            self.assertEqual(self.label.name, '100')
            self.assertEqual(self.slider_value.currentValue, 100)

        assert listener.containsEvent(self.slider,
                                      'object:property-change:accessible-value',
                                      qty=0)

        with listener.listenTo(self.slider):
            self.slider_value.currentValue = 0
            self.assertEqual(self.slider_value.currentValue, 0)
            self.assertEqual(self.label.name, '0')

        assert listener.containsEvent(self.slider,
                                      'object:property-change:accessible-value',
                                      qty=1)

    def test_slider_buttons(self):
        self.slider_value.currentValue = 0
        self.assertEqual(self.slider_value.currentValue, 0)
        self.assertEqual(self.label.name, '0')

        large_decrease = self.slider.getChildAtIndex(0)
        large_increase = self.slider.getChildAtIndex(2)

        listener = EventListener()
        for value in (25, 50, 75, 100):
            with listener.listenTo(self.slider):
                    large_increase.click()
                    self.assertEqual(self.label.name, str(value))
                    self.assertEqual(self.slider_value.currentValue, value)

            assert listener.containsEvent(self.slider,
                                          'object:property-change:accessible-value',
                                          qty=1)

        with listener.listenTo(self.slider):
                large_increase.click()
                self.assertEqual(self.label.name, '100')
                self.assertEqual(self.slider_value.currentValue, 100)

        assert listener.containsEvent(self.slider,
                                      'object:property-change:accessible-value',
                                      qty=0)

        for i in (75, 50, 25, 0):
            with listener.listenTo(self.slider):
                    large_decrease.click()

                    sleep(config.MEDIUM_DELAY)

                    self.assertEqual(self.label.name, str(i))
                    self.assertEqual(self.slider_value.currentValue, i)

            assert listener.containsEvent(self.slider,
                                          'object:property-change:accessible-value',
                                          qty=1)

        with listener.listenTo(self.slider):
                large_decrease.click()
                self.assertEqual(self.label.name, '0')
                self.assertEqual(self.slider_value.currentValue, 0)

        assert listener.containsEvent(self.slider,
                                      'object:property-change:accessible-value',
                                      qty=0)
