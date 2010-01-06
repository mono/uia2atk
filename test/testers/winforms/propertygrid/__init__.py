
##############################################################################
# Written by:  Andres G. Aragoneses <aaragoneses@novell.com>
# Date:        02/24/2009
#              Application wrapper for propertygrid.py
#              Used by the propertygrid-*.py tests
##############################################################################$

'Application wrapper for PropertyGrid'

from strongwind import *

from os.path import exists
from sys import path

def launchPropertyGrid(exe=None):
	'Launch propertygrid with accessibility enabled and return a propertygrid object.  Log an error and return None if something goes wrong'

	if exe is None:
		# make sure we can find the sample application
		harness_dir = path[0]
		i = harness_dir.rfind("/")
		j = harness_dir[:i].rfind("/")
		uiaqa_path = harness_dir[:j]
		exe = '%s/samples/winforms/propertygrid.py' % uiaqa_path
		if not exists(exe):
			raise IOError, "Could not find file %s" % exe

	args = [exe]

	(app, subproc) = cache.launchApplication(args=args, name='ipy', wait=config.LONG_DELAY)

	propertygrid = PropertyGrid(app, subproc)

	cache.addApplication(propertygrid)

	propertygrid.propertyGridFrame.app = propertygrid

	return propertygrid

# class to represent the application
class PropertyGrid(accessibles.Application):

	def __init__(self, accessible, subproc=None): 
		'Get a reference to the PropertyGrid window'
		super(PropertyGrid, self).__init__(accessible, subproc)

		self.findFrame(re.compile('^PropertyGrid control'), logName='Property Grid')
