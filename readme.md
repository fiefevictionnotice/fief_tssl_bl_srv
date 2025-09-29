# ☀ Why does this fork of tssl_bl_srv exist?

Small fixes and improvements.

Fixes: 
- The original server has an example server.txt file, but will only run if no file extension is used with the original dockerfile (i.e. "server" instead of "server.txt"). This is an unnecessary source of confusion.

Improvements:
- This version is more flexible for running multiple instances of the same image on different listening ports. The original dockerfile does not expose multiple TCP ports or expose the SERVER_PORT variable. In this version, if you want to run 10 servers with multiple configurations (different server names, game modes, etc.) from one image on the same virtual machine you can. This is only possible if both are true A) multiple TCP/UDP ports are exposed in the dockerfile, and B) you are able to specify the SERVER_PORT variable with the run command. Both are true in this fork, but neither are true for the original branch. 
- More example configurations.

 Planned:
 Add Chat Commands (DoFAdminTools) as a default module.

# ☀ TSSL Bannerlord Server

TSSL is a **dockerized** **Bannerlord server** setup that allows you to **quickly deploy** to Dedicated server. This guide provides instructions for setting up the server manually, as well as automating the build and deployment process using **GitHub Actions** and **Docker**.

#### 💡 Features:
- **Building** and **Deploying** [Bannerlord Server](https://moddocs.bannerlord.com/multiplayer/hosting_server/) on **Dedicated Server** using [Docker](https://docker.com), [Docker Hub](https://hub.docker.com) and [Github Actions](https://github.com/features/actions)
- **Control** server instance and manage [Docker Container](https://www.docker.com/resources/what-container/) on **Dedicated Server**.
- [Enviornments for Deployment](https://docs.github.com/en/actions/managing-workflow-runs-and-deployments/managing-deployments/managing-environments-for-deployment) as a **Secret Vault**. 


---

## 🚀 Get Started

##### 📥 Install Docker
 Docker is a platform for creating, deploying, and managing lightweight, portable containers for applications. It simplifies development by packaging code and dependencies into isolated environments that work seamlessly across different systems.

Installation Links:
- [Linux Debian](https://docs.docker.com/engine/install/debian/)
- [Windows](https://docs.docker.com/desktop/setup/install/windows-install/) 


##### 🔧 Build Docker Image
```
docker build -t fief_tssl_bl_srv .
```
or, if using Azure Container REgistry:
```
docker build -f dockerfile -t YourRegistryHere.azurecr.io/bannerlord:latest .; docker push YourRegistryHere.azurecr.io/bannerlord:latest
```

##### 🚀 Run Docker Image
```
docker run -d --name bnl-battle \
-e TW_TOKEN=<Your_Taleworlds_Token_Which_Expires_Every_3_Months> \
-e MODULES="_MODULES_*Native*Multiplayer*_MODULES_" \
-e TICK_RATE=60 \
-e SERVER_CFG="Native/server-battle" \
-e SERVER_PORT=7210 \
-p 7210:7210/tcp -p 7210:7210/udp \
yourazureregistry.azurecr.io/bannerlord:latest
```

By contrast, the original branch has no "SERVER_PORT" variable as of writing. 

TCP ports 7210-7220 are exposed for both TCP & UDP in the dockerfile. You can modify this for a broader range of ports if you need to run more than 10 versions of the same image on the same box. 

Additional example with updated TCP Port (works with the same image - note this example assumes you're running DOFAdminTools): 
```
docker run -d --name bnlcc-realbattle \
-e TW_TOKEN=<YourTWTokenHere> \
-e MODULES="_MODULES_*Native*Multiplayer*CCModule*DoFAdminTools*_MODULES_" \
-e TICK_RATE=60 \
-e SERVER_CFG="Native/server-battle-config" \
-e SERVER_PORT=7215 \
-p 7215:7215/tcp -p 7215:7215/udp \
<YourRegistryName>.azurecr.io/bannerlord:latest
```

##### 📝 Other Helpful Example Commands

Stop & remove container (you can't start it back up without removing it first):
```
docker stop server && docker rm server
docker stop bnl-duel && docker rm bnl-duel
```

Review logs (can provide useful information about errors:
```
docker logs -f bnl-skirmish
docker logs -f bnl-duel
docker logs -f bnl-duel | grep -i error
docker logs -f bnl-duel | grep -i warn
docker login
```
If using Azure Container Registry: 
```
az acr login -n <YourRegistryNameHere>
```
Stop All - use at your own risk:
```
docker stop $(docker ps -q)
docker rm $(docker ps -aq)
```
Review the contents of the Docker image without running it (can help detect issues with the config): 
```
docker run -it --rm <YourAzureContainerRegistry>.azurecr.io/bannerlord:latest bash
cat /bl_srv/Modules/Native/server-duel-test
```

#####  ⚙️ Enviornment Variables:
- `-e TW_TOKEN="Your Taleworld Server Token"`
    - Sets your Taleworld server token required for authentication on Taleworld API. Replace value with generated and valid token.

- `-e MODULES="_MODULES_*Native*Multiplayer*_MODULES_"`
    - Defines the modules that will be loaded upon run. You can customize this to include other modules.

- `-e TICK_RATE=60`
    - This variable controls the game's tick rate, which affects how often the game logic is updated. 

- `-e SERVER_CFG="Native/server.txt"`
    - This variable specifies the server configuration path from `Module/` directory inside container.

For more informations about Taleworld Token, check out [Taleworld - Hosting a Custom Server](https://moddocs.bannerlord.com/multiplayer/hosting_server/)

For more informations about Custom Modules, check out [Taleworld - Custom Game Modules](https://moddocs.bannerlord.com/multiplayer/custom_game_mode/)

---
## 📁 Where do I place maps, custom modules, and server configuration files? 
Multiplayer maps should be placed within the Scene Objects folder:
```\tssl_bl_srv\modules\Multiplayer\SceneObj\*```

Custom Modules reside within:
```tssl_bl_srv\modules```
e.g. ```C:\Users\<YourUsername>\Documents\tssl_bl_srv\modules\DoFAdminTools```

Where do I place server configuration files?
```\tssl_bl_srv\modules\Native\*``` 

e.g. ``C:\Users\<Username>\Documents\tssl_bl_srv\modules\Native\server-duel-alt``


## 💡 Getting Started with this Fork - Details for Beginners:

First time GitHub users, read this:
<details>
  <summary>Click to expand</summary>
 
**Summary:**
 Install Git and clone the repository. You can modify the modules, server configuration files, and maps afterwards.
 
**Install Git:**
Download the Git installer: ```https://git-scm.com/download/win```
Run it with the defaults (next → next → finish) - as with everything here, use at your own risk.
Open PowerShell and check it worked:
```git --version```
You should see something like ```git version 2.x.x.```

**Clone the Repo:**
Then run:
```
cd $env:USERPROFILE\Documents
git clone https://github.com/fiefevictionnotice/fief_tssl_bl_srv.git
```

That makes a folder:
```
C:\Users\YourName\Documents\fief_tssl_bl_srv
```

Whenever you add a map, you need to ensure you have an updated server configuration file which actually loads the map in. Then invoke that updated server configuration when you start the server. If you review the provided example configuration files, you can see how both native (built-in) and custom maps are loaded in a server configuration file. You cam mix multiple maps that support different game modes in a server even though that's not used in the example server configuration files.  

</details>


## 📁 Importing Server Files

Anything **placed** in the `modules/` directory will be copied to the `Modules/` directory in **[Docker Container](https://www.docker.com/resources/what-container/)**. This includes custom **scripts**, **assets**, **configurations**, or even **new modules**.

**For more informations please read [Server Files Documentation](https://github.com/vojinpavlovic/tssl_bl_srv/blob/main/docs/server-files.md)**

##### 👉 Note: 

You can use [Github Actions](https://docs.github.com/en/actions) to streamline the process of building images for to [Docker Hub](https:/hub.docker.com) these images can be deployed to a Dedicated Server. 

- ! Workflows actions so far have been **only** **tested** on **Dedicated Sever** with `Debian GNU/Linux 12 (bookworm)` installed.

You must have linux with Docker pre-installed on Dedicated Server and an linux user with permissions to run `docker` command. 
- ! Do not use `root` access. The large part of Linux ecosystem is designed to be run as a limited user, not as root. Running applications as root is very insecure and it can lead you to break your entire system without warning if you're not careful.

---

## ⚠️ Disclaimers

Use at your own risk, there is no official support for this repository.

Note from fork author: I have left the below unmodified from the original branch, but I have not used or tested these workflows; your mileage may vary.


## 🚀 Worfklows

![Deploy To Dedicated Server Deployment Badge](https://github.com/vojinpavlovic/tssl_bl_srv/actions/workflows/deploy-instance.yml/badge.svg) ![Build Image to Docker Hub Deployment Badge](https://github.com/vojinpavlovic/tssl_bl_srv/actions/workflows/build-image.yml/badge.svg) ![Instance actions on Dedicated Server Deployment Badge](https://github.com/vojinpavlovic/tssl_bl_srv/actions/workflows/instance-actions.yml/badge.svg)

A workflow is a **configurable, automated process** defined by a **YAML file** in your repository, workflows can be triggered by repository manually.

Workflows are **stored** in the `.github/workflows directory`. A repository can **have multiple** workflows, each performing tasks, such as:
- **Building** and **Deploying** Docker Images
- **Control** Docker Containers on Dedicated Server
- **Configure** by enviornment or in default enviornment

**For more informations please read [Workflow Documentation](https://github.com/vojinpavlovic/tssl_bl_srv/blob/main/docs/workflows.md)**


## 🔐 Github Stored Secrets

Secrets are variables that you create in an organization, repository, or repository environment. The secrets that you create are available to use in GitHub Actions workflows. GitHub Actions can only read a secret if you explicitly include the secret in a workflow.

**For more informations please read [Secrets Documentation](https://github.com/vojinpavlovic/tssl_bl_srv/blob/main/docs/secrets.md)**


---

This project is licensed under the **MIT** License. See the [LICENSE](https://github.com/vojinpavlovic/tssl_bl_srv/blob/main/LICENSE) file for more details.
