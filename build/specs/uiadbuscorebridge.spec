#
# spec file for package UiaAtkBridge
#

Name:           uiadbuscorebridge
Version:        1.9.0
Release:        1
License:        MIT/X11
Group:          System/Libraries
URL:		http://www.mono-project.com/Accessibility
Source0:        %{name}-%{version}.tar.bz2
BuildRoot:      %{_tmppath}/%{name}-%{version}-build
Requires:	mono-core >= 2.4 gtk-sharp2 >= 2.12.8
Requires:	mono-uia mono-winfxcore at-spi
BuildRequires:	mono-devel gtk-sharp2 ndesk-dbus mono-uia
#BuildRequires:	mono-devel >= 2.4 gtk-sharp2 >= 2.12.8 
#BuildRequires:	mono-uia mono-winfxcore atk-devel gtk2-devel

Summary:        Bridge between UIA providers and Dbus

%description
Bridge between UIA providers and Dbus


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
%dir %_libdir/uiadbuscorebridge
%_libdir/uiadbuscorebridge/DbusCore.dll*
%_libdir/uiadbuscorebridge/UiaDbusCoreBridge.dll*
%_prefix/lib/mono/accessibility/DbusCore.dll
%_prefix/lib/mono/gac/DbusCore
%_prefix/lib/mono/gac/UiaDbusCoreBridge
%{_libdir}/pkgconfig/*.pc


%post

%postun

%changelog
* Thu Apr 09 2009 sshaw@decriptor.com
- packaged UiaDbusCoreBridge version 1.9.0 using the buildservice spec file wizard
