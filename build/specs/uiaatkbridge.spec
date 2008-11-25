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
Version:	0.9
Release:	0
License:        MIT/X11
Group:          System/Libraries
URL:		http://www.mono-project.com/Accessibility
Source0:        %{name}-%{version}.tar.bz2
BuildRoot:      %{_tmppath}/%{name}-%{version}-build
AutoReqProv:    on
Requires:	mono-core >= 2.2 mono-uia mono-winfxcore gtk-sharp2 >= 2.12.6
BuildRequires:	mono-devel gcc gtk-sharp2 >= 2.12.6 mono-uia mono-winfxcore atk-devel
Provides:       %{name}-%{version}
Summary:        UIA to ATK Bridge

%description
Libraries to bridge UIA to ATK 

%prep
%setup -q

%build
%configure --disable-tests
make

%install
make DESTDIR=%{buildroot} install

#file we don't care about
rm -f %{buildroot}/%_libdir/uiaatkbridge/libbridge-glue.a
rm -f %{buildroot}/%_libdir/uiaatkbridge/libbridge-glue.la

%clean
#rm -rf %{buildroot}

%files
%defattr(-,root,root)
%doc COPYING README NEWS
%_prefix/lib/uiaatkbridge
%_prefix/lib/uiaatkbridge/UiaAtkBridge.dll*
%_prefix/lib/mono/gac/UiaAtkBridge

%package -n uiaatkbridge-devel
Group:  Development/Libraries/GNOME
Summary:    UIA to ATK Bridge header files
Requires:   %{name}-%{version}

%description -n uiaatkbridge-devel
UiaAtkBridge devel package

Libraries to bridge UIA to ATK
%files -n uiaatkbridge-devel
%defattr(-,root,root)
%_libdir/uiaatkbridge
%_libdir/uiaatkbridge/libbridge-glue.so*

%post devel -p /sbin/ldconfig

%postun devel -p /sbin/ldconfig

%changelog
