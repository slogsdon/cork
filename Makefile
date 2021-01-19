# Project configuration
PROJECT_NAME = Cork
TEST_PROJECT = test/$(PROJECT_NAME).Test/
TEST_OPTIONS = --no-restore --no-build
NUGET_SOURCE = https://api.nuget.org/v3/index.json
PACKAGES = "$(PROJECT_NAME)" "$(PROJECT_NAME).AspNetCore" "$(PROJECT_NAME).Fable.Http"

# Needed SHELL since I'm using zsh
SHELL := /bin/bash

# COLORS
GREEN  := $(shell tput -Txterm setaf 2)
YELLOW := $(shell tput -Txterm setaf 3)
WHITE  := $(shell tput -Txterm setaf 7)
RESET  := $(shell tput -Txterm sgr0)

TARGET_MAX_CHAR_NUM=24

.PHONY: help

# Help target thanks to github.com/crifan
# See: https://gist.github.com/prwhite/8168133#gistcomment-2278355

## Show this help message
help:
	@echo ''
	@echo 'Usage:'
	@echo '  ${YELLOW}make${RESET} ${GREEN}<target>${RESET}'
	@echo ''
	@echo 'Targets:'
	@awk '/^[a-zA-Z\-\_0-9]+:/ { \
		helpMessage = match(lastLine, /^## (.*)/); \
		if (helpMessage) { \
			helpCommand = substr($$1, 0, index($$1, ":")-1); \
			helpMessage = substr(lastLine, RSTART + 3, RLENGTH); \
			printf "  ${YELLOW}%-$(TARGET_MAX_CHAR_NUM)s${RESET} ${GREEN}%s${RESET}\n", helpCommand, helpMessage; \
		} \
	} \
	{ lastLine = $$0 }' $(MAKEFILE_LIST)

## Alias for `make clean restore build tests`
all: clean restore build tests

## Build package
build:
	@dotnet build --no-restore --verbosity=minimal

## Clean build artifacts
clean:
	@rm -rf src/*/bin src/*/obj test/*/bin test/*/obj examples/*/bin examples/*/obj

## Lint package source files
lint:
	@dotnet dotnet-fsharplint lint $(PROJECT_NAME).sln

## Publish packages to NuGet
nuget-publish: clean
	@for package in $(PACKAGES); do \
		echo  $$package; \
		dotnet pack src/$$package/ ; \
		dotnet nuget push "src/$$package/bin/Debug/$$package.*.nupkg" --api-key $(NUGET_API_KEY) --source $(NUGET_SOURCE) ; \
	done

## Restore package dependencies
restore:
	@dotnet restore --verbosity=minimal
	@npm install

## Run the ASP.NET Core example
run-example-aspnetcore:
	@dotnet run $(TEST_OPTIONS) --project examples/$(PROJECT_NAME).Example.AspNetCore/$(PROJECT_NAME).Example.AspNetCore.fsproj

## Run the Fable / Node.JS example
run-example-fable-http:
	@node examples/$(PROJECT_NAME).Example.Fable.Http/bin/bundle.js

## Run package tests
tests:
	@dotnet test $(TEST_OPTIONS) $(TEST_PROJECT)
	@cd test/$(PROJECT_NAME).Fable.Http.Test && npx mocha bin
