#!/bin/bash

push_to_docker_hub="y"

# Set location
cd /path/to/your/project/Smarthome

# Tag
tag=$(git describe --tags | cut -d '-' -f 1)
if [ -z "$tag" ]; then
  tag="untagged"
fi

# Branch name
branch=$(git rev-parse --abbrev-ref HEAD)

# Create a string for the commit counter
commit_count=$(git rev-list --all --count)
if [ $commit_count -lt 10 ]; then
  commit_count_string="000$commit_count"
elif [ $commit_count -gt 9 -a $commit_count -lt 100 ]; then
  commit_count_string="00$commit_count"
elif [ $commit_count -gt 99 -a $commit_count -lt 1000 ]; then
  commit_count_string="0$commit_count"
else
  commit_count_string=$commit_count
fi

# Concatenate the full version string
if [ "$branch" == "master" ]; then
  full_version_string=$tag
else
  full_version_string="$tag-${branch%%/*}$commit_count_string"
fi

# Build the container
docker build --platform=linux/arm64 -t luckyluke4411/ambientcollector:$full_version_string -f src-docker/Smarthome.AmbientCollector.Api/Dockerfile .

# Push if requested
if [ "$push_to_docker_hub" == "y" ]; then
  echo "Pushing to DockerHub..."
  docker push luckyluke4411/ambientcollector:$full_version_string
  echo "Done."
else
  echo "Pushing to DockerHub skipped. Done."
fi
