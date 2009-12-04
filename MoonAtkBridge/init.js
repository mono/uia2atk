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
//


//ctor
function myA11yObserver()
{
  this.mydump = dump;
  this.cc = Components;
  this.observerService = this.cc.classes["@mozilla.org/observer-service;1"]
                         .getService(this.cc.interfaces.nsIObserverService);
  this.fileLocalClass = this.cc.classes["@mozilla.org/file/local;1"];
  this.event = "quit-application";

  var componentFile = __LOCATION__;
  var componentsDir = componentFile.parent;
  var extensionDir = componentsDir.parent;
  this.controlFilePath = extensionDir.path + "/active.xml";

  this.register();
  this.enable();
}

myA11yObserver.prototype = {
  observe: function(subject, topic, data) {
    //this.mydump ("__DEBUG: myA11yObserver::observe()\n");

    this.write("<root><active>false</active></root>")
  },
  register: function() {
    this.observerService.addObserver(this, this.event, false);
  },
  unregister: function() {
    this.observerService.removeObserver(this, this.event);
  },
  enable: function() {
    //this.mydump ("__DEBUG: myA11yObserver::enable()\n");

    this.write("<root><active>true</active></root>");
  },
  write: function(data) {
    var file = this.fileLocalClass.createInstance(this.cc.interfaces.nsILocalFile);
    file.initWithPath(this.controlFilePath);

    var foStream = this.cc.classes["@mozilla.org/network/file-output-stream;1"]
                   .createInstance(this.cc.interfaces.nsIFileOutputStream);

    foStream.init(file, 0x02 | 0x08 | 0x20, 0666, 0); 
    var converter = this.cc.classes["@mozilla.org/intl/converter-output-stream;1"]
                    .createInstance(this.cc.interfaces.nsIConverterOutputStream);
    converter.init(foStream, "UTF-8", 0, 0);
    converter.writeString(data);
    converter.close();
  }

}

//dump ("__DEBUG: moonlight-a11y@novell.com : init.js\n");

observer = new myA11yObserver();

