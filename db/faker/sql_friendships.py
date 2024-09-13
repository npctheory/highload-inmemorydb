import csv
import os

# Define the CSV file paths
friends_csv_file = './db/csv/fake_friendships.csv'
sql_file_path = './db/initdb/init002_friendships.sql'

# Ensure the directory exists
os.makedirs(os.path.dirname(sql_file_path), exist_ok=True)

# SQL template for connecting to the database
sql_template = """
-- Connect to the database
\\c highloadsocial;

-- Insert data
"""

# Read the friendships from the CSV file
friendships = []
with open(friends_csv_file, mode='r') as file:
    reader = csv.DictReader(file)
    for row in reader:
        friendships.append((row['user_id'], row['friend_id']))

# Write the friendships to the SQL file in batches of 5000
batch_size = 50000
total_rows = len(friendships)
num_batches = (total_rows // batch_size) + (1 if total_rows % batch_size != 0 else 0)

with open(sql_file_path, mode='w') as sql_file:
    sql_file.write(sql_template)
    
    for batch in range(num_batches):
        start_index = batch * batch_size
        end_index = start_index + batch_size
        batch_friendships = friendships[start_index:end_index]
        
        sql_file.write("INSERT INTO friendships (user_id, friend_id) VALUES\n")
        
        values = []
        for user_id, friend_id in batch_friendships:
            value = f"('{user_id}', '{friend_id}')"
            values.append(value)
        
        sql_file.write(",\n".join(values) + ";\n\n")

print(f'SQL file has been written to {sql_file_path}')
