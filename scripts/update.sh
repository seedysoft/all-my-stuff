#!/bin/bash

EXECUTABLE_FILE_NAME="Seedysoft.BlazorWebApp.Server"
WORKER_SERVICE_NAME="ss-BlazorWebAppTest"
WORKER_SERVICE_USER="pi"

# Stop service
./stop-daemon.sh -s "${WORKER_SERVICE_NAME}"

# Move working directory to WORKER_SERVICE_DIRECTORY
WORKER_SERVICE_DIRECTORY="$(dirname "$(readlink -f "$0")")"
echo "'${WORKER_SERVICE_NAME}' will be installed in '${WORKER_SERVICE_DIRECTORY}' ..."
if ! cd "${WORKER_SERVICE_DIRECTORY}"; then
  echo "${COLOR_RED_BOLD}ERROR${COLOR_NO}: Can not cd into '${WORKER_SERVICE_DIRECTORY}' folder"
  exit 1
fi

# Check if we're running from worker's directory
if [ ! -f ./$EXECUTABLE_FILE_NAME ]; then
  echo "${COLOR_RED_BOLD}ERROR${COLOR_NO}: Can not locate '${EXECUTABLE_FILE_NAME}' file in '${WORKER_SERVICE_DIRECTORY}'"
  echo "Is the script in the right directory?"
  exit 1
fi
echo "'${EXECUTABLE_FILE_NAME}' file is in '${WORKER_SERVICE_DIRECTORY}'"

# Check if worker's user owner is root
if [ "${WORKER_SERVICE_USER}" == "root" ] || [ "${WORKER_SERVICE_USER}" == "UNKNOWN" ]; then
  echo "${COLOR_RED_BOLD}ERROR${COLOR_NO}: The owner of '${WORKER_SERVICE_DIRECTORY}' directory is '${WORKER_SERVICE_USER}'"
  echo "Please, change the owner with the command 'chown <user>:<user> -R \"${WORKER_SERVICE_DIRECTORY}\"'"
  echo "The user <user> will be used to run '${WORKER_SERVICE_NAME}'"
  exit 1
fi

# # Extract files from 7z
# echo "Extracting 7z files"
# 7z x -y *.7z

# Start service
./create-daemon.sh -f "${EXECUTABLE_FILE_NAME}" -s "${WORKER_SERVICE_NAME}"

# rm *.7z
