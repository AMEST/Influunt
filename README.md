![CI](https://github.com/AMEST/Influunt/workflows/CI/badge.svg?branch=master)
![hub.docker.com](https://img.shields.io/docker/pulls/eluki/influunt.svg)
![GitHub release (latest by date)](https://img.shields.io/github/v/release/amest/influunt)
![GitHub](https://img.shields.io/github/license/amest/influunt)
# Influunt - rss agregator 
- [Influunt - rss agregator](#influunt---rss-agregator)
  - [Links](#links)
  - [Description](#description)
  - [Features](#features)
  - [Self-hosted Requirements](#self-hosted-requirements)
  - [Configurations](#configurations)
    - [Authentication](#authentication)
    - [Database](#database)
    - [Feed](#feed)
  - [Screenshots](#screenshots)
  - [Using in docker](#using-in-docker)
## Links
* **[Try Influunt](https://influunt.tk)**  
* **[Influunt docker image](https://hub.docker.com/r/eluki/influunt)**
## Description
Simple aggregator of rss news feeds. Implements a single news feed, with the ability to save posts to favorites, for further reading or as a note about an interesting article.   
In order to use the service, you need to log in (so that the user can add his channels and refer to selected articles). Authorization via Google is implemented as standard, but if the self hosted option does not suit you, you can safely replace it with another provider.   

## Features
1. Authorization: Google and Guest (for try service or local / self-hosted using)
2. Infinite news feed (while added news from channels in user feed exists)
3. Receiving news feed in parts (10 news)
4. Favorites (add/remove)
5. Channels (add/edit/change visible in feed/remove)
6. PWA functional with offline mode (request caching)
7. Automatic background fetch new news from channels and push to user feed for active accounts
8. Redis distributed cache (cache remotes channels)

## Self-hosted Requirements
* MongoDB - For storing user data (channels, favorites) and for aspnet core secure storage.
* .NET 8 runtime or Docker for service start
* Optional reverse proxy for https connection

## Configurations 

All configuration may be configured in `appsettings.json` or in Environment variables

### Authentication

* `Authentication:Google:ClientSecret` - client id in google 
* `Authentication:Google:ClientId` - client secret in google 

### Database

Influunt use MongoDB as persistent data storage and Redis for faster distributed cache.

*  `ConnectionStrings:Mongo:ConnectionString` - mongo connection string (example: `mongodb://app:password@localhost/influunt?authSource=admin`)
*  `ConnectionStrings:Redis:ConnectionString` - connection string for Redis cache type

### Feed

* `FeedCrawler:Enabled` - enable background channels fetch (default: `true`). _In the future, can be disabled in api, and enabled in external worker_
* `FeedCrawler:FetchInterval` - interval between fetching news from channels (default: `00:30:00` (30min))
* `FeedCrawler:LastActivityDaysAgo` - Minimal user last activity date, for fetch news. If user don't use service more then n days, don't fetch news from user channel (default 93 days (~3 month))

## Screenshots
|                                                                                            |                                                                                           |
| ------------------------------------------------------------------------------------------ | :---------------------------------------------------------------------------------------: |
| ![Welcome](https://i.postimg.cc/TPkkZRgs/2020-11-28-12-37-08-localhost-06e95107c6d2.png)   |   ![Feed](https://i.postimg.cc/nzg3h0dY/2020-11-28-12-37-48-localhost-2ef2932fe865.png)   |
| ![Favorites](https://i.postimg.cc/D0GBJLHV/2020-11-28-12-38-42-localhost-0a8741ba7b25.png) | ![Channels](https://i.postimg.cc/DZ8Chtcw/2020-11-28-12-39-08-localhost-6a9aac0b8b54.png) |

## Using in docker

**Docker compose file for swarm mode:**
```Dockerfile
version: '3.8'

services:
  host:
    image: eluki/influunt
    environment:
      - "ConnectionStrings:Mongo:ConnectionString=[MONGO DB CONNECTION STRING]"
      - "ConnectionStrings:Redis:ConnectionString=[Redis address]" # optional redis-based distributed cache connection
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
           -e "ConnectionStrings:Redis:ConnectionString=[Redis address]" \
           --restart always \
           --name influunt \
           eluki/influunt
```
