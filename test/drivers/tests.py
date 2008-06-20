#!/usr/bin/env python

# the tests_list list keeps track of enabled tests.  A test will be
# executed only after the tests sample application is added to this list

# this allows tests to be worked on and developed in SVN, but only executed
# as part of the test suite when they are ready

tests_list = [
  "checkbutton_basic_ops.py",
  "checkbutton_basic_ops2.py",
  "checkbutton_basic_ops3.py",
  "checkbutton_basic_ops4.py"
]
