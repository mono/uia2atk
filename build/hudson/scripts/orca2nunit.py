#!/usr/bin/env python

import sys 
import re
import os.path
import time

#XML import stuff
try:
    import xml.etree.ElementTree as ET
except ImportError:
    try:
        import cElementTree as ET # cElementTree is faster
    except ImportError:
        import elementtree.ElementTree as ET # fallback on regular ElementTree

class OrcaTestRun:
    test_suites = []
    start_time = 0
    stop_time = 0
    total_tests = 0
    total_pass = 0
    total_fail = 0

class TestSuite:
    def __init__(self):
        self.name = ""
        self.num_tests = 0
        self.tests = []

class Test:
    def __init__(self):
        self.status = False
        self.log = ""
        self.num = ""
        self.stack_trace = []

class Main:
    def main(self):
        # make sure a file name was passed from the command line
        try:
            assert len(sys.argv) == 2
        except AssertionError:
            print "You must provide a file to open"
        self.file_name = sys.argv[1]
        # open the file
        try:
            self.f = open(self.file_name, 'r')
        except Exception, e:
            print e

        # perform the actual parsing
        self.parse()
        self.nunit_writer()
	# Workaround for Issue 2746 (http://bugs.python.org/issue2746)
        # there are other <> that cause the xml to appear malformed
	# self.clean_xml()

    def parse(self):
        
        # indicate whether the current line is between two test_deliniator_re
        # regular expressions
        in_test = False

        # set some regular expressions to match lines in our tests
        test_re = re.compile("^Test [0-9]+ of [0-9]+ [SUCCEEDED|FAILED]")
        test_deliniator_re = re.compile("^======================")
        test_failure_dumb_re = re.compile("^\[FAILURE WAS")
        test_failure_dumb_loop = False

        for line in self.f:
            if test_deliniator_re.match(line):
                in_test = not in_test
                if in_test:
                    print "Initializing new test suite..."
                    ts = TestSuite()
                else:
                    OrcaTestRun.total_tests += ts.num_tests
                    OrcaTestRun.test_suites.append(ts)
                    print ts.name
            # if the line exists between two test_deliniator_re regular
            # expressions
            if in_test:
                if line.startswith("Running "):
                    print "Test Suite Name %s" % ts.name
                    ts.name = os.path.basename(line.split()[-1])
                    print "Test Suite Name %s" % ts.name
                if test_re.match(line):
                    print "Initializing new test..."
                    line_split = line.split()
                    t = Test()
                    t.num = line_split[1]
                    ts.num_tests = int(line_split[3])
                    if line_split[4] == "SUCCEEDED:":
                        t.status = True
                        OrcaTestRun.total_pass += 1
                        #print "PASS Count %s" %
                    else:
                        t.status = False
                        OrcaTestRun.total_fail += 1
                        test_failure_dumb_loop = True
                    t.log = line.split("%s:" % ts.name)[-1]
                    print ("Adding Test Case " + t.num + " to Test Suite " + ts.name )
                    ts.tests.append(t)
                if test_failure_dumb_re.match(line):
                    test_failure_dumb_loop = False
                if test_failure_dumb_loop:
                    t.stack_trace.append(line)
            else:  
                # this should always be the last line if it doesn't match the
                # test_deliniator_re
                if not test_deliniator_re.match(line):
                    time_split = line.split() 
                    OrcaTestRun.start_time = time_split[0]
                    OrcaTestRun.end_time = time_split[-1]
                



    def nunit_writer(self):
        # Building the root element - test-results
        root = ET.Element("test-results")
        root.attrib['name'] = 'orca uia tests'
        root.attrib['total'] = str(OrcaTestRun.total_tests)
        root.attrib['failures'] = str(OrcaTestRun.total_fail)
        root.attrib['not-run'] = str(OrcaTestRun.total_tests - (OrcaTestRun.total_pass + OrcaTestRun.total_fail))
        root.attrib['date'] = time.strftime("%Y-%m-%d")
        root.attrib['time'] = time.strftime("%H:%M:%S")

        # Building sub element - environment
        environment = ET.SubElement(root, "environment")
        environment.attrib['nunit-version'] = "0"
        environment.attrib['clr-version'] = "0"
        environment.attrib['os-version'] = "openSUSE 11.1 static"
        environment.attrib['platform'] = "Linux"
        environment.attrib['cwd'] = "NA"
        environment.attrib['machine-name'] = "hudson"
        environment.attrib['user'] = "a11y"
        environment.attrib['user-domain'] = "hudson"

        # Building sub element - culture info
        culture = ET.SubElement(root, "culture-info")
        culture.attrib['current-culture'] = ""
        culture.attrib['current-uiculture'] = ""

        # Building test-suite nodes
        print OrcaTestRun.test_suites
        test_suite = OrcaTestRun.test_suites[0]
        print test_suite
        print len(test_suite.tests)
        test_suite = OrcaTestRun.test_suites[1]
        print test_suite
        print len(test_suite.tests)
        test_suite = OrcaTestRun.test_suites[2]
        print test_suite
        print len(test_suite.tests)

        for suite in OrcaTestRun.test_suites:
            # Building sub element - test-suite
            print ("Test Suite " + suite.name)
            ts = ET.SubElement(root, "test-suite")
            ts.attrib['name'] = suite.name
            ts.attrib['success'] = "True"
            ts.attrib['time'] = "0.000"
            ts.attrib['asserts'] = "0"
            results = ET.SubElement(ts, "results")

            # Building test case nodes
            for test in suite.tests:
                print ("Test Case " + test.num)
                tc = ET.SubElement(results, "test-case")
                tc.attrib['name'] = "Test %s" % test.num
                tc.attrib['executed'] = "True"
                tc.attrib['success'] = str(test.status)
                tc.attrib['time'] = "0.000"
                tc.attrib['asserts'] = "0"
                if not test.status:
                    failure = ET.SubElement(tc, "failure")
                    message = ET.SubElement(failure, "message")
		    print "Adding CDATA"
                    message.text = '<![CDATA[%s]]>' % test.log
                    stack_trace_xml = ET.SubElement(failure, "stack-trace")
                    stack_trace_xml.text = ''.join(test.stack_trace)



        f = open('TestResults.xml', 'w')
        f.write('<?xml version="1.0" encoding="utf-8" standalone="no"?>')
        ET.ElementTree(root).write(f)
        f.close()

    def clean_xml(self):
        f = open('TestResults.xml', 'r')
        c = open('testresults.xml', 'w')
        text = f.read()
        f.close()
        new_line = text.replace("&lt;", "<").replace("&gt;", ">")
        c.write(new_line)
        c.close()

if __name__ == '__main__':
    m = Main()
    sys.exit(m.main())
