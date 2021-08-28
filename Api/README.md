#.NET 5 REST API

This is a walk-along project for a course on building web APIs with .NET5, Mongo, Docker.

##How To Run

Build docker image of a project

`docker build -t catalog:v1 .`

Create docker network for project and mongo containers

`docker network create net5tutorial`

Run mongo container and provide username, password and specify volume to store data

`docker run -d --rm --name mongo -p 27017:27017 -v mongodbdata:/data/db -e MONGO_INITDB_ROOT_USERNAME=mongoadmin -e MONGO_INITDB_ROOT_PASSWORD=Pass#word1 --network=net5tutorial mongo`

Run project container and indicate environment variables to connect to Mongo container

`docker run -d --rm -p 8080:80 -e MongoDbSettings:Host=mongo -e MongoDbSettings:Password=Pass#word1 --network=net5tutorial catalog:v1`
