mkdir code
cd code

echo "Cloning git repository"

git clone https://%API_TOKEN%@github.com/%GIT_REPO%.git .

echo "Repository cloned"

echo %CODE_PATH%

%CODE_PATH%ci.bat > \zbuild.log | type \zbuild.log

echo "Recording result"

if %ERRORLEVEL% == 0 (
    echo "Build Successfull"
    echo "SUCCESS" > \%LOG_FILE%_%REPLICA_ID%.complete
) else (
    echo "Build Failed"
	echo "FAILURE" > \%LOG_FILE%_%REPLICA_ID%.complete
)