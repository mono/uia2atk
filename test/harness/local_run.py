#!/usr/bin/env python

##############################################################################
# Written by:  Brian G. Merrell <bgmerrell@novell.com>
# Date:        May 23 2008
# Description: Run the enabled tests on the local machine
##############################################################################

import sys
import getopt
import os
import time
from socket import gethostname
import signal
import subprocess as s
global tests

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

class Settings(object):

  # static variable, set by ctor
  is_smoke = False
  is_quiet = False
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
    opts = []
    args = []
    try:
      opts, args = getopt.getopt(sys.argv[1:],"shql:",["smoke","help","quiet","log="])
    except getopt.GetoptError:
      self.help()
      sys.exit(1)

    for o,a in opts:
      if o in ("-q","--quiet"):
        Settings.is_quiet = True
    for o,a in opts:
      if o in ("-h","--help"):
        self.help()
        abort(0)
      if o in ("-l","--log"):
        Settings.log_path = a
      if o in ("-s","--smoke"):
        Settings.is_smoke = True

  def help(self):
    output("Common Options:")
    output("  -h | --help        Print help information (this message).")
    output("  -q | --quiet       Don't print anything.")
    output("  -l | --log=        Where the log(s) should be stored.")
    output("  -s | --smoke       Run only smoke tests")

  def set_uiaqa_home(self):
    harness_dir = sys.path[0]
    i = harness_dir.rfind("/")
    Settings.uiaqa_home = harness_dir[:i]

class Test(object):

  def __init__(self):

    # conditional import based on whether we want to run smoke tests or
    # regression tests
    global tests
    if Settings.is_smoke:
      import smoke_tests as tests
    else:
      import tests as tests

    self.tests = tests.tests_list

    # set the initial status to 0 (good)
    self.status = 0

  def countdown(self, n):
    ''' Counts down for n seconds and allows the user to abort the program cleanly '''

    remaining = n
    output("Press CTRL+C to abort.")
    output("Continuing in", False)
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
      finally:
        output("")

    # execute the tests
    TIMEOUT = 600 # ten minutes
    output("INFO:  Executing tests...")
    for test in found_tests:
      t = s.Popen(["python", "-u", test])
      i = 0
      while t.poll() is None:
        time.sleep(1)
      r = t.poll()
      if r != 0:
        output("WARNING:  Failed test:  %s" % test)
        self.status = 1
      try:
        self.log(test)
      except InconceivableError, msg:
        output(msg)
        return 1
    return self.status 


  def log(self, test):
    filename = os.path.basename(test)

    # take off the file exension
    dot_index = filename.rfind(".")
    if dot_index > 0:
      filename = filename[:dot_index] # chop off the extension

    control_dir =  os.path.join(Settings.log_path, filename)
    # try to build a useful dir name that will be unique, not y3k compliant :)
    log_dir = os.path.join(control_dir,"%s_%s" %\
                            (gethostname(), time.strftime("%m%d%y_%H%M%S")))

    if not os.path.exists(control_dir):
      try:
        os.mkdir(control_dir)
        time.sleep(5) # XXX: waiting for cifs, but use a better method
      except OSError, err:
        # Errno 17 is "File exists"
        # If another thread created this directory, that's fine.
        if not err.errno == 17:
          output("WARNING:  Could not create log directory!")
          output("WARNING:  Permanent logs will not be stored")
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
    os.system("echo %s > %s/status" % (self.status, log_dir))

  def kill_process(self, pid):
    try:
      output("INFO:  killing process: %s" % pid)
      os.kill(int(pid), signal.SIGKILL)
      #output("INFO:  killed process: %s" % pid)
    except OSError, err:
      # Errno 3 is "No such process"
      if err.errno == 3:
        # If it doesn't exist anymore, cool.
        #output("INFO:  process %s does not exist" % pid)
        pass
        return
      output("WARNING:  Could not kill process: %s" % pid)

  def cleanup(self):
    output("INFO:  Cleaning up:")
    search = "%s/%s" % (Settings.uiaqa_home, "samples")
    # execute the following command to get a pid and a path of the tests
    # that might be running still
    # ps -ax | grep /home/a11y/code/uia2atk/test/samples | awk '{print $1,$6}'
    p1 = s.Popen(["ps","a","x"], stdout=s.PIPE)
    p2 = s.Popen(["grep", search], stdin=p1.stdout, stdout=s.PIPE)
    p3 = s.Popen(["awk", "{print $1}"], stdin=p2.stdout, stdout=s.PIPE)
    for pid in p3.stdout.read().strip().split():
      self.kill_process(pid)
  
class InconceivableError(Exception): pass

class Main(object):

  def main(self, argv=None):
    t = Test()
    r = t.run()
    t.cleanup()
    if Settings.log_path:
      output("INFO:  Logging to:  %s" % Settings.log_path)
    output("INFO:  EXITING %s" % os.path.basename(__file__))
    return r

if __name__ == '__main__':
  Settings()
  main_obj = Main();
  sys.exit(main_obj.main())
