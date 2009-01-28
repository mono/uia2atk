#!/bin/bash

##############################################################################
# Written by:  Brian G. Merrell <bgmerrell@novell.com>
# Date:        June 24 2008
# Description: Download the latest uia2atk rpms and upgrade to them
##############################################################################

VERSION="0.2"
PROG="update_uia2atk_rpms.sh"

OPENSUSE="opensuse"
UBUNTU="ubuntu"
FEDORA="fedora"
TRUNK="trunk"
BRANCHES="branches"
TAGS="tags"

TRUNK=$TRUNK
dir="current"
nodeps=0
force=0
tree="trunk"
verbose=false

SHORTOPTS="d:t:r:nfhv"
LONGOPTS="directory:,tree:,release:,nodeps,force,help,version"

function usage
{
    echo "USAGE: update_uia2atk_rpms [-n|--nodeps] [-f|--force] [-h|--help]"
    echo "       [[-d|--directory=]directory] [[-t|--tree=]<$TRUNK|$BRANCHES|$TAGS>"
    echo "       <-v|--version=><tag or branch version>]"
    echo "OPTIONS:"
    echo "  -n, --nodeps runs rpm with rpm's --nodeps option"
    echo "  -f, --force runs rpm with rpm's --force option"
    echo "  -d, --directory=<RPM directory>"
    echo "  -t, --tree=<$TRUNK|$BRANCHES|$TAGS> (branch or tag version must be specified using --release)"
    echo "  -r, --release=<tag or branch version>, to be used with --tree"
}

function cleanup
{
    # destroy the evidence
    echo "Cleaning up..."
    cd /tmp
    rm -r $TD
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
        -t|--tree)
            tree=$2
            shift 2
            ;;
        -r|--release)
            release=${2/./_}
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

if [ $tree != $TAGS ] && [ $tree != $BRANCHES ] && [ $tree != $TRUNK ]; then
    echo "ERROR: '$tree' is not a valid argument for the --tree option"
    exit 1
fi

if ([ $tree = $TAGS ] || [ $tree = $BRANCHES ]) && [ "$release" = "" ]; then
    echo "ERROR: A $tree version number must be specified"
    exit 1
fi

if [ "$nodeps" = "1" ]; then
    opts="--nodeps"
fi

if [ "$force" = "1" ]; then
    opts=$opts" --force"
fi

# check if the OS is 32 or 64 bit
if [ -d "/usr/lib64" ]; then
    ARCH="64"
else
    ARCH="32"
fi
if [ -f "/etc/fedora-release" ]; then
    DISTRO=$FEDORA
elif [ -f "/etc/SuSE-release" ]; then
    DISTRO=$OPENSUSE
elif [ -f "/usr/bin/ubuntu-bug" ]; then
    DISTRO=$UBUNTU
fi

# figure out the version depending on the distro
if [ $DISTRO = $UBUNTU ]; then
    source "/etc/lsb-release"
    DISTRO_VERSION=${DISTRIB_RELEASE/./}
elif [ $DISTRO = $OPENSUSE ]; then
    DISTRO_VERSION=`tail -1 /etc/SuSE-release |\
                    gawk '{ sub(/\./,"",$3); print $3 }'`
elif [ $DISTRO = $FEDORA ]; then
    DISTRO_VERSION=`head -1 /etc/fedora-release | awk '{ sub(/\./,"",$3); print $3 }'`
fi

if [ $tree = $TRUNK ]; then
    URL="http://build1.sled.lab.novell.com/uia/$tree/$DISTRO$DISTRO_VERSION/$ARCH/$dir"
else
    URL="http://build1.sled.lab.novell.com/uia/$tree/$release/$DISTRO$DISTRO_VERSION/$ARCH/$dir"
fi

# create a temporary directory
TD=`mktemp -d`
cd $TD
if [ $? != "0" ]; then
    echo "ERROR:  failed to create and change to a temporary directory" 1>&2
    exit 1
fi


echo "Downloading RPMs from: $URL"

echo `wget -r -nv --accept=rpm -np -nd -l1 $URL` 2>&1

if [ $? != "0" ]; then
    echo "ERROR:  failed to download the rpms" 1>&2
    cleanup
    exit 1
fi

num_files=`ls -1 | wc -l`

# can't rely on wget's return code
# (https://savannah.gnu.org/bugs/index.php?23348), so return error if
# no files were downloaded
if [ $num_files -eq 0 ]; then
    echo "ERROR:  failed to download the rpms" 
    cleanup
    exit 1
fi

rpm -Uvh $opts *.rpm
if [ $? != "0" ]; then
    echo "ERROR:  failed to update the rpms" 1>&2
    cleanup
    exit 1
fi

