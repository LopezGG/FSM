#!/bin/sh
sol="ex."
fstFile="$1"
inputFile="$2"
outputFile="$3"
ProcessedWordList="wordlist_ex_out"
echo $inputFile
mono ProcessInput.exe /opt/dropbox/15-16/570/hw4/examples/wordlist_ex $ProcessedWordList

cat $ProcessedWordList| while read iLine
do
	 
	var1=$(echo $iLine| carmel -sli -k 1 -OE $fstFile);
        size=${#var1};
        var1="${var1%?}"
        echo $var1
        iLine_proc="$(echo -e "${iLine}" | tr -d '[[:space:]]\"')"
	var2=$iLine_proc" => "; 
        var3=$var2"*NONE*"
        if [ $size != "1" ];then
            var3=$var2$var1
       	fi
        echo $var3 >> $outputFile

done

