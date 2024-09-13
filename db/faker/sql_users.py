import csv
import os

# Define the CSV file path
csv_file = './db/csv/fake_users.csv'
# Define the SQL file path
sql_file = './db/initdb/init001_users.sql'

# SQL template for connecting to the database
sql_template = """
-- Connect to the database
\\c highloadsocial;

-- Insert data
"""

# Ensure the directory exists
os.makedirs(os.path.dirname(sql_file), exist_ok=True)

# Read the CSV file and generate the SQL file
with open(csv_file, mode='r') as file:
    reader = csv.reader(file)
    header = next(reader)  # Skip the header row
    
    rows = list(reader)
    total_rows = len(rows)
    batch_size = 1000
    num_batches = (total_rows // batch_size) + (1 if total_rows % batch_size != 0 else 0)
    
    with open(sql_file, mode='w') as sql_file:
        sql_file.write(sql_template)
        
        for batch in range(num_batches):
            start_index = batch * batch_size
            end_index = start_index + batch_size
            batch_rows = rows[start_index:end_index]
            
            sql_file.write("INSERT INTO users (id, password_hash, first_name, second_name, birthdate, biography, city) VALUES\n")
            
            values = []
            for row in batch_rows:
                id, password_hash, first_name, second_name, birthdate, biography, city = row
                value = f"('{id}', '{password_hash}', '{first_name}', '{second_name}', '{birthdate}', '{biography}', '{city}')"
                values.append(value)
            
            sql_file.write(",\n".join(values) + ";\n\n")

print(f'SQL file has been written to {sql_file.name}')
