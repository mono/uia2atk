
EXTRA_DIST = UiaDbus.dll.config.in

ASSEMBLY_COMPILER_COMMAND = gmcs
ASSEMBLY_COMPILER_FLAGS = -noconfig -unsafe -codepage:utf8 -warn:4 -warnaserror -optimize+ -debug "-define:DEBUG" -keyfile:$(top_srcdir)/mono-uia.snk

ASSEMBLY = bin/Debug/UiaDbus.dll
ASSEMBLY_MDB = $(ASSEMBLY).mdb
COMPILE_TARGET = library
PROJECT_REFERENCES = 
BUILD_DIR = bin/Debug
PACKAGE_FLAG = /package accessibility

UIADBUS_DLL_MDB_SOURCE=bin/Debug/UiaDbus.dll.mdb
UIADBUS_DLL_MDB=$(BUILD_DIR)/UiaDbus.dll.mdb

AL=al2
SATELLITE_ASSEMBLY_NAME=UiaDbus.resources.dll

PROGRAMFILES = \
	$(UIADBUS_DLL_MDB)  


RESGEN=resgen2
	
all: $(ASSEMBLY) $(PROGRAMFILES) $(ASSEMBLY).config

if ENABLE_EXTERNAL_NDESK_DBUS
NDESK_DBUS_FILES =
else
NDESK_DBUS_FILES = $(top_srcdir)/ndesk-dbus/*.cs
endif

SOURCE_FILES =  \
	AssemblyInfo.cs \
	Constants.cs \
	DbusSerializer.cs \
	Interfaces/IApplication.cs \
	Interfaces/IAutomationElement.cs \
	Interfaces/IDockPattern.cs \
	Interfaces/IExpandCollapsePattern.cs \
	Interfaces/IGridItemPattern.cs \
	Interfaces/IGridPattern.cs \
	Interfaces/IInvokePattern.cs \
	Interfaces/IMultipleViewPattern.cs \
	Interfaces/InterfaceConverter.cs \
	Interfaces/IRangeValuePattern.cs \
	Interfaces/IScrollItemPattern.cs \
	Interfaces/IScrollPattern.cs \
	Interfaces/ISelectionItemPattern.cs \
	Interfaces/ISelectionPattern.cs \
	Interfaces/ITableItemPattern.cs \
	Interfaces/ITablePattern.cs \
	Interfaces/ITextPattern.cs \
	Interfaces/ITextPatternRange.cs \
	Interfaces/ITogglePattern.cs \
	Interfaces/ITransformPattern.cs \
	Interfaces/IValuePattern.cs \
	Interfaces/IWindowPattern.cs \
	Point.cs \
	Rect.cs 

FILES =  \
	$(SOURCE_FILES) \
	$(NDESK_DBUS_FILES)

DATA_FILES = 

RESOURCES =

REFERENCES =  \
	System \
	Mono.Posix \
	System.Core \
	$(NDESK_DBUS_LIBS) \
	$(MONO_UIA_LIBS)

DLL_REFERENCES = 

CLEANFILES = $(PROGRAMFILES)

install-data-local: gac-install

uninstall-local: gac-uninstall

include $(top_srcdir)/Makefile.include

$(ASSEMBLY).config: $(srcdir)/UiaDbus.dll.config.in Makefile
	sed -e "s|\@expanded_libdir\@|@expanded_libdir@|" \
	    < $< > $@

$(eval $(call emit_resgen_targets))
$(build_xamlg_list): %.xaml.g.cs: %.xaml
	xamlg '$<'

$(ASSEMBLY) $(ASSEMBLY_MDB): $(build_sources) $(build_resources) $(build_datafiles) $(DLL_REFERENCES) $(PROJECT_REFERENCES) $(build_xamlg_list) $(build_satellite_assembly_list)
	mkdir -p $(shell dirname $(ASSEMBLY))
	$(ASSEMBLY_COMPILER_COMMAND) $(ASSEMBLY_COMPILER_FLAGS) -out:$(ASSEMBLY) -target:$(COMPILE_TARGET) $(build_sources_embed) $(build_resources_embed) $(build_references_ref)
