EXTRA_DIST =  

if ENABLE_DEBUG
ASSEMBLY_COMPILER_COMMAND = smcs
ASSEMBLY_COMPILER_FLAGS =  -codepage:utf8 -warn:4 -warnaserror -optimize+ -keyfile:../mono-uia.snk -debug "-define:DEBUG"

ASSEMBLY = bin/Debug/MoonAtkBridge.dll
ASSEMBLY_MDB = $(ASSEMBLY).mdb
COMPILE_TARGET = library
PROJECT_REFERENCES = 
BUILD_DIR = bin/Debug

UIAUTOMATIONWINFORMS_DLL_MDB_SOURCE=bin/Debug/MoonAtkBridge.dll.mdb
UIAUTOMATIONWINFORMS_DLL_MDB=$(BUILD_DIR)/MoonAtkBridge.dll.mdb
endif

if ENABLE_RELEASE
ASSEMBLY_COMPILER_COMMAND = smcs
ASSEMBLY_COMPILER_FLAGS = -noconfig -codepage:utf8 -warn:4 -warnaserror -optimize+ -keyfile:../mono-uia.snk
ASSEMBLY = bin/Release/MoonAtkBridge.dll
ASSEMBLY_MDB = 
COMPILE_TARGET = library
PROJECT_REFERENCES = 
BUILD_DIR = bin/Release

UIAUTOMATIONWINFORMS_DLL_MDB=
endif

PROGRAMFILES = \
	$(UIAUTOMATIONWINFORMS_DLL_MDB)  

all: $(ASSEMBLY) $(PROGRAMFILES)

FILES = \
	AssemblyInfo.cs \
	Moonlight.AtkBridge/Adapter.cs \
	Moonlight.AtkBridge/AutomationBridge.cs \
	Moonlight.AtkBridge/DynamicAdapterFactory.cs \
	Moonlight.AtkBridge/ImplementsPatternAttribute.cs \
	Moonlight.AtkBridge.PatternImplementors/Value.cs

DATA_FILES = 

RESOURCES =

REFERENCES =  \
	System \
	System.Core \
	System.Windows

DLL_REFERENCES = \
	$(top_srcdir)/bin/glib-atk-sharp.dll

CLEANFILES = $(PROGRAMFILES)

install-data-local: gac-install

uninstall-local: gac-uninstall

include $(top_srcdir)/Makefile.include


$(ASSEMBLY) $(ASSEMBLY_MDB): $(build_sources) $(build_resources) $(build_datafiles) $(DLL_REFERENCES) $(PROJECT_REFERENCES) $(build_xamlg_list) $(build_satellite_assembly_list)
	mkdir -p $(shell dirname $(ASSEMBLY))
	$(ASSEMBLY_COMPILER_COMMAND) $(ASSEMBLY_COMPILER_FLAGS) -out:$(ASSEMBLY) -target:$(COMPILE_TARGET) $(build_sources_embed) $(build_resources_embed) $(build_references_ref)
