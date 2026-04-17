if [ "$(jq '.runs[0].results | length' inspection-results.json)" -gt 0 ]; then
 echo "Lint errors found.  Check inspection-results.json for details."
 exit 1
fi

echo "No lint errors found."