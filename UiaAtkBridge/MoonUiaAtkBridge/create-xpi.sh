#!/bin/bash
rm -rf components && mkdir components && mv Moon* components
rm -f create-xpi.sh
zip -r9 novell-moonlight-a11y.xpi *
