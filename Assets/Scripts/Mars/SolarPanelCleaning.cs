using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class OpportunityLastWordsMinigame : Minigame
{
    [System.Serializable]
    public class QuizQuestion
    {
        [TextArea(2, 3)]
        public string question;

        public string[] answers = new string[4];

        [Range(0, 3)]
        public int correctAnswer;

        [TextArea(2, 3)]
        public string explanation;
    }

    [Header("Quiz Questions")]
    public QuizQuestion[] questions =
    {
        new QuizQuestion
        {
            question = "What was the name of NASA's Mars Exploration Rover that sent the famous final message?",
            answers = new string[]
            {
                "Spirit",
                "Opportunity",
                "Curiosity",
                "Perseverance"
            },
            correctAnswer = 1,
            explanation = "Opportunity was one of NASA's twin Mars Exploration Rovers, alongside Spirit."
        },

        new QuizQuestion
        {
            question = "In which year did Opportunity land on Mars?",
            answers = new string[]
            {
                "2001",
                "2003",
                "2004",
                "2012"
            },
            correctAnswer = 2,
            explanation = "Opportunity landed on Mars on 25 January 2004 UTC."
        },

        new QuizQuestion
        {
            question = "What natural event contributed to the end of Opportunity's mission?",
            answers = new string[]
            {
                "A volcanic eruption",
                "A global dust storm",
                "A meteor impact",
                "A flood"
            },
            correctAnswer = 1,
            explanation = "A planet-encircling dust storm in 2018 blocked sunlight from reaching Opportunity's solar panels."
        },

        new QuizQuestion
        {
            question = "What was Opportunity's final communication famously paraphrased as?",
            answers = new string[]
            {
                "I am lost.",
                "My battery is empty.",
                "My battery is low and it's getting dark.",
                "Mission accomplished."
            },
            correctAnswer = 2,
            explanation = "The phrase became associated with Opportunity's final transmission, although it was not a literal message sent by the rover."
        },

        new QuizQuestion
        {
            question = "How long was Opportunity originally designed to operate on Mars?",
            answers = new string[]
            {
                "30 sols",
                "90 sols",
                "1 Martian year",
                "10 Earth years"
            },
            correctAnswer = 1,
            explanation = "Opportunity was designed for a 90-sol primary mission but operated for almost 15 years."
        }
    };

    [Header("UI References")]
    public TMP_Text questionText;
    public TMP_Text progressText;
    public TMP_Text feedbackText;

    public Button[] answerButtons;
    public TMP_Text[] answerButtonTexts;

    [Header("Completion")]
    public TMP_Text completionText;

    private int currentQuestion;
    private int score;
    private bool answered;

    protected override void Start()
    {
        base.Start();
    }

    protected override void OnOpened()
    {
        currentQuestion = 0;
        score = 0;
        answered = false;

        if (completionText != null)
            completionText.gameObject.SetActive(false);

        ShowQuestion();
    }

    protected override void OnClosed()
    {
        answered = false;
    }

    private void ShowQuestion()
    {
        if (currentQuestion >= questions.Length)
        {
            FinishQuiz();
            return;
        }

        answered = false;

        QuizQuestion question = questions[currentQuestion];

        questionText.text = question.question;

        progressText.text =
            "Question " + (currentQuestion + 1) +
            " / " + questions.Length;

        feedbackText.text = "";

        for (int i = 0; i < answerButtons.Length; i++)
        {
            answerButtons[i].gameObject.SetActive(true);
            answerButtons[i].interactable = true;

            answerButtonTexts[i].text =
                question.answers[i];
        }
    }

    public void SelectAnswer(int answerIndex)
    {
        if (answered || currentQuestion >= questions.Length)
            return;

        answered = true;

        QuizQuestion question = questions[currentQuestion];

        bool correct = answerIndex == question.correctAnswer;

        if (correct)
        {
            score++;

            feedbackText.text =
                "Correct!\n" + question.explanation;
        }
        else
        {
            feedbackText.text =
                "Not quite!\n" + question.explanation;
        }

        for (int i = 0; i < answerButtons.Length; i++)
        {
            answerButtons[i].interactable = false;
        }

        answerButtons[0].onClick.RemoveAllListeners();
        answerButtons[1].onClick.RemoveAllListeners();
        answerButtons[2].onClick.RemoveAllListeners();
        answerButtons[3].onClick.RemoveAllListeners();

        Invoke(nameof(NextQuestion), 2.5f);
    }

    private void NextQuestion()
    {
        currentQuestion++;
        ShowQuestion();
    }

    private void FinishQuiz()
    {
        questionText.text = "Quiz completed!";

        progressText.text = "Final score: " +
                            score + " / " + questions.Length;

        feedbackText.text = "";

        for (int i = 0; i < answerButtons.Length; i++)
        {
            answerButtons[i].gameObject.SetActive(false);
        }

        if (completionText != null)
        {
            completionText.gameObject.SetActive(true);
            completionText.text =
                "Opportunity's mission may have ended, " +
                "but its discoveries continue to inspire.";
        }

        Complete();
    }
}
