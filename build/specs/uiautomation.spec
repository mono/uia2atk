#
# spec file for package UIAutomation
#
# Copyright (c) 2008 SUSE LINUX Products GmbH, Nuernberg, Germany.
# This file and all modifications and additions to the pristine
# package are under the same license as the package itself.
# 
# Please submit bugfixes or comments via http://bugs.opensuse.org/ 
# 
# norootforbuild 
# 


Name:           mono-uia
Version:        0.9.1
Release:        0
License:        MIT/X11
Group:          System/Libraries
URL:		http://www.mono-project.com/Accessibility
Source0:        %{name}-%{version}.tar.bz2
BuildRoot:	%{_tmppath}/%{name}-%{version}-build
AutoReqProv:	on
Requires:	mono-core >= 2.2
BuildRequires:	mono-core >= 2.2 mono-devel mono-nunit
Summary:        Implementations of members and interfaces based on MS UIA API

%description
Implementations of the members and interfaces based on MS UIA API

%prep
%setup -q

%build
%configure
make

%install
make DESTDIR=%{buildroot} install

%clean
rm -rf %{buildroot}

%files
%defattr(-,root,root)
%doc README COPYING NEWS
%{_prefix}/lib/mono/accessibility
%{_prefix}/lib/mono/gac/UIAutomationProvider
%{_prefix}/lib/mono/accessibility/UIAutomationProvider.dll
%{_prefix}/lib/mono/gac/UIAutomationTypes
%{_prefix}/lib/mono/accessibility/UIAutomationTypes.dll
%{_prefix}/lib/mono/gac/UIAutomationBridge
%{_prefix}/lib/mono/accessibility/UIAutomationBridge.dll
%{_prefix}/lib/mono/gac/UIAutomationClient
%{_prefix}/lib/mono/accessibility/UIAutomationClient.dll
%{_libdir}/pkgconfig/*.pc

%package -n mono-winfxcore
License:	MIT/X11
Summary:	Parts of winfx
Group:		Development/Languages/Mono
Requires:	mono-core >= 2.2

%description -n mono-winfxcore
The Mono Project is an open development initiative that is working to
develop an open source, Unix version of the .NET development platform.
Its objective is to enable Unix developers to build and deploy
cross-platform .NET applications. The project will implement various
technologies that have been submitted to the ECMA for standardization.

Parts of winfx

%files -n mono-winfxcore
%defattr(-, root, root)
%{_prefix}/lib/mono/gac/WindowsBase
%{_prefix}/lib/mono/2.0/WindowsBase.dll

%changelog

