EXTRA_DIST =

if ENABLE_DEBUG
ASSEMBLY_COMPILER_COMMAND = gmcs
ASSEMBLY_COMPILER_FLAGS = -lib:@expanded_libdir@/mono/3.0 -lib:@expanded_libdir@/mono/accessibility -noconfig -codepage:utf8 -warn:4 -warnaserror -optimize+ -debug "-define:DEBUG"

ASSEMBLY = bin/Debug/UiaAtkBridgeTest.dll
ASSEMBLY_MDB = $(ASSEMBLY).mdb
COMPILE_TARGET = library
PROJECT_REFERENCES = 
BUILD_DIR = bin/Debug

GAILTEST_DLL_MDB_SOURCE=bin/Debug/UiaAtkBridgeTest.dll.mdb
GAILTEST_DLL_MDB=$(BUILD_DIR)/UiaAtkBridgeTest.dll.mdb

endif

if ENABLE_RELEASE
ASSEMBLY_COMPILER_COMMAND = gmcs
ASSEMBLY_COMPILER_FLAGS = -lib:@expanded_libdir@/mono/3.0 -lib:@expanded_libdir@/mono/accessibility -noconfig -codepage:utf8 -warn:4 -warnaserror -optimize+ -keyfile:../mono-uia.snk
ASSEMBLY = bin/Release/UiaAtkBridgeTest.dll
ASSEMBLY_MDB = 
COMPILE_TARGET = library
PROJECT_REFERENCES = 
BUILD_DIR = bin/Release

GAILTEST_DLL_MDB=

endif

AL=al2
SATELLITE_ASSEMBLY_NAME=.resources.dll

PROGRAMFILES = \
	$(GAILTEST_DLL_MDB)  


RESGEN=resgen2
	
all: $(ASSEMBLY) $(PROGRAMFILES)

FILES =  \
	AssemblyInfo.cs \
	BridgeTester.cs

DATA_FILES = 

RESOURCES = 

EXTRAS = 

UIAUTOMATIONWINFORMS = $(BUILD_DIR)/UIAutomationWinforms.dll
UIAATKBRIDGE = $(BUILD_DIR)/UiaAtkBridge.dll
ATKTEST = $(BUILD_DIR)/AtkTest.dll

REFERENCES =  \
	System  \
	System.Xml  \
	WindowsBase \
	nunit.framework \
	UIAutomationTypes.dll \
	UIAutomationProvider.dll \
	$(GTK_SHARP_20_LIBS) \
	System.Windows.Forms \
	System.Drawing \
	System.Core \
	System.Xml

DLL_REFERENCES = $(UIAUTOMATIONWINFORMS) $(ATKTEST) $(UIAATKBRIDGE)

CLEANFILES = $(PROGRAMFILES) \
	bin/Debug/UIAutomationWinforms.dll \
	bin/Debug/UIAutomationWinforms.dll.mdb \
	bin/Debug/UiaAtkBridge.dll \
	bin/Debug/UiaAtkBridge.dll.mdb \
	bin/Release/UIAutomationWinforms.dll \
	bin/Release/UiaAtkBridge.dll

MAINTAINERCLEANFILES = \
	bridgetest.sh

EXTRA_DIST = \
	bridgetest.sh.in

include $(top_srcdir)/Makefile.include


$(eval $(call emit_resgen_targets))
$(build_xamlg_list): %.xaml.g.cs: %.xaml
	xamlg '$<'

bin/Debug/UIAutomationWinforms.dll: ../../UIAutomationWinforms/UIAutomationWinforms/bin/Debug/UIAutomationWinforms.dll
	@mkdir -p bin/Debug
	cp -a $< $@
	cp -a $<.mdb $@.mdb

bin/Release/UIAutomationWinforms.dll: ../../UIAutomationWinforms/UIAutomationWinforms/bin/Release/UIAutomationWinforms.dll
	@mkdir -p bin/Release
	cp -a $< $@

bin/Debug/UiaAtkBridge.dll: ../UiaAtkBridge/bin/Debug/UiaAtkBridge.dll
	@mkdir -p bin/Debug
	cp -a $< $@
	cp -a $<.mdb $@.mdb

bin/Release/UiaAtkBridge.dll: ../UiaAtkBridge/bin/Release/UiaAtkBridge.dll
	@mkdir -p bin/Release
	cp -a $< $@

$(ASSEMBLY) $(ASSEMBLY_MDB): $(build_sources) $(build_resources) $(build_datafiles) $(DLL_REFERENCES) $(PROJECT_REFERENCES) $(build_xamlg_list) $(build_satellite_assembly_list)
	@mkdir -p $(shell dirname $(ASSEMBLY))
	@chmod +x ./bridgetest.sh
	$(ASSEMBLY_COMPILER_COMMAND) $(ASSEMBLY_COMPILER_FLAGS) -out:$(ASSEMBLY) -target:$(COMPILE_TARGET) $(build_sources_embed) $(build_resources_embed) $(build_references_ref)
