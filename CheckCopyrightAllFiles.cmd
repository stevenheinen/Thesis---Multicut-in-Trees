@echo off
echo Checking copyright for all files...
for /R %%f in (*.cs) do (
	echo %%f
	call CheckCopyrightSingleFile.cmd "%%f"
)