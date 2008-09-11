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
  test_type = ""
  email_addresses = []

  def __init__(self):
      self.argument_parser()

  def argument_parser(self):
    opts = []
    args = []
    try:
      opts, args = getopt.getopt(sys.argv[1:],"shql:e:",["smoke","help","quiet","log=","email="])
    except getopt.GetoptError:
      self.help()

    for o,a in opts:
      if o in ("-q","--quiet"):
        Settings.is_quiet = True
    for o,a in opts:
      if o in ("-h","--help"):
        self.help()
        abort(0)
      if o in ("-s","--smoke"):
        Settings.is_smoke = True
      if o in ("-e","--email"):
        Settings.email_addresses = a.split(',')
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
    
  def __init__ (self,name,ip):
    threading.Thread.__init__(self)
    self.ip = ip
    self.status = -1
    self.name = name

  def run(self):
    smoke_option = ""
    if Settings.is_smoke:
      smoke_option = "--smoke"
    self.status = os.system("ssh -o ConnectTimeout=15 %s@%s DISPLAY=:0 \
                             %s/harness/local_run.py %s --log=%s > \
                             %s/%s 2>&1" %\
                             (machines.USERNAME, self.ip, machines.TEST_DIR,\
                              smoke_option, Settings.remote_log_path,\
                              Settings.local_log_path, self.name))

class Test(object):

  def __init__(self):
    self.machines = machines.machines_dict
    self.test_failed_machines = []
    self.test_failed_message = None
    if Settings.email_addresses is not None:
      self.email_message = []

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
      output("WARNING:  %i/%i failed"\
              % (len(failed_machines), len(self.up_machines)))
    if settings.is_log_ok:
      output("INFO:  Local logs saved to %s" % Settings.local_log_path)
    output("INFO:  Remote logs saved to %s" % Settings.remote_log_path)

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

  def parse_logs(self):
    machine_names = []
    for up_machine in self.up_machines:
      machine_names.append(up_machine)
    for machine_name in machine_names:
      try:
        tmp_log_path = "%s" % (os.path.join(Settings.local_log_path, machine_name))
        f = open(tmp_log_path, 'r')
        tmp_log = f.readlines()
        for line in tmp_log:
          if "Traceback" in line:
            self.email_message += ["\n===============%s===============\n" % machine_name]
            self.email_message += tmp_log
            self.test_failed_machines.append(machine_name)
            break
      except IOError, e:
        output("ERROR:  Could not open log file")
        output(e)

    test_type = ""
    if Settings.is_smoke:
      test_type = "Smoke tests"
    else:
      test_type = "Tests"
    self.test_failed_message = "%s %s\n%s" % \
             (test_type,
              "failed for the following device(s):\n",
              "\n".join(self.test_failed_machines))

  def send_mail(self):

    import smtplib

    output("Preparting e-mail e-mail for:")
    for addr in Settings.email_addresses:
      output("  %s" % addr)

    MESSAGE = ("%s\n\nDETAILS:\n%s" % \
                                      ("".join(self.test_failed_message),
                                       "".join(self.email_message)))
    subject = ""
    if Settings.is_smoke:
      subject = "[Smoke Tests] "
    else:
      subject = "[Regression Tests] "

    if len(self.test_failed_machines) <= 0:
      subject += "PASS!"
    else:
      subject += "FAIL!"

    recipients = Settings.email_addresses
    sender = "no-reply"

    headers = "From: %s\r\nTo: %s\r\nSubject: %s\r\n\r\n" % (sender, recipients, subject)
    
    email = "%s%s" % (headers, MESSAGE)   
 
    output("Sending e-mail...", False)
    s = smtplib.SMTP()
    s.connect()
    try:
      s.sendmail(sender, recipients, email)
    except smtplib.SMTPRecipientsRefused:
      output("ERROR:  ALL RECIPIENTS REFUSED")
      s.close()
      return 1
    except smtplib.SMTPHeloError:
      output("ERROR:  No response to the 'HELO' greeting.")
      s.close()
      return 2
    except smtplib.SMTPSenderRefused:
      output("ERROR:  From address refused")
      s.close()
      return 3
    except smtplib.SMTPDataError:
      output("ERROR")
      s.close()
      return 4
    output("OK")
    s.close()
    

class Main(object):

  def main(self, argv=None):
    t = Test()
    r = t.run()
    if Settings.is_log_ok:
      t.parse_logs()
    if Settings.email_addresses is not None:
      t.send_mail() 
    return r

settings = Settings()

if __name__ == '__main__':
  main_obj = Main();
  sys.exit(main_obj.main())
