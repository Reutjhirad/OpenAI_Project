const baseUrl = "https://localhost:7036/api/";

let currentNumberOfQuestion = 0;
const TOTAL_QUESTIONS = 5;

async function getQuestion() {
    const subject = document.getElementById("subject").value;
    const language = document.getElementById("languages").value;

    const prompt = {
        "Subject": subject,
        "Language": language
    };

    const url = baseUrl + "GPT/GPTChat";
    const params = {
        method: 'POST',
        headers: {
            Accept: 'application/json',
            'Content-Type': 'application/json'
        },
        body: JSON.stringify(prompt)
    };


    try {
        const response = await fetch(url, params);
        if (response.ok) {
            const data = await response.json();
            console.log("Response:", data);

            displayQuestion(data);
        } else {
            console.error("Error:", response.statusText);
        }
    } catch (error) {
        console.error("Error:", error);
    }
}

  
function displayQuestion(data) {

    const jsonObject = JSON.parse(data);
    console.log(jsonObject, '&&&&&&&&&&&7')
    const question = jsonObject.question;
    const answers = jsonObject.answers;

    const questionsList = document.getElementById("questions");
    //questionsList.innerHTML = ""; // Clear previous content

    const questionElement = document.createElement("p");
    questionElement.textContent = question;
    questionsList.appendChild(questionElement);

    let userAnswer = null;
    
    for (let answer of answers) {
        // Create label for the answer
        const answerLabel = document.createElement("label");
        answerLabel.textContent = answer.text;

        // Create radio button for the answer
        const answerRadioButton = document.createElement("input");
        answerRadioButton.type = "radio";
        answerRadioButton.name = "answer";
        answerRadioButton.value = answer.text;

        answerRadioButton.addEventListener('change', function (event) {
            if (event.target.checked) {
                userAnswer = answer;
            }
        });

        // Append the radio button and label to the parent element
        questionsList.appendChild(answerRadioButton);
        questionsList.appendChild(answerLabel);

        // Add line break between each option
        questionsList.appendChild(document.createElement("br"));
    }

    const feedbackArea = document.createElement('p');
    questionsList.appendChild(feedbackArea);


    const submitButton = document.createElement("button");
    submitButton.textContent = 'submit';

    const nextQuestionButton = document.createElement('button');
    nextQuestionButton.textContent = 'לשאלה הבאה';
    nextQuestionButton.setAttribute('hidden', 'hidden');

    submitButton.onclick = () => checkAnswer(question, answers, userAnswer, feedbackArea, nextQuestionButton, questionsList);
    questionsList.appendChild(submitButton);

    
    
    nextQuestionButton.onclick = () => generateNextQuestion();
    questionsList.appendChild(nextQuestionButton);
    
}




function checkAnswer(question, answers, userAnswer, feedbackElement, nextQuestionButton, questionsList) {
    let correctAnswer = answers.find(answer => answer.isCorrect === true);
    feedbackElement.classList.add("feedback");
    feedbackElement.value = "";


    if (userAnswer && userAnswer.isCorrect) {
        feedbackElement.textContent = "התשובה נכונה!!"
        feedbackElement.style.color = "green";

    } else {
        feedbackElement.textContent = "טעות! התשובה הנכונה היא: " + correctAnswer.text;
        feedbackElement.style.color = "red";
    }

    currentNumberOfQuestion++;

    if (currentNumberOfQuestion < TOTAL_QUESTIONS) {
        nextQuestionButton.removeAttribute("hidden");
    }
    else {
        const nextPageButton = document.createElement('button')
        nextPageButton.textContent = 'לעמוד הבא';
        nextPageButton.onclick = () => nextPage();
        questionsList.appendChild(nextPageButton);
    }
    

}

function generateNextQuestion() {
    getQuestion();
}

function nextPage() {
    alert('good');
}

