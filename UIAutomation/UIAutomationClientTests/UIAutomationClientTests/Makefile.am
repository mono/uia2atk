
EXTRA_DIST =  

ASSEMBLY_COMPILER_COMMAND = gmcs
ASSEMBLY_COMPILER_FLAGS =  -noconfig -codepage:utf8 -warn:4 -optimize- -debug "-define:DEBUG;TRACE"
ASSEMBLY = bin/Debug/UIAutomationClientTests.dll

ASSEMBLY_MDB = $(ASSEMBLY).mdb
COMPILE_TARGET = library
BUILD_DIR = bin/Debug/

SAMPLEFORM_EXE = $(BUILD_DIR)/SampleForm.exe
SAMPLEFORM_EXE_SOURCE=../SampleForm/bin/Debug/SampleForm.exe
SAMPLEFORM_EXE_MDB_SOURCE=$(SAMPLEFORM_EXE_SOURCE).mdb
SAMPLEFORM_EXE_MDB=$(SAMPLEFORM_EXE).mdb

WINDOWSBASE_DLL = $(BUILD_DIR)/WindowsBase.dll
WINDOWSBASE_DLL_SOURCE=$(top_builddir)/bin/WindowsBase.dll
WINDOWSBASE_DLL_MDB_SOURCE=$(WINDOWSBASE_DLL_SOURCE).mdb
WINDOWSBASE_DLL_MDB=$(WINDOWSBASE_DLL).mdb

UIAUTOMATIONTYPES_DLL = $(BUILD_DIR)/UIAutomationTypes.dll
UIAUTOMATIONTYPES_DLL_SOURCE=$(top_builddir)/bin/UIAutomationTypes.dll
UIAUTOMATIONTYPES_DLL_MDB_SOURCE=$(UIAUTOMATIONTYPES_DLL_SOURCE).mdb
UIAUTOMATIONTYPES_DLL_MDB=$(UIAUTOMATIONTYPES_DLL).mdb

UIAUTOMATIONCLIENT_DLL = $(BUILD_DIR)/UIAutomationClient.dll
UIAUTOMATIONCLIENT_DLL_SOURCE=$(top_builddir)/bin/UIAutomationClient.dll
UIAUTOMATIONCLIENT_DLL_MDB_SOURCE=$(UIAUTOMATIONCLIENT_DLL_SOURCE).mdb
UIAUTOMATIONCLIENT_DLL_MDB=$(UIAUTOMATIONCLIENT_DLL).mdb

UIAUTOMATIONCLIENTTESTS_DLL_MDB_SOURCE=bin/Debug/UIAutomationClientTests.dll.mdb
UIAUTOMATIONCLIENTTESTS_DLL_MDB=$(BUILD_DIR)/UIAutomationClientTests.dll.mdb

AL=al2
SATELLITE_ASSEMBLY_NAME=$(notdir $(basename $(ASSEMBLY))).resources.dll

PROGRAMFILES = \
	$(SAMPLEFORM_EXE) \
	$(SAMPLEFORM_EXE_MDB) \
	$(WINDOWSBASE_DLL) \
	$(WINDOWSBASE_DLL_MDB) \
	$(UIAUTOMATIONTYPES_DLL) \
	$(UIAUTOMATIONTYPES_DLL_MDB) \
	$(UIAUTOMATIONCLIENT_DLL) \
	$(UIAUTOMATIONCLIENT_DLL_MDB) \
	$(UIAUTOMATIONCLIENTTESTS_DLL_MDB)


RESGEN=resgen2
	
all: $(ASSEMBLY) $(PROGRAMFILES)

FILES =  \
	AutomationElementTest.cs \
	AutomationTest.cs \
	BaseTest.cs \
	ConditionTest.cs \
	InvokePatternTest.cs \
	Properties/AssemblyInfo.cs \
	TogglePatternTest.cs \
	TreeWalkerTest.cs \
	ValuePatternTest.cs 

DATA_FILES = 

RESOURCES = 

EXTRAS = 

PROJECT_REFERENCES =  \
	$(WINDOWSBASE_DLL) \
	$(UIAUTOMATIONTYPES_DLL) \
	$(UIAUTOMATIONCLIENT_DLL)

REFERENCES =  \
	System \
	System.Core \
	System.Xml.Linq \
	System.Data.DataSetExtensions \
	System.Data \
	System.Xml \
	$(NUNIT_LIBS)

DLL_REFERENCES = 

CLEANFILES = $(PROGRAMFILES) \
	bin/Debug/TestResult.xml

include $(top_srcdir)/Makefile.include

$(eval $(call emit-deploy-target,SAMPLEFORM_EXE))
$(eval $(call emit-deploy-target,SAMPLEFORM_EXE_MDB))

$(eval $(call emit-deploy-target,WINDOWSBASE_DLL))
$(eval $(call emit-deploy-target,WINDOWSBASE_DLL_MDB))

$(eval $(call emit-deploy-target,UIAUTOMATIONTYPES_DLL))
$(eval $(call emit-deploy-target,UIAUTOMATIONTYPES_DLL_MDB))

$(eval $(call emit-deploy-target,UIAUTOMATIONCLIENT_DLL))
$(eval $(call emit-deploy-target,UIAUTOMATIONCLIENT_DLL_MDB))


$(eval $(call emit_resgen_targets))
$(build_xamlg_list): %.xaml.g.cs: %.xaml
	xamlg '$<'

$(ASSEMBLY) $(ASSEMBLY_MDB): $(build_sources) $(build_resources) $(build_datafiles) $(DLL_REFERENCES) $(PROJECT_REFERENCES) $(build_xamlg_list) $(build_satellite_assembly_list)
	mkdir -p $(shell dirname $(ASSEMBLY))
	$(ASSEMBLY_COMPILER_COMMAND) $(ASSEMBLY_COMPILER_FLAGS) -out:$(ASSEMBLY) -target:$(COMPILE_TARGET) $(build_sources_embed) $(build_resources_embed) $(build_references_ref)
	@chmod +x swfclienttest.sh
