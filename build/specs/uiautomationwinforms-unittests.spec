#
# spec file for package UIAutomationWinforms-unittests
#
# Copyright (c) 2008 SUSE LINUX Products GmbH, Nuernberg, Germany.
# This file and all modifications and additions to the pristine
# package are under the same license as the package itself.
# 
# Please submit bugfixes or comments via http://bugs.opensuse.org/ 
# 
# norootforbuild 
# 

Name:           uiautomationwinforms
Version:	0.9.1
Release:	0
License:        MIT/X11
Group:          System/Libraries
URL:		http://www.mono-project.com/Accessibility
Source0:        %{name}-%{version}.tar.bz2
BuildRoot:	%{_tmppath}/%{name}-%{version}-build
AutoReqProv:	on
Requires:	mono-core >= 2.2 mono-data gtk-sharp2 >= 2.12.7 
Requires:	mono-uia mono-winfxcore uiaatkbridge 
BuildRequires:	mono-devel >= 2.2 mono-data gtk-sharp2 >= 2.12.7 glib-sharp2 
BuildRequires:	mono-uia mono-winfxcore uiaatkbridge intltool >= 0.21

Summary:        UIAutomationWinforms unit tests

%description
Don't install this package. Seriously. Fo' rizzle.

%prep
%setup -q

%build
%configure
make
cd UIAutomationWinformsTests
chmod +x providertest.sh
./providertest.sh --labels


%install
rm -rf %{buildroot}

%clean

%files
%defattr(-,root,root)
%doc COPYING README NEWS 

%changelog
