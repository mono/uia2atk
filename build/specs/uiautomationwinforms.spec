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
Version:	118540
Release:	0.novell
License:        MIT/X11
Group:          System/Libraries
BuildArch:	noarch
URL:		http://www.mono-project.com/Accessibility
Source0:        %{name}-%{version}.tar.bz2
BuildRoot:	%{_tmppath}/%{name}-%{version}-build
AutoReqProv:	on
Requires:	mono-core >= 2.2 mono-uia mono-winfxcore uiaatkbridge
BuildRequires:	mono-devel mono-nunit mono-uia mono-winfxcore glib-sharp2 gtk-sharp2
Summary:        Mono UIA Provider

%description
Mono UIA Provider

%prep
%setup -q

%build
./configure --prefix=%_prefix --disable-tests
make

%install
make DESTDIR=$RPM_BUILD_ROOT install

%clean
rm -rf $RPM_BUILD_ROOT

%files
%defattr(-,root,root)
%doc 
%_prefix/lib/uiautomationwinforms/UIAutomationWinforms.dll*
%_prefix/lib/mono/gac/UIAutomationWinforms



%if 0%{?fedora_version} || 0%{?rhel_version}
# Allows overrides of __find_provides in fedora distros... (already set to zero on newer suse distros)
%define _use_internal_dependency_generator 0
%endif
%define __find_provides env sh -c 'filelist=($(cat)) && { printf "%s\\n" "${filelist[@]}" | /usr/lib/rpm/find-provides && printf "%s\\n" "${filelist[@]}" | /usr/bin/mono-find-provides ; } | sort | uniq'
%define __find_requires env sh -c 'filelist=($(cat)) && { printf "%s\\n" "${filelist[@]}" | /usr/lib/rpm/find-requires && printf "%s\\n" "${filelist[@]}" | /usr/bin/mono-find-requires ; } | sort | uniq'


%changelog

