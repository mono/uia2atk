
EXTRA_DIST =  

# Warning: This is an automatically generated file, do not edit!

if ENABLE_DEBUG
ASSEMBLY_COMPILER_COMMAND = gmcs
ASSEMBLY_COMPILER_FLAGS = -lib:@expanded_libdir@/mono/3.0 -lib:@expanded_libdir@/mono/accessibility -keyfile:../mono-uia.snk -noconfig -codepage:utf8 -warn:4 -warnaserror -optimize+ -debug "-define:DEBUG"

ASSEMBLY = bin/Debug/UIAutomationWinformsTests.dll
ASSEMBLY_MDB = $(ASSEMBLY).mdb
COMPILE_TARGET = library
PROJECT_REFERENCES =  \
	../UIAutomationWinforms/bin/Debug/UIAutomationWinforms.dll
BUILD_DIR = bin/Debug

UIAUTOMATIONWINFORMS_DLL_SOURCE=../UIAutomationWinforms/bin/Debug/UIAutomationWinforms.dll
UIAUTOMATIONWINFORMSTESTS_DLL_MDB_SOURCE=bin/Debug/UIAutomationWinformsTests.dll.mdb
UIAUTOMATIONWINFORMSTESTS_DLL_MDB=$(BUILD_DIR)/UIAutomationWinformsTests.dll.mdb
UIAUTOMATIONWINFORMS_DLL_MDB_SOURCE=../UIAutomationWinforms/bin/Debug/UIAutomationWinforms.dll.mdb
UIAUTOMATIONWINFORMS_DLL_MDB=$(BUILD_DIR)/UIAutomationWinforms.dll.mdb

endif

if ENABLE_RELEASE
ASSEMBLY_COMPILER_COMMAND = gmcs
ASSEMBLY_COMPILER_FLAGS =  -lib:@expanded_libdir@/mono/3.0 -lib:@expanded_libdir@/mono/accessibility -keyfile:../mono-uia.snk -noconfig -codepage:utf8 -warn:4 -warnaserror -optimize+
ASSEMBLY = bin/Release/UIAutomationWinformsTests.dll
ASSEMBLY_MDB = 
COMPILE_TARGET = library
PROJECT_REFERENCES =  \
	../UIAutomationWinforms/bin/Release/UIAutomationWinforms.dll
BUILD_DIR = bin/Release

UIAUTOMATIONWINFORMS_DLL_SOURCE=../UIAutomationWinforms/bin/Release/UIAutomationWinforms.dll
UIAUTOMATIONWINFORMSTESTS_DLL_MDB=
UIAUTOMATIONWINFORMS_DLL_MDB=

endif

AL=al2
SATELLITE_ASSEMBLY_NAME=MonoTests.Mono.UIAutomation.Winforms.resources.dll

PROGRAMFILES = \
	$(UIAUTOMATIONWINFORMS_DLL) \
	$(UIAUTOMATIONWINFORMSTESTS_DLL_MDB) \
	$(UIAUTOMATIONWINFORMS_DLL_MDB)  


RESGEN=resgen2
	
all: $(ASSEMBLY) $(PROGRAMFILES)

FILES =  \
	AssemblyInfo.cs \
	BaseProviderTest.cs \
	ButtonProviderTest.cs \
	CheckBoxProviderTest.cs \
	CheckedListBoxProviderTest.cs \
	ComboBoxProviderTest.cs \
	DomainUpDownProviderTest.cs \
	ErrorProviderTest.cs \
	GroupBoxProviderTest.cs \
	HelpProviderTest.cs \
	LabelProviderTest.cs \
	LinkLabelProviderTest.cs \
	ListBoxProviderTest.cs \
	MenuStripProviderTest.cs \
	MockBridge.cs \
	NumericUpDownProviderTest.cs \
	PictureBoxProviderTest.cs \
	ProgressBarProviderTest.cs \
	RadioButtonProviderTest.cs \
	RichTextBoxProviderTest.cs \
	ScrollBarProviderTest.cs \
	StatusBarProviderTest.cs \
	TabControlProviderTest.cs \
	TabPageProviderTest.cs \
	TextBoxProviderTest.cs \
	TextNormalizerTest.cs \
	TextRangeProviderTest.cs \
	ToolStripLabelProviderTest.cs \
	ToolStripMenuItemProviderTest.cs \
	ToolStripProviderTest.cs \
	ToolStripTextBoxProviderTest.cs \
	WindowProviderTest.cs \
	WordTokenizerTest.cs 

DATA_FILES = 

RESOURCES = 

EXTRAS = 

REFERENCES =  \
	../UIAutomationWinforms/bin/Debug/UIAutomationWinforms.dll \
	-pkg:mono-nunit \
	UIAutomationTypes \
	UIAutomationBridge \
	UIAutomationProvider \
	UIAutomationClient \
	WindowsBase \
	System \
	System.Drawing \
	System.Windows.Forms

DLL_REFERENCES = 

CLEANFILES = $(PROGRAMFILES) \
	bin/Debug/TestResult.xml

include $(top_srcdir)/Makefile.include

UIAUTOMATIONWINFORMS_DLL = $(BUILD_DIR)/UIAutomationWinforms.dll

$(eval $(call emit-deploy-target,UIAUTOMATIONWINFORMS_DLL))
$(eval $(call emit-deploy-target,UIAUTOMATIONWINFORMS_DLL_MDB))


$(eval $(call emit_resgen_targets))
$(build_xamlg_list): %.xaml.g.cs: %.xaml
	xamlg '$<'

$(ASSEMBLY) $(ASSEMBLY_MDB): $(build_sources) $(build_resources) $(build_datafiles) $(DLL_REFERENCES) $(PROJECT_REFERENCES) $(build_xamlg_list) $(build_satellite_assembly_list)
	@mkdir -p $(shell dirname $(ASSEMBLY))
	$(ASSEMBLY_COMPILER_COMMAND) $(ASSEMBLY_COMPILER_FLAGS) -out:$(ASSEMBLY) -target:$(COMPILE_TARGET) $(build_sources_embed) $(build_resources_embed) $(build_references_ref)
	@chmod +x providertest.sh
