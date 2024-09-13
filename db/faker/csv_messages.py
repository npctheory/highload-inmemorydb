import csv
import uuid
import random
from datetime import datetime, timedelta

# File paths
users_csv_file = './db/csv/fake_users.csv'
friends_csv_file = './db/csv/fake_friendships.csv'
messages_csv_file = './db/csv/fake_messages.csv'

# Date range for random timestamps
start_date = datetime(2024, 4, 25)
end_date = datetime(2024, 10, 30)

def random_date(start, end):
    """Generate a random datetime between `start` and `end`."""
    time_between_dates = end - start
    random_number_of_days = random.randrange(time_between_dates.days)
    random_date = start + timedelta(days=random_number_of_days)
    return random_date

# Load users
user_ids = []
with open(users_csv_file, mode='r') as file:
    reader = csv.DictReader(file)
    for row in reader:
        user_ids.append(row['id'])

# Load friendships
friendships = {}
with open(friends_csv_file, mode='r') as file:
    reader = csv.DictReader(file)
    for row in reader:
        user_id = row['user_id']
        friend_id = row['friend_id']
        if user_id not in friendships:
            friendships[user_id] = []
        friendships[user_id].append(friend_id)

# Prepare messages
messages = []

# Generate messages for each user
for user_id in user_ids:
    if user_id in friendships and friendships[user_id]:
        # Get the first friend
        first_friend = friendships[user_id][0]

        # Generate random timestamps
        timestamp_sent = random_date(start_date, end_date).isoformat()
        timestamp_received = random_date(start_date, end_date).isoformat()

        # Message sent to the first friend
        messages.append({
            'id': str(uuid.uuid4()),
            'sender_id': user_id,
            'receiver_id': first_friend,
            'text': 'Hello',
            'is_read': 'FALSE',
            'timestamp': timestamp_sent
        })

        # Message received from the first friend
        messages.append({
            'id': str(uuid.uuid4()),
            'sender_id': first_friend,
            'receiver_id': user_id,
            'text': 'Hi',
            'is_read': 'FALSE',
            'timestamp': timestamp_received
        })

    # Messages involving LadyGaga
    lady_gaga_id = 'LadyGaga'

    # Generate random timestamps
    timestamp_sent_to_ladygaga = random_date(start_date, end_date).isoformat()
    timestamp_received_from_ladygaga = random_date(start_date, end_date).isoformat()

    # Message sent to LadyGaga
    messages.append({
        'id': str(uuid.uuid4()),
        'sender_id': user_id,
        'receiver_id': lady_gaga_id,
        'text': 'Hello',
        'is_read': 'FALSE',
        'timestamp': timestamp_sent_to_ladygaga
    })

    # Message received from LadyGaga
    messages.append({
        'id': str(uuid.uuid4()),
        'sender_id': lady_gaga_id,
        'receiver_id': user_id,
        'text': 'Hi',
        'is_read': 'FALSE',
        'timestamp': timestamp_received_from_ladygaga
    })

# Write messages to CSV
with open(messages_csv_file, mode='w', newline='') as file:
    writer = csv.DictWriter(file, fieldnames=['id', 'sender_id', 'receiver_id', 'text', 'is_read', 'timestamp'])
    writer.writeheader()
    writer.writerows(messages)

print(f'Fake messages have been written to {messages_csv_file}')
