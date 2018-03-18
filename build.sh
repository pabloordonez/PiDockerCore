#!/bin/bash
dotnet restore .
dotnet publish -c release -o ../.dist -r linux-arm
