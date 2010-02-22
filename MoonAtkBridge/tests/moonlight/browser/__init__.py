# Permission is hereby granted, free of charge, to any person obtaining
# a copy of this software and associated documentation files (the
# "Software"), to deal in the Software without restriction, including
# without limitation the rights to use, copy, modify, merge, publish,
# distribute, sublicense, and/or sell copies of the Software, and to
# permit persons to whom the Software is furnished to do so, subject to
# the following conditions:
#
# The above copyright notice and this permission notice shall be
# included in all copies or substantial portions of the Software.
#
# THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND,
# EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF
# MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND
# NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE
# LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION
# OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION
# WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.
#
# Copyright (c) 2009 Novell, Inc. (http://www.novell.com)
#
# Authors:
#      Brad Taylor <brad@getcoded.net>
#

import os
import re
import signal

from time import sleep
from strongwind import *

def launchAddress(uri, browser='firefox', profile='dev', name='Namoroka', findSLControl=True):
    """
    Launch a browser with the selected uri and return a Browser object.
    """
    if os.environ.has_key('MOON_A11Y_BROWSER'):
        browser = os.environ['MOON_A11Y_BROWSER']
    else:
        print "** MOON_A11Y_BROWSER environment variable not found.  Defaulting to '%s'." % browser

    if os.environ.has_key('MOON_A11Y_BROWSER_PROFILE'):
        profile = os.environ['MOON_A11Y_BROWSER_PROFILE']
    else:
        print "** MOON_A11Y_BROWSER_PROFILE environment variable not found.  Defaulting to '%s'." % profile

    if os.environ.has_key('MOON_A11Y_BROWSER_NAME'):
        name = os.environ['MOON_A11Y_BROWSER_NAME']
    else:
        print "** MOON_A11Y_BROWSER_NAME environment variable not found.  Defaulting to '%s'." % name

    cwd = os.path.dirname(browser)
    if cwd == '':
        cwd = None

    args = [browser, '-no-remote', '-P', profile, uri]

    logString = 'Launch %s.' % name

    (app, proc) = cache.launchApplication(args=args, name=name, cwd=cwd,
                                          find=re.compile('^%s$' % name),
                                          wait=config.LONG_DELAY, cache=True,
                                          logString=logString, setpgid=True)
    browser = FirefoxBrowser(name, app, proc, findSLControl)
    cache.addApplication(browser)
    return browser

def kill_proc(proc):
    proc.poll()
    if self.proc.returncode:
        return True

    os.killpg(proc.pid, signal.SIGKILL)


class FirefoxBrowser(accessibles.Application):
    logName = 'Mozilla Firefox'
    appNameRegex = '^(.*) - (.*)%s$'

    def __init__(self, name, accessible, subproc, findSLControl):
        """
        Get a refrence to the main browser window.
        """
        try:
            super(FirefoxBrowser, self).__init__(accessible, subproc)

            self.proc = subproc

            self.findFrame(re.compile(self.appNameRegex % name), logName='Main')
            self.tabs = self.mainFrame.findAllPageTabLists('')[0]
            self.locationBar = self.mainFrame.findEntry('Search Bookmarks and History')
	    if findSLControl:
                self.slControl = self.mainFrame.findFiller('Silverlight Control')
        except:
            self.kill()
            raise

    def kill(self, should_assert=True):
        # first, verify that the app is running
        self.proc.poll()
        if not self.proc.returncode:
            # close it nicely
            self.mainFrame.altF4()
            sleep(config.MEDIUM_DELAY)

        try:
            self.proc.poll()
            if not self.proc.returncode:
                # still alive?  kill it!
                os.killpg(self.proc.pid, signal.SIGKILL)
                self.proc.wait()

                if should_assert:
                    raise AssertionError('Firefox has not exited cleanly.')
        except:
            print "Unable to send pid %d SIGKILL..." % self.proc.pid

    def assertUrlOpened(self, url):
        for alert in self.mainFrame.findAllAlerts(''):
            labels = alert.findAllLabels('')
            if len(labels) == 0:
                continue

            if not re.match('(.+) prevented this site from opening a pop-up window.', labels[0].text):
                continue

            self.mainFrame.findPushButton('Preferences').press()

            # LAME: This menu isn't accessible!
            # self.mainFrame.findMenuItem("Show '%s'" % url).click()

            self.mainFrame.keyCombo('Down')
            self.mainFrame.keyCombo('Down')
            self.mainFrame.keyCombo('Down')
            self.mainFrame.keyCombo('Down')
            self.mainFrame.keyCombo('Return')

        # wait for the URL to load
        sleep(config.LONG_DELAY)

        # TODO: preserve the tab the user was on before assertUrlOpened was
        # called
        for i in xrange(0, self.tabs.childCount):
            tab = self.tabs.getChildAtIndex(i)
            tab.switch()

            def assertLocation():
                return self.locationBar.text == url

            if utils.retryUntilTrue(assertLocation):
                return True

        return False
