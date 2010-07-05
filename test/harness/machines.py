#!/usr/bin/env python

##############################################################################
# Written by:  Brian G. Merrell <bgmerrell@novell.com>
# Date:        May 23 2008
# Description: A description of the remote test machines
#              
#              
##############################################################################

# A common test directory on all machines (where test files are copied and
# executed
TEST_DIR = "/home/a11y/tests"

LOG_DIR = "/home/a11y/logs"

USERNAME = "a11y"

# Dictionary of available machines
'''
ENABLE ALL THESE MACHINES ONCE TEST ENVIRONMENTS HAVE BEEN SET UP IN THE LAB
machines_dict = {
  "ubuntu32v0" : ("151.155.248.167", "ubuntu32v0.sled.lab.novell.com"),
  "ubuntu64v0" : ("151.155.248.168", "ubuntu64v0.sled.lab.novell.com"),
  "fedora32v0" : ("151.155.248.169", "fedora32v0.sled.lab.novell.com"),
  "fedora64v0" : ("151.155.248.170", "fedora64v0.sled.lab.novell.com"),
  "suse32v0" : ("151.155.248.171", "suse32v0.sled.lab.novell.com"),
  "suse64v0" : ("151.155.248.172", "suse64v0.sled.lab.novell.com"),
  "fakemachine" : ("151.155.224.100", "fakemachine.sled.lab.novell.com"
}
'''
machines_dict = {
  #'oS111-32' : ('147.2.207.217', ''),
  #'oS111-64' : ('147.2.207.218', ''),
  #'oS112-32' : ('147.2.207.219', ''),
  #'oS112-64' : ('147.2.207.220', ''),
  #'ubuntu910-32' : ('147.2.207.221', ''),
  'ubuntu910-64' : ('147.2.207.222', ''),
  #'fedora12-32' : ('147.2.207.223', ''),
  #'fedora12-64' : ('147.2.207.224', '')
}
