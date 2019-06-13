#!/bin/sh

sudo systemctl stop minerva.web.service
sudo systemctl stop minerva.discord.service

sudo systemctl disable minerva.web.service
sudo systemctl disable minerva.discord.service