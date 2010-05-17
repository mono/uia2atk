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

from strongwind.utils import retryUntilTrue, toConstantName

import unittest
import pyatspi

class TestCase(unittest.TestCase):
    def __getattr__(self, name):
        """
        Override all assert* and fail* methods and retry them several times
        before failing.  This accomodates applications which make take a little
        longer to react to an action due to mainloop contention.
        """
        parent = super(unittest.TestCase, self)
        if hasattr(parent, name):
            func = getattr(parent, name)
            if name.startswith('assert') or name.startswith('fail'):
                return lambda *args, **kwargs: \
                    retryUntilTrue(func, args, kwargs)
            return func
        raise AttributeError(name)

    def assertStates(self, control, expected_states):
        """
        Ensure that only the provided states are present.  Fail otherwise.
        """
        states = control._accessible.getState()
        for state in expected_states:
            #
            # Code from strongwind; Covered under the GPL v2
            # Copyright (C) 2007 Medsphere Systems Corporation
            #
            a = 'STATE_' + toConstantName(state)
            v = vars(pyatspi)

            self.assertTrue(a in v, 'Unrecognized state type: %s' % state)
            try:
                self.assertTrue(states.contains(v[a]))
            except AssertionError:
                # Sometimes corba throws this instead of returning False from
                # contains
                self.fail('State %s not found' % state)

            states.remove(v[a])

        self.assertTrue(states.isEmpty(), 'Extra states found')

    def assertInterfaces(self, control, expected_ifaces):
        """
        Ensure that only the expected interfaces are present.  Fail otherwise.
        """
        ifaces = pyatspi.listInterfaces(control._accessible)

        ifaces = map(unicode.lower, ifaces)
        expected_ifaces = map(str.lower, expected_ifaces)

        for iface in ifaces:
            if iface in expected_ifaces:
                expected_ifaces.remove(iface)
            else:
                self.fail("Unexpected interface: %s" % iface)
        self.assertEqual(0, len(expected_ifaces), \
                         "Some interfaces are not implemented: %s" % expected_ifaces)
