
data "azurerm_subnet" "pg" {
  name                 = var.pg_subnet_name
  resource_group_name  = var.pg_subnet_resource_group_name
  virtual_network_name = var.virtual_network_name
}

data "azurerm_private_dns_zone" "priv" {
  name = var.private_dns_zone_name
}

resource "azurerm_postgresql_flexible_server" "postgres" {
  name                   = var.postgres_name
  resource_group_name    = azurerm_resource_group.rg.name
  location               = azurerm_resource_group.rg.location
  version                = var.postgres_version
  delegated_subnet_id    = data.azurerm_subnet.pg.id
  private_dns_zone_id    = data.azurerm_private_dns_zone.priv.id
  administrator_login    = var.admin_user
  administrator_password = var.admin_password
  zone                   = var.zone

  storage_mb = var.storage
  sku_name   = var.sku_name

  tags = local.tags
}

resource "azurerm_postgresql_flexible_server_database" "db" {
  for_each  = toset(var.databases)
  name      = each.key
  server_id = azurerm_postgresql_flexible_server.postgres.id
}

data "dns_cname_record_set" "db_lookup" {
  host = azurerm_postgresql_flexible_server.postgres.fqdn
}

data "azurerm_private_dns_a_record" "dns_record" {
  name                = replace(data.dns_cname_record_set.db_lookup.cname, ".${var.private_dns_zone_name}.", "")
  zone_name           = var.private_dns_zone_name
  resource_group_name = data.azurerm_private_dns_zone.priv.resource_group_name
}

output "db_admin_username" {
  value = azurerm_postgresql_flexible_server.postgres.administrator_login
}

output "db_admin_password" {
  value = azurerm_postgresql_flexible_server.postgres.administrator_password
}

output "db_ip" {
  value = one(data.azurerm_private_dns_a_record.dns_record.records)
}