# Project configuration
PROJECT_NAME = Spile
TEST_PROJECT = test/$(PROJECT_NAME).Test/
TEST_OPTIONS = --no-restore --no-build

# Needed SHELL since I'm using zsh
SHELL := /bin/bash

# COLORS
GREEN  := $(shell tput -Txterm setaf 2)
YELLOW := $(shell tput -Txterm setaf 3)
WHITE  := $(shell tput -Txterm setaf 7)
RESET  := $(shell tput -Txterm sgr0)

TARGET_MAX_CHAR_NUM=20

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
	@cd test/Spile.Fable.Http.Test && dotnet fable webpack --port free
	@cd examples/Spile.Example.Fable.Http && dotnet fable webpack --port free

## Clean build artifacts
clean:
	@rm -rf **/*/bin **/*/obj

## Restore package dependencies
restore:
	@mono .paket/paket.exe update
	@dotnet restore --verbosity=minimal
	@yarn install

run-aspnetcore:
	@dotnet run $(TEST_OPTIONS) --project examples/Spile.Example.AspNetCore/Spile.Example.AspNetCore.fsproj

run-fable-http:
	@node examples/Spile.Example.Fable.Http/bin/bundle.js

## Run package tests
tests:
	@dotnet test $(TEST_OPTIONS) $(TEST_PROJECT)
	@cd test/Spile.Fable.Http.Test && yarn run mocha bin
