import re

with open('pages/planner.cshtml', 'r', encoding='utf-8') as f:
    content = f.read()

# Fix layout wrappers
content = content.replace('<div class="row justify-content-center">', '<div class="flex justify-center">')
content = content.replace('<div class="col-md-9">', '<div class="w-full max-w-4xl">')

# Fix grids
content = content.replace('<div class="row g-4 mb-4">', '<div class="grid grid-cols-1 md:grid-cols-2 lg:grid-cols-3 gap-6 mb-8">')
content = content.replace('<div class="row g-4">', '<div class="grid grid-cols-1 md:grid-cols-2 gap-6">')
content = content.replace('<div class="row g-3 mb-5">', '<div class="grid grid-cols-2 md:grid-cols-4 gap-4 mb-8">')

# Remove col-md-* wrappers for cards (using regex)
# This removes the wrapper div and leaves the inner card
content = re.sub(r'<div class="col-md-\d+(?:\s+col-sm-\d+)?">\s*(<div class="card destination-card.*?>\s*.*?\s*</div>\s*</div>)', r'\1', content, flags=re.DOTALL)
content = re.sub(r'<div class="col-md-\d+">\s*(<div class="card destination-card.*?>\s*.*?\s*</div>\s*</div>)', r'\1', content, flags=re.DOTALL)

# For form elements in step 2, 3, 8 where col-md-6 wraps a form control
content = re.sub(r'<div class="col-md-6">\s*(<label[^>]*>.*?</label>)\s*(<input[^>]*>)\s*</div>', r'<div>\1\n\2</div>', content, flags=re.IGNORECASE)
content = re.sub(r'<div class="col-md-6">\s*(<label[^>]*>.*?</label>)\s*(<select.*?/select>)\s*</div>', r'<div>\1\n\2</div>', content, flags=re.IGNORECASE|re.DOTALL)

# Default col-md-* cleanup
content = re.sub(r'<div class="col-md-\d+">', '<div>', content)

# Fix Bootstrap specific utility classes
content = content.replace('d-none', 'hidden')
content = content.replace('w-100', 'w-full')
content = content.replace('d-flex justify-content-between', 'flex justify-between')
content = content.replace('justify-content-end', 'justify-end')
content = content.replace('align-items-end', 'items-end')

# Rename class for inputs
content = content.replace('form-control', 'w-full placeholder-gray-400')
content = content.replace('form-select', 'w-full')

with open('pages/planner.cshtml', 'w', encoding='utf-8') as f:
    f.write(content)
