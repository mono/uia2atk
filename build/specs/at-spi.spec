#
# spec file for package at-spi (Version 1.9.0 dbus)
#    
# Copyright (c) 2008 SUSE LINUX Products GmbH, Nuernberg, Germany.
# This file and all modifications and additions to the pristine
# package are under the same license as the package itself.
# 
# Please submit bugfixes or comments via http://bugs.opensuse.org/ 
# 
# norootforbuild 
# 

Name:          at-spi
Version:       1.9.0
Release:       0.git
License:       GPL v2 or later
Group:         Development/Libraries/GNOME
URL:           http://www.gnome.org/
Source0:       %{name}-%{version}.tar.bz2
BuildRoot:     %{_tmppath}/%{name}-%{version}-build
AutoReqProv:   on
Requires:      %{name}-lang = %{version}
BuildRequires: fdupes bundle-lang-common-en pkg-config python-devel glib2-devel
BuildRequires: atk-devel >= 1.17.0
BuildRequires: libxml2-devel >= 2.0.0
BuildRequires: dbus-1-devel >= 1.0
BuildRequires: dbus-1-glib-devel >= 0.7.0
BuildRequires: gtk2-devel > 2.10.0
%if 0%{suse_version} == 1100
BuildRequires: gail-devel > 1.9.0 gail-lang > 1.9.0
%endif
BuildRequires: libgail-gnome-devel

Summary:       Assistive Technology Service Provider Interface - dbus

%description
This library, based on ATK, is a general interface for applications to 
make use of the accessibility toolkit.  This version is based on dbus.



Authors:
--------
    Mike Gorse <mike.gorse@novell.com>
    Mark Doffman <mark.doffman@codethink.co.uk>

%package devel
Group:         Development/Libraries/GNOME
Summary:       Include Files and Libraries mandatory for Development
Requires:      %{name} = %{version} atk-devel gtk2-devel dbus-1-devel dbus-1-glib-devel 
%description devel
This package contains all necessary include files and libraries needed
to develop applications that require these.

#%lang_package
%prep
%setup -q 

%build
%configure   \
    --libexecdir=%{_libdir}/%name

%__make %{?_smp_mflags}

%install
make install DESTDIR=$RPM_BUILD_ROOT
#%find_lang %{name}
%fdupes $RPM_BUILD_ROOT

#file we don't care about
find $RPM_BUILD_ROOT -name "*.la" | xargs rm

%clean
rm -rf $RPM_BUILD_ROOT

%post -p /sbin/ldconfig
%post devel -p /sbin/ldconfig

%postun -p /sbin/ldconfig
%postun devel -p /sbin/ldconfig

%files
%defattr(-,root,root)
%doc AUTHORS ChangeLog INSTALL NEWS README COPYING
%_prefix/share/at-spi/dbus/*
%{_libdir}/gtk-2.0/modules/libspiatk.so
%{_libdir}/%{name}/at-spi-registryd

# FIXME: split these off into a separate -python package
%dir %{py_sitedir}/pyatspi/
%{py_sitedir}/pyatspi/*

#%files lang -f %{name}.lang 

%files devel
%defattr(-,root,root)
%{_libdir}/pkgconfig/*.pc
%{_libdir}/*.so*
%{_includedir}/at-spi-1.0/cspi/*.h
%{_includedir}/at-spi-1.0/libspi/*.h
%{_includedir}/at-spi-1.0/login-helper/login-helper.h
%{_includedir}/dbind-0.1/*.h

%changelog

  
