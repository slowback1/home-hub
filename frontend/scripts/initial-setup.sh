# Ensure we're in a valid git repo, and cd to the root
cd $(git rev-parse --show-toplevel) || exit 1

# Copy the example config file to the correct location
cp ./static/config/config.example.json ./static/config/config.json

# Install the node modules in the base application
npm install

# Install the node modules in the cypress tests
cd ./cypress || exit 1
npm install