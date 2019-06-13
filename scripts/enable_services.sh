#!/bin/sh

sudo systemctl enable minerva.web.service
sudo systemctl enable minerva.discord.service

sudo systemctl start minerva.web.service
sudo systemctl start minerva.discord.service