// Permission is hereby granted, free of charge, to any person obtaining
// a copy of this software and associated documentation files (the
// "Software"), to deal in the Software without restriction, including
// without limitation the rights to use, copy, modify, merge, publish,
// distribute, sublicense, and/or sell copies of the Software, and to
// permit persons to whom the Software is furnished to do so, subject to
// the following conditions:
//
// The above copyright notice and this permission notice shall be
// included in all copies or substantial portions of the Software.
//
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND,
// EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF
// MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND
// NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE
// LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION
// OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION
// WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.
//
// Copyright (c) 2009 Novell, Inc. (http://www.novell.com)
//
// Authors:
//      Andres G. Aragoneses <aaragoneses@novell.com>
//      Brad Taylor <brad@getcoded.net>
//

function MoonlightAccessibilityExtension()
{
  // NOTE: It appears that no global variables are available when observe is
  // called, so we must save all global state inside of this.  LAME.

  this.dump = dump; // use this to print to the console
  this.cc = Components;
  this.EXTENSION_ID = "moonlight-a11y@novell.com";

  this.event = "em-action-requested";

  var componentFile = __LOCATION__;
  var componentsDir = componentFile.parent;
  var extensionDir = componentsDir.parent;
  this.sentinelFile = extensionDir.path + "/extension_disabled";

  this.registered = false;
  this.register();

  // NOTE: We will never get the item-enabled signal, since our observer is
  // never attached.  Instead, we assume that if we are constructed, we are
  // enabled.
  this.enable();
}

MoonlightAccessibilityExtension.prototype = {
  observe: function(subject, topic, data) {
    if (data == "item-disabled") {
      subject.QueryInterface(this.cc.interfaces.nsIUpdateItem);
      if (subject.id == this.EXTENSION_ID)
        this.disable();
    }
  },
  disable: function() {
    var file = this.cc.classes["@mozilla.org/file/local;1"]
                      .createInstance(this.cc.interfaces.nsILocalFile);
    var stream = this.cc.classes["@mozilla.org/network/file-output-stream;1"]
                        .createInstance(this.cc.interfaces.nsIFileOutputStream);
    var converter = this.cc.classes["@mozilla.org/intl/converter-output-stream;1"]
                           .createInstance(this.cc.interfaces.nsIConverterOutputStream);

    file.initWithPath(this.sentinelFile);
    if (file.exists())
      return;

    stream.init(file, 0x02 | 0x08 | 0x20, 0666, 0);
    converter.init(stream, "UTF-8", 0, 0);

    // create a blank file (file.create() doesn't work)
    converter.writeString("");
    converter.close();
  },
  enable: function() {
    var file = this.cc.classes["@mozilla.org/file/local;1"]
                      .createInstance(this.cc.interfaces.nsILocalFile);
    file.initWithPath(this.sentinelFile);
    if (file.exists())
      file.remove(false);
  },
  register: function() {
    if (!this.registered) {
      var observerService = this.cc.classes["@mozilla.org/observer-service;1"]
                                   .getService(this.cc.interfaces.nsIObserverService);
      observerService.addObserver(this, this.event, false);
      this.registered = true;
    }
  },
  unregister: function() {
    if (this.registered) {
      var observerService = this.cc.classes["@mozilla.org/observer-service;1"]
                                   .getService(this.cc.interfaces.nsIObserverService);
      observerService.removeObserver(this, this.event, false);
      this.registered = false;
    }
  },
}

observer = new MoonlightAccessibilityExtension();
