variable "environment" {
  type = string
}

variable "location" {
  description = "Specifies the azure location to deploy into"
  type        = string
}

variable "postgres_name" {
  description = "Specifices the name of the postgres server"
  type        = string
}

variable "postgres_version" {
  description = "Specifices the version of the postgres server"
  type        = string
  default     = "14"
}

variable "admin_user" {
  description = "Admin user"
  type        = string
  default     = "psqladmin"
}

variable "admin_password" {
  description = "Password for the admin user"
  type        = string
  sensitive   = true
}

variable "private_dns_zone_name" {
  description = "Private DNS zone name"
  type        = string
}

variable "virtual_network_name" {
  description = "Name of virtual network"
  type        = string
}

variable "zone" {
  description = "Specifices the zone of the postgres server"
  type        = string
}

variable "pg_subnet_name" {
  description = "Specifices the name subnet for the postgres server"
  type        = string
}

variable "pg_subnet_resource_group_name" {
  description = "Resource group name for pg subnet"
  type        = string
}

variable "sku_name" {
  description = "Sku name of the postgres server"
  type        = string
}

variable "storage" {
  description = "Storage in mb"
  type        = number
  default     = 32768
}

variable "resource_group_name" {
  description = "(Required) Specifies the name of the resource group to create"
  type        = string
}

variable "tags" {
  description = "Tags to apply on resources"
  default     = {}
  type        = map(string)
}

variable "databases" {
  type = list(string)
}