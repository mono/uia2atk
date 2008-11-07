
EXTRA_DIST =  

# Warning: This is an automatically generated file, do not edit!

if ENABLE_DEBUG
ASSEMBLY_COMPILER_COMMAND = gmcs
ASSEMBLY_COMPILER_FLAGS = -lib:@expanded_libdir@/mono/2.0 -noconfig -codepage:utf8 -warn:4 -warnaserror -optimize+ -debug "-define:DEBUG" -d:NET_2_0

ASSEMBLY = ../lib/Debug/UIAutomationTypes.dll
ASSEMBLY_MDB = $(ASSEMBLY).mdb
COMPILE_TARGET = library
PROJECT_REFERENCES = 
BUILD_DIR = ../lib/Debug

UIAUTOMATIONWINFORMS_DLL_MDB_SOURCE=bin/Debug/UIAutomationTypes.dll.mdb
UIAUTOMATIONWINFORMS_DLL_MDB=$(BUILD_DIR)/UIAutomationTypes.dll.mdb

endif

if ENABLE_RELEASE
ASSEMBLY_COMPILER_COMMAND = gmcs
ASSEMBLY_COMPILER_FLAGS = -lib:@expanded_libdir@/mono/2.0 -keyfile:../mono.snk -noconfig -codepage:utf8 -warn:4 -warnaserror -optimize+
ASSEMBLY = ../lib/Release/UIAutomationTypes.dll
ASSEMBLY_MDB = 
COMPILE_TARGET = library
PROJECT_REFERENCES = 
BUILD_DIR = ../lib/Release

UIAUTOMATIONWINFORMS_DLL_MDB=

endif

AL=al2
SATELLITE_ASSEMBLY_NAME=Mono.UIAutomation.Types.resources.dll

PROGRAMFILES = \
	$(UIAUTOMATIONWINFORMS_DLL_MDB)  


RESGEN=resgen2
	
all: $(ASSEMBLY) $(PROGRAMFILES)

FILES =  \
        Assembly/AssemblyInfo.cs \
        ../build/common/*.cs \
	System.Windows.Automation/AsyncContentLoadedEventArgs.cs \
	System.Windows.Automation/AutomationElementIdentifiers.cs \
	System.Windows.Automation/AutomationEventArgs.cs \
	System.Windows.Automation/AutomationEvent.cs \
	System.Windows.Automation/AutomationIdentifier.cs \
	System.Windows.Automation/AutomationPattern.cs \
	System.Windows.Automation/AutomationPropertyChangedEventArgs.cs \
	System.Windows.Automation/AutomationProperty.cs \
	System.Windows.Automation/AutomationTextAttribute.cs \
	System.Windows.Automation/ControlType.cs \
	System.Windows.Automation/DockPatternIdentifiers.cs \
	System.Windows.Automation/DockPosition.cs \
	System.Windows.Automation/ElementNotAvailableException.cs \
	System.Windows.Automation/ElementNotEnabledException.cs \
	System.Windows.Automation/ExpandCollapsePatternIdentifiers.cs \
	System.Windows.Automation/ExpandCollapseState.cs \
	System.Windows.Automation/GridItemPatternIdentifiers.cs \
	System.Windows.Automation/GridPatternIdentifiers.cs \
	System.Windows.Automation/InvokePatternIdentifiers.cs \
	System.Windows.Automation/KeyEventArgs.cs \
	System.Windows.Automation/MultipleViewPatternIdentifiers.cs \
	System.Windows.Automation/NoClickablePointException.cs \
	System.Windows.Automation/OrientationType.cs \
	System.Windows.Automation/ProxyAssemblyNotLoadedException.cs \
	System.Windows.Automation/RangeValuePatternIdentifiers.cs \
	System.Windows.Automation/RowOrColumnMajor.cs \
	System.Windows.Automation/ScrollAmount.cs \
	System.Windows.Automation/ScrollItemPatternIdentifiers.cs \
	System.Windows.Automation/ScrollPatternIdentifiers.cs \
	System.Windows.Automation/SelectionItemPatternIdentifiers.cs \
	System.Windows.Automation/SelectionPatternIdentifiers.cs \
	System.Windows.Automation/StructureChangedEventArgs.cs \
	System.Windows.Automation/StructureChangeType.cs \
	System.Windows.Automation/SupportedTextSelection.cs \
	System.Windows.Automation/TableItemPatternIdentifiers.cs \
	System.Windows.Automation/TablePatternIdentifiers.cs \
	System.Windows.Automation/TextPatternIdentifiers.cs \
	System.Windows.Automation/TogglePatternIdentifiers.cs \
	System.Windows.Automation/ToggleState.cs \
	System.Windows.Automation/TransformPatternIdentifiers.cs \
	System.Windows.Automation/TreeScope.cs \
	System.Windows.Automation/ValuePatternIdentifiers.cs \
	System.Windows.Automation/WindowClosedEventArgs.cs \
	System.Windows.Automation/WindowInteractionState.cs \
	System.Windows.Automation/WindowPatternIdentifiers.cs \
	System.Windows.Automation/WindowVisualState.cs \
	System.Windows.Automation.Text/AnimationStyle.cs \
	System.Windows.Automation.Text/BulletStyle.cs \
	System.Windows.Automation.Text/CapStyle.cs \
	System.Windows.Automation.Text/FlowDirections.cs \
	System.Windows.Automation.Text/HorizontalTextAlignment.cs \
	System.Windows.Automation.Text/OutlineStyles.cs \
	System.Windows.Automation.Text/TextDecorationLineStyle.cs \
	System.Windows.Automation.Text/TextPatternRangeEndpoint.cs \
	System.Windows.Automation.Text/TextUnit.cs


DATA_FILES = 

RESOURCES =

REFERENCES =  \
	WindowsBase \
	System

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
	sn -q -R $(ASSEMBLY) ../mono.snk
