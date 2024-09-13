import csv
import os

input_csv_file = './db/csv/fake_messages.csv'
output_sql_file = './db/initdb/init004_messages.sql'

def generate_sql_insert_statements():
    with open(input_csv_file, mode='r') as file:
        reader = csv.DictReader(file)
        messages = list(reader)

    values_sent = []
    values_received = []

    for message in messages:
        id = message['id']
        sender_id = message['sender_id']
        receiver_id = message['receiver_id']
        text = message['text']
        is_read = message['is_read'].upper() == 'TRUE'
        timestamp = message['timestamp']

        values_sent.append(
            f"('{id}', '{sender_id}', '{receiver_id}', '{text}', {is_read}, '{timestamp}')"
        )
        values_received.append(
            f"('{id}', '{sender_id}', '{receiver_id}', '{text}', {is_read}, '{timestamp}')"
        )

    with open(output_sql_file, mode='w') as file:
        file.write("\\c highloadsocial;\n")
        
        # One large INSERT statement for dialog_messages_sent
        file.write(f"INSERT INTO dialog_messages_sent (id, sender_id, receiver_id, text, is_read, timestamp) VALUES\n")
        file.write(",\n".join(values_sent) + ";\n")
        
        # One large INSERT statement for dialog_messages_received
        file.write(f"INSERT INTO dialog_messages_received (id, sender_id, receiver_id, text, is_read, timestamp) VALUES\n")
        file.write(",\n".join(values_received) + ";\n")

    print(f'SQL insert statements have been written to {output_sql_file}')

if __name__ == "__main__":
    generate_sql_insert_statements()