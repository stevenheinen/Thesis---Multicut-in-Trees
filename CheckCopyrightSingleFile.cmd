@echo off
set "t=// This code was written between November 2020 and October 2021 by Steven Heinen (mailto:s.a.heinen@uu.nl\) within a final thesis project of the Computing Science master program at Utrecht University under supervision of J.M.M. van Rooij (mailto:j.m.m.vanrooij@uu.nl\)."
set "s=// This code was written between November 2020 and October 2021 by Steven Heinen (mailto:s.a.heinen@uu.nl^) within a final thesis project of the Computing Science master program at Utrecht University under supervision of J.M.M. van Rooij (mailto:j.m.m.vanrooij@uu.nl^)."
findstr /c:"%t%" %1
if not "%errorlevel%" == "0" (
	echo Missing copyright in file %1!
	pause
	exit 1
)