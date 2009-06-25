
BuildRequires:	subversion

Name:           moonlight
License:        LGPL v2.0 only
Group:          Productivity/Multimedia/Other
Summary:        Novell Moonlight
Url:            http://go-mono.com/moonlight/
Version:        2.0.0
Release:        1
BuildRoot:      %{_tmppath}/%{name}-%{version}-build
BuildRequires:  alsa-devel
BuildRequires:  bison
BuildRequires:  gcc-c++
BuildRequires:  glib2-devel
BuildRequires:  gtk-sharp2
BuildRequires:  gtk2-devel
BuildRequires:  mono-devel
BuildRequires:  monodoc-core
BuildRequires:  mono-core-moon
BuildRequires:  rsvg2-sharp
BuildRequires:  zip
BuildRequires:  zlib-devel
BuildRequires:  libpulse-devel
BuildRequires:  libexpat-devel
BuildRequires:  mozilla-xulrunner190-devel

%description
Moonlight is an open source implementation of Microsoft Silverlight for
Unix systems.



%post

%postun

%files
%defattr(-, root, root)
%doc DUMMY

%prep
svn co svn://151.155.5.148/source/trunk/moon moon
mkdir mcs


%build
%{?env_options}
%{?configure_options}
cd moon
sh autogen.sh

%install
%{?env_options}
touch DUMMY
cd moon
make dist-bzip2

%clean
rm -rf ${RPM_BUILD_ROOT}

%changelog
