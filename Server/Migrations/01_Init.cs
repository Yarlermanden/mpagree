using Dapper;
using SimpleMigrations;

namespace DataSources.Migrations;

[Migration(1, "Create base tables")]
public class InitMigration : Migration
{

	protected override void Up()
	{
		Connection.Execute(@"
			create table player (
				id bigserial not null primary key,
				username text not null,
				ip text not null
			);

			create table event (
				id bigserial not null primary key,
				player_id bigint not null, 
				event_type smallint not null,
				queued_time timestamp with time zone not null default current_timestamp,
				requested_time timestamp with time zone not null default current_timestamp,
				data bytea not null
			);
		");
	}

	protected override void Down()
	{
		Connection.Execute(@"
			drop table player;
			drop table event;
		");
	}
}