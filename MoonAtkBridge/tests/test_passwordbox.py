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

class PasswordBox(TestCase):
    """
    Exercises the PasswordBox control
    """
    @classmethod
    def setup_class(cls):
        cls.app = launchAddress(abspath('assets/PasswordBoxTest/PasswordBoxTest.html'))
        cls.passwordbox = cls.app.slControl.findPasswordText('')
        cls.textblock = cls.app.slControl.findLabel('')

    @classmethod
    def teardown_class(cls):
        cls.app.kill()

    def test_passwordbox_basic(self):
        self.assertEqual('', self.passwordbox.name)

        self.assertInterfaces(self.passwordbox, [
            'accessible', 'component', 'text',
            'editabletext',
        ])

        # GTK+ password entries also have single_line,
        # but Orca doesn't seem to care
        self.assertStates(self.passwordbox, [
            # XXX: Focusable disabled due to #552879
            'editable', 'enabled', # 'focusable',
            'sensitive', 'showing', 'visible',
        ])

        self.passwordbox.grabFocus()
        self.assertStates(self.passwordbox, [
            # XXX: Focusable disabled due to #552879
            'editable', 'enabled', # 'focusable',
            'sensitive', 'showing', 'visible',
            'focused',
        ])

        self.assertEqual(0, self.passwordbox.childCount)

    def test_passwordbox_editable(self):
        edit = self.passwordbox._accessible.queryEditableText()
        listener = EventListener()

        # assert the password box is blank
        self.assertEqual('', self.textblock.name)

        with listener.listenTo(self.passwordbox):
            self.assertTrue(edit.insertText(0, 'hello', 6))

            sleep(config.SHORT_DELAY)

            self.assertEqual('hello', self.textblock.name)
            self.assertEqual('', self.passwordbox.name)

        assert listener.containsEvent(self.passwordbox,
                                      'object:text-changed:insert',
                                      qty=1)

        # XXX: Can't test insertions at a point (because PasswordBox doesn't
        # support retrieving the Value), and can't test deletions.
