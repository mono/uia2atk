
EXTRA_DIST =  

# Warning: This is an automatically generated file, do not edit!

if ENABLE_DEBUG
ASSEMBLY_COMPILER_COMMAND = gmcs
ASSEMBLY_COMPILER_FLAGS = -lib:@expanded_libdir@/mono/2.0 -lib:../lib/Debug -lib:@expanded_libdir@/mono/accessibility -noconfig -codepage:utf8 -warn:4 -warnaserror -optimize+ -debug "-define:DEBUG" -keyfile:../mono.snk

ASSEMBLY = ../lib/Debug/UIAutomationBridge.dll
ASSEMBLY_MDB = $(ASSEMBLY).mdb
COMPILE_TARGET = library
PROJECT_REFERENCES = 
BUILD_DIR = ../lib/Debug

UIAUTOMATIONWINFORMS_DLL_MDB_SOURCE=../lib/Debug/UIAutomationBridge.dll.mdb
UIAUTOMATIONWINFORMS_DLL_MDB=$(BUILD_DIR)/UIAutomationBridge.dll.mdb

endif

if ENABLE_RELEASE
ASSEMBLY_COMPILER_COMMAND = gmcs
ASSEMBLY_COMPILER_FLAGS = -lib:@expanded_libdir@/mono/2.0 -lib:../lib/Release -lib:@expanded_libdir@/mono/accessibility -keyfile:../mono.snk -noconfig -codepage:utf8 -warn:4 -warnaserror -optimize+
ASSEMBLY = ../lib/Release/UIAutomationBridge.dll
ASSEMBLY_MDB = 
COMPILE_TARGET = library
PROJECT_REFERENCES = 
BUILD_DIR = ../lib/Release

UIAUTOMATIONWINFORMS_DLL_MDB=

endif

AL=al2
SATELLITE_ASSEMBLY_NAME=Mono.UIAutomation.Bridge.resources.dll

PROGRAMFILES = \
	$(UIAUTOMATIONWINFORMS_DLL_MDB)  


RESGEN=resgen2
	
all: $(ASSEMBLY) $(PROGRAMFILES)

FILES =  \
	Assembly/AssemblyInfo.cs \
	../build/common/*.cs \
	Mono.UIAutomation.Bridge/IAutomationBridge.cs \
	Mono.UIAutomation.Bridge/IEmbeddedImage.cs \
	Mono.UIAutomation.Bridge/IHypertext.cs

DATA_FILES = 

RESOURCES =

REFERENCES =  \
	WindowsBase \
	System \
	UIAutomationTypes

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
