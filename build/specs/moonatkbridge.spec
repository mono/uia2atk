#
# spec file for package MoonAtkBridge
#

Name:           moonatkbridge
Version:        1.8.90
Release:        1
License:        MIT
Group:          System/Libraries
URL:		http://www.mono-project.com/Accessibility
Source0:        %{name}-%{version}.tar.bz2
BuildRoot:      %{_tmppath}/%{name}-%{version}-build

Summary:        Bridge between moonlight and atk

%description
Bridge between moonlight and atk

%prep
%setup -q

%build
%configure
make %{?_smp_mflags}

%install
make DESTDIR=%{buildroot} install


%clean
rm -rf %{buildroot}

%files
%defattr(-,root,root)

%post

%postun

%changelog

