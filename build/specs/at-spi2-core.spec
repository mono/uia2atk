#
# spec file for package at-spi2-core (Version 0.1.3)
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



Name:           at-spi2-core
Version:        0.1.3
Release:        1
Summary:        Assistive Technology Service Provider Interface - D-Bus based implementation
License:        GPLv2+
Group:          System/Libraries
Url:            http://www.gnome.org/
Source0:        %{name}-%{version}.tar.bz2
Source99:       %{name}-rpmlintrc
BuildRequires:  dbus-1-glib-devel
BuildRequires:  gtk2-devel
BuildRequires:  update-desktop-files
# dbus-daemon is needed to have this work fine
Requires:       dbus-1
# The libraries that were shipped with at-spi2-core were removed, and so
# there's no devel package anymore
Obsoletes:      %{name}-devel <= 0.1.1
BuildRoot:      %{_tmppath}/%{name}-%{version}-build

%description
AT-SPI is a general interface for applications to make use of the
accessibility toolkit. This version is based on dbus.

This package contains the AT-SPI registry daemon. It provides a
mechanism for all assistive technologies to discover and interact
with applications running on the desktop.

%prep
%setup -q

%build
%configure \
        --libexecdir=%{_libexecdir}/at-spi2 \
        --disable-static
%__make %{?jobs:-j%jobs}

%install
%makeinstall
%suse_update_desktop_file at-spi-dbus-bus

%clean
rm -rf %{buildroot}

%post -p /sbin/ldconfig

%postun -p /sbin/ldconfig

%files
%defattr(-,root,root)
%doc AUTHORS COPYING README
%{_sysconfdir}/at-spi2
%{_datadir}/at-spi2
%{_sysconfdir}/xdg/autostart/at-spi-dbus-bus.desktop
%{_libexecdir}/at-spi2/
%{_datadir}/dbus-1/services/org.freedesktop.atspi.Registry.service
%{_bindir}/at-spi-dbus-bus

%changelog
