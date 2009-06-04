#
# spec file for package UIAutomation
#

Name:           mono-uia
Version:        1.0
Release:        1
License:        MIT
Group:          System/Libraries
URL:		http://www.mono-project.com/Accessibility
Source0:        %{name}-%{version}.tar.bz2
BuildRoot:	%{_tmppath}/%{name}-%{version}-build
Requires:	mono-core >= 2.4
BuildRequires:	mono-core >= 2.4 mono-devel >= 2.4 mono-nunit >= 2.4
Summary:        Implementations of members and interfaces based on MS UIA API

%description
User Interface Automation (UIA) is a new accessibility standard

%package devel
License:	MIT
Summary:	mono-uia devel package
Group:		Development/Languages
Requires:	mono-uia == %{version}-%{release}

%description devel
Implementations of the members and interfaces based on MS UIA API

%package -n mono-winfxcore
License:	MIT
Summary:	Parts of winfx
Group:		Development/Languages
Requires:	mono-core >= 2.4

%description -n mono-winfxcore
WinFx components required by User Interface Automation (UIA) for use with Mono

%prep
%setup -q

%build
%configure
#Break build with parrallel make
make

%install
make DESTDIR=%{buildroot} install

%clean
rm -rf %{buildroot}

%files
%defattr(-,root,root)
%doc README COPYING NEWS
%{_prefix}/lib/mono/accessibility
%{_prefix}/lib/mono/gac/UIAutomationProvider
%{_prefix}/lib/mono/accessibility/UIAutomationProvider.dll
%{_prefix}/lib/mono/gac/UIAutomationTypes
%{_prefix}/lib/mono/accessibility/UIAutomationTypes.dll
%{_prefix}/lib/mono/gac/UIAutomationBridge
%{_prefix}/lib/mono/accessibility/UIAutomationBridge.dll
%{_prefix}/lib/mono/gac/UIAutomationClient
%{_prefix}/lib/mono/accessibility/UIAutomationClient.dll

%files devel
%defattr(-,root,root,-)
%{_libdir}/pkgconfig/*.pc

%files -n mono-winfxcore
%defattr(-, root, root)
%{_prefix}/lib/mono/gac/WindowsBase
%{_prefix}/lib/mono/2.0/WindowsBase.dll

%changelog

