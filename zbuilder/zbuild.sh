#!/bin/bash
mkdir /root/.ssh
aws s3 --region "us-west-2" cp s3://$S3_BUCKET/git.key /root/.ssh/id_rsa
chmod 400 /root/.ssh/id_rsa
echo "    IdentityFile ~/.ssh/id_rsa" >> /etc/ssh/ssh_config

ssh -o StrictHostKeyChecking=no git@github.com
echo $GIT_REPO
export CODE_DIR="code"
cd "$(dirname "$0")"
rm -rf $CODE_DIR
mkdir $CODE_DIR
cd "$CODE_DIR"
echo $PWD
git clone git@github.com:$GIT_REPO.git .
git reset --hard $GIT_SHA
cd .$CODE_PATH
echo $PWD
echo "Calling build script"
./ci.sh 2>&1 | tee /zbuild.log
if [ ${PIPESTATUS[0]} -eq 0 ]
then
  echo "Build Successfull"
  echo "SUCCESS" | tee /$LOG_FILE"_"$REPLICA_ID".complete"
else
  echo "Build Failed"
  echo "FAILURE" | tee /$LOG_FILE"_"$REPLICA_ID".complete"
fi
