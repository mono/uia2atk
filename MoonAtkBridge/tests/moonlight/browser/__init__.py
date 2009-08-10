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
import signal

from strongwind import *

def launchAddress(uri, browser='firefox', profile='dev'):
    """
    Launch a browser with the selected uri and return a Browser object.
    """
    if os.environ.has_key('MOON_A11Y_BROWSER'):
        browser = os.environ['MOON_A11Y_BROWSER']
    else:
        print "** MOON_A11Y_BROWSER environment variable not found.  Defaulting to '%s'." % browser

    if os.environ.has_key('MOON_A11Y_BROWSER_PROFILE'):
        dev = os.environ['MOON_A11Y_BROWSER_PROFILE']
    else:
        print "** MOON_A11Y_BROWSER_PROFILE environment variable not found.  Defaulting to '%s'." % profile

    cwd = os.path.dirname(browser)
    args = [browser, '-no-remote', '-P', profile, uri]
    name = 'Minefield'

    # TODO: Remove this when we have linking steps set up
    env = os.environ
    env['MOON_DISABLE_SECURITY_PREVIEW_7'] = '1'
    
    logString = 'Launch %s.' % name

    (app, proc) = cache.launchApplication(args=args, name=name, cwd=cwd,
                                          find=re.compile('^Minefield$'),
                                          wait=config.LONG_DELAY, cache=True,
                                          env=env, logString=logString)
    browser = MinefieldBrowser(app, proc)
    cache.addApplication(browser)
    return browser


# TODO: What if we want to test with Firefox, not Minefield?
class MinefieldBrowser(accessibles.Application):
    logName = 'Mozilla Firefox'

    def __init__(self, accessible, subproc):
        """
        Get a refrence to the main browser window.
        """
        super(MinefieldBrowser, self).__init__(accessible, subproc)
        
        self.proc = subproc

        self.findFrame(re.compile('^(.*) - Minefield$'), logName='Main')
        self.slControl = self.mainFrame.findFiller('Silverlight Control')

    def kill(self):
        os.killpg(self.proc.pid, signal.SIGKILL)
