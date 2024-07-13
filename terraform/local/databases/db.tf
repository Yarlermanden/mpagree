terraform {
  required_providers {
    postgresql = {
      source  = "cyrilgdn/postgresql"
      version = "=1.21.0"
    }
  }
}

variable "databases" {
  type    = list(string)
  default = []
}

variable "owner" {
  type = string
}

resource "postgresql_database" "database" {
  for_each = toset(var.databases)
  name     = each.key
  owner    = var.owner
}