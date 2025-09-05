#!/bin/bash

EXECUTABLE_FILE_NAME="Seedysoft.BlazorWebApp.Server"
WORKER_SERVICE_NAME="ss-BlazorWebAppTest"
WORKER_SERVICE_USER="pi"

# Check minimun required parameters
if [[ $# -ne 1 ]]; then
  echo "Must provide zip file name!"
  exit 1
fi

ZIP_FILE_NAME=$1
if [ -z "${ZIP_FILE_NAME}" ]; then
  echo "No file to update"
  exit 1
fi

source shared.sh
echo "worker_service_name: ${WORKER_SERVICE_NAME}"

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

echo "Calling stop script"
sudo ./stop-daemon.sh -s "${WORKER_SERVICE_NAME}"

echo "Extracting files ..."
unzip -o -q $ZIP_FILE_NAME -d ./

echo "Changing owner and mod ..."
chown pi:pi *; chmod ug+rw *; chmod ug+x *.sh; chmod ug+x Seedysoft.*;

echo "Calling create script"
sudo ./create-daemon.sh -f "${EXECUTABLE_FILE_NAME}" -s "${WORKER_SERVICE_NAME}"

# rm *.7z
