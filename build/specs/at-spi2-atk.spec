#
# spec file for package at-spi2-atk
#    
# Copyright (c) 2008 SUSE LINUX Products GmbH, Nuernberg, Germany.
# This file and all modifications and additions to the pristine
# package are under the same license as the package itself.
# 
# Please submit bugfixes or comments via http://bugs.opensuse.org/ 
# 
# norootforbuild 
# 

Name:           at-spi2-atk
Version:        0.1.2
Release:        1
Summary:        Assistive Technology Service Provider Interface - dbus
License:        GPL v2.0 or later
Group:          System/Libraries
URL:            http://www.gnome.org/
Source0:        %{name}-%{version}.tar.bz2
BuildRoot:      %{_tmppath}/%{name}-%{version}-build
Requires:       at-spi2-core
Requires:       atk
Requires:	dbus-1
Requires:	dbus-1-glib
BuildRequires:  atk-devel
BuildRequires:  dbus-1-devel
BuildRequires:  dbus-1-glib-devel
BuildRequires:	libxml2-devel
#BuildRequires:  update-desktop-files

%description
This library, based on ATK, is a general interface for applications to 
make use of the accessibility toolkit.  This version is based on dbus.

#%package devel
#Group:          Development/Libraries/GNOME
#Summary:        Include Files and Libraries mandatory for Development
#Requires:	%{name} = %{version} 
#
#%description devel
#This package contains all necessary include files and libraries needed
#to develop applications that require these.

%prep
%setup -q

%build
%configure
%__make %{?jobs:-j%jobs}

%install
%makeinstall
find %{buildroot} -type f -name "*.la" -delete -print
#%suse_update_desktop_file atk-bridge

%clean
rm -rf %{buildroot}


%files
%defattr(-,root,root)
%doc AUTHORS COPYING INSTALL README
#%dir %{_datadir}/gnome
#%dir %{_datadir}/gnome/autostart
#%config %{_datadir}/gnome/autostart/atk-bridge.desktop
%{_libdir}/gtk-2.0/modules/libatk-bridge.so

#%files devel
#%defattr(-,root,root)
#%dir %{_includedir}/at-spi-1.0
#%dir %{_includedir}/at-spi-1.0/libspi
#%{_includedir}/at-spi-1.0/libspi/*
#%{_libdir}/libcspi.so
#%dir %{_includedir}/at-spi-1.0/libspi
#%{_includedir}/at-spi-1.0/libspi/*

%changelog
