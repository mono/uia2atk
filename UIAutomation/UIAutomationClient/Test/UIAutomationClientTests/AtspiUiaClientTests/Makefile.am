
EXTRA_DIST =  

if ENABLE_DEBUG
ASSEMBLY_COMPILER_COMMAND = gmcs
ASSEMBLY_COMPILER_FLAGS = -noconfig -codepage:utf8 -warn:4 -warnaserror -optimize+ -debug "-define:DEBUG"

ASSEMBLY = bin/Debug/AtspiUiaClientTests.dll
ASSEMBLY_MDB = $(ASSEMBLY).mdb
COMPILE_TARGET = library
BUILD_DIR = bin/Debug

endif

if ENABLE_RELEASE
ASSEMBLY_COMPILER_COMMAND = gmcs
ASSEMBLY_COMPILER_FLAGS = -noconfig -codepage:utf8 -warn:4 -warnaserror -optimize+
ASSEMBLY = bin/Release/AtspiUiaClientTests.dll
ASSEMBLY_MDB = 
COMPILE_TARGET = library

BUILD_DIR = bin/Release

endif

UIAUTOMATIONCLIENTTESTS = $(BUILD_DIR)/UIAUTOMATIONCLIENTTESTS.dll

AL=al2

RESGEN=resgen2
	
all: $(ASSEMBLY) $(PROGRAMFILES)

FILES =  \
	TogglePatternTest.cs

DATA_FILES = 

RESOURCES = 

EXTRAS = 

PROJECT_REFERENCES = \
	$(BUILD_DIR)/UIAutomationClient.dll \
	$(BUILD_DIR)/UIAutomationTypes.dll \
	$(BUILD_DIR)/UIAutomationClientTests.dll \
	$(BUILD_DIR)/GtkForm.exe

REFERENCES =  \
	System \
	System.Data \
	Mono.Posix \
	-pkg:mono-nunit \
	$(ATSPI_SHARP_LIBS) \
	$(MONO_UIA_LIBS)

CLEANFILES = $(PROGRAMFILES) \
	bin/Debug/TestResult.xml

include $(top_srcdir)/Makefile.include

$(eval $(call emit_resgen_targets))
$(build_xamlg_list): %.xaml.g.cs: %.xaml
	xamlg '$<'

$(ASSEMBLY) $(ASSEMBLY_MDB): $(build_sources) $(build_resources) $(build_datafiles) $(DLL_REFERENCES) $(PROJECT_REFERENCES) $(build_xamlg_list) $(build_satellite_assembly_list)
	@mkdir -p $(shell dirname $(ASSEMBLY))
	$(ASSEMBLY_COMPILER_COMMAND) $(ASSEMBLY_COMPILER_FLAGS) -out:$(ASSEMBLY) -target:$(COMPILE_TARGET) $(build_sources_embed) $(build_resources_embed) $(build_references_ref)
	@chmod +x atspiuiaclienttest.sh

bin/Debug/UIAutomationClient.dll: ../../../../bin/UIAutomationClient.dll
	@mkdir -p bin/Debug
	cp -a $< $@
	cp -a $<.mdb $@.mdb

bin/Release/UIAutomationClient.dll: ../../../../bin/UIAutomationClient.dll
	@mkdir -p bin/Release
	cp -a $< $@

bin/Debug/UIAutomationTypes.dll: ../../../../bin/UIAutomationTypes.dll
	@mkdir -p bin/Debug
	cp -a $< $@
	cp -a $<.mdb $@.mdb

bin/Release/UIAutomationTypes.dll: ../../../../bin/UIAutomationTypes.dll
	@mkdir -p bin/Release
	cp -a $< $@

bin/Debug/GtkForm.exe: ../GtkForm/bin/Debug/GtkForm.exe
	@mkdir -p bin/Debug
	cp -a $< $@
	cp -a $<.mdb $@.mdb

bin/Release/GtkForm.exe: ../GtkForm/bin/Release/GtkForm.exe
	@mkdir -p bin/Release
	cp -a $< $@

bin/Debug/UIAutomationClientTests.dll: ../UIAutomationClientTests/bin/Debug/UIAutomationClientTests.dll
	@mkdir -p bin/Debug
	cp -a $< $@
	cp -a $<.mdb $@.mdb

bin/Release/UIAutomationClientTests.dll: ../UIAutomationClientTests/bin/Release/UIAutomationClientTests.dll
	@mkdir -p bin/Release
	cp -a $< $@
