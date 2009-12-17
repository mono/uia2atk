#
# spec file for package at-spi2-atk (Version 0.1.3)
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



Name:           at-spi2-atk
Version:        0.1.3
Release:        1
Summary:        Assistive Technology Service Provider Interface - GTK+ module
License:        GPLv2+
Group:          System/Libraries
Url:            http://www.gnome.org/
Source0:        %{name}-%{version}.tar.bz2
Source99:       %{name}-rpmlintrc
BuildRequires:  atk-devel
BuildRequires:  dbus-1-glib-devel
BuildRequires:  fdupes
BuildRequires:  gtk2-devel
BuildRequires:  libxml2-devel
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

%prep
%setup -q

%build
%configure
%__make %{?jobs:-j%jobs}

%install
%makeinstall
find %{buildroot} -type f -name "*.la" -delete -print

%clean
rm -rf %{buildroot}

%files
%defattr(-,root,root)
%doc AUTHORS COPYING README
%{_libdir}/gtk-2.0/modules/libatk-bridge.so

%changelog
