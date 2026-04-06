import os

def build_preview(layout_path, view_path, output_path, title):
    with open(layout_path, 'r', encoding='utf-8') as f:
        layout_content = f.read()
    with open(view_path, 'r', encoding='utf-8') as f:
        view_content = f.read()

    # Clean up Razor syntax from view
    lines = view_content.split('\n')
    cleaned_view_lines = []
    
    in_razor_block = False
    for line in lines:
        if line.strip().startswith('@{') or line.strip().startswith('@using (') or line.strip().startswith('@if ('):
            in_razor_block = True
            continue
        if in_razor_block and line.strip() == '}':
            in_razor_block = False
            continue
        if in_razor_block:
            continue
            
        line = line.replace('@Html.AntiForgeryToken()', '')
        line = line.replace('@Html.ValidationMessageFor(m => m.FullName, "", new { @class = "text-danger" })', '')
        line = line.replace('@Html.ValidationMessageFor(m => m.Mobile, "", new { @class = "text-danger" })', '')
        line = line.replace('@Html.ValidationMessageFor(m => m.ExpectedDateTime, "", new { @class = "text-danger" })', '')
        
        # Replace inputs
        line = line.replace('@Html.TextBoxFor(m => m.FullName, new { @class = "form-control", required="required" })', '<input type="text" class="form-control" placeholder="John Doe" required />')
        line = line.replace('@Html.TextBoxFor(m => m.Mobile, new { @class = "form-control", required="required", pattern="\\\\d{10}", title="10 digit mobile" })', '<input type="text" class="form-control" placeholder="9876543210" required />')
        line = line.replace('@Html.TextBoxFor(m => m.Email, new { @class = "form-control", type="email" })', '<input type="email" class="form-control" placeholder="john@example.com" />')
        line = line.replace('@Html.TextBoxFor(m => m.CompanyName, new { @class = "form-control" })', '<input type="text" class="form-control" placeholder="ACME Corp" />')
        line = line.replace('@Html.TextBoxFor(m => m.ExpectedDateTime, new { @class = "form-control", type="datetime-local", required="required" })', '<input type="datetime-local" class="form-control" required />')
        line = line.replace('@Html.TextBoxFor(m => m.Purpose, new { @class = "form-control", required="required" })', '<input type="text" class="form-control" placeholder="Business Meeting" required />')
        line = line.replace('@Html.TextBoxFor(m => m.HostID, new { @class = "form-control", type="number", required="required" })', '<select class="form-control"><option>Mr. Smith</option></select>')
        line = line.replace('@Html.TextBoxFor(m => m.DeptID, new { @class = "form-control", type="number", required="required" })', '<select class="form-control"><option>IT Department</option></select>')
        line = line.replace('@Html.TextBoxFor(m => m.IDProofNumber, new { @class = "form-control", required="required" })', '<input type="text" class="form-control" placeholder="ABCD1234F" required />')
        
        # Clean up model references
        line = line.replace('@Model.TodayExpected', '124')
        line = line.replace('@Model.CheckedIn', '45')
        line = line.replace('@Model.CheckedOut', '62')
        line = line.replace('@Model.PendingApprovals', '17')
        line = line.replace('@Model.TotalRegisteredThisMonth', '1,492')
        
        if line.strip().startswith('@model') or line.strip().startswith('@using'):
            continue
            
        if line.strip() != '':
             cleaned_view_lines.append(line)

    view_html = '\n'.join(cleaned_view_lines)

    # Extract Scripts section manually to avoid regex bracket issues
    scripts_content = ''
    if '@section Scripts {' in view_html:
        parts = view_html.split('@section Scripts {')
        view_html = parts[0]
        
        script_part = parts[1]
        # The script part ends with a closing brace '}' at the very end of the file usually.
        # We can just trim the last '}'
        if script_part.strip().endswith('}'):
            script_part = script_part.strip()[:-1]
            
        scripts_content = script_part
        
    # Clean up layout
    layout_html = layout_content.replace('@RenderBody()', view_html)
    layout_html = layout_html.replace('@ViewBag.Title', title)
    
    # Map explicit navigation links for the previews
    layout_html = layout_html.replace('@Url.Action("Dashboard", "Admin")', 'preview_admin.html')
    layout_html = layout_html.replace('@Url.Action("Dashboard", "Guard")', 'preview_guard_dashboard.html')
    layout_html = layout_html.replace('@Url.Action("TodayList", "Guard")', 'preview_guard.html')
    layout_html = layout_html.replace('@Url.Action("PreRegister", "User")', 'preview_user.html')
    layout_html = layout_html.replace('@Url.Action("Logout", "Account")', 'preview_admin.html')
    
    # Admin module pages
    layout_html = layout_html.replace('@Url.Action("Visitors", "Admin")', 'preview_admin_visitors.html')
    layout_html = layout_html.replace('@Url.Action("Gallery", "Admin")', 'preview_admin_gallery.html')
    layout_html = layout_html.replace('@Url.Action("Reports", "Admin")', 'preview_admin_reports.html')
    layout_html = layout_html.replace('@Url.Action("Users", "Admin")', 'preview_admin_users.html')
    layout_html = layout_html.replace('@Url.Action("Masters", "Admin")', 'preview_admin_masters.html')
    layout_html = layout_html.replace('@Url.Action("AuditLog", "Admin")', 'preview_admin_auditlog.html')
    layout_html = layout_html.replace('@Url.Action("Search", "Guard")', 'preview_guard_search.html')
    layout_html = layout_html.replace('@Url.Action("MyVisits", "User")', 'preview_user_myvisits.html')
    
    # Clean up any remaining URL Actions to #
    import re
    layout_html = re.sub(r'@Url\.Action\("[^"]*",\s*"[^"]*"\)', '#', layout_html)
    layout_html = layout_html.replace('@RenderSection("Scripts", required: false)', scripts_content)

    with open(output_path, 'w', encoding='utf-8') as f:
        f.write(layout_html)

# Guard Dashboard
build_preview(
    'Views/Shared/_Layout_Guard.cshtml', 
    'Views/Guard/Dashboard.cshtml', 
    'preview_guard_dashboard.html', 
    "Guard Dashboard"
)

# Admin
build_preview(
    'Views/Shared/_Layout_Admin.cshtml', 
    'Views/Admin/Dashboard.cshtml', 
    'preview_admin.html', 
    'Admin Dashboard'
)

# Guard Today List
build_preview(
    'Views/Shared/_Layout_Guard.cshtml', 
    'Views/Guard/TodayList.cshtml', 
    'preview_guard.html', 
    "Today's List"
)

# User
build_preview(
    'Views/Shared/_Layout_User.cshtml', 
    'Views/User/PreRegister.cshtml', 
    'preview_user.html', 
    'Pre-Register Visitor'
)

# Admin Visitors
build_preview('Views/Shared/_Layout_Admin.cshtml', 'Views/Admin/Visitors.cshtml', 'preview_admin_visitors.html', 'Visitor Management')

# Admin Gallery
build_preview('Views/Shared/_Layout_Admin.cshtml', 'Views/Admin/Gallery.cshtml', 'preview_admin_gallery.html', 'Photo Gallery')

# Admin Reports
build_preview('Views/Shared/_Layout_Admin.cshtml', 'Views/Admin/Reports.cshtml', 'preview_admin_reports.html', 'Analytics & Reports')

# Admin Users
build_preview('Views/Shared/_Layout_Admin.cshtml', 'Views/Admin/Users.cshtml', 'preview_admin_users.html', 'System Users')

# Admin Masters
build_preview('Views/Shared/_Layout_Admin.cshtml', 'Views/Admin/Masters.cshtml', 'preview_admin_masters.html', 'Master Data Settings')

# Admin Audit Log
build_preview('Views/Shared/_Layout_Admin.cshtml', 'Views/Admin/AuditLog.cshtml', 'preview_admin_auditlog.html', 'Security Audit Log')

# Guard Search
build_preview('Views/Shared/_Layout_Guard.cshtml', 'Views/Guard/Search.cshtml', 'preview_guard_search.html', 'Search Visitor')

# User My Visits
build_preview('Views/Shared/_Layout_User.cshtml', 'Views/User/MyVisits.cshtml', 'preview_user_myvisits.html', 'My Visits')

print("All previews generated!")
