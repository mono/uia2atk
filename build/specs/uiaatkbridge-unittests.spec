#
# spec file for package UiaAtkBridge-unittests
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
Version:	121166
Release:	0
License:        MIT/X11
Group:          System/Libraries
URL:		http://www.mono-project.com/Accessibility
Source0:        %{name}-%{version}.tar.bz2
BuildRoot:      %{_tmppath}/%{name}-%{version}-build
AutoReqProv:    on
Requires:	mono-core >= 2.2 gtk-sharp2 >= 2.12.7 mono-uia mono-winfxcore 
BuildRequires:	mono-devel >= 2.2 gtk-sharp2 >= 2.12.7 
BuildRequires:	mono-uia mono-winfxcore atk-devel gtk2-devel mono-nunit

Summary:        UiaAtkBridge unit tests

%description
This sets up an env. that runs uiaatkbridge unit tests

%prep
%setup -q

%build
%configure
make

%install
cd UiaAtkBridge/Test/UiaAtkBridgeTest
./bridgetests.sh

%clean
rm -rf %{buildroot}/*
touch DUMMY

%files
%defattr(-,root,root)
%doc DUMMY

%post

%postun

%changelog
