#!/usr/bin/env python

##
# Written by:  Brad Taylor <brad@getcoded.net>
# Date:        02/24/2009
##

import threading
import pyatspi

class EventListener(threading.Thread):
    """
    Listens for events using pyatspi.Registry and provides an easy mechanism
    to test that an event was fired.
    
    Example:

    >>> from strongwind import *
    Error importing yaml module; tags will not be parsed
    eventlistener: 
    >>> import pyatspi
    >>> import eventlistener
    >>> desktop = pyatspi.Registry.getDesktop(0)
    >>> nautilus = Application(pyatspi.findDescendant(desktop, lambda x: x.name == 'nautilus', breadth_first=True))
    >>> icon_view = nautilus.findLayeredPane('Icon View')    
    >>> icon_view.clearSelection()
    >>> listener = EventListener(event_types='object:selection-changed')
    >>> listener.start()
    >>> icon_view.selectChild(0)
    >>> # Make sure to stop the listener as soon as you're done testing.
    >>> listener.stop()
    >>> listener.containsEvent(icon_view, 'object:selection-changed')
    True
    >>> listener.getNumEvents(icon_view, 'object:selection-changed')
    1
    >>> listener.getNumEvents(icon_view)
    1
    >>> listener.getNumEvents()
    1
    """
    def __init__(self, condition=threading.Condition(), event_types=None):
        threading.Thread.__init__(self)
        self.cond = condition
        self.daemon = False
        self.events = {}
        self.event_types = ()
        if not event_types == None:
            self.listenFor(event_types)

    def run(self):
        """
        Runs pyatspi.Registry's main loop in a thread.  This should not be used
        externally.  Instead, use .start().
        """
        pyatspi.Registry.start()

    def stop(self):
        """
        Stops the pyatspi.Registry's main loop, causing it to stop listening
        to events.  This _must_ be done before a test is completed, otherwise,
        python will hang waiting for the thread to stop executing.
        """
        for name in self.event_types:
            pyatspi.Registry.deregisterEventListener(self._eventFired, name)

        pyatspi.Registry.stop()
    
    def listenFor(self, event_types):
        """
        Tells the EventListener instance what types of events to listen for.
        
        @param event_types: a Tuple or string of the event type(s)
        """
        for name in self.event_types:
            pyatspi.Registry.deregisterEventListener(self._eventFired, name)

        if type(event_types) is str:
            event_types = (event_types, )
        
        self.event_types = event_types
        for name in event_types:
            pyatspi.Registry.registerEventListener(self._eventFired, name)

    def containsEvent(self, target, type):
        """
        Returns if the EventListener has seen a event of the given type on the
        given target.
        
        @param target: the requested event target
        @param type: the type of event to listen for
        @return: returns True if we've seen the event, False otherwise
        @rtype: boolean
        """
        self.cond.acquire()
        acc = target._accessible
        if self.events.has_key(acc):
            for evt in self.events[acc]:
                if evt.type == type:
                    self.cond.release()
                    return True
        self.cond.release()
        return False

    def getNumEvents(self, target=None, type=None):
        """
        Returns the number of events matching the provided critera.
        
        @param target: the requested event target, or None
        @param type: the type of event to listen to, or None
        @return: the number of events that match
        @rtype: int
        """
        self.cond.acquire()
        ret = 0
        if target == None:
            ret = sum([len(t) for a, t in self.events.iteritems()])
        elif self.events.has_key(target._accessible):
            if type == None:
                ret = len(self.events[target._accessible])
            else:
                ret = sum([int(e.type == type) for e in self.events[target._accessible]])
        self.cond.release()
        return ret

    def clearQueuedEvents(self):
        """
        Clears the internal list of events used for querying.
        """
        self.cond.acquire()
        self.events.clear()
        self.cond.release()
    
    def _eventFired(self, evt):
        self.cond.acquire()
        if not self.events.has_key(evt.source):
            self.events[evt.source] = []
        self.events[evt.source].append(evt)
        self.cond.notify()
        self.cond.release()

if __name__ == "__main__":
    import doctest
    doctest.testmod()
