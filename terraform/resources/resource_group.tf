locals {
  tags = merge(var.tags, {
    env            = var.environment
    service_domain = "mpagree"
  })
}

resource "azurerm_resource_group" "rg" {
  name     = var.resource_group_name
  location = var.location

  tags = local.tags
}