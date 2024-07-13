do $$ declare
    r record;
begin
    for r in (select schemaname, tablename from pg_tables where schemaname not in ('pg_catalog', 'information_schema', 'public')) loop
        execute 'drop table if exists ' || quote_ident(r.schemaname) || '.' || quote_ident(r.tablename) || ' cascade';
    end loop;
end $$;