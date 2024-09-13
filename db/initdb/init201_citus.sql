\c highloadsocial;
CREATE EXTENSION citus;
SELECT master_add_node('pg_worker1', 5432);
SELECT master_add_node('pg_worker2', 5432);

SELECT create_reference_table('users');
SELECT create_distributed_table('dialog_messages_sent', 'sender_id');
SELECT create_distributed_table('dialog_messages_received', 'receiver_id');