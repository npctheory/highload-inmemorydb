import csv
import os

# Define the file paths
posts_csv_file = './db/csv/fake_posts.csv'
sql_file_path = './db/initdb/init003_posts.sql'

# Ensure the directory exists
os.makedirs(os.path.dirname(sql_file_path), exist_ok=True)

# SQL template for connecting to the database
sql_template = """
-- Connect to the database
\\c highloadsocial;

-- Insert data
"""

# Read the posts from the CSV file
posts = []
with open(posts_csv_file, mode='r') as file:
    reader = csv.DictReader(file)
    for row in reader:
        post_id = row['id']
        text = row['text'].replace("'", "''")  # Escape single quotes for SQL
        user_id = row['user_id']
        created_at = row['created_at']
        posts.append((post_id, text, user_id, created_at))

# Write the posts to the SQL file in batches of 5000
batch_size = 50000
total_rows = len(posts)
num_batches = (total_rows // batch_size) + (1 if total_rows % batch_size != 0 else 0)

with open(sql_file_path, mode='w') as sql_file:
    sql_file.write(sql_template)
    
    for batch in range(num_batches):
        start_index = batch * batch_size
        end_index = start_index + batch_size
        batch_posts = posts[start_index:end_index]
        
        sql_file.write("INSERT INTO posts (id, text, user_id, created_at) VALUES\n")
        
        values = []
        for post_id, text, user_id, created_at in batch_posts:
            value = f"('{post_id}', '{text}', '{user_id}', '{created_at}')"
            values.append(value)
        
        sql_file.write(",\n".join(values) + ";\n\n")

print(f'SQL file has been written to {sql_file_path}')
