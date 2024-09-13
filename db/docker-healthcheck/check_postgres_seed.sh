#!/bin/bash

# Check if PostgreSQL is ready to accept connections
pg_isready -d $POSTGRES_DB -U $POSTGRES_USER -h localhost

if [ $? -ne 0 ]; then
  echo "PostgreSQL is not ready"
  exit 1
fi

# Verify connection to the specific database
psql -d $POSTGRES_DB -U $POSTGRES_USER -h localhost -c "SELECT 1;" > /dev/null 2>&1

if [ $? -ne 0 ]; then
  echo "Unable to connect to database $POSTGRES_DB"
  exit 1
fi

# Check if the seed data is loaded
if psql -d $POSTGRES_DB -U $POSTGRES_USER -h localhost -tAc "SELECT seed_completed FROM seed_status WHERE seed_completed = TRUE LIMIT 1;" | grep -q 't'; then
  echo "Seed data is loaded"
  exit 0
else
  echo "Seed data not loaded yet"
  exit 1
fi
