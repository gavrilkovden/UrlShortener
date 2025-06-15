const form = document.getElementById('urlForm');

// Получаем id из URL, если есть — загружаем данные
const params = new URLSearchParams(window.location.search);
const id = params.get('id');

if (id) {
    document.getElementById('id').value = id;
    fetch(`/api/urls/${id}`)
        .then(res => {
            if (!res.ok) throw new Error('Не удалось загрузить данные');
            return res.json();
        })
        .then(data => {
            document.getElementById('originalUrl').value = data.originalUrl;
        })
        .catch(err => alert(err.message));
}

form.addEventListener('submit', async e => {
    e.preventDefault();

    const id = document.getElementById('id').value;
    const originalUrl = document.getElementById('originalUrl').value.trim();

    const urlPattern = /^(https?:\/\/)([a-z0-9\-]+\.)+[a-z]{2,}(:\d+)?(\/[^\s]*)?$/i;
    if (!urlPattern.test(originalUrl)) {
        alert("Введите корректный URL");
        return;
    }

    const method = id ? 'PUT' : 'POST';
    const url = id ? `/api/urls/${id}` : '/api/urls';

    // Отправляем только { originalUrl }, как ожидает API
    const res = await fetch(url, {
        method,
        headers: { 'Content-Type': 'application/json' },
        body: JSON.stringify({ originalUrl })
    });

    if (res.ok) {
        alert('Сохранено!');
        window.location.href = 'index.html';
    } else {
        // Пытаемся получить сообщение ошибки от API
        const err = await res.json().catch(() => null);
        if (err && err.errors) {
            // Если ModelState ошибки
            const messages = Object.values(err.errors).flat().join('\n');
            alert('Ошибка:\n' + messages);
        } else {
            const text = await res.text();
            alert('Ошибка: ' + text);
        }
    }
});
