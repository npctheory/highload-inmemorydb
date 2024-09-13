import csv
import os
import random

users_csv_file = './db/csv/fake_users.csv'
friends_csv_file = './db/csv/fake_friendships.csv'

os.makedirs(os.path.dirname(friends_csv_file), exist_ok=True)

user_ids = []
with open(users_csv_file, mode='r') as file:
    reader = csv.DictReader(file)
    for row in reader:
        user_ids.append(row['id'])

ladygaga_id = 'LadyGaga'

friendships = []
for user_id in user_ids:
    if user_id == ladygaga_id:
        continue
    
    friends = random.sample([uid for uid in user_ids if uid != user_id and uid != ladygaga_id], 200)
    
    friendships.extend([(user_id, friend_id) for friend_id in friends])

for user_id in user_ids:
    if user_id != ladygaga_id:
        friendships.append((user_id, ladygaga_id))

with open(friends_csv_file, mode='w', newline='') as file:
    writer = csv.writer(file)
    writer.writerow(['user_id', 'friend_id'])
    writer.writerows(friendships)

print(f'Fake friendships have been written to {friends_csv_file}')
