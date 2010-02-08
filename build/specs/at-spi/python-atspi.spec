#
# spec file for package python-atspi (Version 0.1.3)
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



Name:           python-atspi
%define _name   pyatspi
Version:        0.1.6
Release:        51
Summary:        Assistive Technology Service Provider Interface - Python bindings
License:        LGPL v2.0
Group:          Development/Libraries/Python
Url:            http://www.gnome.org/
Source0:        %{_name}-%{version}.tar.bz2
BuildRequires:  fdupes
BuildRequires:  python
Requires:       dbus-1-python
Requires:       python-gobject2
Requires:       python-gtk
# The bindings are really useful only if the at-spi registry is running. But
# it's not a strict runtime dependency.
Recommends:     at-spi2-core
# Old versions of at-spi 1.x provided the same files
Conflicts:      at-spi < 1.29.3
# Virtual package, so that it's possible to use either python-atspi or
# python-atspi-corba
Provides:       pyatspi
BuildRoot:      %{_tmppath}/%{name}-%{version}-build
%if %suse_version > 1110
BuildArch:      noarch
%endif
%if %suse_version <= 1110
%define python_sitelib %{py_sitedir}
%endif
%py_requires

%description
AT-SPI is a general interface for applications to make use of the
accessibility toolkit. This version is based on dbus.

This package contains the python bindings for AT-SPI.

%prep
%setup -q -n %{_name}-%{version}

%build
%configure --disable-relocate
%__make %{?jobs:-j%jobs}

%install
%makeinstall
%fdupes %{buildroot}%{python_sitelib}

%clean
rm -rf %{buildroot}

%files
%defattr(-,root,root)
%doc AUTHORS COPYING README
%{python_sitelib}/pyatspi/

%changelog
