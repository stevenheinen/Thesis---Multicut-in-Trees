echo Adding copyright to all files...
for /R %%f in (*.cs) do (
	echo %%f
	call CheckCopyrightSingleFile.cmd "%%f"
)