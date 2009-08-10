#
# spec file for package at-spi-atk
#    
# Copyright (c) 2008 SUSE LINUX Products GmbH, Nuernberg, Germany.
# This file and all modifications and additions to the pristine
# package are under the same license as the package itself.
# 
# Please submit bugfixes or comments via http://bugs.opensuse.org/ 
# 
# norootforbuild 
# 

Name:           at-spi-atk
Version:        2.0.0
Release:        1
Summary:        Assistive Technology Service Provider Interface - dbus
License:        GPL v2.0 or later
Group:          System/Libraries
URL:            http://www.gnome.org/
Source0:        %{name}-%{version}.tar.bz2
BuildRoot:      %{_tmppath}/%{name}-%{version}-build
Requires:	at-spi2-core
Requires:	python
BuildRequires:	at-spi2-core-devel
BuildRequires:	fdupes
BuildRequires:	libxml2-devel

%description
This library, based on ATK, is a general interface for applications to 
make use of the accessibility toolkit.  This version is based on dbus.

%package -n libcspi0
Group:		System/Libraries
Summary:	C bindings for at-spi
Requires:	%{name} = %{version}

%description -n libcspi0
C bindings for at-spi used mostly for gok

%package devel
Group:          Development/Libraries/GNOME
Summary:        Include Files and Libraries mandatory for Development
Requires:	%{name} = %{version} 

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
find %{buildroot} -type f -name "*.la" -delete -print
%fdupes $RPM_BUILD_ROOT

%clean
rm -rf %{buildroot}

%post -n libcspi0 -p /sbin/ldconfig

%postun -n libcspi0 -p /sbin/ldconfig

%files
%defattr(-,root,root)
%doc AUTHORS COPYING INSTALL README
%dir %{py_sitedir}/pyatspi
%{py_sitedir}/pyatspi/*

%files -n libcspi0
%defattr(-,root,root)
%{_libdir}/libcspi.so.*

%files devel
%defattr(-,root,root)
%dir %{_includedir}/at-spi-1.0
%dir %{_includedir}/at-spi-1.0/cspi
%{_includedir}/at-spi-1.0/cspi/*
%{_libdir}/libcspi.so
%dir %{_includedir}/at-spi-1.0/libspi
%{_includedir}/at-spi-1.0/libspi/*
%{_libdir}/gtk-2.0/modules/libspiatk.so

%changelog

