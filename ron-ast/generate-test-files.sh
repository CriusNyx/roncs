#!/bin/bash

for file in testFiles/*.ron; do
  filename=$(basename -- "$file");
  name="${filename%.*}";
  input=$file;
  ronOutput="ast/$name.ron";
  jsonOutput="ast/$name.json";
  cargo run -- $input $ronOutput
  cargo run -- $input $jsonOutput
done