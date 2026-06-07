#!/bin/bash

name=$1
project=$2
context=$3

# add usage validation
if [ -z "$name" ] || [ -z "$project" ] || [ -z "$context" ]; then
    echo "Usage: $0 <migration_name> <project_name> <context_name>"
    exit 1
fi

dotnet ef migrations add $name --startup-project ./src/CRM --project ./src/$project -o Data/Migrations --context $context