EXTRA_DIST = \
	expansions.m4 \
	mono.snk \
	winfx3.pub

if HAS_MONO_2_5
 CODE_SUBDIRS = UIAutomationTypes UIAutomationBridge UIAutomationProvider UIAutomationSource UIAutomationClient
else
 CODE_SUBDIRS = WindowsBase UIAutomationTypes UIAutomationBridge UIAutomationProvider UIAutomationSource UIAutomationClient
endif

if ENABLE_TESTS
 SUBDIRS = $(CODE_SUBDIRS) UIAutomationClientTests data
else
 SUBDIRS = $(CODE_SUBDIRS) data
endif
