ENV ?= stage

.PHONY: fmt
fmt: ## Format terraform files
	terraform fmt --recursive

.PHONY: init
init: ## Initialize terraform
	terraform -chdir=$(ENV) init

.PHONY: plan
plan: ## Plan
	terraform -chdir=$(ENV) plan -parallelism=4 -out $(ENV).plan

.PHONY: apply
apply: ## Apply previous plan
	terraform -chdir=$(ENV) apply -parallelism=4 $(ENV).plan

.import-db-users: import-db-users
import-db-users: ## Import DB users
	terraform -chdir=$(ENV) import 'module.database.postgresql_role.user["mpagree"]' mpagree

# create-key-file-stage:
# 	az storage blob upload --subscription="2fc0babb-4fb1-450b-b764-e9225be91d69" --account-name="ckinstageplatform" --container-name="id-container" --file=empty.txt --name=data-protection-key

# create-key-file-prod:
# 	az storage blob upload --subscription="b4e2745a-dc96-43d6-887f-2d554676dc2a" --account-name="ckinprodplatform" --container-name="id-container" --file=empty.txt --name=data-protection-key

.PHONY: help
help: ## Display help
I	@grep -E '^[.a-zA-Z_-]+:.*?## .*$$' $(MAKEFILE_LIST) | sort | awk 'BEGIN {FS = ":.*?## "}; {printf "\033[36m%-30s\033[0m %s\n", $$1, $$2}'