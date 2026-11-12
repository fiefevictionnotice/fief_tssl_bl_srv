# Debian 12 (bookworm). Debian 11 (bullseye) reached end of life in 2026.
FROM debian:bookworm-slim

# Set environment variables
ENV STEAMCMDDIR=/steamcmd
ENV BANNERLORDDIR=/bl_srv
ENV PATH="$PATH:/usr/share/dotnet"
ENV DOTNET_CLI_TELEMETRY_OPTOUT=1

# Linux dependencies: steamcmd needs the 32-bit C/C++ runtime; the server needs curl, ICU, OpenSSL 3 and
# libatomic (libRgl.so / libGame.so link against libatomic.so.1).
RUN apt-get update && apt-get install -y --no-install-recommends \
    libatomic1 \
    wget \
    ca-certificates \
    curl \
    tar \
    gnupg \
    unzip \
    lib32gcc-s1 \
    lib32stdc++6 \
    libcurl4 \
    libssl3 \
    libicu72 \
    libgl1 \
    libglu1-mesa && \
    rm -rf /var/lib/apt/lists/*

# Microsoft package repository (Debian 12)
RUN wget -q https://packages.microsoft.com/config/debian/12/packages-microsoft-prod.deb && \
    dpkg -i packages-microsoft-prod.deb && \
    rm packages-microsoft-prod.deb && \
    apt-get update

# .NET 8 + ASP.NET Core 8 runtime. Bannerlord 1.5.6 moved the dedicated server from .NET 6 to .NET 8
# (TaleWorlds.Starter.DotNetCore.Linux.runtimeconfig.json asks for Microsoft.NETCore.App 8.0.0).
RUN apt-get install -y --no-install-recommends aspnetcore-runtime-8.0 && \
    rm -rf /var/lib/apt/lists/*

# Add i386 architecture for 32-bit libraries
RUN dpkg --add-architecture i386

# Install SteamCMD
RUN mkdir -p ${STEAMCMDDIR} && \
    curl -o ${STEAMCMDDIR}/steamcmd_linux.tar.gz https://steamcdn-a.akamaihd.net/client/installer/steamcmd_linux.tar.gz && \
    tar -xvzf ${STEAMCMDDIR}/steamcmd_linux.tar.gz -C ${STEAMCMDDIR} && \
    rm ${STEAMCMDDIR}/steamcmd_linux.tar.gz

# Create Bannerlord server directory
RUN mkdir -p ${BANNERLORDDIR}

# Set working directory to SteamCMD
WORKDIR ${STEAMCMDDIR}

# Install Bannerlord server using SteamCMD. A freshly installed steamcmd often fails its first app_update with
# "Missing configuration", so let it self-update first and retry the install up to 3 times.
RUN ./steamcmd.sh +quit || true; \
    for i in 1 2 3; do \
      ./steamcmd.sh +force_install_dir ${BANNERLORDDIR} +login anonymous +app_info_update 1 +app_update 1863440 validate +quit && \
      [ -f ${BANNERLORDDIR}/bin/Linux64_Shipping_Server/TaleWorlds.Starter.DotNetCore.Linux.dll ] && exit 0; \
      echo "steamcmd attempt $i failed, retrying"; sleep 5; \
    done; exit 1

# Set working directory to Bannerlord server directory
WORKDIR ${BANNERLORDDIR}/bin/Linux64_Shipping_Server

# Copy the ASP.NET Core 8 runtime files next to the server (whatever 8.0.x patch version apt installed)
RUN ASPNET=$(ls -d /usr/share/dotnet/shared/Microsoft.AspNetCore.App/8.* | sort -V | tail -1) && \
    echo "ASP.NET Core runtime: $ASPNET" && \
    cp -R "$ASPNET"/. ${BANNERLORDDIR}/bin/Linux64_Shipping_Server/

# Copy the configuration file from the host to the container

#COPY /config/ ${BANNERLORDDIR}/Modules/Native/

# Copy the map files from the host to the container
COPY modules/ ${BANNERLORDDIR}/Modules/

# Expose TCP port 7210
EXPOSE 7210/tcp
EXPOSE 7211/tcp
EXPOSE 7212/tcp
EXPOSE 7213/tcp
EXPOSE 7214/tcp
EXPOSE 7215/tcp
EXPOSE 7216/tcp
EXPOSE 7217/tcp
EXPOSE 7218/tcp
EXPOSE 7219/tcp
EXPOSE 7220/tcp
#TCP/UDP ports could be expanded in your own dockerfile if needed.

# Expose UDP port 7210
EXPOSE 7210/udp
EXPOSE 7211/udp
EXPOSE 7212/udp
EXPOSE 7213/udp
EXPOSE 7214/udp
EXPOSE 7215/udp
EXPOSE 7216/udp
EXPOSE 7217/udp
EXPOSE 7218/udp
EXPOSE 7219/udp
EXPOSE 7220/udp

# Run the server
# SERVER_PORT is no longer hardcoded in this fork.
CMD ["/bin/sh", "-c", "dotnet TaleWorlds.Starter.DotNetCore.Linux.dll $MODULES /dedicatedcustomserverconfigfile ../../Modules/$SERVER_CFG /tickrate $TICK_RATE /dedicatedcustomserverauthtoken $TW_TOKEN /dedicatedcustomserver ${SERVER_PORT} USER 0 /playerhosteddedicatedserver"]
