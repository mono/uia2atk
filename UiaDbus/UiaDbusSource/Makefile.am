
EXTRA_DIST =  

# Warning: This is an automatically generated file, do not edit!

if ENABLE_DEBUG
ASSEMBLY_COMPILER_COMMAND = gmcs
ASSEMBLY_COMPILER_FLAGS = -noconfig -codepage:utf8 -warn:4 -warnaserror -optimize+ -debug "-define:DEBUG" -keyfile:$(top_srcdir)/mono-uia.snk

ASSEMBLY = bin/Debug/UiaDbusSource.dll
ASSEMBLY_MDB = $(ASSEMBLY).mdb
COMPILE_TARGET = library
PROJECT_REFERENCES = 
BUILD_DIR = bin/Debug

UIADBUSSOURCE_DLL_MDB_SOURCE=bin/Debug/UiaDbusSource.dll.mdb
UIADBUSSOURCE_DLL_MDB=$(BUILD_DIR)/UiaDbusSource.dll.mdb

endif

if ENABLE_RELEASE
ASSEMBLY_COMPILER_COMMAND = gmcs
ASSEMBLY_COMPILER_FLAGS = -keyfile:$(top_srcdir)/mono-uia.snk -noconfig -codepage:utf8 -warn:4 -warnaserror -optimize+
ASSEMBLY = bin/Release/UiaDbusSource.dll
ASSEMBLY_MDB = 
COMPILE_TARGET = library
PROJECT_REFERENCES = 
BUILD_DIR = bin/Release

UIADBUSSOURCE_DLL_MDB=

endif

AL=al2
SATELLITE_ASSEMBLY_NAME=UiaDbusSource.resources.dll

PROGRAMFILES = \
	$(UIADBUSSOURCE_DLL_MDB)  


RESGEN=resgen2
	
all: $(ASSEMBLY) $(PROGRAMFILES)

FILES =  \
	AssemblyInfo.cs \
	DbusExceptionTranslator.cs \
	EventHandlerManager.cs \
	UiaDbusAutomationSource.cs \
	UiaDbusElement.cs \
	UiaDbusInvokePattern.cs \
	UiaDbusTextPattern.cs \
	UiaDbusTextPatternRange.cs \
	UiaDbusValuePattern.cs

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
