
EXTRA_DIST =  

ASSEMBLY_COMPILER_COMMAND = mcs
ASSEMBLY_COMPILER_FLAGS = -lib:@expanded_libdir@/mono/2.0 -lib:../bin -noconfig -codepage:utf8 -warn:4 -warnaserror -optimize+ -debug "-define:DEBUG" -d:NET_2_0 -delaysign+ -keyfile:@abs_top_srcdir@/winfx3.pub

ASSEMBLY = ../bin/UIAutomationHelpers.dll
ASSEMBLY_MDB = $(ASSEMBLY).mdb
COMPILE_TARGET = library
PROJECT_REFERENCES = 
BUILD_DIR = ../bin
PACKAGE_FLAG = /package accessibility

UIAUTOMATIONHELPERS_DLL_MDB_SOURCE=../bin/UIAutomationHelpers.dll.mdb
UIAUTOMATIONHELPERS_DLL_MDB=$(BUILD_DIR)/UIAutomationHelpers.dll.mdb


AL=al2
SATELLITE_ASSEMBLY_NAME=Mono.UIAutomation.Helpers.resources.dll

PROGRAMFILES = \
	$(UIAUTOMATIONHELPERS_DLL_MDB)


RESGEN=resgen2

all: $(ASSEMBLY) $(PROGRAMFILES)

PROJECT_SOURCE_FILES =  \
	Mono.UIAutomation.Helpers/EnvironmentVaribles.cs

FILES =  \
	AssemblyInfo.cs \
	../build/common/*.cs \
	$(PROJECT_SOURCE_FILES)


DATA_FILES = 

RESOURCES =

REFERENCES =  \
	WindowsBase \
	System \
	System.Core

DLL_REFERENCES = 

CLEANFILES = $(PROGRAMFILES)

install-data-local: gac-install

uninstall-local: gac-uninstall

include $(top_srcdir)/Makefile.include


$(eval $(call emit_resgen_targets))
$(build_xamlg_list): %.xaml.g.cs: %.xaml
	xamlg '$<'

$(ASSEMBLY) $(ASSEMBLY_MDB): $(build_sources) $(build_resources) $(build_datafiles) $(DLL_REFERENCES) $(PROJECT_REFERENCES) $(build_xamlg_list) $(build_satellite_assembly_list)
	mkdir -p $(shell dirname $(ASSEMBLY))
	$(ASSEMBLY_COMPILER_COMMAND) $(ASSEMBLY_COMPILER_FLAGS) -out:$(ASSEMBLY) -target:$(COMPILE_TARGET) $(build_sources_embed) $(build_resources_embed) $(build_references_ref)
	sn -q -R $(ASSEMBLY) $(top_srcdir)/mono.snk
