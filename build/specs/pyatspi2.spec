#
# spec file for package pyatspi2
#    
# Copyright (c) 2008 SUSE LINUX Products GmbH, Nuernberg, Germany.
# This file and all modifications and additions to the pristine
# package are under the same license as the package itself.
# 
# Please submit bugfixes or comments via http://bugs.opensuse.org/ 
# 
# norootforbuild 
# 

Name:           pyatspi2
Version:        0.1.2
Release:        1
BuildArch:      noarch
Summary:        Assistive Technology Service Provider Interface - dbus
License:        GPL v2.0 or later
Group:          System/Libraries
URL:            http://www.gnome.org/
Source0:        pyatspi-%{version}.tar.bz2
BuildRoot:      %{_tmppath}/%{name}-%{version}-build
Requires:       at-spi2-atk-%{version}
Requires:       at-spi2-core-%{version}
BuildRequires:	dbus-1-python
BuildRequires:  fdupes
BuildRequires:	glib2-devel
BuildRequires:  python

%description
The python bindings for the new at-spi2 that is based on dbus


%prep
%setup -q -n pyatspi-%{version}

%build
%configure
%__make %{?jobs:-j%jobs}

%install
%makeinstall
%fdupes %{buildroot}%{python_sitelib}

%clean
rm -rf %{buildroot}

%files
%defattr(-,root,root)
%dir %{python_sitelib}/pyatspi
%{python_sitelib}/pyatspi/*.py*


%changelog
* Fri Oct 30 2009 sshaw@decriptor.com
- packaged pyatspi2 version 0.1.2 using the buildservice spec file wizard
