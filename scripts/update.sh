#!/bin/sh

sudo service minerva.web stop
sudo service minerva.discord stop

cd /home/orfasanti/minerva
sudo git pull origin master

cd /home/orfasanti/minerva/src/Minerva.Web
sudo dotnet publish -c Release -o /var/aspnetcore/Minerva.Web

cd /home/orfasanti/minerva/src/Minerva.Discord
sudo dotnet publish -c Release -o /var/dotnetcore/Minerva.Discord

sudo service minerva.web start
sudo service minerva.discord start
