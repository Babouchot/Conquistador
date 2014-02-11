exports = module.exports = getQuestions;

var questions = [

    {
    'type': "open",
    'title': "In which decade was the telephone invented?",
    'answer': 1870,
    'id' : 0
    },
    {
        'type': "open",
        'title': "In which year was Pulitzer Prize established?",
        'answer': 1917,
        'id' : 0
    },
    {
        'type': "open",
        'title': "The average salinity of seawater in parts per thousand is...",
        'answer': 35,
        'id' : 0
    },
    {
        'type': "open",
        'title': "In which decade with the first transatlantic radio broadcast occur?",
        'answer': 1900,
        'id' : 0
    },
    {
        'type': "open",
        'title': "In what year was the '@' chosen for its use in e-mail addresses?",
        'answer': 1972,
        'id' : 0
    },
    {
        'type': "open",
        'title': "In what year was the UNIX operating system created?",
        'answer': 1969,
        'id' : 0
    },
    {
        'type': "open",
        'title': "Track and field star Carl Lewis won how many gold medals at the 1984 Olympic games?",
        'answer': 4,
        'id' : 0
    },
    {
        'type': "open",
        'title': "Compact discs, (according to the original CD specifications) hold how many minutes of music?",
        'answer': 74,
        'id' : 0
    },
    {
        'type': "open",
        'title': "In what year was the Coca Cola company created?",
        'answer': 1886,
        'id' : 0
    },
    {
        'type': "open",
        'title': "How many members did The Spice Girls have?",
        'answer': 5,
        'id' : 0
    },
    {
        'type': "open",
        'title': "What is the value of a 'W' in the French Scrabble?",
        'answer': 10,
        'id' : 0
    },
    {
        'type': "open",
        'title': "What is the first perfect number?",
        'answer': 6,
        'id' : 0
    },
    {
        'type': "open",
        'title': "What is the 7th prime number?",
        'answer': 17,
        'id' : 0
    },
    {
        'type': "open",
        'title': "How many letters of the greek alphabet are vowels?",
        'answer': 8,
        'id' : 0
    },
    {
        'type': "open",
        'title': "How many players are on the field in a soccer team?",
        'answer': 11,
        'id' : 0
    },
    {
        'type': "open",
        'title': "How many players are on the field in a basketball team?",
        'answer': 5,
        'id' : 0
    },
    {
        'type': "open",
        'title': "How many players are on the field in a union rugby team?",
        'answer': 15,
        'id' : 0
    },
    {
        'type': "open",
        'title': "How many pounds is a kilograms approximately?",
        'answer': 2,
        'id' : 0
    },
    {
        'type': "open",
        'title': "How many holes are in a standard round of golf?",
        'answer': 18,
        'id' : 0
    },
    {
        'type': "open",
        'title': "How many red balls are on a snooker table at the start of a frame (traditional full game version)?",
        'answer': 15,
        'id' : 0
    },
    {
        'type': "open",
        'title': "A Nebuchadnezzar wine/champagne bottle equates to how many normal bottles?",
        'answer': 20,
        'id' : 0
    },
    {
        'type': "open",
        'title': "The age that Brian Jones, Janis Joplin, Jimmy Hendrix, Jim Morrison and Kurt Kobain all died?",
        'answer': 27,
        'id' : 0
    },
    {
        'type': "open",
        'title': "How many planets are in our solar system?",
        'answer': 8,
        'id' : 0
    },
    {
        'type': "open",
        'title': "How many points is a converted try?",
        'answer': 7,
        'id' : 0
    },
    {
        'type': "open",
        'title': "How many rings are in the olympic flag?",
        'answer': 5,
        'id' : 0
    },
    {
        'type': "open",
        'title': "In what year did the Berlin Wall fall?",
        'answer': 1991,
        'id' : 0
    },
    {
        'type': "open",
        'title': "In what year was the president of France, M. Rene Coty, elected?",
        'answer': 1954,
        'id' : 0
    },
    {
        'type': "open",
        'title': "How many dots are on a dice?",
        'answer': 21,
        'id' : 0
    },
    {
        'type': "open",
        'title': "How many days is the average human menstrual cycle?",
        'answer': 28,
        'id' : 0
    },
    {
        'type': "open",
        'title': "How many stars are there on an American flag?",
        'answer': 50,
        'id' : 0
    },
    {
        'type': "open",
        'title': "What is the answer to the life, the universe, and everything?",
        'answer': 42,
        'id' : 0
    },
    {
        'type': "open",
        'title': "How many stripes does the US flag have?",
        'answer': 13,
        'id' : 0
    },
    {
        'type': "open",
        'title': "What is the sum, in degrees, of the angles in a triangle?",
        'answer': 180,
        'id' : 0
    },
    {
        'type': "open",
        'title': "In computing, how many bits are in one byte?",
        'answer': 8,
        'id' : 0
    },
    {
        'type': "open",
        'title': "How may moons does the planet Mars have?",
        'answer': 2,
        'id' : 0
    },
    {
        'type': "open",
        'title': "How many colours are in the rainbow?",
        'answer': 7,
        'id' : 0
    },
    {
        'type': "open",
        'title': "How many steps are there in the Eiffel Tower?",
        'answer': 1665,
        'id' : 0
    },
    {
        'type': "open",
        'title': "How many prime numbers are there between 10 and 20?",
        'answer': 4,
        'id' : 0
    },
    { //Basic General Knowledge
        'type': "qcm",
        'title': "Grand Central Terminal, Park Avenue, New York is the world's...",
        'answer': "largest railway station",
        'false1': "highest railway station",
        'false2': "longest railway station",
        'false3': "None of the above",
        'id' : 0
    },
    {
        'type': "qcm",
        'title': "Entomology is the science that studies",
        'answer': "Insects",
        'false1': "Behavior of human beings",
        'false2': "The origin and history of technical and scientific terms",
        'false3': "The formation of rocks",
        'id' : 0
    },
    {
        'type': "qcm",
        'title': "Eritrea, which became the 182nd member of the UN in 1993, is in the continent of",
        'answer': "Africa",
        'false1': "Asia",
        'false2': "Europe",
        'false3': "Australia",
        'id' : 0
    },
    {
        'type': "qcm",
        'title': "For which of the following disciplines is Nobel Prize awarded?",
        'answer': "All of the above",
        'false1': "Physics and Chemistry",
        'false2': "Physiology or Medicine",
        'false3': "Literature, Peace and Economics",
        'id' : 0
    },
    {
        'type': "qcm",
        'title': "Hitler party which came into power in 1933 is known as",
        'answer': "Nazi Party",
        'false1': "Labour Party",
        'false2': "Foam Party",
        'false3': "Democratic Party",
        'id' : 0
    },
    { //Famous Personalities
        'type': "qcm",
        'title': "Who is the father of Geometry?",
        'answer': "Euclid",
        'false1': "Aristotle",
        'false2': "Pythagoras",
        'false3': "Kepler",
        'id' : 0
    },
    { //World Geography
        'type': "qcm",
        'title': "The intersecting lines drawn on maps and globes are...",
        'answer': "geographic grids",
        'false1': "latitudes",
        'false2': "longitudes",
        'false3': "None of the above",
        'id' : 0
    },
    {
        'type': "qcm",
        'title': "The humidity of the air depends upon...",
        'answer': "All of the above",
        'false1': "temperature",
        'false2': "location",
        'false3': "weather",
        'id' : 0
    },
    {
        'type': "qcm",
        'title': "The largest glaciers are...",
        'answer': "continental glaciers",
        'false1': "mountain glaciers",
        'false2': "alpine glaciers",
        'false3': "piedmont glaciers",
        'id' : 0
    },
    {
        'type': "qcm",
        'title': "The largest gold producing country in the world is...",
        'answer': "China",
        'false1': "Canada",
        'false2': "South Africa",
        'false3': "USA",
        'id' : 0
    },
    {
        'type': "qcm",
        'title': "The largest country of the world by geographical area is...",
        'answer': "Russia",
        'false1': "Vatican City",
        'false2': "Australia",
        'false3': "USA",
        'id' : 0
    },
    { //Inventions
        'type': "qcm",
        'title': "Who invented the Ballpoint Pen?",
        'answer': "Biro Brothers",
        'false1': "Waterman Brothers",
        'false2': "Bicc Brothers",
        'false3': "Write Brothers",
        'id' : 0
    },
    {
        'type': "qcm",
        'title': "Which scientist discovered the radioactive element radium?",
        'answer': "Marie Curie",
        'false1': "Isaac Newton",
        'false2': "Albert Einstein",
        'false3': "Benjamin Franklin",
        'id' : 0
    },
    {
        'type': "qcm",
        'title': "What Galileo invented?",
        'answer': "Thermometer",
        'false1': "Barometer",
        'false2': "Microscope",
        'false3': "Pendulum clock",
        'id' : 0
    },
    {
        'type': "qcm",
        'title': "What invention caused many deaths while testing it?",
        'answer': "Parachute",
        'false1': "Dynamite",
        'false2': "Ladders",
        'false3': "Race cars",
        'id' : 0
    },
    {
        'type': "qcm",
        'title': "What now-ubiquitous device was invented by Zenith engineer Eugene Polley in 1955?",
        'answer': "Remote control",
        'false1': "Microwave oven",
        'false2': "VCR",
        'false3': "Calculator",
        'id' : 0
    },
    { //World Organisations
        'type': "qcm",
        'title': "The United Nations Conference on Trade and Development (UNCTAD) is located at which of the following places?",
        'answer': "Geneva",
        'false1': "Rome",
        'false2': "Paris",
        'false3': "Vienna",
        'id' : 0
    },
    {
        'type': "qcm",
        'title': "Amnesty International is an organisation associated with which of the following fields?",
        'answer': "Protection of human rights",
        'false1': "Protection of Cruelty to animals",
        'false2': "Environment protection",
        'false3': "Protection of historic monuments",
        'id' : 0
    },
    {
        'type': "qcm",
        'title': "Which of the following is used as the logo of the World Wide Fund for Nature (WWF)?",
        'answer': "Panda",
        'false1': "Deer",
        'false2': "Camel",
        'false3': "Lion",
        'id' : 0
    },
    {
        'type': "qcm",
        'title': "Which of the following countries is not a member of the G-8 group?",
        'answer': "Spain",
        'false1': "Germany",
        'false2': "France",
        'false3': "Italy",
        'id' : 0
    },
    {
        'type': "qcm",
        'title': "The working language(s) of the UNESCO is/are...",
        'answer': "English, French and Russian",
        'false1': "French only",
        'false2': "English only",
        'false3': "English and French",
        'id' : 0
    },
    { //General Science
        'type': "qcm",
        'title': "Which of the following is used in pencils?",
        'answer': "Graphite",
        'false1': "Silicon",
        'false2': "Charcoal",
        'false3': "Phosphorous",
        'id' : 0
    },
    {
        'type': "qcm",
        'title': "Bromine is a...",
        'answer': "red liquid",
        'false1': "black solid",
        'false2': "colourless gas",
        'false3': "highly inflammable gas",
        'id' : 0
    },
    {
        'type': "qcm",
        'title': "The hardest substance available on earth is",
        'answer': "Diamond",
        'false1': "Gold",
        'false2': "Iron",
        'false3': "Platinum",
        'id' : 0
    },
    {
        'type': "qcm",
        'title': "Soda water contains...",
        'answer': "carbon dioxide",
        'false1': "carbonic acid",
        'false2': "sulphuric acid",
        'false3': "nitrous acid",
        'id' : 0
    },
    {
        'type': "qcm",
        'title': "The filament of an electric bulb is made of...",
        'answer': "tungsten",
        'false1': "nichrome",
        'false2': "graphite",
        'false3': "iron",
        'id' : 0
    },
    { //Technology
        'type': "qcm",
        'title': "What is part of a database that holds only one type of information?",
        'answer': "Field",
        'false1': "Report",
        'false2': "Record",
        'false3': "File",
        'id' : 0
    },
    {
        'type': "qcm",
        'title': "'OS' computer abbreviation usually means?",
        'answer': "Operating System",
        'false1': "Order of Significance",
        'false2': "Open Software",
        'false3': "Optical Sensor",
        'id' : 0
    },
    {
        'type': "qcm",
        'title': "'.MOV' extension refers usually to what kind of file?",
        'answer': "Animation/movie file",
        'false1': "Image file",
        'false2': "Audio file",
        'false3': "MS Office document",
        'id' : 0
    },
    {
        'type': "qcm",
        'title': "Which is a type of Electrically-Erasable Programmable Read-Only Memory?",
        'answer': "Flash",
        'false1': "RAM",
        'false2': "SRAM",
        'false3': "BIOS",
        'id' : 0
    },
    {
        'type': "qcm",
        'title': "Who developed Yahoo?",
        'answer': "David Filo & Jerry Yang",
        'false1': "Dennis Ritchie & Ken Thompson",
        'false2': "Vint Cerf & Robert Kahn",
        'false3': "Steve Case & Jeff Bezos",
        'id' : 0
    },
    {
        'type': "qcm",
        'title': "What was the first ARPANET message?",
        'answer': "lo",
        'false1': "hello world",
        'false2': "foo",
        'false3': "bar",
        'id' : 0
    },
    {
        'type': "qcm",
        'title': "Who co-created the UNIX operating system in 1969 with Dennis Ritchie?",
        'answer': "Ken Thompson",
        'false1': "Bjarne Stroustrup",
        'false2': "Linus Torvalds",
        'false3': "Steve Wozniak",
        'id' : 0
    },
    { //Easy Questions
        'type': "qcm",
        'title': "Which movie would you hear the song 'Hakuna Matata'?",
        'answer': "Lion King",
        'false1': "Hercules",
        'false2': "Beauty and the Beast",
        'false3': "Cinderella",
        'id' : 0
    },
    {
        'type': "qcm",
        'title': "What color would you get if you mix red and yellow together?",
        'answer': "Orange",
        'false1': "Pink",
        'false2': "Gray",
        'false3': "Black",
        'id' : 0
    },
    {
        'type': "qcm",
        'title': "What is the name of the dog in Garfield?",
        'answer': "Odie",
        'false1': "Tyson",
        'false2': "Oliver",
        'false3': "Fred",
        'id' : 0
    },
    {
        'type': "qcm",
        'title': "The highest mountain in the world is the...",
        'answer': "Mount Everest",
        'false1': "Mount Kangchenjunga",
        'false2': "Mount Kilimandjaro",
        'false3': "Mount Annapurna",
        'id' : 0
    }
];

function getQuestions () {
    return questions;
}