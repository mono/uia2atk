import threading
import pyatspi

class EventListener(threading.Thread):
    def __init__(self, condition=threading.Condition(), event_names=None):
        threading.Thread.__init__(self)
        self.cond = condition
        self.daemon = False
        self.events = {}
        if not event_names == None:
            self.listenFor(event_names)

    def run(self):
        pyatspi.Registry.start()

    def stop(self):
        pyatspi.Registry.stop()
    
    def listenFor(self, event_names):
        if type(event_names) is str:
            event_names = (event_names, )
        for name in event_names:
            pyatspi.Registry.registerEventListener(self._eventFired, name)

    def containsEvent(self, target, type):
        self.cond.acquire()
        acc = target._accessible
        if self.events.has_key(acc):
            for evt in self.events[acc]:
                if evt.type == type:
                    self.cond.release()
                    return True
        self.cond.release()
        return False

    def clearQueuedEvents(self):
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
