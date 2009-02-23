#!/usr/bin/env python

import pyatspi
import time
import subprocess as s
import sys

def printHierarchy (obj, depth):
	indentStr = ""
	print depth, ": ", obj.name, " (", obj.getRoleName(), ")"
	try:
		q = obj.queryText()
		print "  text: ", q.getText(0,-1)
	except NotImplementedError:
		pass
	try:
		q = obj.queryHypertext()
		for i in xrange(q.getNLinks()):
			link = q.getLink (i)
			print "  link ", i, " (", link.startIndex, ",", link.endIndex, "): ", link.getURI(0)
	except NotImplementedError:
		pass
	count = obj.childCount
	for i in range (0, count):
		printHierarchy (obj.getChildAtIndex (i), depth + 1)

reg = pyatspi.Registry
desktop = reg.getDesktop(0)
#obj = pyatspi.findDescendant(desktop, lambda x: (x.name == 'ipy' or x.name == sys.argv[1].split('/')[-1]) and x.getRole() == pyatspi.ROLE_APPLICATION)
#if (obj):
	#printHierarchy (obj, 0)
printHierarchy (desktop, 0)
