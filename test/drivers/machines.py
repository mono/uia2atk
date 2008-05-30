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
TEST_DIR = "/home/a11y/test"

# Dictionary of available machines
machine_dict = {
  "uiaqa" : ("151.155.248.166", "uiaqa.sled.lab.novell.com"),
  "ubuntu32v0" : ("151.155.248.167", "ubuntu32v0.sled.lab.novell.com"),
  "ubuntu64v0" : ("151.155.248.168", "ubuntu64v0.sled.lab.novell.com"),
  "fedora32v0" : ("151.155.248.169, fedora32v0.sled.lab.novell.com"),
  "fedora64v0" : ("151.155.248.170, fedora64v0.sled.lab.novell.com"),
  "suse32v0" : ("151.155.248.171, suse32v0.sled.lab.novell.com"),
  "suse64v0" : ("151.155.248.172, suse64v0.sled.lab.novell.com")
}


