%define		debug_package %{nil}

#
# spec file for package UiaDbus
#

Name:           uiadbus
Version:        1.8.90
Release:        1
License:        MIT
Group:          System/Libraries
URL:		http://www.mono-project.com/Accessibility
Source0:        http://ftp.novell.com/pub/mono/sources/uiadbus/%{name}-%{version}.tar.bz2
BuildRoot:      %{_tmppath}/%{name}-%{version}-%{release}-root-%(%{__id_u} -n)
Summary:        UiaDbus components of UIA on Linux
BuildRequires:	glib-sharp2
BuildRequires:	mono-devel >= 2.6
BuildRequires:	mono-uia-devel >= 1.8.90
BuildRequires:	ndesk-dbus

%description
UiaDbus is another communication channel for UIA on Linux between the client and provider

%package devel
License:        MIT
Summary:        Devel package for UiaDbus
Group:          Development/Libraries/Mono
Requires:       uiadbuscorebridge == %{version}-%{release}

%description devel
UiaDbus is another communication channel for UIA on Linux between the client and provider

%prep
%setup -q

%build
%configure --disable-tests
make %{?_smp_mflags}

%install
rm -rf %{buildroot}
make DESTDIR=%{buildroot} install


%clean
rm -rf %{buildroot}

%files
%defattr(-,root,root)
%dir %{_prefix}/lib/mono/gac/UiaDbus
%{_prefix}/lib/mono/gac/UiaDbus/*
%dir %{_prefix}/lib/mono/gac/UiaDbusBridge
%{_prefix}/lib/mono/gac/UiaDbusBridge/*
%dir %{_prefix}/lib/mono/gac/UiaDbusSource
%{_prefix}/lib/mono/gac/UiaDbusSource/*
%{_prefix}/lib/mono/accessibility/UiaDbus.dll
%dir %{_libdir}/uiadbus
%{_libdir}/uiadbus/UiaDbus.dll*
%{_libdir}/uiadbus/UiaDbusBridge.dll*
%{_libdir}/uiadbus/UiaDbusSource.dll*

%files devel
%defattr(-,root,root)
%{_libdir}/pkgconfig/mono-uia-dbus.pc

%changelog
* Mon Nov 30 2009 <sshaw@decriptor.com> - 1.8.90-1
- packaged UiaDbus version 1.9.0 using the buildservice spec file wizard
