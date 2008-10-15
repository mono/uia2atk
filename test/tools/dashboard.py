import os
import getopt
import sys

def output(s, newline=True):
  if not Settings.is_quiet:
    if newline:
      print s
    else:
      print s,

class Settings(object):

    def __init__(self):
        pass

    def argument_parser(self):
        opts = []
        args = []
        try:
          opts, args = getopt.getopt(sys.argv[1:],"qh",["help","quiet"])
        except getopt.GetoptError:
          self.help()
          sys.exit(1)

        for o,a in opts:
          if o in ("-q","--quiet"):
            Settings.is_quiet = True
        for o,a in opts:
          if o in ("-h","--help"):
            self.help()
            sys.exit(0)

class Dashboard(object):
    def update(self, log):
        pass

    def update_all(self):
        raise NotImplementedError

class XMLparse(object):
    pass

class PageBuilder(object):
    pass

if __name__ == "__main__":
    d = Dashboard()
    d.update_all()
