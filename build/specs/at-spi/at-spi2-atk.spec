#
# spec file for package at-spi2-atk (Version 0.3.3)
#
# Copyright (c) 2010 SUSE LINUX Products GmbH, Nuernberg, Germany.
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

%define IS_DEFAULT_ATSPI_STACK 1

Name:           at-spi2-atk
Version:        0.3.3
Release:        1
Summary:        Assistive Technology Service Provider Interface - GTK+ module
License:        GPLv2+
Group:          System/Libraries
Url:            http://www.gnome.org/
Source0:        %{name}-%{version}.tar.bz2
Source99:       %{name}-rpmlintrc
# PATCH-FIX-OPENSUSE at-spi2-atk-gtk-64-fix.patch bnc611766 sshaw@decriptor.com -- at-spi2 should set GTK_PATH64 rather than GTK_PATH
Patch0:         at-spi2-atk-gtk-64-fix.patch
BuildRequires:  atk-devel
BuildRequires:  dbus-1-glib-devel
BuildRequires:  fdupes
BuildRequires:  gconf2-devel
BuildRequires:  gtk2-devel
BuildRequires:  intltool
%if ! %IS_DEFAULT_ATSPI_STACK
BuildRequires:  update-desktop-files
%endif
# The GTK+ module is useful only if the at-spi registry is running. But it's
# not a strict runtime dependency.
Recommends:     at-spi2-core
# The library that was shipped with at-spi2-atk was removed.
Obsoletes:      libcspi0 <= 0.1.1
Obsoletes:      libcspi-devel <= 0.1.1
# Old versions of at-spi 1.x provided the same files
Conflicts:      at-spi < 1.29.3
# We want to have this package installed if the user has gtk2 and the at-spi
# stack already installed
Supplements:    packageand(at-spi2-core:gtk2)
BuildRoot:      %{_tmppath}/%{name}-%{version}-build

%description
AT-SPI is a general interface for applications to make use of the
accessibility toolkit. This version is based on dbus.

This package contains a GTK+ module for at-spi, based on ATK.

%lang_package
%prep
%setup -q
%ifarch x86_64
%patch0 -p1
%endif

%build
# We pass --enable-relocate when we want another at-spi stack (like at-spi) by default.
%configure \
%if %IS_DEFAULT_ATSPI_STACK
        --disable-relocate
%else
        --enable-relocate
%endif
%__make %{?jobs:-j%jobs}

%install
%makeinstall
# en@shaw isn't supported in openSUSE <= 11.2
%if 0%{?suse_version} <= 1120
%{__rm} %{buildroot}%{_datadir}/locale/en@shaw/LC_MESSAGES/*
%endif

find %{buildroot} -type f -name "*.la" -delete -print
%find_gconf_schemas
%find_lang %{name}
%if ! %IS_DEFAULT_ATSPI_STACK
%{__install} -d -m 0755 %{buildroot}%{_sysconfdir}/xdg/autostart
%{__mv} %{buildroot}%{_datadir}/gnome/autostart/atk-bridge.desktop %{buildroot}%{_sysconfdir}/xdg/autostart/
%suse_update_desktop_file atk-bridge
%endif

%clean
rm -rf %{buildroot}

%pre -f %{name}.schemas_pre

%preun -f %{name}.schemas_preun

%posttrans -f %{name}.schemas_posttrans

%files -f %{name}.schemas_list
%defattr(-,root,root)
%doc AUTHORS COPYING README
# FIXME when gtk 3.0 comes out
%{_libdir}/gtk-3.0
%if %IS_DEFAULT_ATSPI_STACK
%{_libdir}/gtk-2.0/modules/libatk-bridge.so
%{_libdir}/gtk-3.0/modules/libatk-bridge.so
%else
%{_libdir}/gtk-2.0/modules/at-spi-dbus/
%{_libdir}/gtk-3.0/modules/at-spi-dbus/
%{_sysconfdir}/xdg/autostart/atk-bridge.desktop
%endif

%files lang -f %{name}.lang

%changelog
