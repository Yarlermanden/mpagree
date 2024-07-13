terraform {
  required_providers {
    postgresql = {
      source  = "cyrilgdn/postgresql"
      version = "=1.21.0"
    }
  }
}

variable "database_name" {
  type     = string
  nullable = false
}

variable "database_owner" {
  type     = string
  nullable = false
}

variable "users_can_create_database" {
  type     = bool
  default  = false
  nullable = false
}

variable "users" {
  type = list(object({
    name     = string
    password = string
  }))
  default  = []
  nullable = false
}