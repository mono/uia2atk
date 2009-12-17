#
# spec file for package at-spi (Version 1.28.0)
#
# Copyright (c) 2009 SUSE LINUX Products GmbH, Nuernberg, Germany.
#
# All modifications and additions to the file contributed by third parties
# remain the property of their copyright owners, unless otherwise agreed
# upon. The license for this file, and modifications and additions to the
# file, is the same license as for the pristine package itself (unless the
# license for the pristine package is not an Open Source License, in which
# case the license is the MIT License). An "Open Source License" is a
# license that conforms to the Open Source Definition (Version 1.9)
# published by the Open Source Initiative.

# Please submit bugfixes or comments via http://bugs.opensuse.org/
#



Name:           at-spi
Version:        1.28.0
Release:        1
License:        GPL v2 or later ; LGPL v2.1 or later
Summary:        Assistive Technology Service Provider Interface
Url:            http://www.gnome.org/
Group:          Development/Libraries/GNOME
Source:         %{name}-%{version}.tar.bz2
BuildRequires:  fdupes
BuildRequires:  gconf2-devel
BuildRequires:  gtk-doc
BuildRequires:  gtk2-devel
BuildRequires:  intltool
BuildRequires:  libbonobo-devel
BuildRequires:  libidl-devel
BuildRequires:  python
BuildRequires:  translation-update-upstream
BuildRequires:  update-desktop-files
Requires:       %{name}-lang = %{version}
# Needed for the python bindings
Requires:       python-gnome
Requires:       python-orbit
BuildRoot:      %{_tmppath}/%{name}-%{version}-build
%gconf_schemas_prereq

%description
This library, based on ATK, is a general interface for applications to
make use of the accessibility toolkit.

%package devel
License:        GPL v2 or later ; LGPL v2.1 or later
Summary:        Include Files and Libraries mandatory for Development
Group:          Development/Libraries/GNOME
Requires:       %{name} = %{version}
Requires:       atk-devel
Requires:       gtk2-devel
Requires:       libbonobo-devel

%description devel
This package contains all necessary include files and libraries needed
to develop applications that require these.

%package doc
License:        GPL v2 or later ; LGPL v2.1 or later
Summary:        Additional Package Documentation
Group:          Development/Libraries/GNOME
Requires:       %{name} = %{version}

%description doc
This package contains optional documentation provided in addition to
this package's base documentation.

%lang_package
%prep
%setup -q
translation-update-upstream

%build
%configure\
        --disable-static \
	--libexecdir=%{_prefix}/lib/at-spi
make %{?jobs:-j%jobs} referencetopdir=%{_docdir}/%{name}/reference

%install
make install referencetopdir=%{_docdir}/%{name}/reference DESTDIR=%{buildroot}
find %{buildroot} -type f -name "*.la" -delete -print
%suse_update_desktop_file at-spi-registryd
%find_gconf_schemas
%find_lang %{name}
cp AUTHORS COPYING ChangeLog NEWS README %{buildroot}%{_docdir}/%{name}
%fdupes %{buildroot}

%clean
rm -rf %{buildroot}

%post -p /sbin/ldconfig

%postun -p /sbin/ldconfig

%pre -f %{name}.schemas_pre

%preun -f %{name}.schemas_preun

%posttrans -f %{name}.schemas_posttrans

%files -f %{name}.schemas_list
%defattr (-, root, root)
%doc %dir %{_docdir}/%{name}
%doc %{_docdir}/%{name}/AUTHORS
%doc %{_docdir}/%{name}/COPYING
%doc %{_docdir}/%{name}/ChangeLog
%doc %{_docdir}/%{name}/NEWS
%doc %{_docdir}/%{name}/README
%{_libdir}/*.so.*
%{_libdir}/bonobo/servers/Accessibility_Registry.server
%{_libdir}/gtk-2.0/modules/*.so
%{_libdir}/orbit-2.0/*.so
%{_prefix}/lib/at-spi
%{_sysconfdir}/xdg/autostart/*.desktop
# FIXME: split these off into a separate -python package
%dir %{py_sitedir}/pyatspi/
%{py_sitedir}/pyatspi/*

%files lang -f %{name}.lang

%files devel
%defattr (-, root, root)
%{_includedir}/at-spi-1.0
%{_libdir}/*.so
%{_libdir}/pkgconfig/*.pc
%{_datadir}/idl/at-spi-1.0

%files doc
%defattr (-, root, root)
%{_datadir}/gtk-doc/html/at-spi-cspi
%doc %{_docdir}/%{name}/reference

%changelog

