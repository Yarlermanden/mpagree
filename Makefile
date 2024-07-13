GIT_HASH ?= $(shell git log --format="%h" -n 1)
VERSION?=${GIT_HASH}
IMAGE_NAME=mpagree
IMAGE_NAME_TEST?=$(IMAGE_NAME)-$(VERSION)-test
DOCKER_IMAGE=coihub.azurecr.io/$(IMAGE_NAME)

.PHONY: build
build:
	docker build -f Dockerfile . -t $(DOCKER_IMAGE):$(VERSION)

.PHONY: push
push: 
	docker push $(DOCKER_IMAGE):$(VERSION)

remove-cert:
	rm ${HOME}/.aspnet/https/ci-id.pfx && rm ${HOME}/.aspnet/https/ci-account.pfx

refresh-cert: remove-cert generate-cert

generate-cert: generate-id-cert generate-acc-cert

generate-id-cert:
	test -s ${HOME}/.aspnet/https/ci-id.pfx || dotnet dev-certs https -ep ${HOME}/.aspnet/https/ci-id.pfx -p pw

generate-acc-cert:
	test -s ${HOME}/.aspnet/https/ci-account.pfx || dotnet dev-certs https -ep ${HOME}/.aspnet/https/ci-account.pfx -p pw

setup-local: generate-cert
	(cd terraform/local; make setup)

run-stack: generate-cert
	STRIPE_API_KEY=$(STRIPE_API_KEY) docker-compose -p platform -f docker-compose.local.yml up --build

test: test-setup build-test run-test

test-setup: testresults remove-testresults

testresults:
	mkdir testresults

remove-testresults:
	-rm testresults/*

build-test:
	docker compose -f docker-compose.test.yml down --remove-orphans
	docker compose -f docker-compose.test.yml build

run-test:
	-docker compose -f docker-compose.test.yml run --name platform-${VERSION}-t test
	docker cp platform-${VERSION}-t:/tests/results.xml ./testresults/platform-testresults.xml
	docker compose -f docker-compose.test.yml down --remove-orphans