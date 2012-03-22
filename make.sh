#!/bin/sh

if [ -z $REVISION ]; then
    REVISION=`svn update . | cut -d' ' -f3 | cut -d'.' -f1`
fi

if [ -z $XSD_PATH ]; then
    XSD_PATH=../../../trunk/web/xsd
fi

REVISION=$REVISION make
