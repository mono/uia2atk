#
# spec file for package UiaAtkBridge-unittests
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
Version:	0.9.1
Release:	0
License:        MIT/X11
Group:          System/Libraries
URL:		http://www.mono-project.com/Accessibility
Source0:        %{name}-%{version}.tar.bz2
Source1:        uiautomationwinforms.tar.bz2        
BuildRoot:      %{_tmppath}/%{name}-%{version}-build
AutoReqProv:    on
Requires:       mono-core >= 2.2 gtk-sharp2 >= 2.12.7 mono-uia mono-winfxcore 
BuildRequires:	mono-devel >= 2.2 gtk-sharp2 >= 2.12.7 
BuildRequires:	mono-uia mono-winfxcore atk-devel gtk2-devel mono-nunit
BuildRequires:  mono-nunit xorg-x11-server-extra metacity bc gtk2-engines gnome-themes
BuildRequires:  3ddiag cabextract xterm ghostscript-x11
BuildRequires:  openssh-askpass x11-input-synaptics xorg-x11-libX11-ccache xorg-x11
BuildRequires:  xorg-x11-Xvnc numlockx freeglut x11-tools translation-update ConsoleKit-x11 
BuildRequires:  xorg-x11-xauth icewm-default icewm-gnome wine fvwm2 fvwm2-gtk
BuildRequires:  fvwm-themes gv mmv pmidi xine-ui xosview xpp xosd desktop-data-openSUSE-extra

Summary:        UiaAtkBridge unit tests

%description
Don't install this package. Seriously. Fo' rizzle.

%prep
%setup -q

%build
cd ../
cp ../SOURCES/uiautomationwinforms.tar.bz2 .
tar -jxvf uiautomationwinforms.tar.bz2
cd uiautomationwinforms*
sh autogen.sh
make
cd ../uiaatkbridge*
%configure
make
export DISPLAY=:1
Xvfb -ac -screen 0 1280x1024x16 -br :1 &
metacity &


cd /Test/UiaAtkBridgeTest
./bridgetests.sh

%install
rm -rf %{buildroot}/*
touch DUMMY

%clean

%files
%defattr(-,root,root)
%doc DUMMY

%changelog
