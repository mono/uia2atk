thisdir = UIAutomationClient
include ../build/rules.make

LIBRARY = UIAutomationClient.dll
LIBRARY_PACKAGE = accessibility

LIB_MCS_FLAGS = -r:System -r:WindowsBase -r:UIAutomationTypes -r:UIAutomationProvider
TEST_MCS_FLAGS = $(LIB_MCS_FLAGS) -r:UIAutomationClient.dll

include ../build/library.make
