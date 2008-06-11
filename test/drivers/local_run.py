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
from socket import gethostname
import signal


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
  log_path = None
  COUNTDOWN = 5

  def __init__(self):
      self.argument_parser()
      self.set_uiaqa_home()
      self.set_log_path()

  def set_log_path(self):
    if Settings.log_path is None:
      Settings.log_path = "%s/logs" % Settings.uiaqa_home
    if not os.path.exists(Settings.log_path):
      output("ERROR:  Log path '%s' does not exist." % Settings.log_path)
      abort(1)
    output("INFO:  Logging to:  %s" % Settings.log_path)

  def argument_parser(self):
    try:
      opts, args = getopt.getopt(sys.argv[1:],"hql:",["help","quiet","log="])
    except getopt.GetoptError:
      self.help()

    for o,a in opts:
      if o in ("-q","--quiet"):
        Settings.is_quiet = True
    for o,a in opts:
      if o in ("-h","--help"):
        self.help()
        abort(0)
      if o in ("-l","--log"):
        Settings.log_path = a

  def help(self):
    output("Common Options:")
    output("  -h | --help        Print help information (this message).")
    output("  -q | --quiet       Don't print anything.")
    output("  -l | --log=        Where the log(s) should be stored.")

  def set_uiaqa_home(self):
    drivers_dir = sys.path[0]
    i = drivers_dir.rfind("/")
    Settings.uiaqa_home = drivers_dir[:i]

class Test(object):

  pids = []

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
    found_tests = [] # store the full path of the test here

    for test in self.tests:
      test_path = os.path.join(Settings.uiaqa_home, "testers/%s" % test)
      if not os.path.exists(test_path):
        unfound_tests.append(test)
      else:
        found_tests.append(os.path.join(Settings.uiaqa_home,
                                        "testers/%s" % test))

    num_unfound_tests = len(unfound_tests)
    if num_unfound_tests > 0:
      output("WARNING:  The following tests were not found:")
      for unfound_test in unfound_tests:
        output("  %s" % unfound_test)
      output("WARNING:  %i/%i unfound tests!"\
               % (num_unfound_tests, len(self.tests)))
      try:
        self.countdown(Settings.COUNTDOWN)
      except KeyboardInterrupt:
        return 0

    # execute the tests
    output("INFO:  Executing tests...")
    for test in found_tests:
      #os.system(test)
      self.pids.append(os.spawnl(os.P_WAIT, test))
      try:
        self.log(test)
      except InconceivableError, msg:
        print msg
        return 1

    # cleanup
    output("INFO:  Cleaning up...")
    for pid in self.pids: 
      try:
        os.kill(pid, signal.SIGKILL)
      except OSError, err:
          # Errno 3  is "No such process"
          if not err.errno == 3:
            output("WARNING:  Could not kill process '%s'" % pid)

  def log(self, test):
    filename = os.path.basename(test)

    # take off the file exension
    dot_index = filename.rfind(".")
    if dot_index > 0:
      filename = filename[:dot_index] # chop off the extension

    control_dir =  os.path.join(Settings.log_path, filename)
    # try to build a useful dir name that will be unique
    log_dir = os.path.join(control_dir,"%s_%s" %\
                            (gethostname(), time.strftime("%H%M%S",\
                             time.localtime())))

    if not os.path.exists(control_dir):
      try:
        os.mkdir(control_dir)
        time.sleep(5) # XXX: waiting for cifs, but use a better method
      except OSError, err:
        # Errno 17 is "File exists"
        # If another thread created this directory, that's fine.
        if not err.errno == 17:
          print "WARNINGS:  Could not create log directory!"
          print "WARNINGS:  Permanent logs will not be stored"
          return 0
  
    if os.path.exists(log_dir):
        raise InconceivableError,\
                "ERROR:  Inconceivable!  %s already exists!" % log_dir
    
    os.mkdir(log_dir)
    time.sleep(5) # XXX: waiting for cifs, but use a better method

    # copy over the resource files
    # XXX: change the log files to reference the resources from 
    # a static location so we don't have to copy these every time and
    # waste time/space
    os.system("cp -r /tmp/strongwind/* %s" % log_dir)
    os.system("cp -r %s/resources/* %s" % (Settings.uiaqa_home, log_dir))
      
class InconceivableError(Exception): pass

class Main(object):

  def main(self, argv=None):
    t = Test()
    return t.run()


settings = Settings()

if __name__ == '__main__':
  main_obj = Main();
  sys.exit(main_obj.main())
