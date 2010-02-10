%define		debug_package %{nil}
#
# spec file for package at-spi-sharp
#

Name:           at-spi-sharp
Version:        1.8.94
Release:        1
License:        MIT
Group:          System/Libraries
URL:            http://www.mono-project.com/Accessibility
Source0:        http://ftp.novell.com/pub/mono/sources/mono-uia/%{name}-%{version}.tar.bz2
BuildRoot:      %{_tmppath}/%{name}-%{version}-%{release}-root-%(%{__id_u} -n)
Requires:       mono-core >= 2.6
BuildRequires:	mono-devel >= 2.6
BuildRequires:	mono-uia >= 1.8.94
BuildRequires:	ndesk-dbus-glib-devel
BuildRequires:	pkg-config

Summary:        C# bindings for at-spi

%description
C# mono bindings for at-spi

%package devel
Group:		Development/Libraries/mono
Summary:	Devel package for at-spi-sharp mono bindings
Requires:	%{name} = %{version}

%description devel
Devel package that contains the pc file for at-spi-sharp

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
%dir %{_prefix}/lib/mono/gac/at-spi-sharp/
%{_prefix}/lib/mono/accessibility/at-spi-sharp.dll
%{_prefix}/lib/mono/gac/at-spi-sharp/*

%files devel
%defattr(-,root,root)
%{_libdir}/pkgconfig/at-spi-sharp.pc


%changelog
* Mon Nov 30 2009 Stephen Shaw <sshaw@decriptor.com> = 1.8.90-1
- Initial RPM
