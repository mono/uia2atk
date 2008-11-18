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
import commands as c
from socket import gethostname
import signal
import subprocess as s
import fileinput
global tests

UPDATE_SCRIPT = "update_uia2atk_pkgs.sh"

# simply takes a string s as input and prints it if running verbosely
def output(s, newline=True):
  if not Settings.is_quiet:
    if newline:
      print s
    else:
      print s,

def abort(status):
  ''' exit according to status '''
  sys.exit(status)

class Settings(object):

  # static variable, set by ctor
  is_smoke = False
  is_quiet = False
  uiaqa_home = None
  log_path = None
  COUNTDOWN = 5
  should_update = False

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
      opts, args = getopt.getopt(sys.argv[1:],"ushql:",["update","smoke","help","quiet","log="])
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
      if o in ("-u","--update"):
        Settings.should_update = True

  def help(self):
    output("Common Options:")
    output("  -h | --help        Print help information (this message).")
    output("  -q | --quiet       Don't print anything.")
    output("  -l | --log=        Where the log(s) should be stored.")
    output("  -s | --smoke       Run only smoke tests")
    output("  -u | --update      Update packages on remote machines")

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
 
  def update(self):
    import urllib
    u = urllib.urlopen("http://build1.sled.lab.novell.com/uia/current/rpm_revs")
    revision_list = u.readlines()
    self.revisions = "".join(revision_list[3:])
    u.close()
    # mega hack to get the most recent revision directory
    date = revision_list[0].split()[-1]
    u = urllib.urlopen("http://build1.sled.lab.novell.com/uia")
    todays_dirs = []
    for line in u.readlines():
        if date in line:
            # lf is left find
            # rf is right find
            lf = line.find(date)
            rf = line.rfind(date) 
            todays_dirs.append(line[lf:rf].strip('/">'))
    if len(todays_dirs) > 1:
      newest_dir = todays_dirs[-2]
    else:
      newest_dir = todays_dirs[0]
    update_script = \
                  os.path.join(Settings.uiaqa_home, "tools/%s" % UPDATE_SCRIPT)
    output("INFO:  Updating packages:")
    t = s.Popen(["/usr/bin/sudo", update_script, "-f", "--directory=%s" % newest_dir], stdout=s.PIPE, stderr=s.STDOUT)
    o = []
    while True:
        o_tmp = t.stdout.readline()
        if o_tmp != '':
            o.append(o_tmp) 
            print o_tmp.rstrip()
        if o_tmp == '' and t.poll() is not None:
            break
    r = t.poll()
    package_status_path = "%s/%s_package_status" % \
                            (Settings.log_path, gethostname().split(".")[0])
    if r != 0:
        # create the package_status file.  delete it first so that it 
        # is picked up as a new file by qamon
        os.system("rm -f %s" % package_status_path)
        os.system("echo 1 > %s" % package_status_path)
        os.system("echo --- >> %s" % package_status_path)
        f = open(package_status_path, 'a+')
        f.write("".join(o))
        f.close()
        return 1
    else:
        os.system("rm -f %s/%s_package_status" % \
                                            (Settings.log_path, gethostname()))
        os.system("echo 0 > %s/%s_package_status" % \
                                            (Settings.log_path, gethostname()))
        os.system("echo --- >> %s" % package_status_path)
        f = open(package_status_path, 'a+')
        f.write("".join(o))
        f.close()
        return 0

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

    # create directories for text logs if they don't already exist in the
    # log_path
    test_type = "smoke" if Settings.is_smoke else "regression"
    test_type_dir = os.path.join(Settings.log_path, test_type)
    if not os.path.exists(test_type_dir):
      os.mkdir(os.path.join(test_type_dir))
      time.sleep(5) # XXX: waiting for cifs :( use a better method

    # execute the tests
    TIMEOUT = 600 # ten minutes
    output("INFO:  Executing tests...")
    for test in found_tests:
      # set the initial status to 0 (good)
      self.status = 0
      self.set_test_file_info(test)
      file_path = os.path.join(Settings.log_path, test_type, self.control_name)
      self.write_top_portion(file_path)
      t = s.Popen(["python", "-u", test], stdout=s.PIPE, stderr=s.PIPE)
      s.Popen(["tee", "-a", file_path], stdin=t.stdout, stderr=t.stderr)
      i = 0
      while t.poll() is None:
        time.sleep(1)
      r = t.poll()
      if r != 0:
        output("WARNING:  Failed test:  %s" % test)
        self.status = 1
      else:
        # if the test was successful prefix the file with a 0, so we can
        # easily tell that the test was successful based on this log
        f = open(file_path,'r')
        first_char = f.read(1)
        f.close()
        if first_char != "0":
            f = open(file_path,'r')
            log = []
            for line in f:
                log.append(line)
            f.close()
            f = open(file_path, 'w')
            f.write("0\n")
            f.write("".join(log))
            f.close()
      try:
        self.log(test)
      except InconceivableError, msg:
        output(msg)
        return 1
    return self.status 

  def write_top_portion(self, file_path):
    '''Create the top portion of the summary text log file.  This portion
    consists of a chronological list of machine names and dates for which
    the test has failed; the portion ends with a SEPARATOR.  The Strongwind
    stdout and stderr info for an individual Strongwind test will be dumped
    after the SEPARATOR'''

    def write_file(f, is_new):
      if is_new:
        f.writelines([self.log_dir,'\n'])
        if Settings.should_update:
          f.write("%s%s" % ("  ", self.revisions.replace("\n","\n  ")))
        f.write(SEPARATOR)
      else:
        # chronological list of machine names and dates for which the test has
        # failed with the most recent log at the end (after SEPARATOR)
        old_file_tests = []
        for line in f:
          if line == SEPARATOR:
            break
          else:
            old_file_tests.append(line)
        f.close()
        f = open(file_path,'w')
        f.writelines(old_file_tests)
        f.writelines([self.log_dir,'\n'])
        if Settings.should_update:
          f.write("%s%s%s" % \
                               ("  ",
                                self.revisions.replace("\n","\n  ").strip(),
                                "\n"))
        f.write(SEPARATOR)

    # the separator denotes the division between the list of the consecutive
    # failed tests and the most recent log
    SEPARATOR = "---\n"

    # if the previous test was successful, delete the file
    try:
      f = open(file_path,'r')
      if f.read(1) == "0":
        f.close()
        os.system("rm -rf file_path")   
      f.close()
    except IOError:
      pass

    # write a new file if a text file hasn't beeen created for the test,
    # otherwise modify the existing file
    try:
      is_new = False
      f = open(file_path,'a+')
      write_file(f, is_new)
    except IOError, err:
      print err
      is_new = True
      f = open(file_path,'w+')
      write_file(f, is_new)
    f.close()

  def set_test_file_info(self, test):
    self.filename = os.path.basename(test)
    self.control_name = os.path.basename(test).split("_")[0]

    # take off the file exension
    dot_index = self.filename.rfind(".")
    if dot_index > 0:
      self.filename = self.filename[:dot_index] # chop off the extension

    self.control_dir = os.path.join(Settings.log_path, self.filename)
    # try to build a useful dir name that will be unique, not y3k compliant :)
    self.log_dir = os.path.join(self.control_dir,"%s_%s" %\
                            (gethostname(), time.strftime("%m%d%y_%H%M%S")))

  def log(self, test):
    if not os.path.exists(self.control_dir):
      try:
        os.mkdir(self.control_dir)
        time.sleep(5) # XXX: waiting for cifs, but use a better method
      except OSError, err:
        # Errno 17 is "File exists"
        # If another thread created this directory, that's fine.
        if not err.errno == 17:
          output("WARNING:  Could not create log directory!")
          output("WARNING:  Permanent logs will not be stored")
          return 0
  
    if os.path.exists(self.log_dir):
        raise InconceivableError,\
                "ERROR:  Inconceivable!  %s already exists!" % self.log_dir
    
    os.mkdir(self.log_dir)
    time.sleep(5) # XXX: waiting for cifs, but use a better method

    # copy over the resource files
    # XXX: change the log files to reference the resources from 
    # a static location so we don't have to copy these every time and
    # waste time/space
    os.system("echo %s > %s/time" % (time.time(), self.log_dir))
    os.system("echo %s > %s/status" % (self.status, self.log_dir))
    os.system("cp -r /tmp/strongwind/* %s" % self.log_dir)
    os.system("cp -r %s/resources/* %s" % (Settings.uiaqa_home, self.log_dir))

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
    r = None
    if Settings.should_update:
      r = t.update()
    # if the return status of the update is 0 (success)
    # or we're not updating packages run the tests
    if r is None or r == 0:
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
