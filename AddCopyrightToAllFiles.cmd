title Adding copyright statements
echo Finding files...
for /R %%f in (*.cs) do (
	echo %%f
	call AddCopyrightToSingleFile.cmd "%%f"
)
pause