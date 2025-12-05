#!/bin/bash

echo "================================================"
echo "  MyFlix - Personal Netflix Clone Builder"
echo "================================================"
echo ""

echo "[1/3] Restoring NuGet packages..."
dotnet restore
if [ $? -ne 0 ]; then
    echo ""
    echo "Build Failed! Please check the error messages above."
    exit 1
fi

echo ""
echo "[2/3] Building the application..."
dotnet build -c Release
if [ $? -ne 0 ]; then
    echo ""
    echo "Build Failed! Please check the error messages above."
    exit 1
fi

echo ""
echo "[3/3] Publishing single-file executable..."
# Detect OS
if [[ "$OSTYPE" == "linux-gnu"* ]]; then
    dotnet publish -c Release -r linux-x64 --self-contained -p:PublishSingleFile=true -o ./publish
elif [[ "$OSTYPE" == "darwin"* ]]; then
    dotnet publish -c Release -r osx-x64 --self-contained -p:PublishSingleFile=true -o ./publish
else
    dotnet publish -c Release -r linux-x64 --self-contained -p:PublishSingleFile=true -o ./publish
fi

if [ $? -ne 0 ]; then
    echo ""
    echo "Build Failed! Please check the error messages above."
    exit 1
fi

echo ""
echo "================================================"
echo "  Build Successful!"
echo "================================================"
echo ""
echo "Your executable is located at:"
echo "  $(pwd)/publish/MyNetflixClone"
echo ""
echo "To distribute:"
echo "  1. Copy the executable from publish folder"
echo "  2. Copy the Media folder (with your movies)"
echo "  3. Copy movies.db (if you have an existing library)"
echo "  4. Copy appsettings.json"
echo ""
