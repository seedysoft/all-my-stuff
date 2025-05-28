#!/bin/bash

echo "${#} arguments in $0"

# Check minimun required parameters
if [[ $# -ne 1 ]]; then
    echo "Must provide zip file name!"
    exit 1
fi

source shared.sh

echo "worker_service_name: ${WORKER_SERVICE_NAME}"

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

# Extract files from zip
echo "Extracting files" 
unzip -o -q $1 -d ./

chown pi:pi *
chmod ug+rw *

# Start service
./create-daemon.sh -f "${EXECUTABLE_FILE_NAME}" -s "${WORKER_SERVICE_NAME}"

rm *.zip
