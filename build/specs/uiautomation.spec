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
Version:        0.9
Release:        0.svn
License:        MIT/X11
Group:          System/Libraries
BuildArch:	noarch
URL:		http://www.mono-project.com/Accessibility
Source0:        %{name}-%{version}.tar.bz2
BuildRoot:	%{_tmppath}/%{name}-%{version}-build
AutoReqProv:	on
Requires:	mono-core >= 2.2
BuildRequires:	mono-devel mono-nunit
Summary:        Implementations of members and interfaces based on MS UIA API

%description
Implementations of the members and interfaces based on MS UIA API

%prep
%setup -q

%build
./configure --prefix=%_prefix
make

%install
make DESTDIR=$RPM_BUILD_ROOT install

%clean
rm -rf $RPM_BUILD_ROOT

%files
%defattr(-,root,root)
#%doc README COPYING
%_prefix/lib/mono/gac/UIAutomationProvider
%_prefix/lib/mono/accessibility/UIAutomationProvider.dll
%_prefix/lib/mono/gac/UIAutomationTypes
%_prefix/lib/mono/accessibility/UIAutomationTypes.dll
%_prefix/lib/mono/gac/UIAutomationBridge
%_prefix/lib/mono/accessibility/UIAutomationBridge.dll
%_prefix/lib/mono/gac/UIAutomationClient
%_prefix/lib/mono/accessibility/UIAutomationClient.dll
%_libdir/pkgconfig/*.pc

%package -n mono-winfxcore
License:	MIT/X11
Summary:	Parts of winfx
Group:		Development/Languages/Mono
Requires:	mono-core >= 1.9

%description -n mono-winfxcore
The Mono Project is an open development initiative that is working to
develop an open source, Unix version of the .NET development platform.
Its objective is to enable Unix developers to build and deploy
cross-platform .NET applications. The project will implement various
technologies that have been submitted to the ECMA for standardization.

Database connectivity for Mono.



Authors:
--------
    Miguel de Icaza <miguel@ximian.com>
    Paolo Molaro <lupus@ximian.com>
    Dietmar Maurer <dietmar@ximian.com>

%files -n mono-winfxcore
%defattr(-, root, root)
%_prefix/lib/mono/gac/WindowsBase
%_prefix/lib/mono/2.0/WindowsBase.dll


%if 0%{?fedora_version} || 0%{?rhel_version}
# Allows overrides of __find_provides in fedora distros... (already set to zero on newer suse distros)
%define _use_internal_dependency_generator 0
%endif
%define __find_provides env sh -c 'filelist=($(cat)) && { printf "%s\\n" "${filelist[@]}" | /usr/lib/rpm/find-provides && printf "%s\\n" "${filelist[@]}" | /usr/bin/mono-find-provides ; } | sort | uniq'
%define __find_requires env sh -c 'filelist=($(cat)) && { printf "%s\\n" "${filelist[@]}" | /usr/lib/rpm/find-requires && printf "%s\\n" "${filelist[@]}" | /usr/bin/mono-find-requires ; } | sort | uniq'


%changelog

