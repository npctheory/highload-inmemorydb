import csv
import hashlib
import os
import random
from faker import Faker

fake = Faker()
num_users = 5000
csv_directory = './db/csv/'
csv_file = os.path.join(csv_directory, 'fake_users.csv')
os.makedirs(csv_directory, exist_ok=True)
password = "password"
def generate_password_hash(password, salt):
    hash_object = hashlib.sha256((password + salt).encode())
    return hash_object.hexdigest()

def generate_human_readable_id():
    word1 = fake.word().capitalize()
    word2 = fake.word().capitalize()
    number = random.randint(1000, 9999)
    return f"{word1}{word2}{number}"

def add_special_users(writer):
    special_users = [
        ('Admin', 'password', 'Admin', 'Admin', '1970-01-01', 'Administration', 'Moscow'),
        ('User', 'password', 'User', 'User', '1970-01-01', 'Work', 'Moscow'),
        ('LadyGaga', 'password', 'Lady', 'Gaga', '1986-03-28', 'Music', 'New York City')
    ]

    for user in special_users:
        user_id, raw_password, first_name, second_name, birthdate, biography, city = user
        salt = os.urandom(16).hex()
        password_hash = generate_password_hash(raw_password, salt)
        writer.writerow([user_id, f"{salt}:{password_hash}", first_name, second_name, birthdate, biography, city])

with open(csv_file, mode='w', newline='') as file:
    writer = csv.writer(file)
    writer.writerow(['id', 'password_hash', 'first_name', 'second_name', 'birthdate', 'biography', 'city'])

    add_special_users(writer)

    for _ in range(num_users):
        user_id = generate_human_readable_id()
        first_name = fake.first_name()
        second_name = fake.last_name()
        birthdate = fake.date_of_birth(minimum_age=18, maximum_age=90).isoformat()
        biography = fake.word()
        city = fake.city()

        salt = os.urandom(16).hex()
        password_hash = generate_password_hash(password, salt)
        writer.writerow([user_id, f"{salt}:{password_hash}", first_name, second_name, birthdate, biography, city])

print(f'{num_users} fake users have been written to {csv_file}')
