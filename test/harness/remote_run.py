#!/usr/bin/env python

##############################################################################
# Written by:  Brian G. Merrell <bgmerrell@novell.com>
# Date:        May 23 2008
# Description: Run the enabled tests on the remote machines
##############################################################################

# import the enabled tests
import machines

# other imports
import sys
import getopt
import os
import time
import threading


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
  is_quiet = None
  remote_log_path = machines.LOG_DIR
  local_log_path = "/tmp/uiaqa"
  is_log_ok = True
  COUNTDOWN = 10

  def __init__(self):
      self.argument_parser()

  def argument_parser(self):
    opts = []
    args = []
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
        Settings.local_log_path = a
        if not os.path.exists(Settings.local_log_path):
          output("ERROR:  Log path does not exist.")
          abort(1)

  def help(self):
    output("Common Options:")
    output("  -h | --help        Print help information (this message).")
    output("  -q | --quiet       Don't print anything.")
    output("  -l | --log=        Where the log(s) should be stored.")

class Ping(threading.Thread):

   def __init__ (self,name,ip):
      threading.Thread.__init__(self)
      self.ip = ip
      self.status = -1
      self.name = name
   def run(self):
      self.status = os.system("ping -q -c2 %s > /dev/null" % self.ip)

class Kickoff(threading.Thread):
    
  def __init__ (self,name,ip):
    threading.Thread.__init__(self)
    self.ip = ip
    self.status = -1
    self.name = name
  def run(self):
    self.status = os.system("ssh -o ConnectTimeout=15 %s@%s DISPLAY=:0 \
                             %s/harness/local_run.py --log=%s > %s/%s 2>&1" %\
                             (machines.USERNAME, self.ip, machines.TEST_DIR,\
                              Settings.remote_log_path,\
                              Settings.local_log_path, self.name))

class Test(object):

  def __init__(self):
    self.machines = machines.machines_dict


  def countdown(self, n):
    ''' Counts down for n seconds and allows the user to abort the program
    cleanly '''

    remaining = n
    output("Press CTRL+C to abort.")
    output("Continuing in ", False)
    for i in range(n):
      output(str(remaining), False)
      remaining-=1
      sys.stdout.flush()
      time.sleep(1)

  def check_machines(self):
    output("Checking machine status:")
    machine_names = self.machines.keys()
    ping_list = []
    self.up_machines = []
    down_machines = []
    lock = threading.Lock()
    for machine_name in machine_names:
      t = Ping(machine_name, self.machines[machine_name][0])
      ping_list.append(t)
      t.start()
    for t in ping_list:
      t.join()
      lock.acquire()
      #output("  %s (%s):...." % (t.name, t.ip), False) 
      output("  %-12s (%10s) ==>" % (t.name, t.ip), False) 
      if t.status == 0:
        output("UP")
        self.up_machines.append(t.name)
      else:
        down_machines.append(t.name)
        output("DOWN")
      lock.release()
    output("")
    if len(down_machines) > 0:
      output("WARNING:  %i/%i machines did not respond"\
              % (len(down_machines), len(self.machines)))
      try:
        self.countdown(Settings.COUNTDOWN)
      except KeyboardInterrupt:
        return 0
      output("")
    return len(self.up_machines)

  def execute_tests(self):
    output("Kicking off remote tests:")
    test_list = []
    failed_machines = []
    good_machines = []
    lock = threading.Lock()
    for up_machine in self.up_machines:
      t = Kickoff(up_machine, self.machines[up_machine][0])
      test_list.append(t)
      t.start()

    while True:
      one_alive_thread = False
      dead_threads = []
      for t in test_list:
        if not t.isAlive():
          dead_threads.append(t)
          lock.acquire()
          output("  %-12s (%10s) ==>" % (t.name, t.ip), False) 
          if t.status == 0:
            good_machines.append(t.name)
            output("DONE")
          else:
            failed_machines.append(t.name)
            output("FAILED")
          lock.release()
        else:
          one_alive_thread = True
      if not one_alive_thread:
        break
      for dead_thread in dead_threads:
        test_list.remove(dead_thread)
      time.sleep(1)
       
    if len(failed_machines) > 0:
      output("WARNING:  %i/%i failed to kick off"\
              % (len(failed_machines), len(self.up_machines)))
    if settings.is_log_ok:
      output("INFO:  Remote logs saved to %s" % Settings.remote_log_path)
      output("INFO:  Local logs saved to %s" % Settings.local_log_path)

  def setup_logging(self):
    # delete old local log directory if it exists
    os.system("rm -rf %s" % Settings.local_log_path)
    try:
      os.mkdir(Settings.local_log_path)
    except OSError, msg:
        Settings.is_log_ok = False
        output(msg)
        output("WARNINGS:  Could not create %s directory!" % \
                Settings.local_log_path)
        output("WARNINGS:  Local logs will not be stored")

  def run(self):
    if not self.check_machines():
      return 1
    self.setup_logging()
    self.execute_tests() 

class Main(object):

  def main(self, argv=None):
    t = Test()
    return t.run()


settings = Settings()

if __name__ == '__main__':
  main_obj = Main();
  sys.exit(main_obj.main())
