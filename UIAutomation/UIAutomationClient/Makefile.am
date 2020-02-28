
EXTRA_DIST =  

ASSEMBLY_COMPILER_COMMAND = mcs
ASSEMBLY_COMPILER_FLAGS = -lib:@expanded_libdir@/mono/2.0 -lib:../bin -lib:@expanded_libdir@/mono/accessibility -noconfig -codepage:utf8 -warn:4 -optimize+ -debug "-define:DEBUG" -d:NET_2_0 -delaysign+ -keyfile:@abs_top_srcdir@/winfx3.pub

ASSEMBLY = ../bin/UIAutomationClient.dll
ASSEMBLY_MDB = $(ASSEMBLY).mdb
COMPILE_TARGET = library
PROJECT_REFERENCES =  \
	../bin/UIAutomationBridge.dll \
	../bin/UIAutomationTypes.dll \
	../bin/UIAutomationSource.dll \
	../bin/UIAutomationProvider.dll

BUILD_DIR = ../bin
PACKAGE_FLAG = /package accessibility

UIAUTOMATIONCLIENT_DLL_MDB_SOURCE=../bin/UIAutomationClient.dll.mdb
UIAUTOMATIONCLIENT_DLL_MDB=$(BUILD_DIR)/UIAutomationClient.dll.mdb

AL=al2
SATELLITE_ASSEMBLY_NAME=Mono.UIAutomation.Client.resources.dll

PROGRAMFILES = \
	$(UIAUTOMATIONCLIENT_DLL_MDB)  


RESGEN=resgen2
	
all: $(ASSEMBLY) $(PROGRAMFILES)

PROJECT_SOURCE_FILES =  \
	Mono.UIAutomation.ClientSource/ClientAutomationBridge.cs \
	Mono.UIAutomation.ClientSource/ClientAutomationSource.cs \
	Mono.UIAutomation.ClientSource/ClientElement.cs \
	Mono.UIAutomation.ClientSource/ClientEventManager.cs \
	Mono.UIAutomation.ClientSource/ClientInvokePattern.cs \
	System.Windows.Automation.Text/TextPatternRange.cs \
	System.Windows.Automation/AndCondition.cs \
	System.Windows.Automation/Automation.cs \
	System.Windows.Automation/AutomationElement.AutomationElementInformation.cs \
	System.Windows.Automation/AutomationElement.cs \
	System.Windows.Automation/AutomationElementCollection.cs \
	System.Windows.Automation/AutomationElementMode.cs \
	System.Windows.Automation/AutomationFocusChangedEventArgs.cs \
	System.Windows.Automation/BasePattern.cs \
	System.Windows.Automation/BoolCondition.cs \
	System.Windows.Automation/CacheRequest.cs \
	System.Windows.Automation/ClientSettings.cs \
	System.Windows.Automation/ClientSideProviderDescription.cs \
	System.Windows.Automation/Condition.cs \
	System.Windows.Automation/DesktopElement.cs \
	System.Windows.Automation/DockPattern.cs \
	System.Windows.Automation/ExpandCollapsePattern.cs \
	System.Windows.Automation/GridItemPattern.cs \
	System.Windows.Automation/GridPattern.cs \
	System.Windows.Automation/InvokePattern.cs \
	System.Windows.Automation/LegacyIAccessiblePattern.cs \
	System.Windows.Automation/MultipleViewPattern.cs \
	System.Windows.Automation/NativeMethods.cs \
	System.Windows.Automation/NotCondition.cs \
	System.Windows.Automation/OrCondition.cs \
	System.Windows.Automation/PropertyConditon.cs \
	System.Windows.Automation/RangeValuePattern.cs \
	System.Windows.Automation/ScrollItemPattern.cs \
	System.Windows.Automation/ScrollPattern.cs \
	System.Windows.Automation/SelectionItemPattern.cs \
	System.Windows.Automation/SelectionPattern.cs \
	System.Windows.Automation/SourceManager.cs \
	System.Windows.Automation/TableItemPattern.cs \
	System.Windows.Automation/TablePattern.cs \
	System.Windows.Automation/TextPattern.cs \
	System.Windows.Automation/TogglePattern.cs \
	System.Windows.Automation/TransformPattern.cs \
	System.Windows.Automation/TreeWalker.cs \
	System.Windows.Automation/TreeWalker.TreeIterator.cs \
	System.Windows.Automation/ValuePattern.cs \
	System.Windows.Automation/WindowPattern.cs

FILES =  \
        AssemblyInfo.cs \
        ../build/common/*.cs \
	$(PROJECT_SOURCE_FILES)

DATA_FILES = 

RESOURCES =

REFERENCES =  \
	WindowsBase \
	System \
	System.Core \
	Mono.Posix \
	UIAutomationHelpers \
	$(GTK_SHARP_30_LIBS)

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
