#
# spec file for package at-spi2-core
#    
# Copyright (c) 2008 SUSE LINUX Products GmbH, Nuernberg, Germany.
# This file and all modifications and additions to the pristine
# package are under the same license as the package itself.
# 
# Please submit bugfixes or comments via http://bugs.opensuse.org/ 
# 
# norootforbuild 
# 

Name:           at-spi2-core
Version:        0.1.0
Release:        1
Summary:        Assistive Technology Service Provider Interface - dbus
License:        GPL v2.0 or later
Group:          System/Libraries
URL:            http://www.gnome.org/
Source0:        %{name}-%{version}.tar.bz2
BuildRoot:      %{_tmppath}/%{name}-%{version}-build
Requires:	dbus-1
Requires:	dubs-1-glib
Requires:	python
BuildRequires:  dbus-1-devel
BuildRequires:  dbus-1-glib-devel
BuildRequires:  glib2-devel
BuildRequires:  gtk2-devel
BuildRequires:  python
BuildRequires:  update-desktop-files
Obsoletes:	at-spi

%description
This library, based on ATK, is a general interface for applications to 
make use of the accessibility toolkit.  This version is based on dbus.

%package devel
Group:          Development/Libraries/GNOME
Summary:        Include Files and Libraries mandatory for Development
Requires:	%{name} = %{version} 
Requires:	atk-devel
Requires:	dbus-1-devel
Requires:	dbus-1-glib-devel
Requires:	gtk2-devel
Obsoletes:	at-spi-devel

%description devel
This package contains all necessary include files and libraries needed
to develop applications that require these.

%prep
%setup -q 

%build
%configure
%__make %{?jobs:-j%jobs}

%install
%makeinstall
%suse_update_desktop_file at-spi-registryd
find %{buildroot} -type f -name "*.la" -delete -print

%clean
rm -rf %{buildroot}

%post -p /sbin/ldconfig

%postun -p /sbin/ldconfig

%files
%defattr(-,root,root)
%doc AUTHORS COPYING INSTALL README
%{_datadir}/%{name}
%config %{_sysconfdir}/xdg/autostart/at-spi-registryd.desktop
%{_libdir}/libdbind.so.*
%{_libdir}/libdroute.so.*
%{_libdir}/at-spi-registryd
%{_datadir}/dbus-1/services/org.freedesktop.atspi.Registry.service

%files devel
%defattr(-,root,root)
%{_includedir}/dbind-0.1
%{_includedir}/droute-0.1
%{_libdir}/libdbind.so
%{_libdir}/libdroute.so
%{_libdir}/pkgconfig/dbind-0.1.pc
%{_libdir}/pkgconfig/droute-0.1.pc

%changelog

