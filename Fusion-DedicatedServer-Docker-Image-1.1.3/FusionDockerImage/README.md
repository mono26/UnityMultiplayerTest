# Fusion Dedicated Server Docker base image

## Setup Docker

1. Go to [https://www.docker.com/get-started/].
2. Install and setup your Docker Service.

## Preparing the Fusion Server for a Docker Image

1. Based on the [Fusion Dedicated Server sample](https://doc.photonengine.com/en-us/fusion/current/technical-samples/fusion-dedicated-server).
2. Build a the `Dedicated Server`:
    - Set `Linux` as the `Target Platform` and `x86_64` as `Architecture`.
    - Check the `Server Build` flag.
    - Set the executable name to `server.x86_64`.
3. Copy all the build files to the `bin` folder.

## Create the Fusion Server Docker Image

1. Open a terminal on the current folder.
2. Make sure the `Docker` service is running in your system.
3. Run: `docker build -t <your_custom_image_name> .`
    - Example: `docker build -t fusion-dedicatedserver .`
4. A new `Docker Image` with the name `fusion-dedicatedserver` will be created on your local repository.

## Run the Fusion Server as a Docker Container

1. Open a terminal on the current folder.
2. Make sure the `Docker` service is running in your system.
3. Run: `docker run -d -p <host_custom_port>:27015/udp <your_custom_image_name>`
    - Example: `docker run -d -p 27015:27015/udp fusion-dedicatedserver`
    - By default, the Fusion Dedicated Server will bind to port `27015`, and this port is already exposed by the Docker image (check the `Dockerfile`). Running the command above, that port will be mapped to the host `27015` port as well.
    - Running a detached Container (`-d` argument) makes it run independently of the current terminal.

### Optional Start Arguments

The Fusion Server Docker Image is prepared to accept the same arguments the standalone build can accepts, but those need have different argument names, because how the `entrypoint.sh` script read those.
Check the argument list below for more info:

- `-s <custom_session_name>`: Use a Custom Session ID Name. Default: Random GUID Session Name
- `-r <custom_region>`: Connect the Server to a Custom Region. Default: Best Region
- `-l <custom_lobby>`: Join a Custom Lobby. Default: Join the Default `ClientServer` Lobby.
- `-i <custom_public_ip>`: Set a Custom Public IP of the Server. Default: Empty, the Server will use the STUN Service to discover its Public IP.
- `-p <custom_public_port>`: Set a Custom Public Port of the Server. Default: Empty, the Server will use the STUN Service to discover its Public Port.

Check the `run_server.sh` script for a more structured way of starting a new Fusion Server Container using the optional arguments.
