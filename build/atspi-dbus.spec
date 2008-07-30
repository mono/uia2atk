#
# spec file for package at-spi
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
License:       unknown
Group:         Development/Languages
BuildArch:     i586
URL:           GPL2
Source0:       %{name}-%{version}.tar.bz2
BuildRoot:     %{_tmppath}/%{name}-%{version}-build
AutoReqProv:   on
#Requires:      
BuildRequires: atk
Summary:       at-spi (dbus)

%description
at-spi major break, moving from orbit/corba to dbus

%prep
%setup -q 

%build
./configure  --prefix=%_prefix
make

%install
make install DESTDIR=${RPM_BUILD_ROOT}

#file we don't care about
rm -f $RPM_BUILD_ROOT/%_prefix/lib/gtk-2.0/modules/libspiatk.la
rm -f $RPM_BUILD_ROOT/%_prefix/lib/libcspi.la
rm -f $RPM_BUILD_ROOT/%_prefix/lib/libdbind.la
rm -f $RPM_BUILD_ROOT/%_prefix/lib/libloginhelper.la
rm -f $RPM_BUILD_ROOT/%_prefix/lib/python/site-packages/pyatspi/*.pyc
rm -f $RPM_BUILD_ROOT/%_prefix/lib/python/site-packages/pyatspi/*.pyo

%clean
rm -rf $RPM_BUILD_ROOT

#%post

#%preun

%files
%defattr(-,root,root)
%doc README COPYING
%_prefix/include/at-spi-1.0/cspi/spi-impl.h
%_prefix/include/at-spi-1.0/cspi/spi-listener.h
%_prefix/include/at-spi-1.0/cspi/spi-roletypes.h
%_prefix/include/at-spi-1.0/cspi/spi-stateset.h
%_prefix/include/at-spi-1.0/cspi/spi-statetypes.h
%_prefix/include/at-spi-1.0/cspi/spi.h
%_prefix/include/at-spi-1.0/libspi/event-types.h
%_prefix/include/at-spi-1.0/libspi/generated-types.h
%_prefix/include/at-spi-1.0/libspi/keymasks.h
%_prefix/include/at-spi-1.0/libspi/spi-dbus.h
%_prefix/include/at-spi-1.0/libspi/spi-types.h
%_prefix/include/at-spi-1.0/login-helper/login-helper.h
%_prefix/include/dbind-0.1/dbind-any.h
%_prefix/include/dbind-0.1/dbind.h
%_prefix/lib/python/site-packages/pyatspi/*.py
%_prefix/lib/gtk-2.0/modules/libspiatk.so
%_prefix/lib/libcspi.so*
%_prefix/lib/libdbind.so*
%_prefix/lib/libloginhelper.so*
%_prefix/lib/pkgconfig/*.pc
%_prefix/libexec/at-spi-registryd
%_prefix/share/at-spi/dbus/*



%changelog
