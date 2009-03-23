#
# spec file for package UiaAtkBridge-unittests
#
# Copyright (c) 2008 SUSE Linux Products GmbH, Nuernberg, Germany.
# This file and all modifications and additions to the pristine
# package are under the same license as the package itself.
# 
# Please submit bugfixes or comments via http://bugs.opensuse.org/ 
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
BuildRequires:	mono-devel >= 2.2 mono-data gtk-sharp2 >= 2.12.7 glib-sharp2
BuildRequires:	mono-uia mono-winfxcore atk-devel gtk2-devel mono-nunit  intltool >= 0.35
BuildRequires:  mono-nunit at-spi xorg-x11-Xvfb subversion uiaatkbridge bc gconf2 
#xorg-x11-server-extra metacity bc gtk2-engines gnome-themes
#BuildRequires:  3ddiag cabextract xterm ghostscript-x11 at-spi subversion uiaatkbridge
#BuildRequires:  openssh-askpass x11-input-synaptics xorg-x11-libX11-ccache xorg-x11
#BuildRequires:  xorg-x11-Xvnc numlockx freeglut x11-tools translation-update ConsoleKit-x11 
#BuildRequires:  xorg-x11-xauth icewm-default icewm-gnome wine fvwm2 fvwm2-gtk
#BuildRequires:  fvwm-themes gv mmv pmidi xine-ui xosview xpp xosd desktop-data-openSUSE-extra
%define         X_display       ":98"

Summary:        UiaAtkBridge unit tests

%description
Don't install this package. Seriously. Fo' rizzle.

%prep
%setup -q

%build
cd ../
svn co svn://151.155.5.148/source/trunk/uia2atk/test ./test
cp ../SOURCES/uiautomationwinforms.tar.bz2 .
tar -jxvf uiautomationwinforms.tar.bz2
cd uiautomationwinforms-*
./configure --prefix=/usr
make
cd ..
mv uiautomationwinforms-* UIAutomationWinforms
cd uiaatkbridge*
./configure --prefix=/usr
make
svn export svn://151.155.5.148/source/trunk/uia2atk/UiaAtkBridge/atspimon.py
cd Test/UiaAtkBridgeTest
chmod +x bridgetest.sh
./bridgetest.sh

%install
export DISPLAY=%{X_display}
#Xvfb -ac -screen 0 1280x1024x16 -br :1 &
Xvfb %{X_display} >& Xvfb.log &
trap "kill $! || true" EXIT
sleep 10
#metacity &
gconftool-2 --type bool --set /desktop/gnome/interface/accessibility true
rm -rf %{buildroot}/*

%clean

%files
%defattr(-,root,root)
%doc Xvfb.log

%changelog
