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
  is_quiet = False
  remote_log_path = machines.LOG_DIR
  local_log_path = "/tmp/uiaqa"
  is_log_ok = True
  COUNTDOWN = 5
  is_smoke = False
  email_addresses = None
  from_address = "no-reply"
  should_update = False
  package_failed_machines = []
  test_failed_machines = []
  
  def __init__(self):
      self.argument_parser()

  def argument_parser(self):
    opts = []
    args = []
    try:
      opts, args = getopt.getopt(sys.argv[1:],"ushql:e:f:",["smoke","help","quiet","log=","email=","update","from="])
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
      if o in ("-s","--smoke"):
        Settings.is_smoke = True
      if o in ("-u","--update"):
        Settings.should_update = True
      if o in ("-e","--email"):
        Settings.email_addresses = a.split(',')
      if o in ("-f","--from"):
        Settings.from_address = a
      if o in ("-l","--log"):
        Settings.local_log_path = a
        if not os.path.exists(Settings.local_log_path):
          output("ERROR:  Log path does not exist.")
          abort(1)

  def help(self):
    output("Common Options:")
    output("  -h | --help    Print help information (this message).")
    output("  -q | --quiet   Don't print anything.")
    output("  -l | --log=    Where the log(s) should be stored.")
    output("  -s | --smoke   Run only smoke tests.")
    output("  -u | --update  Update packages on remote machines")
    output("  -e | --email=  Send e-mail results to comma delineated recipients")

class Ping(threading.Thread):

   def __init__ (self,name,ip):
      threading.Thread.__init__(self)
      self.ip = ip
      self.status = -1
      self.name = name
   def run(self):
      self.status = os.system("ping -q -c2 %s > /dev/null" % self.ip)

class Kickoff(threading.Thread):

  package_failed_machines = []
  test_failed_machines = []
    
  def __init__ (self,name,ip):
    threading.Thread.__init__(self)
    self.ip = ip
    self.test_status = 0
    self.pkg_status = 0
    self.name = name

  def run(self):
    smoke_option = lambda: Settings.is_smoke == True and "--smoke" or ""
    update_option = lambda: Settings.is_smoke == True and "--update" or ""
    if self.pkg_status == 0:
      self.test_status = os.system("ssh -o ConnectTimeout=15 %s@%s DISPLAY=:0 python -u %s/harness/local_run.py %s --log=%s >> %s/%s 2>&1" %\
                          (machines.USERNAME, self.ip,
                           machines.TEST_DIR,
                           " ".join([smoke_option(), update_option()]).strip(),
                           Settings.remote_log_path,
                           Settings.local_log_path,
                           self.name))
      if self.test_status != 0:
        Kickoff.test_failed_machines.append(self.name)

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
    s = ""
    if Settings.is_smoke:
      s = "smoke "
    output("Kicking off remote %stests..." % s)
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
          output("  TEST COMPLETE:  %-12s (%10s) ==>" % (t.name, t.ip), False) 
          if t.pkg_status == 0 and t.test_status == 0:
            good_machines.append(t.name)
            output("OK")
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
      output("WARNING:  %i/%i failed"\
              % (len(failed_machines), len(self.up_machines)))
    if settings.is_log_ok:
      output("INFO:  Local logs saved to %s" % Settings.local_log_path)
    output("INFO:  Remote logs saved to %s" % Settings.remote_log_path)
    if t.pkg_status == 0 and t.test_status == 0:
      return 0
    else:
      return 1

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
    return self.execute_tests() 

  def compose_mail_message(self):
  
    import urllib

    test_type = lambda: Settings.is_smoke == True and "smoke tests" or "tests"
    status = lambda: len(Kickoff.package_failed_machines) + \
                    len(Kickoff.test_failed_machines) > 0 \
                    and "failed" or "succeeded"
  
    revisions = urllib.urlopen("http://build1.sled.lab.novell.com/uia/current/rpm_revs").read()

    # mathematic union of the failed machines, we will use this to grab
    # the local logs we want to include in the e-mail
    failed_machines = list(set(Kickoff.package_failed_machines + \
                                                Kickoff.test_failed_machines))

    # summary message for the first part of the e-mail
    self.summary_message = []

    if (len(Kickoff.test_failed_machines) > 0):
      self.summary_message.append("%s\n%s\n\n" % \
                ("Strongwind tests failed for the following device(s):\n",
                "\n".join(failed_machines)))

    if (len(Kickoff.package_failed_machines) > 0):
      self.summary_message.append("%s\n%s\n\n" % \
                ("Package updates failed for the following device(s):\n",
                "\n".join(Kickoff.package_failed_machines)))

    self.summary_message.append("The above %s %s for the following packages:\n\n%s\n\n" % (test_type(), status(), revisions))

    
    # add local logs to the detailed message, which will be the second
    # part of the e-mail
    self.detailed_message = []
    for machine_name in failed_machines:
      try:
        tmp_log_path = "%s" % (os.path.join(Settings.local_log_path, machine_name))
        f = open(tmp_log_path, 'r')
        tmp_log = f.readlines()
        self.detailed_message += ["\n===============%s===============\n" % machine_name]
        self.detailed_message += tmp_log
      except IOError, e:
        output("ERROR:  Could not open log file")
        output(e)

  def send_mail(self):

    import smtplib

    output("Preparing e-mail for:")
    for addr in Settings.email_addresses:
      output("  %s" % addr)

    MESSAGE = ("%s\n\nDETAILS:\n%s" % \
                                      ("".join(self.summary_message),
                                       "".join(self.detailed_message)))
    subject = ""
    if Settings.is_smoke:
      subject = "[Smoke Tests] "
    else:
      subject = "[Regression Tests] "

    if len(Kickoff.test_failed_machines) + \
                                   len(Kickoff.package_failed_machines) <= 0:
      subject += "PASS!"
    else:
      subject += "FAIL!"

    to_addrs = ' '.join(Settings.email_addresses)
    from_addr = Settings.from_address

    try:
      f = open("%s/email" % Settings.local_log_path,'w')
      f.write(MESSAGE)      
      f.close()
      output("Sending e-mail...", False)
      email_cmd = 'mailx -s "%s" -r "%s" %s < %s/email' % \
                      (subject, from_addr, to_addrs, Settings.local_log_path)
      r = os.system(email_cmd)
      assert r == 0 
      output("OK")
    except IOError:
      outupt("ERROR")
    except AssertionError:
      output("ERROR")
   

class Main(object):

  def main(self, argv=None):
    t = Test()
    r = t.run()
    if Settings.email_addresses is not None:
      if Settings.is_log_ok:
        t.compose_mail_message()
      t.send_mail() 
    return r

settings = Settings()

if __name__ == '__main__':
  main_obj = Main();
  sys.exit(main_obj.main())
