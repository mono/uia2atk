#!/usr/bin/env python
import xml.etree.ElementTree as ET
tree = ET.ElementTree()
tree.parse("uia.xml")
modules = tree.find("modules") 
for module in modules:
  if module.tag == "gtk-sharp212":
    m = module
    deps = m.find("dependencies")
    for dep in deps:
      print dep.text
