%define		debug_package %{nil}
#
# spec file for package UIAutomation
#

Name:           mono-uia
Version:        2.0
Release:        1
License:        MIT
Group:          System/Libraries
URL:            http://www.mono-project.com/Accessibility
Source0:        http://ftp.novell.com/pub/mono/sources/mono-uia/%{name}-%{version}.tar.bz2
Patch0:         mono-uia-libdir.patch
BuildRoot:      %{_tmppath}/%{name}-%{version}-%{release}-root-%(%{__id_u} -n)
Requires:       mono-core >= 2.4
BuildRequires:  gtk-sharp2-devel >= 2.12.8
BuildRequires:  mono-core >= 2.4
BuildRequires:  mono-devel >= 2.4
BuildRequires:  mono-nunit >= 2.4
Summary:        Implementations of members and interfaces based on MS UIA API

%description
User Interface Automation (UIA) is a new accessibility standard

%package devel
License:        MIT
Summary:        mono-uia devel package
Group:          Development/Languages
Requires:       mono-uia == %{version}-%{release}

%description devel
Implementations of the members and interfaces based on MS UIA API

%prep
%setup -q
%patch0 -p1

%build
%configure
#Break build with parrallel make
make

%install
rm -rf %{buildroot}
make DESTDIR=%{buildroot} install

%clean
rm -rf %{buildroot}

%files
%defattr(-,root,root,-)
%doc README COPYING NEWS
%{_libdir}/mono/accessibility
%{_libdir}/mono/gac/UIAutomationProvider
%{_libdir}/mono/accessibility/UIAutomationProvider.dll
%{_libdir}/mono/gac/UIAutomationTypes
%{_libdir}/mono/accessibility/UIAutomationTypes.dll
%{_libdir}/mono/gac/UIAutomationBridge
%{_libdir}/mono/accessibility/UIAutomationBridge.dll
%{_libdir}/mono/gac/UIAutomationClient
%{_libdir}/mono/accessibility/UIAutomationClient.dll
%{_libdir}/mono/gac/UIAutomationSource
%{_libdir}/mono/accessibility/UIAutomationSource.dll

%files devel
%defattr(-,root,root,-)
%{_libdir}/pkgconfig/*.pc

%changelog
* Mon Nov 30 2009 Stephen Shaw <sshaw@decriptor.com> = 1.8.90-1
- Major updates.  Added Client side work. Removed winfxcore.  Upstreamed.
* Thu Apr 30 2009 Stephen Shaw <sshaw@decriptor.com> - 1.0-1
- Initial RPM
