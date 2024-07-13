setup:
	(docker container inspect pg && docker start pg) || true
	terraform init
	terraform plan -out local.plan
	terraform apply -parallelism=1 local.plan

wipe-db:
	PGPASSWORD=dpadmin psql -h localhost -U dpadmin -d dp -f wipe.sql