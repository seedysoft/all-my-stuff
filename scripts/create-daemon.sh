#!/bin/bash

############################################################
# Help                                                     #
############################################################
Help(){
  echo "This script will install a systemd service"
  echo
  echo "Usage: bash $0 [-h] -f executable-file-name -s service-name [-u user]"
  echo "options:"
  echo "-h              Show this help"
  echo "-f  [REQUIRED]  Executable file name"
  echo "-s  [REQUIRED]  Service name"
  echo "-u  [pi]        User and group that executes service"
  # echo "      --favorite_food  |  -f     []                    chocolate or pizza?"
  # echo "      --secret         |  -s     [!@#%^&*?/.,[]{}+-|]  special characters"
  # echo "      --language       |  -lang  [C.UTF-8]             default value can be a variable"
  # echo "      --password       |  -p     [REQUIRED]            What is your password?"
  # echo "      --happy          |  -hp    [FLAG]                Flag for indicating that you are happy"
  # echo "      --ci             |  -ci    [FLAG]                Flag for indicating it is a CI/CD process"
  # echo "      --username       |  -un    [willywonka]          Username fetched from environment variable"
  echo
}

############################################################
############################################################
# Main program                                             #
############################################################
############################################################

# Setting up colors
COLOR_RED_BOLD="$(printf '\033[1;31m')"
COLOR_GREEN_BOLD="$(printf '\033[1;32m')"
COLOR_NO="$(printf '\033[0m')" # No Color

# Check if the install script is running as root
# if [ "$EUID" -ne 0 ]; then
  # echo "${COLOR_RED_BOLD}ERROR${COLOR_NO}: Please run this script as root"
  # exit 1
# fi

echo "${#} arguments"

EXECUTABLE_FILE_NAME=
WORKER_SERVICE_NAME=
WORKER_SERVICE_USER="pi"

############################################################
# Process the input options.                               #
############################################################
while getopts ":f:h:s:u" option; do
  case $option in
    f) # executable file name
      EXECUTABLE_FILE_NAME=$OPTARG;;

    h) # display Help
      Help
      exit 1;;

    s) # Enter a service name
      if [[ $OPTARG == *.service ]]; then
        WORKER_SERVICE_NAME=$OPTARG
      else
        WORKER_SERVICE_NAME=$(echo $OPTARG.service)
      fi;;

    u) # User and Group
      WORKER_SERVICE_USER=$OPTARG;;

    \?) # Invalid option
      echo "Error: Invalid option '${option}'"
      exit 1;;

   esac
done

# Check minimun required parameters
if [[ $# -lt 3 ]]; then
    Help
    exit 1
fi

# Check if worker service is running
echo "Checking if the service '${WORKER_SERVICE_NAME}' is running ..."
if systemctl is-active --quiet "${WORKER_SERVICE_NAME}"; then
  echo "Service '${WORKER_SERVICE_NAME}' is running"

  # Stop and unload the service
  if systemctl stop "${WORKER_SERVICE_NAME}"; then
    echo "Service '${WORKER_SERVICE_NAME}' stopped"
  else
    echo "${COLOR_RED_BOLD}ERROR${COLOR_NO}: The service '${WORKER_SERVICE_NAME}' Can not be stopped"
    exit 1
  fi
else
  echo "Service '${WORKER_SERVICE_NAME}' is not running"
fi

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
echo "'${WORKER_SERVICE_NAME}' will be executed with the user '${WORKER_SERVICE_USER}'"

# Write the systemd service descriptor
WORKER_SERVICE_FULLPATH="/etc/systemd/system/${WORKER_SERVICE_NAME}"
echo "Creating '${WORKER_SERVICE_NAME}' unit file in '${WORKER_SERVICE_FULLPATH}' ..."
cat > "${WORKER_SERVICE_FULLPATH}" <<EOL
[Unit]
Description=Long running daemon created from .NET worker template
After=network.target

[Service]
Type=notify

User=${WORKER_SERVICE_USER}
Group=${WORKER_SERVICE_USER}

# will set the Current Working Directory (CWD)
WorkingDirectory=${WORKER_SERVICE_DIRECTORY}

# systemd will run this executable to start the service
ExecStart=${WORKER_SERVICE_DIRECTORY}/${EXECUTABLE_FILE_NAME}

TimeoutStopSec=30

EnvironmentFile=/etc/environment

# to query logs using journalctl, set a logical name here  
SyslogIdentifier='${WORKER_SERVICE_NAME}'

# ensure the service restarts after crashing
Restart=always

# amount of time to wait before restarting the service
RestartSec=20

[Install]
WantedBy=multi-user.target

EOL
if [ $? -ne 0 ]; then
  echo "${COLOR_RED_BOLD}ERROR${COLOR_NO}: Can not create the file '${WORKER_SERVICE_FULLPATH}'"
  echo "The UnitPath of systemd changes from one distribution to another. You may have to edit the script and change the path manually"
  exit 1
fi

# Reload systemd daemon
echo "Installing '${WORKER_SERVICE_NAME}' ..."
if ! systemctl daemon-reload; then
  echo "${COLOR_RED_BOLD}ERROR${COLOR_NO}: Can not reload systemd daemon"
  exit 1
fi

# Enable the service for following restarts
if ! systemctl enable "${WORKER_SERVICE_NAME}"; then
  echo "${COLOR_RED_BOLD}ERROR${COLOR_NO}: Can not enable the service '${WORKER_SERVICE_NAME}'"
  exit 1
fi
echo "'${WORKER_SERVICE_NAME}' enabled"

# Run the service
if systemctl start "${WORKER_SERVICE_NAME}"; then
  echo "${COLOR_GREEN_BOLD}Service '${WORKER_SERVICE_NAME}' successfully installed and launched!${COLOR_NO}"
else
  echo "${COLOR_RED_BOLD}ERROR${COLOR_NO}: Can not start the service '${WORKER_SERVICE_NAME}'"
  exit 1
fi