#
# spec file for package olive
#
# Copyright (c) 2008 SUSE LINUX Products GmbH, Nuernberg, Germany.
# This file and all modifications and additions to the pristine
# package are under the same license as the package itself.
# 
# Please submit bugfixes or comments via http://bugs.opensuse.org/ 
#
# norootforbuild
#

Name:     	olive
Version:	112243
Release:	0.novell
Group:		Development/Tools
License:	LGPL
URL:		http://www.go-mono.com
BuildRoot:	/var/tmp/%{name}-%{version}-root
Autoreqprov:    on
BuildArch:      noarch
Source0:	%{name}-%{version}.tar.bz2
Requires:	mono-core
BuildRequires:	mono-devel mono-extras, mono-wcf
Summary:	Mono Olive

%description
Various .NET 3.0 bits.
	  
%prep
%setup -q

%build
%configure
make

%install
make install DESTDIR=${RPM_BUILD_ROOT}

# change .pc files to /usr/share/pkgconfig/
mkdir -p $RPM_BUILD_ROOT%{_prefix}/share
mv $RPM_BUILD_ROOT%{_prefix}/lib/pkgconfig $RPM_BUILD_ROOT%{_prefix}/share/

%clean
rm -rf "$RPM_BUILD_ROOT"

%if 0%{?fedora_version} || 0%{?rhel_version}
# Allows overrides of __find_provides in fedora distros... (already set to zero on newer suse distros)
%define _use_internal_dependency_generator 0
%endif
%define __find_provides env sh -c 'filelist=($(cat)) && { printf "%s\\n" "${filelist[@]}" | /usr/lib/rpm/find-provides && printf "%s\\n" "${filelist[@]}" | /usr/bin/mono-find-provides ; } | sort | uniq'
%define __find_requires env sh -c 'filelist=($(cat)) && { printf "%s\\n" "${filelist[@]}" | /usr/lib/rpm/find-requires && printf "%s\\n" "${filelist[@]}" | /usr/bin/mono-find-requires ; } | sort | uniq'

%files
%defattr(-, root, root)
%doc README
%_prefix/bin/svcutil
%_prefix/lib/mono/3.0/svcutil.exe*
%_prefix/bin/client-proxy-gen
%_prefix/lib/mono/3.0/client-proxy-gen.exe*
%_prefix/bin/sts
%_prefix/lib/mono/3.0/sts.exe*
%_prefix/bin/xamlc
%_prefix/lib/mono/3.0/xamlc.exe*
%_prefix/lib/mono/gac/PresentationFramework
%_prefix/lib/mono/3.0/PresentationFramework.dll*
%_prefix/lib/mono/gac/UIAutomationProvider
%_prefix/lib/mono/accessibility/UIAutomationProvider.dll
%_prefix/lib/mono/gac/UIAutomationTypes
%_prefix/lib/mono/accessibility/UIAutomationTypes.dll
%_prefix/lib/mono/gac/UIAutomationBridge
%_prefix/lib/mono/accessibility/UIAutomationBridge.dll
%_prefix/lib/mono/gac/WindowsBase
%_prefix/lib/mono/3.0/WindowsBase.dll*
%_prefix/lib/mono/gac/System.Workflow.Runtime
%_prefix/lib/mono/3.0/System.Workflow.Runtime.dll*
%_prefix/lib/mono/gac/System.Workflow.Activities
%_prefix/lib/mono/3.0/System.Workflow.Activities.dll*
%_prefix/lib/mono/gac/System.Workflow.ComponentModel
%_prefix/lib/mono/3.0/System.Workflow.ComponentModel.dll*
%_prefix/lib/mono/gac/PresentationCore
%_prefix/lib/mono/3.0/PresentationCore*
%_prefix/share/pkgconfig/*.pc

%changelog
