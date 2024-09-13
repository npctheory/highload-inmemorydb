CREATE USER docker;
CREATE DATABASE highloadsocial;
GRANT ALL PRIVILEGES ON DATABASE highloadsocial TO docker;
ALTER SYSTEM SET wal_level = 'logical';
SELECT pg_reload_conf();

\c highloadsocial;

CREATE EXTENSION citus;

\c postgres;

CREATE TABLE IF NOT EXISTS seed_status (
    id SERIAL PRIMARY KEY,
    seed_completed BOOLEAN NOT NULL,
    completed_at TIMESTAMP DEFAULT CURRENT_TIMESTAMP
);

INSERT INTO seed_status (seed_completed) VALUES (TRUE);
