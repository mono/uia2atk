#
# spec file for package UiaAtkBridge
#
# Copyright (c) 2008 SUSE Linux Products GmbH, Nuernberg, Germany.
# This file and all modifications and additions to the pristine
# package are under the same license as the package itself.
#       
# Please submit bugfixes or comments via http://bugs.opensuse.org/ 
#            
# norootforbuild 
# 

Name:           uiaatkbridge
Version:	110576
Release:	0.novell
License:        MIT/X11
Group:          System/Libraries
URL:		http://www.mono-project.com/Accessibility
Source0:        %{name}-%{version}.tar.bz2
BuildRoot:      %{_tmppath}/%{name}-%{version}-build
AutoReqProv:    on
Requires:	mono-core >= 1.9 olive gtk-sharp2
#Requires:	mono-core >= 1.9 gtk-sharp2 olive
BuildRequires:	mono-devel gcc gtk-sharp2 olive atk-devel
Summary:        UIA to ATK Bridge

%description
Libraries to bridge UIA to ATK 


%prep
%setup -q

%build
./configure --prefix=%_prefix
make

%install
make DESTDIR=$RPM_BUILD_ROOT install

#file we don't care about
rm -f $RPM_BUILD_ROOT/%_prefix/lib/uiaatkbridge/libbridge-glue.a
rm -f $RPM_BUILD_ROOT/%_prefix/lib/uiaatkbridge/libbridge-glue.la

%clean
rm -rf $RPM_BUILD_ROOT

%files
%defattr(-,root,root)
%doc 
%_prefix/lib/uiaatkbridge/UiaAtkBridge.dll*
#FIXME: create a devel packagie ? 
%_prefix/lib/uiaatkbridge/libbridge-glue.so*
%_prefix/lib/mono/gac/UiaAtkBridge


%if 0%{?fedora_version} || 0%{?rhel_version}
# Allows overrides of __find_provides in fedora distros... (already set to zero on newer suse distros)
%define _use_internal_dependency_generator 0
%endif
%define __find_provides env sh -c 'filelist=($(cat)) && { printf "%s\\n" "${filelist[@]}" | /usr/lib/rpm/find-provides && printf "%s\\n" "${filelist[@]}" | /usr/bin/mono-find-provides ; } | sort | uniq'
%define __find_requires env sh -c 'filelist=($(cat)) && { printf "%s\\n" "${filelist[@]}" | /usr/lib/rpm/find-requires && printf "%s\\n" "${filelist[@]}" | /usr/bin/mono-find-requires ; } | sort | uniq'


%changelog
