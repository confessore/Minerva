#!/bin/sh

sudo systemctl stop minerva.web.service
sudo systemctl stop minerva.discord.service

sudo systemctl disable minerva.web.service
sudo systemctl disable minerva.discord.service

read -p "Discord App Id: " id
read -p "Discord App Secret: " secret
read -p "Minerva Discord Key: " minerva
read -p "Riot API Key: " riot

sudo cp ./services/minerva.web.service ./services/minerva.web.service.backup
sudo cp ./services/minerva.discord.service ./services/minerva.discord.service.backup

sudo sed -i '/AppId=/s/$/'"$id"'/' ./services/minerva.discord.service.backup
sudo sed -i '/AppSecret=/s/$/'"$secret"'/' ./services/minerva.discord.service.backup
sudo sed -i '/Minerva=/s/$/'"$minerva"'/' ./services/minerva.discord.service.backup
sudo sed -i '/Riot=/s/$/'"$riot"'/' ./services/minerva.discord.service.backup

sudo mv ./services/minerva.web.service.backup /etc/systemd/system/minerva.web.service
sudo mv ./services/minerva.discord.service.backup /etc/systemd/system/minerva.discord.service

sudo systemctl enable minerva.web.service
sudo systemctl enable minerva.discord.service

sudo systemctl start minerva.web.service
sudo systemctl start minerva.discord.service
