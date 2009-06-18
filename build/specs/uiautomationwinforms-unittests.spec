#
# spec file for package UIAutomationWinforms-unittests
#
# Copyright (c) 2008 SUSE Linux Products GmbH, Nuernberg, Germany.
# This file and all modifications and additions to the pristine
# package are under the same license as the package itself.
# 
# Please submit bugfixes or comments via http://bugs.opensuse.org/ 
# 
# norootforbuild 
# 

Name:           uiautomationwinforms
Version:	1.0
Release:	0
License:        MIT/X11
Group:          System/Libraries
URL:		http://www.mono-project.com/Accessibility
Source0:        %{name}-%{version}.tar.bz2
BuildRoot:      %{_tmppath}/%{name}-%{version}-build
Requires:       mono-core gtk-sharp2 mono-data
Requires:       mono-uia mono-winfxcore uiaatkbridge 
BuildRequires:	mono-devel mono-data gtk-sharp2 glib-sharp2
BuildRequires:	mono-uia mono-uia-devel mono-winfxcore uiaatkbridge intltool >= 0.35
BuildRequires:  mono-nunit metacity xorg-x11-Xvfb bc
%define		X_display		":98"

Summary:        UIAutomationWinforms unit tests

%description
Don't install this package. Seriously. Fo' rizzle.

%prep
%setup -q

%build
%configure
make
export DISPLAY=%{X_display}
Xvfb %{X_display} >& Xvfb.log &
trap "kill $! || true" EXIT
sleep 10
metacity &
#Xvfb -ac -screen 0 1280x1024x16 -br :1 &
cd UIAutomationWinformsTests
chmod +x providertest.sh
./providertest.sh

%install
rm -rf %{buildroot}/*
touch DUMMY

%clean

%files
%defattr(-,root,root)
%doc DUMMY

%changelog
