#!/usr/bin/env python

import pyatspi
import time
import subprocess as s
import sys

def printHierarchy (obj, depth):
	indentStr = ""
	print str(depth) + ": " + obj.name + " (" + obj.getRoleName() + "): " + states(obj)
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

def states(acc):
    stateSet = acc.getState()
    states = stateSet.getStates()
    state_strings = []
    for state in states:
        state_strings.append(pyatspi.stateToString(state))
    state_string = ' '.join(state_strings)
    return state_string

def relations(acc):
    # create the relations string
    relations = acc.getRelationSet()
    if relations:
        relation_strings = []
        for relation in relations:
            relation_strings.append( \
                          pyatspi.relationToString(relation.getRelationType()))
        rel_string = ' '.join(relation_strings)
    else:
        rel_string = ''

reg = pyatspi.Registry
desktop = reg.getDesktop(0)
obj = pyatspi.findDescendant(desktop, lambda x: (x.name == 'ipy' or x.name == sys.argv[1].split('/')[-1]) and x.getRole() == pyatspi.ROLE_APPLICATION)
if (obj):
	printHierarchy (obj, 0)
#printHierarchy (desktop, 0)
