# vim: set ts=4 sw=4 expandtab et: coding=UTF-8
#!/usr/bin/env python

import os
import re
import getopt
import sys
import subprocess

try:
    import xml.etree.ElementTree as ET
except ImportError:
    try:
        import cElementTree as ET # cElementTree is faster
    except ImportError:
        import elementtree.ElementTree as ET # fallback on regular ElementTree

def output(s, newline=True):
    if not Settings.is_quiet:
        if newline:
            print s
        else:
            print s,

def abort(s):
    sys.exit(s)

class Settings(object):

    svn_trunk = "svn://anonsvn.mono-project.com/source/trunk"
    svn_branches = "svn://anonsvn.mono-project.com/source/branches"
    svn_tags = "svn://anonsvn.mono-project.com/source/tags"
    is_quiet = False
    log_dir = None
    svn_revision = 0
    workspace_path = none
    project_name = none


    def __init__(self):
        pass

    def argument_parser(self):
        opts = []
        args = []

        try:
            opts, args = getopt.getopt(sys.argv[1:], "qhrwp:",["help","quiet","revision=","workspace=","project="])
        except getopt.GetoptError:
            self.help()
            abort(1)

        for o,a in opts:
            if o in ("-q","--quit"):
                Settings.is_quiet = True
        for o,a in opts:
            if o in ("-h","--help"):
                self.help()
                sys.exit(0)
            if o in ("-r","--revision"):
                Settings.revision = a
            if o in ("-w","--workspace"):
                Settings.workspace_path = a
            if o in ("-p","--project"):
                Settings.project_name = a

        try:
            Settings.log_dir = args[0]
        except IndexError, e:
            output("Usage: mktarball ")
            output("  -h                  | --help                       Print help information (this message)")
            output("  -q                  | --quiet                      Don't print anything")
            output("  -r <svn revision>   | --revision=<svn revision>    Last changed svn revision")
            output("  -w <workspace path> | --workspace=<workspace path> Path to hudson workspace")
            output("  -p <project>        | --project=<project name>     The name of the project to be built")
            

class build(object):

   def __init__(self):
        pass 


if __name__ == "__main__":
    s = Settings()
    s.argument_parser()
    
