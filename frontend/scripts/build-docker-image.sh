# Usage
# Env Variables:
# REGISTRY: the docker registry to push the image to. ex: hub.docker:5000
# BUILD_NUMBER: the unique to this build number.  ex: 1
# PROJECT_NAME: The name of the project.  Defaults to "frontend". ex: my-cool-project-name

cd $(git rev-parse --show-toplevel) || exit 1

if [ -z ${REGISTRY} ]; then
  echo "REGISTRY is unset, please set it before running this script";
  exit 1
fi

if [ -z ${BUILD_NUMBER} ]; then
  echo "BUILD_NUMBER is unset, please set it before running this script";
  exit 1
fi

if [ -z ${PROJECT_NAME} ]; then
  echo "PROJECT_NAME is unset, setting to 'frontend'";
  PROJECT_NAME="frontend"
fi

TAG="${PROJECT_NAME}:${BUILD_NUMBER}"

docker build -t=${TAG} -f docker/Dockerfile --target=final .
docker image tag ${TAG} ${REGISTRY}/${TAG}

docker image push ${REGISTRY}/${TAG}