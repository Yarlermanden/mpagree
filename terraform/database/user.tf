resource "time_sleep" "wait_10_seconds" {
  create_duration = "10s"
}

resource "postgresql_role" "user" {
  for_each = { for u in var.users : u.name => u }

  name     = each.value.name
  password = each.value.password

  login              = true
  encrypted_password = true

  create_database = var.users_can_create_database

  depends_on = [
    time_sleep.wait_10_seconds
  ]

  lifecycle {
    ignore_changes = [
      password
    ]
  }
}

resource "postgresql_schema" "schema" {
  for_each = { for u in var.users : u.name => u }
  name     = each.value.name
  owner    = each.value.name

  depends_on = [postgresql_role.user]
}

resource "postgresql_schema" "my_schema" {
  name  = "api"
  owner = var.database_owner

  depends_on = [
    time_sleep.wait_10_seconds
  ]
}



resource "postgresql_grant" "schema_usage" {
  for_each = { for u in var.users : u.name => u }

  database    = var.database_name
  role        = each.value.name
  schema      = postgresql_schema.my_schema.name
  object_type = "schema"
  privileges  = ["USAGE", "CREATE"]

  depends_on = [postgresql_role.user]
}

resource "postgresql_default_privileges" "tables" {
  for_each = { for u in var.users : u.name => u }

  role     = "public"
  database = var.database_name
  schema   = postgresql_schema.my_schema.name

  owner       = each.value.name
  object_type = "table"
  privileges  = ["SELECT"]

  depends_on = [postgresql_role.user]
}

resource "postgresql_default_privileges" "sequences" {
  for_each = { for u in var.users : u.name => u }

  role     = "public"
  database = var.database_name
  schema   = postgresql_schema.my_schema.name

  owner       = each.value.name
  object_type = "sequence"
  privileges  = ["SELECT"]

  depends_on = [postgresql_role.user]
}

resource "postgresql_default_privileges" "functions" {
  for_each = { for u in var.users : u.name => u }

  role     = "public"
  database = var.database_name
  schema   = postgresql_schema.my_schema.name

  owner       = each.value.name
  object_type = "function"
  privileges  = ["EXECUTE"]

  depends_on = [postgresql_role.user]
}