import os

REPLACEMENTS = [
    ('using Npgsql;', 'using Microsoft.Data.Sqlite;'),
    ('NpgsqlConnection', 'SqliteConnection'),
    ('NpgsqlCommand', 'SqliteCommand'),
    ('NpgsqlParameter', 'SqliteParameter'),
]

files_to_patch = [
    r"c:\Users\Surya Narayana\Desktop\visitor management\DAL\UserDAL.cs",
    r"c:\Users\Surya Narayana\Desktop\visitor management\DAL\FileUploadDAL.cs",
    r"c:\Users\Surya Narayana\Desktop\visitor management\DAL\VisitorDAL.cs",
    r"c:\Users\Surya Narayana\Desktop\visitor management\DAL\DBHelper.cs"
]

for file_path in files_to_patch:
    with open(file_path, 'r', encoding='utf-8') as f:
        content = f.read()
    
    for old, new in REPLACEMENTS:
        content = content.replace(old, new)
        
    with open(file_path, 'w', encoding='utf-8') as f:
        f.write(content)

print("DAL files updated to replace Npgsql with Microsoft.Data.Sqlite")
