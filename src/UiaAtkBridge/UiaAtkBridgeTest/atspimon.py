#!/usr/bin/env python

##############################################################################
# Written by:  Brian G. Merrell <bgmerrell@novell.com>
# Date:        June 17 2008
# Description: Monitor at-spi events
##############################################################################

import sys
import os
import pyatspi
import atexit
import time
import getopt

# simply takes a string s as input and prints it if running verbosely
def output(s, newline=True):
  if not Settings.is_quiet:
    if newline:
      print s
    else:
      print s,
  if Settings.log_file:
    if not Settings.log_file.closed:
      if newline:
        Settings.log_file.write(s)
      else:
        Settings.log_file.write("%s\n" % s)


def abort(status):
  ''' exit according to status '''
  exit(status)


class Settings(object):

  # static variable, set by ctor
  is_quiet = None
  log_path = None
  source_app = None
  log_file = None

  def __init__(self):
    self.argument_parser()
    try:
      Settings.log_file = open(Settings.log_path, 'w')
    except IOError:
      output("Error opening log file!")
      abort(1)

  def help(self):
    output("Common Options:")
    output("  -h | --help        Print help information (this message).")
    output("  -q | --quiet       Don't print anything.")
    output("  -l | --log=        Where the log should be stored.")
    output("  -s | --source=     The accessible application to monitor.")
    output('                     Use the "Name" column in Accerciser.')

  def argument_parser(self):
    opts = []
    args = []
    try:
      opts, args = getopt.getopt(sys.argv[1:],\
                     "hql:s:",["help","quiet","log=","source="])
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
        Settings.log_path = a
      if o in ("-s","--source"):
        Settings.source_app = a

class Monitor:

  def __init__(self):
    self.events = pyatspi.EVENT_TREE.keys()
    for sub_events in pyatspi.EVENT_TREE.itervalues():
      self.events.extend(sub_events)
    self.events = list(set([event.strip(':') for event in self.events]))
    self.events.sort()


  def _eventFilter(self, event):
    '''
    Determine if an event's source should make the event filtered.
    
    @param event: The given at-spi event.
    @type event: Accessibility.Event
    
    @return: False if the event should be filtered.
    @rtype: boolean
    '''
    if (hasattr(event.source, 'getApplication')):
      app = event.source.getApplication()
      if app:
        app = app.name
        return settings.source_app in app
    return False


  def _handleEvent(self, event):
    '''
    Main at-spi event client. If event passes filtering requirements, log it.
    
    @param event: The at-spi event recieved.
    @type event: Accessibility.Event
    '''

    if not self._eventFilter(event):
      return

    output('%s(%s, %s, %s)\n\tsource: ' % (event.type,
                                           event.detail1,
                                           event.detail2,
                                           event.any_data), False)
    output(str(event.source), False)
    output('\n\tapplication: ', False)
    output(str(event.host_application), False)
    output('\n', False)
 
  
  def _insertEventIntoBuffer(self, event):
    ''' 
    Inserts given event in to text buffer. Creates hyperlinks for
    the events context accessibles.
    
    @param event: The at-spi event we are inserting.
    @type event: Accessibility.Event
    '''
    self._writeText('%s(%s, %s, %s)\n\tsource: ' %
                      (event.type, event.detail1, 
                       event.detail2, event.any_data))
    hyperlink = self._createHyperlink(event.source)
    self._writeText(str(event.source), hyperlink)
    self._writeText('\n\tapplication: ')
    hyperlink = self._createHyperlink(event.host_application)
    self._writeText(str(event.host_application), hyperlink)
    self._writeText('\n') 

  def begin(self):
    output("Registering Event Listener...")
    pyatspi.Registry.registerEventListener(self._handleEvent,
                                             *self.events)
    pyatspi.Registry.start()
  
  def end(self):
    output("Deregistering Event Listener...")
    pyatspi.Registry.deregisterEventListener(self._handleEvent,
                                               *self.events)
    pyatspi.Registry.stop()
    Settings.log_file.close()

class Main(object):

  def main(self, argv=None):
    m = Monitor()
    try:
      m.begin() 
    except KeyboardInterrupt:
      m.end()

settings = Settings()

if __name__ == '__main__':
  main_obj = Main();
  sys.exit(main_obj.main())
