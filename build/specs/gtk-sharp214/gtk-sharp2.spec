Name:           gtk-sharp2
%define _name gtk-sharp
%ifarch ppc64
BuildRequires:  mono-biarchcompat
%endif
Url:            http://gtk-sharp.sf.net
License:        GPL v2 or later; LGPL v2.1 or later
Group:          System/GUI/GNOME
Summary:        .Net Language Bindings for GTK+
Patch0:         gtk-sharp-optflags.patch
Patch1:         gtk-sharp-revert_unportable_relocatable.patch
Patch2:         gtk-sharp-makefile.patch
Patch3:         gtk-sharp-find_gtkhtml_ver.patch
Patch4:         gtk-sharp-fix_vte_so_version.patch
Patch5:         gnome-sharp-revert_unportable_relocatable.patch
# PATCH-FIX-OPENSUSE Fix: Program returns random data in a function
Patch6:         gtk-warn-fix.patch
%define old_version 2.4.3
%define new_version 2.8.5
%define new_split_version 2.10.4
%define two_twelve_version 2.12.6
#####  suse  ####
%if 0%{?suse_version}
## which gtk version ###
%if %suse_version < 1010
%define _version %old_version
%endif
%if %suse_version == 1010
%define _version %new_version
%endif
%if %suse_version == 1020
%define _version %new_split_version
%endif
%if %suse_version >= 1030
%define _version %two_twelve_version
%endif
# Not needed with rpm .config dep search
#%define gtkhtml_requires gtkhtml2
%define new_suse_buildrequires librsvg-devel mono-devel vte-devel gnome-panel-devel  monodoc-core update-desktop-files
%if %sles_version == 10
BuildRequires:  %{new_suse_buildrequires} -gnome-panel-devel gnome-panel-nld-devel
%endif
%if %suse_version >= 1020
BuildRequires:  %{new_suse_buildrequires} gtkhtml2-devel
%endif
%if %suse_version == 1010
BuildRequires:  %{new_suse_buildrequires} gtkhtml2-devel
%endif
%endif
#################
####  fedora  ####
%if 0%{?fedora_version}
%define env_options export MONO_SHARED_DIR=/tmp
%if 0%{?fedora_version} < 6
%define _version %new_version
%endif
%if 0%{?fedora_version} == 6
%define _version %new_split_version
%endif
%if 0%{?fedora_version} == 7
%define _version %new_split_version
%endif
%if 0%{?fedora_version} >= 8
%define _version %two_twelve_version
%endif
# All fedora distros (5 and 6) have the same names, requirements
BuildRequires:  gnome-panel-devel gtkhtml3-devel libgnomeprintui22-devel librsvg2-devel mono-devel monodoc-core vte-devel
# Not needed with rpm .config dep search
#%define gtkhtml_requires gtkhtml2
%endif
# RHEL
%if 0%{?rhel_version} >= 500
%define env_options export MONO_SHARED_DIR=/tmp
%define _version %new_split_version
BuildRequires:  gnome-panel-devel gtkhtml3-devel libgnomeprintui22-devel librsvg2-devel mono-devel monodoc-core vte-devel
%endif
#################
##############
### Options that relate to a version of gtk#, not necessarily a distro
# Define true for 2.10 and 2.12
#  (Must do this inside of shell... rpm can't handle this expression)
%define platform_desktop_split %(if test x%_version = x%new_split_version || test x%_version = x%two_twelve_version ; then  echo "1" ; else echo "0" ; fi)
# define true for 2.12.0
%define include_atk_glue %(if test x%_version = x%two_twelve_version  ; then echo "1" ; else echo "0" ; fi )
###
##############
# Need to put this stuff down here after Version: gets defined
Version:        %_version
Release:        16
Source:         %{_name}-%{version}.tar.bz2
BuildRoot:      %{_tmppath}/%{name}-%{version}-build

%description
This package contains Mono bindings for gtk+, gdk, atk, and pango.



%package gapi
License:        LGPL v2.1 or later
Group:          System/GUI/GNOME
Summary:        C Source Parser and C Generator
Requires:       perl-XML-LibXML-Common perl-XML-LibXML perl-XML-SAX

%description gapi
The gtk-sharp-gapi package includes the parser and code generator used
by the GTK if you want to bind GObject-based libraries, or need to
compile a project that uses it to bind such a library.



%package -n gtk-sharp2-doc
License:        GPL v2 or later; LGPL v2.1 or later
Summary:        Monodoc documentation for gtk-sharp2
Group:          System/GUI/GNOME
# Disable this for now, as it's a circular dep
#  Works ok in autobuild/buildservice, not so well in monobuild
#Requires:       mono-tools

%description -n gtk-sharp2-doc
This package contains the gtk-sharp2 documentation for monodoc.



%package -n glib-sharp2
License:        GPL v2 or later; LGPL v2.1 or later
Summary:        Mono bindings for glib
Group:          System/GUI/GNOME

%description -n glib-sharp2
This package contains Mono bindings for glib.



%package -n glade-sharp2
License:        GPL v2 or later; LGPL v2.1 or later
Group:          System/GUI/GNOME
Summary:        Mono bindings for glade

%description -n glade-sharp2
This package contains Mono bindings for glade.



%package -n gtk-sharp2-complete
License:        GPL v2 or later; LGPL v2.1 or later
Group:          System/GUI/GNOME
Summary:        GTK+ and GNOME bindings for Mono (virtual package)
Requires:       glade-sharp2 = %{version}-%{release}
Requires:       glib-sharp2 = %{version}-%{release}
Requires:       gtk-sharp2 = %{version}-%{release}
Requires:       gtk-sharp2-doc = %{version}-%{release}
Requires:       gtk-sharp2-gapi = %{version}-%{release}
%if %platform_desktop_split == 0
Requires:       art-sharp2 = %{version}-%{release}
Requires:       gconf-sharp2 = %{version}-%{release}
Requires:       gnome-sharp2 = %{version}-%{release}
Requires:       gnome-vfs-sharp2 = %{version}-%{release}
Requires:       gtkhtml-sharp2 = %{version}-%{release}
Requires:       rsvg-sharp2 = %{version}-%{release}
Requires:       vte-sharp2 = %{version}-%{release}
%endif

%description -n gtk-sharp2-complete
Gtk# is a library that allows you to build fully native graphical GNOME
applications using Mono. Gtk# is a binding to GTK+, the cross platform
user interface toolkit used in GNOME. It includes bindings for Gtk,
Atk, Pango, Gdk, libgnome, libgnomeui and libgnomecanvas.  (Virtual
package which depends on all gtk-sharp2 subpackages)



%if %platform_desktop_split == 0

%package -n gnome-sharp2
License:        LGPL v2.1 or later
Summary:        Mono bindings for Gnome
Group:          System/GUI/GNOME

%description -n gnome-sharp2
This package contains Mono bindings for Gnome.



%package -n rsvg-sharp2
License:        GPL v2 or later; LGPL v2.1 or later
Summary:        Mono bindings for rsvg
Group:          System/GUI/GNOME
# Not needed with rpm .config dep search
#Requires:       librsvg

%description -n rsvg-sharp2
This package contains Mono bindings for librsvg.



%package -n gtkhtml-sharp2
License:        GPL v2 or later; LGPL v2.1 or later
Summary:        Mono bindings for gtkhtml
Group:          System/GUI/GNOME
# Not needed with rpm .config dep search
#Requires:       %gtkhtml_requires

%description -n gtkhtml-sharp2
This package contains Mono bindings for gtkhtml.



%package -n gnome-vfs-sharp2
License:        GPL v2 or later; LGPL v2.1 or later
Summary:        Mono bindings for gnomevfs
Group:          System/GUI/GNOME
# Not needed with rpm .config dep search
#Requires:       gnome-vfs2

%description -n gnome-vfs-sharp2
This package contains Mono bindings gnomevfs.



%package -n art-sharp2
License:        GPL v2 or later; LGPL v2.1 or later
Summary:        Mono bindings for libart
Group:          System/GUI/GNOME
# Not needed with rpm .config dep search
#Requires:       libart_lgpl

%description -n art-sharp2
This package contains Mono bindings for libart.



%package -n vte-sharp2
License:        GPL v2 or later; LGPL v2.1 or later
Group:          System/GUI/GNOME
Summary:        Mono bindings for vte
# Not needed with rpm .config dep search
#Requires:       vte

%description -n vte-sharp2
This package contains Mono bindings for vte.



%package -n gconf-sharp2
License:        GPL v2 or later; LGPL v2.1 or later
Summary:        Mono bindings for gconf
Group:          System/GUI/GNOME

%description -n gconf-sharp2
This package contains Mono bindings for gconf and gconf peditors.



%package -n gio-sharp2
License:        GPL v2 or later; LGPL v2.1 or later
Summary:        Mono bindings for gio
Group:          System/GUI/GNOME

%description -n gio-sharp2
This package contains Mono bindings for gconf and gconf peditors.



%endif

%prep
%setup -q -n %{_name}-%{version}
#if [ %version \< 2.10.3 ] ; then
#%patch0 -p1
#fi
%if %platform_desktop_split == 0
%patch1 -p1
# 2.8.4 and later on 2.8.x branch doesn't need this patch
if [ %version \< 2.8.4 ] ; then
%patch2
fi
%patch3 -p1
%patch4 -p1
%patch5 -p1
%endif
if [ %version == %two_twelve_version ] ; then
%patch6
fi

%build
%{?env_options}
autoreconf -f -i
# FIXME: windowmanager.c:*: warning: dereferencing type-punned pointer will break strict-aliasing rules
export CFLAGS="$RPM_OPT_FLAGS -fno-strict-aliasing"
%configure\
	--libexecdir=%{_prefix}/lib\
	--enable-debug
make

%install
%{?env_options}
%makeinstall
rm $RPM_BUILD_ROOT%{_libdir}/*.*a
# Special handling for new files
touch %name.files
# atk glue for now...
%define atk_glue %{_libdir}/libatksharpglue-2.so
%if 0%{?include_atk_glue}
echo "%atk_glue" >> %name.files
%endif

%clean
rm -rf $RPM_BUILD_ROOT

%files -f %name.files
%defattr(-, root, root)
%{_libdir}/libgdksharpglue-2.so
%{_libdir}/libgtksharpglue-2.so
%{_libdir}/libpangosharpglue-2.so
%{_libdir}/pkgconfig/gtk-sharp-2.0.pc
%{_libdir}/pkgconfig/gtk-dotnet-2.0.pc
%{_prefix}/lib/mono/gac/*atk-sharp
%{_prefix}/lib/mono/gtk-sharp-2.0/*atk-sharp.dll
%{_prefix}/lib/mono/gac/*gdk-sharp
%{_prefix}/lib/mono/gtk-sharp-2.0/*gdk-sharp.dll
%{_prefix}/lib/mono/gac/*gtk-dotnet
%{_prefix}/lib/mono/gtk-sharp-2.0/*gtk-dotnet.dll
%{_prefix}/lib/mono/gac/*gtk-sharp
%{_prefix}/lib/mono/gtk-sharp-2.0/*gtk-sharp.dll
%{_prefix}/lib/mono/gac/*pango-sharp
%{_prefix}/lib/mono/gtk-sharp-2.0/*pango-sharp.dll

%files gapi
%defattr(-, root, root)
%{_bindir}/gapi2-codegen
%{_bindir}/gapi2-fixup
%{_bindir}/gapi2-parser
%{_datadir}/gapi-2.0
%{_libdir}/pkgconfig/gapi-2.0.pc
%{_prefix}/lib/gtk-sharp-2.0/gapi_codegen.exe
%{_prefix}/lib/gtk-sharp-2.0/gapi-fixup.exe
%{_prefix}/lib/gtk-sharp-2.0/gapi-parser.exe
%{_prefix}/lib/gtk-sharp-2.0/gapi_pp.pl
%{_prefix}/lib/gtk-sharp-2.0/gapi2xml.pl

%files -n gtk-sharp2-doc
%defattr(-, root, root)
%doc COPYING ChangeLog README
%{_prefix}/lib/monodoc

%files -n glib-sharp2
%defattr(-, root, root)
%{_libdir}/libglibsharpglue-2.so
%{_libdir}/pkgconfig/glib-sharp-2.0.pc
%{_prefix}/lib/mono/gac/*glib-sharp
%{_prefix}/lib/mono/gtk-sharp-2.0/*glib-sharp.dll

%files -n glade-sharp2
%defattr(-, root, root)
%{_libdir}/libgladesharpglue-2.so
%{_libdir}/pkgconfig/glade-sharp-2.0.pc
%{_prefix}/lib/mono/gac/*glade-sharp
%{_prefix}/lib/mono/gtk-sharp-2.0/*glade-sharp.dll

%files -n gio-sharp2
%defattr(-, root, root)
%{_prefix}/lib/libgiosharpglue-2.so
%{_prefix}/lib/mono/gac/*gio-sharp
%{_prefix}/lib/gtk-sharp-2.0/*gio-sharp.dll
%{_libdir}/pkgconfig/gio-sharp-2.0.pc

%files -n gtk-sharp2-complete
%defattr(-, root, root)
## This is the 'base' package so we put the common dirs of all in this package
# Otherwise, this package doesn't get created!
%dir %{_prefix}/lib/mono/gtk-sharp-2.0
%dir %{_prefix}/lib/gtk-sharp-2.0
##############################################################################
############# FILELIST START of packages split as gnome-sharp ################
%if %platform_desktop_split == 0

%files -n gnome-sharp2
%defattr(-,root,root)
%{_libdir}/libgnomesharpglue-2.so
%{_libdir}/pkgconfig/gnome-sharp-2.0.pc
%{_prefix}/lib/mono/gac/*gnome-sharp
%{_prefix}/lib/mono/gtk-sharp-2.0/*gnome-sharp.dll

%files -n rsvg-sharp2
%defattr(-,root,root)
%{_libdir}/pkgconfig/rsvg-sharp-2.0.pc
%{_prefix}/lib/mono/gac/*rsvg-sharp
%{_prefix}/lib/mono/gtk-sharp-2.0/*rsvg-sharp.dll

%files -n gtkhtml-sharp2
%defattr(-,root,root)
%{_libdir}/pkgconfig/gtkhtml-sharp-2.0.pc
%{_prefix}/lib/mono/gac/*gtkhtml-sharp
%{_prefix}/lib/mono/gtk-sharp-2.0/*gtkhtml-sharp.dll

%files -n gnome-vfs-sharp2
%defattr(-,root,root)
%{_libdir}/pkgconfig/gnome-vfs-sharp-2.0.pc
%{_prefix}/lib/mono/gac/*gnome-vfs-sharp
%{_prefix}/lib/mono/gtk-sharp-2.0/*gnome-vfs-sharp.dll

%files -n art-sharp2
%defattr(-,root,root)
%{_libdir}/pkgconfig/art-sharp-2.0.pc
%{_prefix}/lib/mono/gac/*art-sharp
%{_prefix}/lib/mono/gtk-sharp-2.0/*art-sharp.dll

%files -n vte-sharp2
%defattr(-, root, root)
%{_libdir}/libvtesharpglue-2.so
%{_libdir}/pkgconfig/vte-sharp-2.0.pc
%{_prefix}/lib/mono/gac/*vte-sharp
%{_prefix}/lib/mono/gtk-sharp-2.0/*vte-sharp.dll

%files -n gconf-sharp2
%defattr(-, root, root)
%{_bindir}/gconfsharp2-schemagen
%{_libdir}/pkgconfig/gconf-sharp-2.0.pc
%{_prefix}/lib/gtk-sharp-2.0/gconfsharp-schemagen.exe
%{_prefix}/lib/mono/gac/*gconf-sharp
%{_prefix}/lib/mono/gtk-sharp-2.0/*gconf-sharp.dll
# Other distros place these in gnome-sharp2??
%{_prefix}/lib/mono/gac/*gconf-sharp-peditors
%{_prefix}/lib/mono/gtk-sharp-2.0/*gconf-sharp-peditors.dll

%endif
############### FILELIST END of packages split as gnome-sharp ################
##############################################################################
%if 0%{?fedora_version} || 0%{?rhel_version}
# Allows overrides of __find_provides in fedora distros... (already set to zero on newer suse distros)
%define _use_internal_dependency_generator 0
%endif
%define __find_provides env sh -c 'filelist=($(cat)) && { printf "%s\\n" "${filelist[@]}" | /usr/lib/rpm/find-provides && printf "%s\\n" "${filelist[@]}" | /usr/bin/mono-find-provides ; } | sort | uniq'
%define __find_requires env sh -c 'filelist=($(cat)) && { printf "%s\\n" "${filelist[@]}" | /usr/lib/rpm/find-requires && printf "%s\\n" "${filelist[@]}" | /usr/bin/mono-find-requires ; } | sort | uniq'

%changelog
