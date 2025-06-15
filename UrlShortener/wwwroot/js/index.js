async function loadUrls() {
    const res = await fetch('/api/urls');
    if (!res.ok) {
        alert('Ошибка загрузки ссылок');
        return;
    }
    const urls = await res.json();

    const tbody = document.querySelector('#urlTable tbody');
    tbody.innerHTML = '';

    urls.forEach(u => {
        const row = document.createElement('tr');
        row.innerHTML = `
            <td><a href="${u.originalUrl}" target="_blank" rel="noopener noreferrer">${u.originalUrl}</a></td>
            <td><a href="/s/${u.shortCode}" target="_blank" rel="noopener noreferrer">/s/${u.shortCode}</a></td>
            <td>${new Date(u.createdAt).toLocaleString()}</td>
            <td>${u.clicks}</td>
            <td><button onclick="editUrl(${u.id})">Редактировать</button></td>
            <td><button onclick="deleteUrl(${u.id})">Удалить</button></td>
        `;
        tbody.appendChild(row);
    });
}

function editUrl(id) {
    window.location.href = `edit.html?id=${id}`;
}

async function deleteUrl(id) {
    if (!confirm('Удалить ссылку?')) return;

    const res = await fetch(`/api/urls/${id}`, { method: 'DELETE' });
    if (res.ok) {
        loadUrls();
    } else {
        alert('Ошибка удаления');
    }
}

loadUrls();
