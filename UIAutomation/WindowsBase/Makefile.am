
EXTRA_DIST =  

ASSEMBLY_COMPILER_COMMAND = gmcs
ASSEMBLY_COMPILER_FLAGS = -lib:@expanded_libdir@/mono/2.0 -noconfig -codepage:utf8 -warn:4 -warnaserror -optimize+ -debug "-define:DEBUG" -d:NET_2_0

ASSEMBLY = ../bin/WindowsBase.dll
ASSEMBLY_MDB = $(ASSEMBLY).mdb
COMPILE_TARGET = library
PROJECT_REFERENCES = 
BUILD_DIR = ../bin
PACKAGE_FLAG = 

WINDOWSBASE_DLL_MDB_SOURCE=../bin/WindowsBase.dll.mdb
WINDOWSBASE_DLL_MDB=$(BUILD_DIR)/WindowsBase.dll.mdb

AL=al2
SATELLITE_ASSEMBLY_NAME=

PROGRAMFILES = \
	$(WINDOWSBASE_DLL_MDB)  


RESGEN=resgen2
	
all: $(ASSEMBLY) $(PROGRAMFILES)

FILES =  \
        Assembly/AssemblyInfo.cs \
        ../build/common/*.cs \
	System.Windows/Point.cs \
	System.Windows/Rect.cs \
	System.Windows/Size.cs \
	System.Windows/Vector.cs \
	System.Windows.Media/Matrix.cs

DATA_FILES = 

RESOURCES =

REFERENCES =  \
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
