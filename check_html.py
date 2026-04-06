import re

html = open('preview_admin_users.html', 'r', encoding='utf-8').read()

checks = [
    ('Add User button handler',    r'addUserBtn'),
    ('Edit button handler',        r'edit-btn'),
    ('Save button handler',        r'saveBtn'),
    ('Toggle status handler',      r'toggle-btn'),
    ('Reset PW button handler',    r'reset-btn'),
    ('Confirm reset handler',      r'confirmReset'),
    ('Live search',                r'userSearch'),
    ('Name validation',            r'errName'),
    ('Email validation',           r'errEmail'),
    ('Role validation',            r'errRole'),
    ('Password validation',        r'errPw'),
    ('Bootstrap Modal',            r'bootstrap\.Modal'),
    ('SweetAlert2',                r'Swal\.fire'),
    ('Toastr',                     r'toastr\.'),
    ('Row prepend on Add',         r'prepend'),
    ('Row update on Edit',         r'roleBadges'),
    ('Spinner on save',            r'spinner-border'),
    ('CDN jQuery',                 r'jquery.*min\.js'),
    ('CDN Bootstrap JS',           r'bootstrap.*bundle'),
    ('CDN SweetAlert2',            r'sweetalert2'),
    ('CDN Toastr',                 r'toastr.*min\.js'),
]

passed = 0
failed = 0
for name, pattern in checks:
    if re.search(pattern, html, re.IGNORECASE | re.DOTALL):
        print(f'PASS: {name}')
        passed += 1
    else:
        print(f'FAIL: {name}')
        failed += 1

print(f'\nResult: {passed}/{len(checks)} passed')
