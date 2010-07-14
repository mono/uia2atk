%define		debug_package %{nil}
#
# spec file for package UiaDbus
#

Name:           uiadbus
Version:        2.0.94
Release:        1
License:        MIT
Group:          System/Libraries
URL:            http://www.mono-project.com/Accessibility
Source0:        http://ftp.novell.com/pub/mono/sources/mono-uia/%{name}-%{version}.tar.bz2
Patch0:         uiadbus-libdir.patch
BuildRoot:      %{_tmppath}/%{name}-%{version}-%{release}-root-%(%{__id_u} -n)
BuildRequires:  gtk-sharp2-devel >= 2.12.8
BuildRequires:  mono-devel >= 2.4
BuildRequires:  mono-uia-devel >= 2.0.94
BuildRequires:  ndesk-dbus
Summary:        UiaDbus components of UIA on Linux

%description
UiaDbus is another communication channel for UIA on Linux between the client and provider

%package devel
License:        MIT
Summary:        Devel package for UiaDbus
Group:          Development/Languages
Requires:       uiadbus == %{version}-%{release}

%description devel
UiaDbus is another communication channel for UIA on Linux between the client and provider

%prep
%setup -q
%patch0 -p1

%build
%configure --disable-tests
#make %{?_smp_mflags}
make

%install
rm -rf %{buildroot}
make DESTDIR=%{buildroot} install

%clean
rm -rf %{buildroot}

%files
%defattr(-,root,root,-)
%doc README COPYING NEWS
%{_libdir}/mono/gac/UiaDbus/*
%{_libdir}/mono/gac/UiaDbusBridge/*
%{_libdir}/mono/gac/UiaDbusSource/*
%{_libdir}/mono/accessibility/UiaDbus.dll
%{_libdir}/uiadbus/UiaDbus.dll*
%{_libdir}/uiadbus/UiaDbusBridge.dll*
%{_libdir}/uiadbus/UiaDbusSource.dll*

%files devel
%defattr(-,root,root,-)
%{_libdir}/pkgconfig/mono-uia-dbus.pc

%changelog
* Mon Nov 30 2009 <sshaw@decriptor.com> - 1.8.90-1
- packaged UiaDbus version 1.9.0 using the buildservice spec file wizard
