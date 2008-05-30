#!/usr/bin/env python

##############################################################################
# Written by:  Brian G. Merrell <bgmerrell@novell.com>
# Date:        May 23 2008
# Description: Run the enabled tests on the local machines
##############################################################################

# import the enabled tests
import tests

# other imports
import sys
import getopt
import os
import time


# simply takes a string s as input and prints it if running verbosely
def output(s, newline=True):
  if not Settings.is_quiet:
    if newline:
      print s
    else:
      print s,

def abort(status):
  ''' exit according to status '''
  exit(status)


class Singleton(object):
  _instance = None

  def __new__(cls, *args, **kwargs):
    if not cls._instance:
      cls._instance = super(Singleton, cls).__new__(
                            cls, *args, **kwargs)
      return cls._instance


class Settings(object):

  # static variable, set by ctor
  is_quiet = None
  uiaqa_home = None

  def __init__(self):
      self.argument_parser()
      self.set_uiaqa_home()

  def argument_parser(self):
    try:
      opts, args = getopt.getopt(sys.argv[1:],"hq",["help","quiet"])
    except getopt.GetoptError:
      self.help()

    for o,a in opts:
      if o in ("-q","--quiet"):
        Settings.is_quiet = True
    for o,a in opts:
      if o in ("-h","--help"):
        self.help()
        sys.exit(0)

  def set_uiaqa_home(self):
    drivers_dir = sys.path[0]
    i = drivers_dir.rfind("/")
    Settings.UIAQA_HOME = drivers_dir[:i]

class Test(object):

  def __init__(self):
    self.tests = tests.tests_list

  def countdown(self, n):
    ''' Counts down for n seconds and allows the user to abort the program cleanly '''

    remaining = n
    output("Press CTRL+C to abort.")
    output("Continuing in ", False)
    for i in range(n):
      output(str(remaining), False)
      remaining-=1
      sys.stdout.flush()
      time.sleep(1)

  def run(self):
    unfound_tests = []
    for test in self.tests:
      test_path = os.path.join(Settings.UIAQA_HOME, "testers/%s" % test)
      if not os.path.exists(test_path):
        unfound_tests.append(test)

    num_unfound_tests = len(unfound_tests)
    if num_unfound_tests > 0:
      output("WARNING:  There were %i unfound tests!" % num_unfound_tests)
      output("WARNING:  The following tests were not found:")
      for unfound_test in unfound_tests:
        output("  %s" % unfound_test)
      try:
        self.countdown(10)
      except KeyboardInterrupt:
        return 0

class Main(object):

  def main(self, argv=None):
    t = Test()
    t.run()

  def help(self):
    output("Common Options:")
    output("  -h | --help        Print help information (this message).")
    output("  -q | --quiet       Don't print anything.")

settings = Settings()

if __name__ == '__main__':
  main_obj = Main();
  sys.exit(main_obj.main())
