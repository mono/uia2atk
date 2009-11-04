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

class TextBlock(TestCase):
    """
    Exercises the TextBlock control
    """
    @classmethod
    def setup_class(cls):
        cls.app = launchAddress(abspath('assets/TextBlockTest/TextBlockTest.html'))
        cls.textblock = cls.app.slControl.findLabel('Hello World!')
        cls.button = cls.app.slControl.findPushButton('Change Text')

    @classmethod
    def teardown_class(cls):
        cls.app.kill()

    def test_textblock_basic(self):
        self.assertEqual('Hello World!', self.textblock.name)

        self.assertInterfaces(self.textblock, [
            'accessible', 'component'
        ])

        # GTK+ labels also have multi_line, but Orca doesn't seem to care
        self.assertStates(self.textblock, [
            'enabled', 'sensitive', 'showing', 'visible'
        ])

        self.assertEqual(0, self.textblock.childCount)

    def test_textblock_events(self):
        self.assertEqual('Hello World!', self.textblock.name)

        listener = EventListener()
        with listener.listenTo(self.textblock):
            self.button.click()
            self.assertEqual('Goodbye World!', self.textblock.name)

        # GTK+ fires text-changed:delete, text-changed:insert,
        # object:visible-data-changed and object:bounds-changed, none of which
        # we can fire because TextBlockAutomationPeer (a
        # FrameworkElementAutomationPeer) isn't specific enough to fire these
        # events.

        assert listener.containsEvent(self.textblock,
                                      'object:property-change:accessible-name',
                                      qty=1)
