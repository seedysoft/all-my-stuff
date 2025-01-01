#!/bin/bash

############################################################
# Help                                                     #
############################################################
Help(){
  echo "This script will stop a systemd service"
  echo
  echo "Usage: bash $0 [-h] -f executable-file-name -s service-name [-u user]"
  echo "options:"
  echo "-h              Show this help"
  echo "-s  [REQUIRED]  Service name"
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

WORKER_SERVICE_NAME=

############################################################
# Process the input options.                               #
############################################################
while getopts ":f:h:s:u" option; do
  case $option in
    h) # display Help
      Help
      exit 1;;

    s) # Enter a service name
      if [[ $OPTARG == *.service ]]; then
        WORKER_SERVICE_NAME=$OPTARG
      else
        WORKER_SERVICE_NAME=$(echo $OPTARG.service)
      fi;;

    \?) # Invalid option
      echo "Error: Invalid option '${option}'"
      exit 1;;

   esac
done

# Check minimun required parameters
if [[ $# -lt 2 ]]; then
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
