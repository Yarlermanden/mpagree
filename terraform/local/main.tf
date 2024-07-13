terraform {
  required_providers {
    docker = {
      source  = "kreuzwerker/docker"
      version = "=3.0.2"
    }
    postgresql = {
      source  = "cyrilgdn/postgresql"
      version = "=1.21.0"
    }
  }

  backend "local" {
    path = "local.tfstate"
  }
}

resource "docker_network" "network" {
  name   = "dp-local"
  driver = "bridge"
}

resource "docker_image" "pg" {
  name         = "postgres:14.7"
  keep_locally = true
}

resource "docker_container" "pg" {
  name  = "pg"
  image = docker_image.pg.image_id

  env = [
    "POSTGRES_USER=dpadmin",
    "POSTGRES_PASSWORD=dpadmin",
    "POSTGRES_DB=dp",
  ]

  ports {
    internal = "5432"
    external = "5432"
  }

  networks_advanced {
    name = docker_network.network.id
  }
}

provider "postgresql" {
  host     = "localhost"
  port     = 5432
  database = "dp"
  username = "dpadmin"
  password = "dpadmin"
  sslmode  = "disable"
}

module "database" {
  source         = "../database"
  database_name  = "dp"
  database_owner = "dpadmin"
  users = [
    {
      name     = "mpagree"
      password = "mpagree"
    }
  ]
  users_can_create_database = true

  depends_on = [
    docker_container.pg
  ]
}