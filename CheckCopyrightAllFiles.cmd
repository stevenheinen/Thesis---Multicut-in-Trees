@echo off
echo Checking copyright for all files...
cd MulticutInTrees
for /R %%f in (*.cs) do (
	echo %%f|findstr /i /L "\\obj\\">nul
	if errorlevel 1 (
		echo %%f|findstr /i /L "\\bin\\">nul
		if errorlevel 1 (
			echo %%f
			call ..\CheckCopyrightSingleFile.cmd "%%f"
		) else (
			echo skipping %%f
		)
	) else (
		echo skipping %%f
	)
)
cd ..\TESTS_MulticutInTrees
for /R %%f in (*.cs) do (
	echo %%f|findstr /i /L "\\obj\\">nul
	if errorlevel 1 (
		echo %%f|findstr /i /L "\\bin\\">nul
		if errorlevel 1 (
			echo %%f
			call ..\CheckCopyrightSingleFile.cmd "%%f"
		) else (
			echo skipping %%f
		)
	) else (
		echo skipping %%f
	)
)
cd ..