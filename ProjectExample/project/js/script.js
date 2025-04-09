// Загрузка советов из XML
async function loadEvacuationTips() {
    try {
        const response = await fetch('tips.xml');
        const data = await response.text();
        const parser = new DOMParser();
        const xmlDoc = parser.parseFromString(data, 'text/xml');
        const tips = xmlDoc.getElementsByTagName('tip');
        
        const tipsList = document.getElementById('evacuationTips');
        tipsList.innerHTML = '';
        
        Array.from(tips).forEach((tip, index) => {
            const li = document.createElement('li');
            li.textContent = tip.textContent;
            li.dataset.index = index;
            li.addEventListener('click', () => markTipAsRead(li));
            tipsList.appendChild(li);
        });
        
        updateProgress();
    } catch (error) {
        console.error('Ошибка загрузки советов:', error);
    }
}

// Отметка совета как прочитанного
function markTipAsRead(tipElement) {
    tipElement.classList.toggle('read');
    updateProgress();
    showNotification('Совет ' + (tipElement.classList.contains('read') ? 'отмечен как прочитанный' : 'снова непрочитанный'));
}

// Обновление прогресса чтения
function updateProgress() {
    const tips = document.querySelectorAll('#evacuationTips li');
    const readTips = document.querySelectorAll('#evacuationTips li.read');
    const progress = (readTips.length / tips.length) * 100;
    
    document.getElementById('readingProgress').style.width = `${progress}%`;
    document.getElementById('progressText').textContent = `${readTips.length}/${tips.length} прочитано`;
}

// Показ уведомления
function showNotification(message) {
    const notification = document.getElementById('notification');
    notification.textContent = message;
    notification.classList.add('show');
    
    setTimeout(() => {
        notification.classList.remove('show');
    }, 3000);
}

// Инициализация теста
function initQuiz() {
    const quizContainer = document.getElementById('quizContainer');
    const questions = [
        {
            question: "Что нужно делать при срабатывании сигнала тревоги?",
            options: [
                "Продолжать работать",
                "Немедленно начать эвакуацию",
                "Ждать указаний начальства"
            ],
            correct: 1,
            explanation: "При срабатывании сигнала тревоги необходимо немедленно начать эвакуацию, не дожидаясь дополнительных указаний."
        },
        {
            question: "Как правильно двигаться при сильном задымлении?",
            options: [
                "Бежать к выходу",
                "Пригнуться и двигаться к выходу",
                "Ждать помощи у окна"
            ],
            correct: 1,
            explanation: "При сильном задымлении нужно пригнуться, так как внизу меньше дыма, и двигаться к выходу, прикрывая дыхательные пути влажной тканью."
        },
        {
            question: "Можно ли пользоваться лифтом во время эвакуации?",
            options: [
                "Да, если нет дыма",
                "Нет, никогда",
                "Только если спешите"
            ],
            correct: 1,
            explanation: "Лифтами во время эвакуации пользоваться нельзя - они могут отключиться из-за перебоев с электричеством."
        }
    ];
    
    questions.forEach((q, qIndex) => {
        const questionElement = document.createElement('div');
        questionElement.className = 'quiz-question';
        questionElement.innerHTML = `
            <h3>${qIndex + 1}. ${q.question}</h3>
            <div class="quiz-options">
                ${q.options.map((option, oIndex) => `
                    <label class="quiz-option">
                        <input type="radio" name="question${qIndex}" value="${oIndex}" 
                               onchange="checkIfQuizCompleted()">
                        ${option}
                    </label>
                `).join('')}
            </div>
        `;
        quizContainer.appendChild(questionElement);
    });
}

// Проверка завершения теста
function checkIfQuizCompleted() {
    const allQuestions = document.querySelectorAll('.quiz-question');
    let answeredCount = 0;
    
    allQuestions.forEach(question => {
        if (question.querySelector('input:checked')) {
            answeredCount++;
        }
    });
    
    if (answeredCount === allQuestions.length) {
        checkQuizAnswers();
    }
}

// Проверка ответов теста
function checkQuizAnswers() {
    const questions = [
        {
            correct: 1,
            explanation: "При срабатывании сигнала тревоги необходимо немедленно начать эвакуацию."
        },
        {
            correct: 1,
            explanation: "При сильном задымлении нужно пригнуться, так как внизу меньше дыма."
        },
        {
            correct: 1,
            explanation: "Лифтами во время эвакуации пользоваться категорически нельзя."
        }
    ];

    let score = 0;
    const results = [];
    const quizContainer = document.getElementById('quizContainer');
    const questionElements = quizContainer.querySelectorAll('.quiz-question');

    questionElements.forEach((questionElement, index) => {
        const selectedOption = questionElement.querySelector('input:checked');
        const isCorrect = selectedOption && parseInt(selectedOption.value) === questions[index].correct;
        
        if (isCorrect) {
            score++;
            questionElement.classList.add('correct');
            questionElement.classList.remove('incorrect');
        } else {
            questionElement.classList.add('incorrect');
            questionElement.classList.remove('correct');
            
            // Найдем правильный ответ и пометим его
            const correctOption = questionElement.querySelector(`input[value="${questions[index].correct}"]`);
            if (correctOption) {
                correctOption.parentElement.classList.add('correct-answer');
            }
        }

        results.push({
            question: questionElement.querySelector('h3').textContent,
            isCorrect,
            explanation: questions[index].explanation
        });
    });

    showQuizResult(score, questionElements.length, results);
}

// Показ результатов теста
function showQuizResult(score, totalQuestions, results) {
    const quizResult = document.getElementById('quizResult');
    quizResult.innerHTML = '';

    const resultHeader = document.createElement('h3');
    resultHeader.textContent = `Результат: ${score} из ${totalQuestions}`;
    quizResult.appendChild(resultHeader);

    if (score === totalQuestions) {
        quizResult.className = 'quiz-result perfect';
        resultHeader.innerHTML += ' 🎉 Отлично! Вы отлично знаете правила эвакуации!';
    } else if (score >= totalQuestions * 0.7) {
        quizResult.className = 'quiz-result good';
        resultHeader.innerHTML += ' 👍 Хорошо, но есть что улучшить!';
    } else {
        quizResult.className = 'quiz-result poor';
        resultHeader.innerHTML += ' ❌ Вам нужно изучить правила эвакуации внимательнее!';
    }

    const detailsSection = document.createElement('div');
    detailsSection.className = 'quiz-details';
    quizResult.appendChild(detailsSection);

    results.forEach((result, index) => {
        const detailItem = document.createElement('div');
        detailItem.className = `detail-item ${result.isCorrect ? 'correct' : 'incorrect'}`;
        
        const statusIcon = document.createElement('span');
        statusIcon.className = 'detail-status';
        statusIcon.textContent = result.isCorrect ? '✓ Верно' : '✗ Неверно';
        detailItem.appendChild(statusIcon);
        
        const questionText = document.createElement('p');
        questionText.className = 'detail-question';
        questionText.textContent = `${index + 1}. ${result.question}`;
        detailItem.appendChild(questionText);
        
        if (!result.isCorrect) {
            const explanationText = document.createElement('p');
            explanationText.className = 'detail-explanation';
            explanationText.textContent = result.explanation;
            detailItem.appendChild(explanationText);
        }
        
        detailsSection.appendChild(detailItem);
    });

    quizResult.style.display = 'block';
    showNotification(`Тест завершен! Ваш результат: ${score} из ${totalQuestions}`);
    
    // Прокручиваем страницу к результатам
    quizResult.scrollIntoView({ behavior: 'smooth' });
}

// Инициализация при загрузке страницы
document.addEventListener('DOMContentLoaded', () => {
    loadEvacuationTips();
    initQuiz();
    
    // Показываем приветственное сообщение
    setTimeout(() => {
        showNotification('Изучите правила эвакуации и проверьте свои знания!');
    }, 1000);
});