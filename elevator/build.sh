npm install
[ $? -eq 0 ]  || exit 1

npm run lint
[ $? -eq 0 ]  || exit 1

npm test
[ $? -eq 0 ]  || exit 1

npm run start
[ $? -eq 0 ]  || exit 1
