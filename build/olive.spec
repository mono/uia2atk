
# norootforbuild

Name:     	olive
Version: 	81875
Release:	1.novell
Vendor:		Novell, Inc.
Distribution:	Novell Packages for SuSE Linux 10.0 / i586
License:	LGPL
BuildRoot:	/var/tmp/%{name}-%{version}-root
Autoreqprov:    on
BuildArch:      noarch
URL:		http://www.go-mono.com
Source0:	%{name}-%{version}.tar.bz2
Requires:	mono-core
BuildRequires:	mono-devel mono-extras
Summary:	Mono Olive
Group:		Development/Tools

%description
Various .NET 3.0 bits.
	  

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
%_prefix/lib/pkgconfig/*.pc


%prep
%setup -q

%build
./configure --prefix=%_prefix
make

%install
make install DESTDIR=${RPM_BUILD_ROOT}

%clean
rm -rf "$RPM_BUILD_ROOT"

%if 0%{?fedora_version} || 0%{?rhel_version}
# Allows overrides of __find_provides in fedora distros... (already set to zero on newer suse distros)
%define _use_internal_dependency_generator 0
%endif
%define __find_provides env sh -c 'filelist=($(cat)) && { printf "%s\\n" "${filelist[@]}" | /usr/lib/rpm/find-provides && printf "%s\\n" "${filelist[@]}" | /usr/bin/mono-find-provides ; } | sort | uniq'
%define __find_requires env sh -c 'filelist=($(cat)) && { printf "%s\\n" "${filelist[@]}" | /usr/lib/rpm/find-requires && printf "%s\\n" "${filelist[@]}" | /usr/bin/mono-find-requires ; } | sort | uniq'

%changelog
