
EXTRA_DIST =  

ASSEMBLY_COMPILER_COMMAND = mcs
ASSEMBLY_COMPILER_FLAGS = -lib:@expanded_libdir@/mono/2.0 -lib:../bin -lib:@expanded_libdir@/mono/accessibility -noconfig -codepage:utf8 -warn:4 -warnaserror -optimize+ -debug "-define:DEBUG" -d:NET_2_0 -delaysign+ -keyfile:@abs_top_srcdir@/winfx3.pub

ASSEMBLY = ../bin/UIAutomationProvider.dll
ASSEMBLY_MDB = $(ASSEMBLY).mdb
COMPILE_TARGET = library
PROJECT_REFERENCES = 
BUILD_DIR = ../bin
PACKAGE_FLAG = /package accessibility

UIAUTOMATIONPROVIDER_DLL_MDB_SOURCE=../bin/UIAutomationProvider.dll.mdb
UIAUTOMATIONPROVIDER_DLL_MDB=$(BUILD_DIR)/UIAutomationProvider.dll.mdb

AL=al2
SATELLITE_ASSEMBLY_NAME=Mono.UIAutomation.Provider.resources.dll

PROGRAMFILES = \
	$(UIAUTOMATIONPROVIDER_DLL_MDB)  


RESGEN=resgen2
	
all: $(ASSEMBLY) $(PROGRAMFILES)

FILES =  \
        AssemblyInfo.cs \
        ../build/common/*.cs \
        $(PROJECT_SOURCE_FILES)

PROJECT_SOURCE_FILES =  \
	System.Windows.Automation.Provider/AutomationInteropProvider.cs \
	System.Windows.Automation.Provider/ICaretProvider.cs \
	System.Windows.Automation.Provider/IClipboardProvider.cs \
	System.Windows.Automation.Provider/IDockProvider.cs \
	System.Windows.Automation.Provider/IEditableRangeProvider.cs \
	System.Windows.Automation.Provider/IEmbeddedImageProvider.cs \
	System.Windows.Automation.Provider/IExpandCollapseProvider.cs \
	System.Windows.Automation.Provider/IGridItemProvider.cs \
	System.Windows.Automation.Provider/IGridProvider.cs \
	System.Windows.Automation.Provider/IInsertDeleteTextProvider.cs \
	System.Windows.Automation.Provider/IInvokeProvider.cs \
	System.Windows.Automation.Provider/ILegacyIAccessibleProvider.cs \
	System.Windows.Automation.Provider/IMultipleViewProvider.cs \
	System.Windows.Automation.Provider/IRangeValueProvider.cs \
	System.Windows.Automation.Provider/IRawElementProviderAdviseEvents.cs \
	System.Windows.Automation.Provider/IRawElementProviderFragment.cs \
	System.Windows.Automation.Provider/IRawElementProviderFragmentRoot.cs \
	System.Windows.Automation.Provider/IRawElementProviderHwndOverride.cs \
	System.Windows.Automation.Provider/IRawElementProviderSimple.cs \
	System.Windows.Automation.Provider/IScrollItemProvider.cs \
	System.Windows.Automation.Provider/IScrollProvider.cs \
	System.Windows.Automation.Provider/ISelectionItemProvider.cs \
	System.Windows.Automation.Provider/ISelectionProvider.cs \
	System.Windows.Automation.Provider/ITableItemProvider.cs \
	System.Windows.Automation.Provider/ITableProvider.cs \
	System.Windows.Automation.Provider/ITextProvider.cs \
	System.Windows.Automation.Provider/ITextRangeProvider.cs \
	System.Windows.Automation.Provider/IToggleProvider.cs \
	System.Windows.Automation.Provider/ITransformProvider.cs \
	System.Windows.Automation.Provider/IValueProvider.cs \
	System.Windows.Automation.Provider/IWindowProvider.cs \
	System.Windows.Automation.Provider/NavigateDirection.cs \
	System.Windows.Automation.Provider/ProviderOptions.cs

DATA_FILES = 

RESOURCES =

REFERENCES =  \
	WindowsBase \
	System \
	System.Core \
	UIAutomationHelpers \
	UIAutomationBridge \
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
	sn -q -R $(ASSEMBLY) $(top_srcdir)/mono.snk
