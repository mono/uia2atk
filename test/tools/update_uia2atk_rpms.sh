#!/bin/bash

##############################################################################
# Written by:  Brian G. Merrell <bgmerrell@novell.com>
# Date:        June 24 2008
# Description: Download the latest uia2atk rpms and upgrade to them
##############################################################################

opts=""

function usage
{
    echo "usage: update_uia2atk_rpms [[[-n|--nodeps] [-f|--force]] | [-h|--help]]"
    echo ""
    echo "-n|--nodeps runs rpm with rpm's --nodeps option"
    echo "-f|--force runs rpm with rpm's --force option"
}

while [ "$1" != "" ]; do
    case $1 in
        -n | --nodeps )         nodeps=1
                                ;;
        -f | --force )          force=1
                                ;;
        -h | --help )           usage
                                exit
                                ;;
        * )                     usage
                                exit 1
    esac
    shift
done

if [ "$nodeps" = "1" ]; then
    opts="--nodeps"
fi

if [ "$force" = "1" ]; then
    opts=$opts" --force"
fi

# get the latest rpms at http://build1.sled.lab.novell.com/uia/
# and update the machine 

# create a temporary directory
TD=`mktemp -d`
cd $TD
if [ $? != "0" ]; then
    echo "Error:  failed to create and change to a temporary directory"
    exit
fi
wget --accept=rpm --progress=dot -r -np -nd -l1 http://build1.sled.lab.novell.com/uia/
if [ $? != "0" ]; then
    echo "Error:  failed to download the rpms"
    exit
fi
rpm -Uvh $opts *.rpm
if [ $? != "0" ]; then
    echo "Error:  failed update the rpms"
    exit
fi

# destroy the evidence
cd ..
rm -r $TD
