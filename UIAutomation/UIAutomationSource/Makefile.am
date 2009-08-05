
EXTRA_DIST =  

ASSEMBLY_COMPILER_COMMAND = gmcs
ASSEMBLY_COMPILER_FLAGS = -lib:@expanded_libdir@/mono/2.0 -lib:../bin -lib:@expanded_libdir@/mono/accessibility -noconfig -codepage:utf8 -warn:4 -warnaserror -optimize+ -debug "-define:DEBUG" -d:NET_2_0

ASSEMBLY = ../bin/UIAutomationSource.dll
ASSEMBLY_MDB = $(ASSEMBLY).mdb
COMPILE_TARGET = library
PROJECT_REFERENCES = 
BUILD_DIR = ../bin
PACKAGE_FLAG = /package accessibility

UIAUTOMATIONSOURCE_DLL_MDB_SOURCE=../bin/UIAutomationSource.dll.mdb
UIAUTOMATIONSOURCE_DLL_MDB=$(BUILD_DIR)/UIAutomationSource.dll.mdb

AL=al2
SATELLITE_ASSEMBLY_NAME=Mono.UIAutomation.Source.resources.dll

PROGRAMFILES = \
	$(UIAUTOMATIONSOURCE_DLL_MDB)  


RESGEN=resgen2
	
all: $(ASSEMBLY) $(PROGRAMFILES)

PROJECT_SOURCE_FILES =  \
	Mono.UIAutomation.Source/IAutomationSource.cs \
	Mono.UIAutomation.Source/IElement.cs \
	Mono.UIAutomation.Source/IGridItemPattern.cs \
	Mono.UIAutomation.Source/IGridPattern.cs \
	Mono.UIAutomation.Source/IMultipleViewPattern.cs \
	Mono.UIAutomation.Source/IRangeValuePattern.cs \
	Mono.UIAutomation.Source/IScrollPattern.cs \
	Mono.UIAutomation.Source/ISelectionItemPattern.cs \
	Mono.UIAutomation.Source/ISelectionPattern.cs \
	Mono.UIAutomation.Source/ITableItemPattern.cs \
	Mono.UIAutomation.Source/ITablePattern.cs \
	Mono.UIAutomation.Source/ITextPattern.cs \
	Mono.UIAutomation.Source/ITextPatternRange.cs \
	Mono.UIAutomation.Source/ITransformPattern.cs \
	Mono.UIAutomation.Source/IValuePattern.cs \
	Mono.UIAutomation.Source/IWindowPattern.cs

FILES =  \
        AssemblyInfo.cs \
        ../build/common/*.cs \
	$(PROJECT_SOURCE_FILES)

DATA_FILES = 

RESOURCES =

REFERENCES =  \
	WindowsBase \
	System \
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
