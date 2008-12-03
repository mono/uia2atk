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
Version:	0.9
Release:	0
License:        MIT/X11
Group:          System/Libraries
BuildArch:	noarch
URL:		http://www.mono-project.com/Accessibility
Source0:        %{name}-%{version}.tar.bz2
BuildRoot:	%{_tmppath}/%{name}-%{version}-build
AutoReqProv:	on
Requires:	mono-core >= 2.2 gtk-sharp2 >= 2.12.7 
Requires:	mono-uia mono-winfxcore uiaatkbridge 
BuildRequires:	mono-devel >= 2.2 gtk-sharp2 >= 2.12.7 glib-sharp2 
BuildRequires:	mono-nunit mono-uia mono-winfxcore uiaatkbridge

Summary:        Implementation of UIA providers for Mono's Winforms controls

%description
Implementation of UIA providers for Mono's Winforms controls

%prep
%setup -q

%build
%configure --disable-tests
make

%install
make DESTDIR=%{buildroot} install

%clean
rm -rf %{buildroot}

%files
%defattr(-,root,root)
%doc COPYING README NEWS 
%_prefix/lib/uiautomationwinforms/UIAutomationWinforms.dll*
%_prefix/lib/mono/gac/UIAutomationWinforms

%changelog
