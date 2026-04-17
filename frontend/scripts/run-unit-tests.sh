cd $(git rev-parse --show-toplevel) || exit 1

TAG=frontend-tests-runner:latest
NAME=frontend-test-runner

docker container remove ${NAME} || true
docker image remove ${TAG} || true

docker build -t=${TAG} -f docker/Dockerfile --target=test .

rm -rf ./testOutput || true
mkdir testOutput

docker run --name=${NAME} -v ./testOutput:/app/testOutput ${TAG}

docker wait ${NAME}
