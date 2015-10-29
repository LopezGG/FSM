#!/bin/sh
lexiconPath="$1"
MorphRulesPath="$2"
output_path="$3"
FirstQuestion=false
mono FSM.exe  $lexiconPath $MorphRulesPath $output_path $FirstQuestion  

