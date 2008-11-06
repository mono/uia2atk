thisdir = class

# For now, Mono.Security.Win32 is built on Linux too.
# However, it isn't tested on Linux, since it depends on a Win32 dll.
net_3_0_dirs = \
	System.Workflow.ComponentModel	\
	System.Workflow.Activities	\
	System.Workflow.Runtime		\
	System.Workflow.ComponentModel	\
	System.Workflow.Activities	\
	PresentationCore		\
	PresentationFramework		\
	UIAutomationTypes		\
	UIAutomationBridge		\
	UIAutomationProvider		\
	UIAutomationClient

net_3_0_SUBDIRS := $(net_3_0_dirs)

SUBDIRS = $(net_3_0_dirs)

include ../build/rules.make

DIST_PROFILE_DIRS = net_3_0

dist-local: dist-default
	-$(MKINSTALLDIRS) $(DIST_PROFILE_DIRS:%=$(distdir)/lib/%)


DISTFILES = msfinal.pub winfx3.pub mono.snk mono.pub ecma.pub

.PHONY: all-local $(STD_TARGETS:=-local)
all-local $(STD_TARGETS:=-local):
	@:

