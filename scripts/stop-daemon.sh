#!/bin/bash

source shared.sh

############################################################
# Help                                                     #
############################################################
Help(){
  echo "This script will stop a systemd service"
  echo
  echo "Usage: bash $0 [-h] -s service-name"
  echo "options:"
  echo "-h              Show this help"
  # echo "-f  [REQUIRED]  Executable file name"
  echo "-s  [REQUIRED]  Service name"
  # echo "-u  [pi]        User and group that executes service"
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

# Check if the install script is running as root
# if [ "$EUID" -ne 0 ]; then
  # echo "${COLOR_RED_BOLD}ERROR${COLOR_NO}: Please run this script as root"
  # exit 1
# fi

echo "${#} arguments in $0"

############################################################
# Process the input options.                               #
############################################################
while getopts ":f:h:s:u" option; do
  case $option in
    h) # display Help
      Help
      exit 1;;

    s) # Enter a service name
      WORKER_SERVICE_NAME=$OPTARG;;

    \?) # Invalid option
      echo "Error: Invalid option '${option}'"
      exit 1;;

   esac
done

# Check minimun required parameters
if [[ $# -ne 2 ]]; then
    Help
    exit 1
fi

if [[ ! $WORKER_SERVICE_NAME == *.service ]]; then
  WORKER_SERVICE_NAME=$(echo $WORKER_SERVICE_NAME.service)
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
