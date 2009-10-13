
EXTRA_DIST =  

# Warning: This is an automatically generated file, do not edit!

if ENABLE_DEBUG
ASSEMBLY_COMPILER_COMMAND = gmcs
ASSEMBLY_COMPILER_FLAGS = -noconfig -codepage:utf8 -warn:4 -warnaserror -optimize+ -debug "-define:DEBUG" -keyfile:$(top_srcdir)/mono-uia.snk

ASSEMBLY = bin/Debug/UiaDbusBridge.dll
ASSEMBLY_MDB = $(ASSEMBLY).mdb
COMPILE_TARGET = library
PROJECT_REFERENCES = 
BUILD_DIR = bin/Debug

UIADBUSBRIDGE_DLL_MDB_SOURCE=bin/Debug/UiaDbusBridge.dll.mdb
UIADBUSBRIDGE_DLL_MDB=$(BUILD_DIR)/UiaDbusBridge.dll.mdb

endif

if ENABLE_RELEASE
ASSEMBLY_COMPILER_COMMAND = gmcs
ASSEMBLY_COMPILER_FLAGS = -keyfile:$(top_srcdir)/mono-uia.snk -noconfig -codepage:utf8 -warn:4 -warnaserror -optimize+
ASSEMBLY = bin/Release/UiaDbusBridge.dll
ASSEMBLY_MDB = 
COMPILE_TARGET = library
PROJECT_REFERENCES = 
BUILD_DIR = bin/Release

UIADBUSBRIDGE_DLL_MDB=

endif

AL=al2
SATELLITE_ASSEMBLY_NAME=UiaDbusBridge.resources.dll

PROGRAMFILES = \
	$(UIADBUSBRIDGE_DLL_MDB)  


RESGEN=resgen2
	
all: $(ASSEMBLY) $(PROGRAMFILES)

FILES =  \
	Application.cs \
	AssemblyInfo.cs \
	AutomationBridge.cs \
	Wrappers/InvokePatternWrapper.cs \
	Wrappers/ProviderElementWrapper.cs \
	Wrappers/TextPatternRangeWrapper.cs \
	Wrappers/TextPatternWrapper.cs \
	Wrappers/TransformPatternWrapper.cs \
	Wrappers/ValuePatternWrapper.cs 

DATA_FILES = 

RESOURCES =

REFERENCES =  \
	System \
	Mono.Posix \
	System.Core \
	$(NDESK_DBUS_LIBS) \
	$(MONO_UIA_LIBS) \
	$(top_builddir)/UiaDbus/bin/Debug/UiaDbus.dll

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
