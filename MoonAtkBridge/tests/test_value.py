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

from os.path import abspath
import pyatspi

class ValueTextBox(TestCase):
    """
    Exercises the basic functionality of the Value class using a TextBox
    control
    """
    @classmethod
    def setup_class(cls):
        cls.app = launchAddress(abspath('assets/ValueTest/ValueTest.html'))
        cls.textbox = cls.app.slControl.findText('Hello World!')
        cls.textbox_text = cls.textbox._accessible.queryText()

    @classmethod
    def teardown_class(cls):
        cls.app.kill()

    def test_role(self):
        self.assertEqual(self.textbox.roleName, 'text')

    def test_add_selection(self):
        # This isn't fully implemented, so ensure we don't crash
        self.assertFalse(self.textbox_text.addSelection(0, 1))

    def test_caret_offset(self):
        # This isn't fully implemented, so ensure we don't crash
        self.assertEqual(self.textbox_text.caretOffset, 0)

    def test_character_count(self):
        self.assertEqual(self.textbox_text.characterCount, 12)

    def test_get_attribute_run(self):
        # This isn't fully implemented, so ensure we don't crash
        self.assertEqual(self.textbox_text.getAttributeRun(0, 0), ([], 0, 0))

    def test_get_attributes(self):
        self.assertEqual(self.textbox_text.getAttributes(0), ('', 0, 0))

    # TODO: test_get_bounded_ranges

    def test_get_character_at_offset(self):
        self.assertEqual(self.textbox_text.getCharacterAtOffset(-1), 0L)
        self.assertEqual(self.textbox_text.getCharacterAtOffset(12), 0L)

        for i, c in enumerate('Hello World!'):
            self.assertEqual(chr(self.textbox_text.getCharacterAtOffset(i)), c)

    # TODO: test_character_extents

    def test_get_default_attributes(self):
        # This isn't fully implemented, so ensure we don't crash
        self.assertEqual(self.textbox_text.getDefaultAttributes(), '')

    def test_get_n_selections(self):
        # This isn't fully implemented, so ensure we don't crash
        self.assertEqual(self.textbox_text.getNSelections(), 0)

    def test_get_offset_at_point(self):
        # This isn't fully implemented, so ensure we don't crash
        self.assertEqual(self.textbox_text.getOffsetAtPoint(0, 0, pyatspi.WINDOW_COORDS), -1)

    # TODO: test_get_range_extents

    def test_get_selection(self):
        self.assertEqual(self.textbox_text.getSelection(0), (0, 0))

    def test_get_text(self):
        t = self.textbox_text

        self.assertEqual(t.getText(0, -1), 'Hello World!')

        self.assertEqual(t.getText(0, 0), '')
        self.assertEqual(t.getText(0, 1), 'H')
        self.assertEqual(t.getText(0, 5), 'Hello')
        self.assertEqual(t.getText(0, t.characterCount), 'Hello World!')
        self.assertEqual(t.getText(0, 15), 'Hello World!')

    def test_get_text_after_offset(self):
        t = self.textbox_text

        self.assertEqual(t.getTextAfterOffset(0, pyatspi.TEXT_BOUNDARY_CHAR), \
                         ('e', 1, 2))
        self.assertEqual(t.getTextAfterOffset(5, pyatspi.TEXT_BOUNDARY_CHAR), \
                         ('W', 6, 7))
        self.assertEqual(t.getTextAfterOffset(20, pyatspi.TEXT_BOUNDARY_CHAR), \
                         ('', 12, 12))

        self.assertEqual(t.getTextAfterOffset(0, pyatspi.TEXT_BOUNDARY_LINE_START), \
                         ('', 12, 12))
        self.assertEqual(t.getTextAfterOffset(0, pyatspi.TEXT_BOUNDARY_LINE_END), \
                         ('', 12, 12))

        self.assertEqual(t.getTextAfterOffset(0, pyatspi.TEXT_BOUNDARY_SENTENCE_START), \
                         ('', 12, 12))
        self.assertEqual(t.getTextAfterOffset(0, pyatspi.TEXT_BOUNDARY_SENTENCE_END), \
                         ('', 12, 12))

        self.assertEqual(t.getTextAfterOffset(0, pyatspi.TEXT_BOUNDARY_WORD_START), \
                         ('World!', 6, 12))
        self.assertEqual(t.getTextAfterOffset(0, pyatspi.TEXT_BOUNDARY_WORD_END), \
                         (' World', 5, 11))

        self.assertEqual(t.getTextAfterOffset(8, pyatspi.TEXT_BOUNDARY_WORD_START), \
                         ('', 12, 12))
        self.assertEqual(t.getTextAfterOffset(12, pyatspi.TEXT_BOUNDARY_WORD_END), \
                         ('', 12, 12))

    def test_get_text_at_offset(self):
        t = self.textbox_text

        self.assertEqual(t.getTextAtOffset(0, pyatspi.TEXT_BOUNDARY_CHAR), \
                         ('H', 0, 1))
        self.assertEqual(t.getTextAtOffset(5, pyatspi.TEXT_BOUNDARY_CHAR), \
                         (' ', 5, 6))
        self.assertEqual(t.getTextAtOffset(20, pyatspi.TEXT_BOUNDARY_CHAR), \
                         ('', 12, 12))

        self.assertEqual(t.getTextAtOffset(0, pyatspi.TEXT_BOUNDARY_LINE_START), \
                         ('Hello World!', 0, 12))
        self.assertEqual(t.getTextAtOffset(0, pyatspi.TEXT_BOUNDARY_LINE_END), \
                         ('Hello World!', 0, 12))

        self.assertEqual(t.getTextAtOffset(0, pyatspi.TEXT_BOUNDARY_SENTENCE_START), \
                         ('Hello World!', 0, 12))
        self.assertEqual(t.getTextAtOffset(0, pyatspi.TEXT_BOUNDARY_SENTENCE_END), \
                         ('Hello World!', 0, 12))

        self.assertEqual(t.getTextAtOffset(0, pyatspi.TEXT_BOUNDARY_WORD_START), \
                         ('Hello ', 0, 6))
        self.assertEqual(t.getTextAtOffset(0, pyatspi.TEXT_BOUNDARY_WORD_END), \
                         ('Hello', 0, 5))

        self.assertEqual(t.getTextAtOffset(8, pyatspi.TEXT_BOUNDARY_WORD_START), \
                         ('World!', 6, 12))
        self.assertEqual(t.getTextAtOffset(12, pyatspi.TEXT_BOUNDARY_WORD_END), \
                         ('!', 11, 12))

    def test_get_text_before_offset(self):
        t = self.textbox_text

        self.assertEqual(t.getTextBeforeOffset(0, pyatspi.TEXT_BOUNDARY_CHAR), \
                         ('', 0, 0))
        self.assertEqual(t.getTextBeforeOffset(5, pyatspi.TEXT_BOUNDARY_CHAR), \
                         ('o', 4, 5))
        self.assertEqual(t.getTextBeforeOffset(20, pyatspi.TEXT_BOUNDARY_CHAR), \
                         ('!', 11, 12))

        self.assertEqual(t.getTextBeforeOffset(0, pyatspi.TEXT_BOUNDARY_LINE_START), \
                         ('', 0, 0))
        self.assertEqual(t.getTextBeforeOffset(0, pyatspi.TEXT_BOUNDARY_LINE_END), \
                         ('', 0, 0))

        self.assertEqual(t.getTextBeforeOffset(0, pyatspi.TEXT_BOUNDARY_SENTENCE_START), \
                         ('', 0, 0))
        self.assertEqual(t.getTextBeforeOffset(0, pyatspi.TEXT_BOUNDARY_SENTENCE_END), \
                         ('', 0, 0))

        self.assertEqual(t.getTextBeforeOffset(0, pyatspi.TEXT_BOUNDARY_WORD_START), \
                         ('', 0, 0))
        self.assertEqual(t.getTextBeforeOffset(0, pyatspi.TEXT_BOUNDARY_WORD_END), \
                         ('', 0, 0))

        self.assertEqual(t.getTextBeforeOffset(8, pyatspi.TEXT_BOUNDARY_WORD_START), \
                         ('Hello ', 0, 6))
        self.assertEqual(t.getTextBeforeOffset(12, pyatspi.TEXT_BOUNDARY_WORD_END), \
                         (' World', 5, 11))

    def test_remove_selection(self):
        self.assertFalse(self.textbox_text.removeSelection(0))

    def test_set_caret_offset(self):
        self.assertTrue(self.textbox_text.setCaretOffset(1))
        self.assertEqual(self.textbox_text.caretOffset, 1)
        self.assertTrue(self.textbox_text.setCaretOffset(0))
        self.assertEqual(self.textbox_text.caretOffset, 0)

    def test_set_selection(self):
        self.assertFalse(self.textbox_text.setSelection(0, 0, 2))
