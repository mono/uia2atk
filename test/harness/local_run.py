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
from urlparse import urljoin
import signal
import subprocess as s
import fileinput
global tests

UPDATE_SCRIPT = "update_uia2atk_pkgs.sh"
TRUNK = "trunk"
BRANCHES = "branches"
TAGS = "tags"
OPENSUSE = "opensuse"
FEDORA = "fedora"
UBUNTU = "ubuntu"
HOSTNAME = gethostname().split(".")[0]

# simply takes a string s as input and prints it if running verbosely
def output(s, newline=True):
  if not Settings.is_quiet:
    if newline:
      print s
    else:
      print s

def abort(status):
  ''' exit according to status '''
  sys.exit(status)

def makedirs(name, mode=0777):
  """
  Super-mkdir; create a leaf directory and all intermediate ones.
  Works like mkdir, except that any intermediate path segment (not
  just the rightmost) will be created if it does not exist.  This is
  recursive.  This is stolen from Python's os module, with short sleep
  added after each individual mkdir to account for CIFS/NFS being slow
  """
  head, tail = os.path.split(name)
  if not tail:
    head, tail = os.path.split(head)
  if head and tail and not os.path.exists(head):
    try:
      makedirs(head, mode)
    except OSError, e:
      # be happy if someone already created the path
      if e.errno != errno.EEXIST:
          raise
    if tail == os.curdir:  # xxx/newdir/. exists if xxx/newdir exists
      return
  os.mkdir(name, mode)
  # XXX: Waiting for CIFS, use a better method
  time.sleep(5)

class Settings(object):

  # static variable, set by ctor
  is_smoke = False
  is_quiet = False
  uiaqa_home = None
  log_path = None
  COUNTDOWN = 5
  should_update = False
  version = None
  os_version = None
  tree = TRUNK
  distro = None
  distro_version = None
  is_force = False
  is_nodeps = False
  component = None
  control = None
  # this is returned to the console.  it will get set to 1 if any tests fail
  return_code = 0
  uiadll_path = "uiaclient/Tests/bin/Debug"

  # any applications that are run from our tests should be listed here.
  # this is so we can clean them up if they don't get closed due to a
  # test failure
  third_party_apps = ["gcalctool","firefox","gnome-calculator"]

  def __init__(self):
    self.argument_parser()
    self.set_uiaqa_home()

  def argument_parser(self):
    opts = []
    args = []
    try:
      opts, args = getopt.getopt(sys.argv[1:],"funshql:v:t:c:o:",["update","smoke","help","quiet","force","nodeps","log=","os-version=","version=","tree=","component=","control="])
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
      if o in ("-f","--force"):
        Settings.is_force = True
      if o in ("-n","--nodeps"):
        Settings.is_nodeps = True
      if o in ("-v","--version"):
        Settings.version = a.replace('.','_')
      if o in ("-t","--tree"):
        t = a.lower()
        if t == "branch":
          t = "branches"
        elif t == "tag":
          t = "tags"
        Settings.tree = t
      if o in ("-c","--component"):
        Settings.component = a
      if o in ("-o","--control"):
        Settings.control = a

      if Settings.tree == BRANCHES:
        assert Settings.version is not None, \
                            "You must specify a branch version using --version"
        abort(1)
      elif Settings.tree == TAGS:
        assert Settings.version is not None, \
                            "You must specify a tag version using --version"
        abort(1)
      elif Settings.tree == TRUNK:
        # doesn't require an additional option
        pass
      else:
        output("Invalid tree '%s' specified" % Settings.tree)
        abort(1)

  def help(self):
    output("Usage: local_run.py [options]")
    output("Options:")
    output("  -h | --help        Print help information (this message).")
    output("  -q | --quiet       Don't print anything.")
    output("  -l | --log=        Where the log(s) should be stored.")
    output("  -s | --smoke       Run only smoke tests")
    output("  -u | --update      Update packages on remote machines")
    output("  -f | --force       Force the update (using rpm --force)")
    output("  -n | --nodeps      Do not check deps on the update (using rpm --nodps)")
    output("  -t | --tree=       The part of the SVN tree desired (i.e., tags, branches, or")
    output("                     trunk).  The default is trunk.")
    output("  -c | --component=  Select at least and only one component to test (i.e.,")
    output("                     winforms or moonlight).")
    output("  -o | --control=    Select a control to test.")
    output("  -v | --version=    The tags or branches version desired")

  def set_uiaqa_home(self):
    harness_dir = sys.path[0]
    i = harness_dir.rfind("/")
    Settings.uiaqa_home = harness_dir[:i]

class Test(object):

  def __init__(self, component):
    # conditional import based on whether we want to run smoke tests or
    # regression tests
    global tests
    if Settings.is_smoke:
      import smoke_tests as tests
    else:
      import tests as tests

    self.component = str(Settings.component)
    # dynamically evaluate tests_list with component name
    try:
      ttests = eval('tests.%s_tests_list' % self.component)
    except AttributeError:
      output("ERROR:  No component found!")
      abort(1)

    if Settings.control is not None:
      self.control = str(Settings.control) + '-regression.py'
      if self.control in ttests:
        self.tests = []
        self.tests.append(self.control)
      else:
        output("ERROR:  No control found!")
        abort(1)
    else:
      self.tests = ttests

    self.set_log_path()

  def set_log_path(self):
    if Settings.log_path is None:
      Settings.log_path = "%s/logs/%s" % (Settings.uiaqa_home, Settings.component)
    if not os.path.exists(Settings.log_path):
      makedirs(Settings.log_path)
      output("ERROR:  Log path '%s' does not exist." % Settings.log_path)
      abort(1)
    output("INFO:  Logging to:  %s" % Settings.log_path)

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

  def find_distro(self):
    # returns None if we don't know (or don't care)
    if os.path.exists("/etc/fedora-release"):
      Settings.distro = FEDORA
    elif os.path.exists("/etc/SuSE-release"):
      Settings.distro = OPENSUSE
    elif os.path.exists("/usr/bin/ubuntu-bug"):
      Settings.distro = UBUNTU
    else:
      Settings.distro = None

  def find_distro_version(self):
    assert Settings.distro is not None, "Distro has not been deteced"
    if Settings.distro == FEDORA:
      f = open('/etc/fedora-release', 'r')
      release = f.readline()
      try:
        Settings.distro_version = release.split()[2].replace('.','')
      except IndexError:
        pass
    elif Settings.distro == OPENSUSE:
      f = open('/etc/SuSE-release', 'r')
      release = f.readline()
      try:
        Settings.distro_version = release.split()[1].replace('.','')
      except IndexError:
        pass
    elif Settings.distro == UBUNTU:
      f = open('/etc/lsb-release', 'r')
      release = f.readline()
      release = f.readline()
      try:
        Settings.distro_version = release.split('=')[1].replace('.','')
      except IndexError:
        pass
    else:
      Settings.distro_version = None

  def update(self):
    import urllib
    # need to determine the distro and the distro version
    self.find_distro()
    self.find_distro_version()
    url_part1 = "http://build1.sled.lab.novell.com/uia/"
    osystem = ""
    arch = ""
    if os.path.exists("/usr/lib64"):
      arch = "64"
    else:
      arch = "32"
    if Settings.tree == TRUNK:
      url_part2 = "/".join((Settings.tree,
                           "%s%s" % (Settings.distro, Settings.distro_version),
                           arch,
                           "current",
                           "rpm_revs"))
    else:
      url_part2 = "/".join((Settings.tree,
                           Settings.version,
                           "%s%s" % (Settings.distro, Settings.distro_version),
                           arch,
                           "current",
                           "rpm_revs"))

    url = urljoin(url_part1, url_part2)
    u = urllib.urlopen(url)
    self.newest_dir = u.readline().strip()

    update_script = \
                  os.path.join(Settings.uiaqa_home, "tools/%s" % UPDATE_SCRIPT)

    if Settings.is_force:
      force_option = "--force"
    else:
      force_option = ""

    if Settings.is_nodeps:
      nodeps_option = "--nodeps"
    else:
      nodeps_option = ""

    output("INFO:  Updating packages:")
    t = s.Popen(["/usr/bin/sudo",
                 update_script,
                 "%s" % force_option,
                 "%s" % nodeps_option,
                 "--directory=%s" % self.newest_dir],
                 stdout=s.PIPE,
                 stderr=s.STDOUT)
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
                            (Settings.log_path, HOSTNAME)
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
                                          (Settings.log_path, HOSTNAME))
      os.system("echo 0 > %s/%s_package_status" % \
                                          (Settings.log_path, HOSTNAME))
      os.system("echo --- >> %s" % package_status_path)
      f = open(package_status_path, 'a+')
      f.write("".join(o))
      f.close()
      return 0

  def run(self):
    unfound_tests = []
    found_tests = [] # store the full path of the test here

    for test in self.tests:
      if self.component.startswith('uiaclient'):
          self.component = "uiaclient"
          test_path = os.path.join(Settings.uiaqa_home, "testers/%s/%s" % (self.component, test))
      else:
          test_path = os.path.join(Settings.uiaqa_home, "testers/%s/%s" % (self.component, test))
      if not os.path.exists(test_path):
          unfound_tests.append(test)
      else:
          found_tests.append(os.path.join(Settings.uiaqa_home,
                                        "testers/%s/%s" % (self.component, test)))

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
    # the status of all tests.  this is set to 1 if one or more tests fail.
    # this is the value that is returned by the function.  If 0 is returned,
    # it indicates that all tests ran successfully (i.e., a perfect run!)
    self.status_all = 0
    for test in found_tests:
      # the status of a single test.  initial status is 0 (good)
      self.status = 0
      self.set_test_file_info(test)
      file_dir_path = os.path.join(Settings.log_path, test_type, self.control_name)
      if not os.path.exists(file_dir_path):
        os.mkdir(os.path.join(file_dir_path))
        time.sleep(5)  # XXX: waiting for cifs :( use a better method
      file_path = os.path.join(file_dir_path, HOSTNAME)
      self.write_top_portion(file_path)
      t = s.Popen(["python", "-u", test], stdout=s.PIPE, stderr=s.STDOUT)
      s.Popen(["tee", "-a", file_path], stdin=t.stdout, stderr=t.stderr)
      i = 0
      while t.poll() is None:
        time.sleep(1)
      r = t.poll()
      if r != 0:
        output("WARNING:  Failed test:  %s" % test)
        # if any tests fail, set the status and status_all to 1
        self.status = 1
        self.status_all = 1
        self.cleanup()
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
    return self.status_all

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
          f.write("%s%s" % ("  ", self.newest_dir))
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
        output("INFO:  Writing to file: %s" % file_path)
        f = open(file_path,'w')
        f.writelines(old_file_tests)
        f.writelines([self.log_dir,'\n'])
        if Settings.should_update:
          f.write("%s%s%s" % ("  ", self.newest_dir, "\n"))
        f.write(SEPARATOR)

    # the separator denotes the division between the list of the consecutive
    # failed tests and the most recent log
    SEPARATOR = "---\n"

    # if the previous test was successful, delete the file
    try:
      f = open(file_path,'r')
      if f.read(1) == "0":
        f.close()
        os.system("rm -rf %s" % file_path)
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
    self.control_name = os.path.basename(test).split("-")[0]

    # take off the file exension
    dot_index = self.filename.rfind(".")
    if dot_index > 0:
      self.filename = self.filename[:dot_index] # chop off the extension

    self.control_dir = os.path.join(Settings.log_path, self.filename)
    # try to build a useful dir name that will be unique, not y3k compliant :)
    self.log_dir = os.path.join(self.control_dir, HOSTNAME, time.strftime("%m%d%y_%H%M%S"))

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

    # os.makedirs() does not work here because cifs is slow
    makedirs(self.log_dir)

    # copy over the resource files
    # XXX: change the log files to reference the resources from
    # a static location so we don't have to copy these every time and
    # waste time/space
    if self.component.startswith("uiaclient"):
        uiaclient_test_path = os.path.join(Settings.uiaqa_home, Settings.uiadll_path)
        os.system("cp %s/*.png %s" % (uiaclient_test_path, self.log_dir))
        os.system("cp %s/procedures.xml %s" % (uiaclient_test_path, self.log_dir))
        os.system("cp -r %s/Resources/* %s" % (uiaclient_test_path, self.log_dir))

    else:
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

  def cleanup(self, final=False):
    if final:
      output("INFO:  Checking for any rogue processes...")
      # hack to kill any third part applications we open with our tests, it
      # would be nice to do this more intelligently
      FNULL = open('/dev/null')
      for app in Settings.third_party_apps:
        s.Popen(["killall","%s" % app], stderr=FNULL, stdout=FNULL)
      FNULL.close()
    else:
      output("INFO:  Cleaning up failed test:")
    search = "%s/%s" % (Settings.uiaqa_home, "samples")
    # execute the following command to get a pid and a path of the tests
    # that might be running still
    # ps -ax | grep /home/a11y/code/uia2atk/test/samples | awk '{print $1,$6}'
    p1 = s.Popen(["ps","a","x"], stdout=s.PIPE)
    p2 = s.Popen(["grep", search], stdin=p1.stdout, stdout=s.PIPE)
    p3 = s.Popen(["grep", "-v", "grep"], stdin=p2.stdout, stdout=s.PIPE)
    p4 = s.Popen(["awk", "{print $1, $7}"], stdin=p3.stdout, stdout=s.PIPE)
    processes = dict(line.strip().split() for line in p4.stdout)
    if final and len(processes) > 0:
      output("WARNING:  The following processes never exited:")
      for pid in processes:
        print "  %s (%s)" % (pid, processes[pid])
      output("WARNING:  The above processes will now be killed.")
    for pid in processes:
      self.kill_process(pid)

class InconceivableError(Exception): pass

class Main(object):

  def main(self, argv=None):
    t = Test(Settings.component)
    r = None
    if Settings.should_update:
      r = t.update()
    # if the return status of the update is 0 (success)
    # or we're not updating packages run the tests
    if r is None or r == 0:
      r = t.run()
      t.cleanup(True)
    if Settings.log_path:
      output("INFO:  Logging to:  %s" % Settings.log_path)
    output("INFO:  EXITING %s" % os.path.basename(__file__))
    return r

if __name__ == '__main__':
  Settings()
  main_obj = Main();
  sys.exit(main_obj.main())
