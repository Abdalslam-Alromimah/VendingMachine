-- Initialize the database
CREATE DATABASE vendingmachine;

-- Connect to the vendingmachine database
\c vendingmachine;

-- Create extensions if needed
CREATE EXTENSION IF NOT EXISTS "uuid-ossp";

-- The Entity Framework migrations will handle table creation