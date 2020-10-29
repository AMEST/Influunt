# Influunt - rss agregator 
### Links
* **[Try Influunt](https://influunt.tk)**  
* **[Influunt docker image](https://hub.docker.com/r/eluki/influunt)**
### Description
Simple aggregator of rss news feeds. Implements a single news feed, with the ability to save posts to favorites, for further reading or as a note about an interesting article.   
In order to use the service, you need to log in (so that the user can add his channels and refer to selected articles). Authorization via Google is implemented as standard, but if the self hosted option does not suit you, you can safely replace it with another provider.   

### Requirements
* MongoDB - For storing user data (channels, favorites) and for aspnet core secure storage.
* AspNet Core 3.1 runtime or Docker for service start
* Optional reverse proxy for https connection

### Using in docker

**Docker compose file for swarm mode:**
```Dockerfile
version: '3.8'

services:
  host:
    image: eluki/influunt
    environment:
      - "ConnectionStrings:Mongo:ConnectionString=[MONGO DB CONNECTION STRING]"
      - "Authentication:Google:ClientSecret=[GOOGLE CLIENT SECRET]"
      - "Authentication:Google:ClientId=[GOOGLE CLIENT ID]"
    ports:
     - target: 80
       published: 30002
       protocol: tcp
       mode: host
    deploy:
      replicas: 1
    logging:
      driver: "json-file"
      options:
        max-size: "3m"
        max-file: "3"
```
Run stack:
```bash
docker stack deploy -c deploy.yml influunt
```

**Docker run:**
```bash
docker run -d \
           -p 30002:80 \
           -e "ConnectionStrings:Mongo:ConnectionString=[MONGO DB CONNECTION STRING]" \
           -e "Authentication:Google:ClientSecret=[GOOGLE CLIENT SECRET]" \
           -e "Authentication:Google:ClientId=[GOOGLE CLIENT ID]" \
           --restart always \
           --name influunt \
           eluki/influunt
```