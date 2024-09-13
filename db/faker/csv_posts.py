import csv
import os
import random
import string
from faker import Faker
from datetime import datetime

# Initialize Faker library
fake = Faker()

# Define the file paths
users_csv_file = './db/csv/fake_users.csv'
posts_csv_file = './db/csv/fake_posts.csv'

# Ensure the directory exists
os.makedirs(os.path.dirname(posts_csv_file), exist_ok=True)

# Read the user IDs from the CSV file
user_ids = []
with open(users_csv_file, mode='r') as file:
    reader = csv.DictReader(file)
    for row in reader:
        user_ids.append(row['id'])

# Function to generate random creation timestamp from 2007 onwards
def generate_random_timestamp():
    start_date = datetime(2007, 1, 1)
    end_date = datetime.now()
    random_date = fake.date_time_between(start_date=start_date, end_date=end_date)
    return random_date.isoformat()

# Function to generate text without punctuation (other than periods)
def generate_clean_text():
    text = fake.sentence(nb_words=10, variable_nb_words=True)
    # Remove all punctuation except for periods
    text = text.translate(str.maketrans('', '', string.punctuation.replace('.', '')))
    return text.strip()

# Generate fake posts
posts = []
for user_id in user_ids:
    for _ in range(50):  # Each user gets 20 posts
        post_id = fake.uuid4()
        post_text = generate_clean_text()  # Generate clean text without unwanted punctuation
        created_at = generate_random_timestamp()
        posts.append([post_id, post_text, user_id, created_at])

# Write the posts to the CSV file
with open(posts_csv_file, mode='w', newline='') as file:
    writer = csv.writer(file, quoting=csv.QUOTE_MINIMAL)
    # Write the header
    writer.writerow(['id', 'text', 'user_id', 'created_at'])
    # Write the posts data
    writer.writerows(posts)

print(f'Fake posts have been written to {posts_csv_file}')
