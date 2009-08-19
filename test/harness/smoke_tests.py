# the tests_list list keeps track of enabled tests.  A test will be
# executed only after the tests sample application is added to this list

# this allows tests to be worked on and developed in SVN, but only executed
# as part of the test suite when they are ready

# only add finished tests that should execute successfully to this list

tests_list = [
  "winforms/picturebox_smoke_test.py",
  "winforms/label_smoke_test.py",
  "winforms/statusbar_smoke_test.py"
]
