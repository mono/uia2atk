#
# spec file for package UIAutomationWinforms
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
Version:        1.8.94
Release:        1
License:        MIT/X11
Group:          System/Libraries
URL:            http://www.mono-project.com/Accessibility
Source0:        %{name}-%{version}.tar.bz2
BuildRoot:      %{_tmppath}/%{name}-%{version}-build
Requires:       glib-sharp2 >= 2.12.8
Requires:       gtk-sharp2 >= 2.12.8
Requires:       mono-core >= 2.6
Requires:       mono-data >= 2.6
Requires:       mono-uia >= 1.8.94
Requires:       mono-winfxcore
Requires:       uiaatkbridge >= 1.8.94
BuildRequires:  gtk-sharp2 >= 2.12.8
BuildRequires:  glib-sharp2 >= 2.12.8 
BuildRequires:	mono-devel >= 2.6
BuildRequires:  mono-data >= 2.6
BuildRequires:	mono-nunit >= 2.6
BuildRequires:  mono-uia >= 1.8.94
BuildRequires:  mono-uia-devel >= 1.8.94
BuildRequires:  mono-winfxcore
BuildRequires:  uiaatkbridge >= 1.8.94
BuildRequires:  intltool

Summary:        Implementation of UIA providers

%description
Implementation of UIA providers for Mono's Winforms controls

%prep
%setup -q

%build
%configure --disable-tests
#Breaks make
make

%install
make DESTDIR=%{buildroot} install
%find_lang UIAutomationWinforms

%clean
rm -rf %{buildroot}

%files -f UIAutomationWinforms.lang
%defattr(-,root,root)
%doc COPYING README NEWS 
%dir %_libdir/uiautomationwinforms
%_libdir/uiautomationwinforms/UIAutomationWinforms.dll*
%_prefix/lib/mono/gac/UIAutomationWinforms

%changelog
