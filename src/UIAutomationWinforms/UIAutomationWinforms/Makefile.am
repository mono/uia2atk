
EXTRA_DIST =  

# Warning: This is an automatically generated file, do not edit!

if ENABLE_DEBUG
ASSEMBLY_COMPILER_COMMAND = gmcs
ASSEMBLY_COMPILER_FLAGS = -lib:@expanded_libdir@/mono/3.0 -lib:@expanded_libdir@/mono/accessibility -noconfig -codepage:utf8 -warn:4 -optimize+ -debug "-define:DEBUG" -keyfile:../mono-uia.snk

ASSEMBLY = bin/Debug/UIAutomationWinforms.dll
ASSEMBLY_MDB = $(ASSEMBLY).mdb
COMPILE_TARGET = library
PROJECT_REFERENCES = 
BUILD_DIR = bin/Debug

UIAUTOMATIONWINFORMS_DLL_MDB_SOURCE=bin/Debug/UIAutomationWinforms.dll.mdb
UIAUTOMATIONWINFORMS_DLL_MDB=$(BUILD_DIR)/UIAutomationWinforms.dll.mdb

endif

if ENABLE_RELEASE
ASSEMBLY_COMPILER_COMMAND = gmcs
ASSEMBLY_COMPILER_FLAGS =  -noconfig -codepage:utf8 -warn:4 -optimize+
ASSEMBLY = bin/Release/UIAutomationWinforms.dll
ASSEMBLY_MDB = 
COMPILE_TARGET = library
PROJECT_REFERENCES = 
BUILD_DIR = bin/Release

UIAUTOMATIONWINFORMS_DLL_MDB=

endif

AL=al2
SATELLITE_ASSEMBLY_NAME=Mono.UIAutomation.Winforms.resources.dll

PROGRAMFILES = \
	$(UIAUTOMATIONWINFORMS_DLL_MDB)  


RESGEN=resgen2
	
all: $(ASSEMBLY) $(PROGRAMFILES)

FILES =  \
	Mono.UIAutomation.Winforms.Behaviors/ComboBoxExpandCollapseProviderBehavior.cs \
	Mono.UIAutomation.Winforms.Behaviors/ComboBoxItemProviderBehavior.cs \
	Mono.UIAutomation.Winforms.Behaviors/ComboBoxProviderBehavior.cs \
	Mono.UIAutomation.Winforms.Behaviors/ComboBoxSelectionProviderBehavior.cs \
	Mono.UIAutomation.Winforms.Behaviors/ComboBoxValueProviderBehavior.cs \
	Mono.UIAutomation.Winforms.Behaviors/IProviderBehavior.cs \
	Mono.UIAutomation.Winforms.Behaviors/LinkLabelInvokeProviderBehavior.cs \
	Mono.UIAutomation.Winforms.Behaviors/ListBoxItemProviderBehavior.cs \
	Mono.UIAutomation.Winforms.Behaviors/ListBoxItemScrollProviderBehavior.cs \
	Mono.UIAutomation.Winforms.Behaviors/ListBoxItemSelectionProviderBehavior.cs \
	Mono.UIAutomation.Winforms.Behaviors/ListBoxProviderBehavior.cs \
	Mono.UIAutomation.Winforms.Behaviors/ListBoxScrollProviderBehavior.cs \
	Mono.UIAutomation.Winforms.Behaviors/ListBoxSelectionProviderBehavior.cs \
	Mono.UIAutomation.Winforms.Behaviors/ProviderBehavior.cs \
	Mono.UIAutomation.Winforms.Behaviors/RadioButtonSelectionProviderBehavior.cs \
	Mono.UIAutomation.Winforms.Events/ComboBoxExpandCollapseStateEvent.cs \
	Mono.UIAutomation.Winforms.Events/ComboBoxItemElementSelectedEvent.cs \
	Mono.UIAutomation.Winforms.Events/DefaultBoundingRectanglePropertyEvent.cs \
	Mono.UIAutomation.Winforms.Events/DefaultHasKeyboardFocusPropertyEvent.cs \
	Mono.UIAutomation.Winforms.Events/DefaultInvokedEvent.cs \
	Mono.UIAutomation.Winforms.Events/DefaultIsEnabledPropertyEvent.cs \
	Mono.UIAutomation.Winforms.Events/DefaultIsOffscreenPropertyEvent.cs \
	Mono.UIAutomation.Winforms.Events/DefaultNamePropertyEvent.cs \
	Mono.UIAutomation.Winforms.Events/DefaultTextChangedEvent.cs \
	Mono.UIAutomation.Winforms.Events/DefaultToggleStatePropertyEvent.cs \
	Mono.UIAutomation.Winforms.Events/ElementSelectedEvent.cs \
	Mono.UIAutomation.Winforms.Events/LinkLabelInvokedEvent.cs \
	Mono.UIAutomation.Winforms.Events/ProviderEvent.cs \
	Mono.UIAutomation.Winforms.Events/ProviderEventType.cs \
	Mono.UIAutomation.Winforms.Events/StructureChangedEvent.cs \
	Mono.UIAutomation.Winforms.Events/TextBoxHasKeyBoardFocusPropertyEvent.cs \
	Mono.UIAutomation.Winforms.Events/ValuePatternValueIsReadOnlyEvent.cs \
	Mono.UIAutomation.Winforms.Events/ValuePatternValuePropertyEvent.cs \
	Mono.UIAutomation.Winforms/AssemblyInfo.cs \
	Mono.UIAutomation.Winforms/ButtonProvider.cs \
	Mono.UIAutomation.Winforms/CheckBoxProvider.cs \
	Mono.UIAutomation.Winforms/ComboBoxItemProvider.cs \
	Mono.UIAutomation.Winforms/ComboBoxProvider.cs \
	Mono.UIAutomation.Winforms/FormListener.cs \
	Mono.UIAutomation.Winforms/FragmentControlProvider.cs \
	Mono.UIAutomation.Winforms/FragmentRootControlProvider.cs \
	Mono.UIAutomation.Winforms/GroupBoxProvider.cs \
	Mono.UIAutomation.Winforms/Helper.cs \
	Mono.UIAutomation.Winforms/IConnectable.cs \
	Mono.UIAutomation.Winforms/LabelProvider.cs \
	Mono.UIAutomation.Winforms/LinkLabelProvider.cs \
	Mono.UIAutomation.Winforms/ListBoxItemProvider.cs \
	Mono.UIAutomation.Winforms/ListBoxProvider.cs \
	Mono.UIAutomation.Winforms/NumericUpDownProvider.cs \
	Mono.UIAutomation.Winforms/ProviderFactory.cs \
	Mono.UIAutomation.Winforms/RadioButtonProvider.cs \
	Mono.UIAutomation.Winforms/SimpleControlProvider.cs \
	Mono.UIAutomation.Winforms/StatusBarProvider.cs \
	Mono.UIAutomation.Winforms/TextBoxProvider.cs \
	Mono.UIAutomation.Winforms/TextNormalizer.cs \
	Mono.UIAutomation.Winforms/TextNormalizerPoints.cs \
	Mono.UIAutomation.Winforms/TextRangeProvider.cs \
	Mono.UIAutomation.Winforms/WindowProvider.cs \
	Mono.UIAutomation.Winforms/WordToken.cs \
	Mono.UIAutomation.Winforms/WordTokenCollection.cs \
	Mono.UIAutomation.Winforms/WordTokenizer.cs \
	Mono.UIAutomation.Winforms/WrapperFragmentControlProvider.cs 

DATA_FILES = 

RESOURCES = 

REFERENCES =  \
	UIAutomationTypes \
	UIAutomationProvider \
	WindowsBase \
	System \
	System.Core \
	System.Drawing \
	System.Windows.Forms

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
