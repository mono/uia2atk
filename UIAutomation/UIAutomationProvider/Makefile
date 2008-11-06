thisdir = UIAutomationProvider
include ../build/rules.make

LIBRARY = UIAutomationProvider.dll
LIBRARY_PACKAGE = accessibility

LIB_MCS_FLAGS = -r:System -r:UIAutomationBridge -r:UIAutomationTypes -r:WindowsBase
TEST_MCS_FLAGS = $(LIB_MCS_FLAGS) -r:UIAutomationProvider.dll

include ../build/library.make
