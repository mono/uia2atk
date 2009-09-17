
EXTRA_DIST =  

ASSEMBLY_COMPILER_COMMAND = gmcs
ASSEMBLY_COMPILER_FLAGS = -noconfig -codepage:utf8 -warn:4 -warnaserror -optimize+ -debug "-define:DEBUG"

ASSEMBLY = bin/Debug/AtspiUiaClientTests.dll
ASSEMBLY_MDB = $(ASSEMBLY).mdb
COMPILE_TARGET = library
BUILD_DIR = bin/Debug

GTKFORM_EXE = $(BUILD_DIR)/GtkForm.exe
GTKFORM_EXE_SOURCE=../GtkForm/bin/Debug/GtkForm.exe
GTKFORM_EXE_MDB_SOURCE=$(GTKFORM_EXE_SOURCE).mdb
GTKFORM_EXE_MDB=$(GTKFORM_EXE).mdb

if HAS_MONO_2_5
else
WINDOWSBASE_DLL = $(BUILD_DIR)/WindowsBase.dll
WINDOWSBASE_DLL_SOURCE=$(top_builddir)/bin/WindowsBase.dll
WINDOWSBASE_DLL_MDB_SOURCE=$(WINDOWSBASE_DLL_SOURCE).mdb
WINDOWSBASE_DLL_MDB=$(WINDOWSBASE_DLL).mdb
endif

UIAUTOMATIONTYPES_DLL = $(BUILD_DIR)/UIAutomationTypes.dll
UIAUTOMATIONTYPES_DLL_SOURCE=$(top_builddir)/bin/UIAutomationTypes.dll
UIAUTOMATIONTYPES_DLL_MDB_SOURCE=$(UIAUTOMATIONTYPES_DLL_SOURCE).mdb
UIAUTOMATIONTYPES_DLL_MDB=$(UIAUTOMATIONTYPES_DLL).mdb

UIAUTOMATIONCLIENT_DLL = $(BUILD_DIR)/UIAutomationClient.dll
UIAUTOMATIONCLIENT_DLL_SOURCE=$(top_builddir)/bin/UIAutomationClient.dll
UIAUTOMATIONCLIENT_DLL_MDB_SOURCE=$(UIAUTOMATIONCLIENT_DLL_SOURCE).mdb
UIAUTOMATIONCLIENT_DLL_MDB=$(UIAUTOMATIONCLIENT_DLL).mdb

#Needed by UIAutomationClient.dll
UIAUTOMATIONSOURCE_DLL = $(BUILD_DIR)/UIAutomationSource.dll
UIAUTOMATIONSOURCE_DLL_SOURCE=$(top_builddir)/bin/UIAutomationSource.dll
UIAUTOMATIONSOURCE_DLL_MDB_SOURCE=$(UIAUTOMATIONSOURCE_DLL_SOURCE).mdb
UIAUTOMATIONSOURCE_DLL_MDB=$(UIAUTOMATIONSOURCE_DLL).mdb

#Needed by UIAutomationClient.dll
UIAUTOMATIONPROVIDER_DLL = $(BUILD_DIR)/UIAutomationProvider.dll
UIAUTOMATIONPROVIDER_DLL_SOURCE=$(top_builddir)/bin/UIAutomationProvider.dll
UIAUTOMATIONPROVIDER_DLL_MDB_SOURCE=$(UIAUTOMATIONPROVIDER_DLL_SOURCE).mdb
UIAUTOMATIONPROVIDER_DLL_MDB=$(UIAUTOMATIONPROVIDER_DLL).mdb

UIAUTOMATIONCLIENTTESTS_DLL = $(BUILD_DIR)/UIAutomationClientTests.dll
UIAUTOMATIONCLIENTTESTS_DLL_SOURCE=../UIAutomationClientTests/bin/Debug/UIAutomationClientTests.dll
UIAUTOMATIONCLIENTTESTS_DLL_MDB_SOURCE=$(UIAUTOMATIONCLIENTTESTS_DLL_SOURCE).mdb
UIAUTOMATIONCLIENTTESTS_DLL_MDB=$(UIAUTOMATIONCLIENTTESTS_DLL).mdb

AL=al2

PROGRAMFILES = \
	$(GTKFORM_EXE) \
	$(GTKFORM_EXE_MDB) \
	$(WINDOWSBASE_DLL) \
	$(WINDOWSBASE_DLL_MDB) \
	$(UIAUTOMATIONTYPES_DLL) \
	$(UIAUTOMATIONTYPES_DLL_MDB) \
	$(UIAUTOMATIONCLIENT_DLL) \
	$(UIAUTOMATIONCLIENT_DLL_MDB) \
	$(UIAUTOMATIONSOURCE_DLL) \
	$(UIAUTOMATIONSOURCE_DLL_MDB) \
	$(UIAUTOMATIONPROVIDER_DLL) \
	$(UIAUTOMATIONPROVIDER_DLL_MDB) \
	$(UIAUTOMATIONCLIENTTESTS_DLL) \
	$(UIAUTOMATIONCLIENTTESTS_DLL_MDB)

RESGEN=resgen2
	
all: $(ASSEMBLY) $(PROGRAMFILES)

FILES =  \
	InvokePatternTest.cs \
	TablePatternTest.cs \
	TogglePatternTest.cs \
	ValuePatternTest.cs

DATA_FILES = 

RESOURCES = 

EXTRAS = 

PROJECT_REFERENCES = \
	$(WINDOWSBASE_DLL) \
	$(UIAUTOMATIONTYPES_DLL) \
	$(UIAUTOMATIONCLIENT_DLL) \
	$(UIAUTOMATIONSOURCE_DLL) \
	$(UIAUTOMATIONPROVIDER_DLL) \
	$(UIAUTOMATIONCLIENTTESTS_DLL)

REFERENCES =  \
	System \
	System.Data \
	Mono.Posix \
	$(WINDOWSBASE_LIBS) \
	$(NUNIT_LIBS)

CLEANFILES = $(PROGRAMFILES) \
	bin/Debug/TestResult.xml

include $(top_srcdir)/Makefile.include

$(eval $(call emit-deploy-target,GTKFORM_EXE))
$(eval $(call emit-deploy-target,GTKFORM_EXE_MDB))

if HAS_MONO_2_5
else
$(eval $(call emit-deploy-target,WINDOWSBASE_DLL))
$(eval $(call emit-deploy-target,WINDOWSBASE_DLL_MDB))
endif

$(eval $(call emit-deploy-target,UIAUTOMATIONTYPES_DLL))
$(eval $(call emit-deploy-target,UIAUTOMATIONTYPES_DLL_MDB))

$(eval $(call emit-deploy-target,UIAUTOMATIONCLIENT_DLL))
$(eval $(call emit-deploy-target,UIAUTOMATIONCLIENT_DLL_MDB))

$(eval $(call emit-deploy-target,UIAUTOMATIONSOURCE_DLL))
$(eval $(call emit-deploy-target,UIAUTOMATIONSOURCE_DLL_MDB))

$(eval $(call emit-deploy-target,UIAUTOMATIONPROVIDER_DLL))
$(eval $(call emit-deploy-target,UIAUTOMATIONPROVIDER_DLL_MDB))

$(eval $(call emit-deploy-target,UIAUTOMATIONCLIENTTESTS_DLL))
$(eval $(call emit-deploy-target,UIAUTOMATIONCLIENTTESTS_DLL_MDB))

$(eval $(call emit_resgen_targets))
$(build_xamlg_list): %.xaml.g.cs: %.xaml
	xamlg '$<'

$(ASSEMBLY) $(ASSEMBLY_MDB): $(build_sources) $(build_resources) $(build_datafiles) $(DLL_REFERENCES) $(PROJECT_REFERENCES) $(build_xamlg_list) $(build_satellite_assembly_list)
	@mkdir -p $(shell dirname $(ASSEMBLY))
	$(ASSEMBLY_COMPILER_COMMAND) $(ASSEMBLY_COMPILER_FLAGS) -out:$(ASSEMBLY) -target:$(COMPILE_TARGET) $(build_sources_embed) $(build_resources_embed) $(build_references_ref)
	@chmod +x atspiuiaclienttest.sh
