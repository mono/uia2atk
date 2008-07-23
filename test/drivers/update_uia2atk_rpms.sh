#!/bin/bash

# get the latest rpms at http://build1.sled.lab.novell.com/uia/
# and update the machine 

# create a temporary directory
TD=`mktemp -d`
cd $TD
wget --accept=rpm --progress=dot -r -np -nd -l1 http://build1.sled.lab.novell.com/uia/
rpm -Uvh *.rpm

# destroy the evidence
cd ..
rm -r $TD
