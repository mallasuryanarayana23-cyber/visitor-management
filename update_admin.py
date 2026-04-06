import hashlib
import os
import sqlite3
import re

# 1. Compute hash
password = b"Surya@189489"
hash_hex = hashlib.sha256(password).hexdigest()

admin_no = "8142027323"

def replace_in_file(filepath):
    if not os.path.exists(filepath): return
    with open(filepath, 'r', encoding='utf-8') as f:
        content = f.read()

    # Find the INSERT for ADMIN_01 and replace
    new_content = re.sub(r"VALUES \('ADMIN_01', '[a-f0-9]+',", f"VALUES ('{admin_no}', '{hash_hex}',", content)
    
    with open(filepath, 'w', encoding='utf-8') as f:
        f.write(new_content)

replace_in_file(r"c:\Users\Surya Narayana\Desktop\visitor management\Database\Oracle_Schema.sql")
replace_in_file(r"c:\Users\Surya Narayana\Desktop\visitor management\Database\Postgres_Schema.sql")

# Also update the sqlite db immediately if it exists, so the user can login
db_path = r"c:\Users\Surya Narayana\Desktop\visitor management\Data\VisitorManagement.db"
if os.path.exists(db_path):
    try:
        conn = sqlite3.connect(db_path)
        cur = conn.cursor()
        
        # Check if table VMS_USERS exists
        cur.execute("SELECT name FROM sqlite_master WHERE type='table' AND name='VMS_USERS'")
        if cur.fetchone():
            cur.execute("UPDATE VMS_USERS SET USERNAME=?, PASSWORD_HASH=? WHERE ROLE='ADMIN'", (admin_no, hash_hex))
            if cur.rowcount == 0:
                cur.execute("INSERT INTO VMS_USERS (USERNAME, PASSWORD_HASH, FULL_NAME, ROLE, IS_ACTIVE) VALUES (?, ?, 'System Administrator', 'ADMIN', 1)", (admin_no, hash_hex))
            conn.commit()
    except Exception as e:
        print("Sqlite DB error:", e)

print(f"Updated Admin to {admin_no} with new hashed password in schemas and database.")
