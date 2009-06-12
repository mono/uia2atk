
#
# spec file for package UiaAtkBridge
#

Name:           uiaatkbridge
Version:	1.0
Release:	1
License:        MIT
Group:          System/Libraries
URL:		http://www.mono-project.com/Accessibility
Source0:        http://ftp.novell.com/pub/mono/sources/uiaatkbridge/%{name}-%{version}.tar.bz2
BuildRoot:      %{_tmppath}/%{name}-%{version}-%{release}-root-%(%{__id_u} -n)
Requires:	mono-core >= 2.4 gtk-sharp2 >= 2.12.8
Requires:	mono-uia >= 1.0 mono-winfxcore >= 1.0 at-spi 2.24.0 
BuildRequires:	mono-devel >= 2.4 gtk-sharp2 >= 2.12.8
BuildRequires:	mono-uia >= 1.0 mono-winfxcore >= 1.0 atk-devel gtk2-devel

Summary:        Bridge between UIA providers and ATK

%description
The bridge contains adapter Atk.Objects that wrap UIA providers.  Adapter
behavior is determined by provider ControlType and supported pattern interfaces.
The bridge implements interfaces from UIAutomationBridge which allow the UI
Automation core to send it automation events and provider information.

%prep
%setup -q

%build
%configure --disable-tests
make %{?_smp_mflags}

%install
rm -rf %{buildroot}
make DESTDIR=%{buildroot} install

#file we don't care about
rm -f %{buildroot}/%_libdir/uiaatkbridge/libbridge-glue.a
rm -f %{buildroot}/%_libdir/uiaatkbridge/libbridge-glue.la

%clean
rm -rf %{buildroot}

%files
%defattr(-,root,root,-)
%doc COPYING README NEWS
%dir %_libdir/uiaatkbridge
%_libdir/uiaatkbridge/UiaAtkBridge.dll*
%_libdir/uiaatkbridge/libbridge-glue.so*
%_libdir/mono/gac/UiaAtkBridge


%post -p /sbin/ldconfig

%postun -p /sbin/ldconfig

%changelog
* Thu Apr 30 2009 Stephen Shaw <sshaw@decriptor.com> - 1.0-1
- Initial RPM
