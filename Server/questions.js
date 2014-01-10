exports = module.exports = getQuestions;

var questions = [
    {
        'type': "open",
        'title': "How many balls does Gwenn have ?",
        'answer': 2
    },
    {
        'type': "qcm",
        'title': "What is love ?",
        'answer': "tu tu tu tuu tu tu tuuuu",
        'false1': "A game engine",
        'false2': "Jerome",
        'false3': "Yann"
    },
    {
        'type': "qcm",
        'title': "Where is Gwenn ?",
        'answer': "Gwenn is in the kitchen",
        'false1': "Gwenn is in the coding room",
        'false2': "Gwenn is at Polytech",
        'false3': "Gwenn is in the street"
    }
];

function getQuestions () {
    return questions;
}