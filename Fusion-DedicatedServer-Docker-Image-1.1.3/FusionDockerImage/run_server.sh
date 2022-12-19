#!/bin/bash
echo "Run Fusion Dedicated Server Container"

# Container Settings
id=""         # "--name custom_container_name"
session=""    # "-s custom_session_name"
region=""     # "-r custom_region"
lobby=""      # "-l custom_lobby"
publicip=""   # "-i custom_public_ip"
publicport="" # "-p custom_public_port"

# Run a Fusion Server as a new Container
docker run -d -p 27015:27015/udp $id fusion-dedicatedserver $session $region $lobby $publicip $publicport