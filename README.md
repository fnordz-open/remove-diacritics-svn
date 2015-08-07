# remove-diacritics-svn
Rename files (keep svn history) replacing special characters recursively in a svn folder

In order to run, just copy the remove-diacritics-svn.exe and svn.exe files to the root directory and run it. At the end, commit the changes.

Note that rename directories can be problematic, needing more that one execution followed by an "svn update". This tool can be improved to check this problem, commit, update, etc.. But I choose to keep the commits manual.
