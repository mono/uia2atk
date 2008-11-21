#!/bin/bash

##############################################################################
# Written by:  Brian G. Merrell <bgmerrell@novell.com>
# Date:        June 24 2008
# Description: Download the latest uia2atk rpms and upgrade to them
##############################################################################

VERSION="0.2"
PROG="update_uia2atk_rpms.sh"
dir="current"
nodeps=0
force=0
verbose=false

SHORTOPTS="d:nfhv"
LONGOPTS="directory:,nodeps,force,help,version"

function usage
{
    echo "usage: update_uia2atk_rpms [[[-n|--nodeps] [-f|--force]] | [-h|--help]] [[-d|--directory=]directory]"
    echo ""
    echo "-n, --nodeps runs rpm with rpm's --nodeps option"
    echo "-f, --force runs rpm with rpm's --force option"
    echo "-d, --directory=[directory from http://build1.sled.lab.novell.com/uia]"
}

if $(getopt -T >/dev/null 2>&1) ; [ $? = 4 ] ; then # New longopts getopt.
    OPTS=$(getopt -o $SHORTOPTS --long $LONGOPTS -n "$PROG" -- "$@")
else # Old classic getopt.
    # Special handling for --help and --version on old getopt.
    case $1 in --help) print_help ; exit 0 ;; esac
    case $1 in --version) print_version ; exit 0 ;; esac
    OPTS=$(getopt $SHORTOPTS "$@")
fi

if [ $? -ne 0 ]; then
    echo "'$PROG --help' for more information" 1>&2
    exit 1
fi

eval set -- "$OPTS"

while [ $# -gt 0 ]; do
    case $1 in
        -h|--help)
            usage
            exit 0
            ;;
        -v|--version)
            echo $VERSION
            exit 0
            ;;
        -f|--force)
            force=1
            shift
            ;;
        -n|--nodeps)
            nodeps=1
            shift
            ;;
        -d|--directory)
            dir=$2
            shift 2
            ;;
        --)
            shift
            break
            ;;
        *)
            echo "ERROR: option processing error: $1" 1>&2
            exit 1
            ;;
    esac
done

if [ "$nodeps" = "1" ]; then
    opts="--nodeps"
fi

if [ "$force" = "1" ]; then
    opts=$opts" --force"
fi

# get the latest rpms at http://build1.sled.lab.novell.com/uia/$dir
# and update the machine 

# create a temporary directory
TD=`mktemp -d`
cd $TD
if [ $? != "0" ]; then
    echo "ERROR:  failed to create and change to a temporary directory" 1>&2
    exit 1
fi

# check if the OS is 32 or 64 bit and set the URL accordingly
if [ -d "/usr/lib64" ]; then
    if [ -f "/etc/fedora-release" ];then
        URL="http://build1.sled.lab.novell.com/uia/fedora/64/$dir/"
    else
        URL="http://build1.sled.lab.novell.com/uia/opensuse/64/$dir/"
    fi
else
    if [ -f "/etc/fedora-release" ];then
        URL="http://build1.sled.lab.novell.com/uia/fedora/32/$dir/"
    else
        URL="http://build1.sled.lab.novell.com/uia/opensuse/32/$dir/"
    fi
fi

echo `wget -r -nv --accept=rpm -np -nd -l1 $URL` 2>&1

if [ $? != "0" ]; then
    echo "ERROR:  failed to download the rpms" 1>&2
    exit 1
fi

num_files=`ls -1 | wc -l`

# can't rely on wget's return code
# (https://savannah.gnu.org/bugs/index.php?23348), so return error if
# no files were downloaded
if [ $num_files -eq 0 ]; then
    echo "ERROR:  failed to download the rpms" 
    exit 1
fi

rpm -Uvh $opts *.rpm
if [ $? != "0" ]; then
    echo "ERROR:  failed to update the rpms" 1>&2
    exit 1
fi

# destroy the evidence
cd ..
rm -r $TD
