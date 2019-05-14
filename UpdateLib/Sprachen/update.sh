#!/bin/sh
for f in *.lng; do
        lang=`echo $f | sed "s/.lng//" | tr '[:upper:]' '[:lower:]'`
        wget -O "$f" "https://www.mal-was-anderes.de/translation/export.php?programm=update&translang=$lang"
done
