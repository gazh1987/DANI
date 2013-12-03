using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Speech.Synthesis;

namespace Dani
{
    class Program
    {
        static string ulysses = "ulysses.txt";
        static string htapos = "htapos.txt";
        static string grimm = "grimm.txt";
        static string convo = "knowledge.txt";
        static List<Words> wordList;
        static List<string> saveList;

        //addWord method: adds new words to the word list
        public static void addWord(string word, string followUp)
        {
            //Add Word to List
            Words w = new Words();
            w.Word = word;

            FollowUp f = new FollowUp();
            f.Word = followUp;
            f.Count = 1;
            w.Fol.Add(f);

            wordList.Add(w);
        }

        //addFollowUpWord Method: adds follow up words to the follow up list. It first checks to see if the word is already in the list. If it is
        //it will just increment the count. If it isnt it will add the word to the follow up list and initialise the count to 1.
        public static void addFollowUpWord(string word, string followUp)
        {
            int index = 0;
            bool foundInFollowUp = false;
            for (int i = 0; i < wordList.Count; i++)
            {
                if (wordList[i].Word == word) 
                {
                    for (int j = 0; j < wordList[i].Fol.Count; j++)
                    {
                        if (wordList[i].Fol[j].Word == followUp)
                        {
                            foundInFollowUp = true;
                            index = j;
                        } //end if
                    }//end for

                    if (foundInFollowUp == true)
                    {
                        wordList[i].Fol[index].Count = wordList[i].Fol[index].Count + 1;
                    }//end if
                    else
                    {
                        FollowUp follow = new FollowUp();
                        follow.Word = followUp;
                        follow.Count = 1;
                        wordList[i].Fol.Add(follow);
                    }//end else
                }//end for
            }//end for
        }//end method addFollowUp

        //daniTalk Method: For making DANI talk. First of all it picks a random word from the input text and makes that the first word in the 
        //reply array. It then finds that word in the wordlist, looks for the most popular follow up word for that word in its followup word
        //list and places that word in the next space in the reply array. It then finds that word in the wrodlist and repeats the whole process
        //until the array is full or until there is no followUp word.
        public static void daniTalk(string[] parsedText)
        {
            int max = 0;
            string[] Reply = new string[15];
            Random rnd = new Random();
            int num = rnd.Next(parsedText.Length);
            Reply[0] = parsedText[num];

            for (int i = 1; i < Reply.Length; i++)
            {
                for (int j = 0; j < wordList.Count; j++)
                {
                    if (Reply[i - 1] == wordList[j].Word)
                    {
                        for (int k = 0; k < wordList[j].Fol.Count; k++)
                        {
                            if (wordList[j].Fol[k].Count > max)
                            {
                                max = k;
                                Reply[i] = wordList[j].Fol[max].Word;
                            }//end if
                        }//end for
                    }//end if
                }//end for
            }//end for

            //Converts the Reply array to a string so when we use the speech synthesizer. This is so when we use the sppech synth to make DANI talk,
            //we pass the string talk into it intead of the array so as to make the speech flow more naturally. (When we pass in the array there are
            //big pauses between words as the synth accsesses each array element.)
            string talk = ArrayToString(Reply);

            //Placing the words on screen
            Console.Write("$DANI: ");
            for (int index = 0; index < Reply.Length; index++)
            {
                Console.Write(Reply[index]);
                Console.Write(" "); 
            }//end for

            //Code for making DANI speak
            using (SpeechSynthesizer synth = new SpeechSynthesizer())
            {
                synth.SelectVoiceByHints(VoiceGender.Male);
                synth.Speak(talk);
            }

            Console.WriteLine();

        }//end method daniTalk

        //preDefinedQuestions: A number of pre defined questions you can ask DANI.
        public static bool preDefinedQuestions(string chat)
        {
            bool go = false;
            string c = chat.ToLower();
            //Ask Dani his name
            if (c.IndexOf("your name") != -1 )
            {
                Console.WriteLine("$DANI: My name is DANI");
                using (SpeechSynthesizer synth = new SpeechSynthesizer())
                {
                    synth.SelectVoiceByHints(VoiceGender.Male);
                    synth.Speak("My name is DANI");
                }
                go = true;
            }
            //Ask Dani the Time
            else if (c.IndexOf("what time") != -1 || c.IndexOf("the time") != -1)
            {
                DateTime time = DateTime.Now;              
                string format = "HH:mm:ss";    
                Console.WriteLine("$DANI: The current time is " + time.ToString(format));
                using (SpeechSynthesizer synth = new SpeechSynthesizer())
                {
                    synth.SelectVoiceByHints(VoiceGender.Male);
                    synth.Speak("The current time is " + time.ToString(format));
                }
                go = true;
            }
            //Ask Dani what age he is.
            else if (c.IndexOf("old are you") != -1 || c.IndexOf("age are you") != -1)
            {
                DateTime oldDate = new DateTime(2013, 10, 11);
                DateTime newDate = DateTime.Now;

                // Difference in days, hours, and minutes.
                TimeSpan ts = newDate - oldDate;
                // Difference in days.
                int differenceInDays = ts.Days;

                Console.WriteLine("$DANI: I am " + differenceInDays + " days old");
                using (SpeechSynthesizer synth = new SpeechSynthesizer())
                {
                    synth.SelectVoiceByHints(VoiceGender.Male);
                    synth.Speak("I am " + differenceInDays + " days old");
                }
                go = true;
            }

            
            return go;
        }
  
        //loadFile method: For loading either book or previous conversations. Reads all the lines from the file, parses the text and fills up the word
        //lists with the parsed text.
        public static void loadfile(string text)
        {
            string[] lines = System.IO.File.ReadAllLines(text);
            for (int i = 0; i < lines.Length; i++)
            {
                string[] parse = lines[i].ToLower().Split(' ');

                for (int j = 0; j < parse.Length - 1; j++)
                {
                    Words w = new Words();
                    w.Word = parse[j];

                    FollowUp f = new FollowUp();
                    f.Word = parse[j + 1];
                    f.Count = 1;
                    w.Fol.Add(f);

                    wordList.Add(w);
                }//end for
            }//end for
        }//end method loadBook

        //learn method: takes the input text we want DANI to remember, cuts off the first word ("it will always be learn") and saves the rest to file.
        public static void learn(string [] array)
        {
            List<string> learn = new List<string>();
            string [] learned = new string[array.Length];

            for (int i = 1; i < array.Length; i++)
            {
                learned[i - 1] = array[i];
            }
            string l = ArrayToString(learned);
            learn.Add(l);
            File.AppendAllLines("learned.txt", learn);

            Console.WriteLine("$DANI: I have just learned that " + l);
            using (SpeechSynthesizer synth = new SpeechSynthesizer())
            {
                synth.SelectVoiceByHints(VoiceGender.Male);
                synth.Speak("I have just learned that " + l);
            }
        }//end method learn
        
        //remember method: Dani reads in all the lines of the learned.txt file and checks the lastword in each line. If the last word in the 
        //question asked is in the line from the file, dani will print that line to screen. If a match can not be found DANI will
        //say it doesnt know the answer and continue to the main method.
        public static void remember(string[] array, string lastWord)
        {
            bool found = false;
            string[] lines = System.IO.File.ReadAllLines("learned.txt");
            for (int i = 0; i < lines.Length; i++)
            {
                string[] parse = lines[i].ToLower().Split(' ');
                for (int j = 0; j < parse.Length; j++)
                {
                    if (parse[j].IndexOf(lastWord) != -1)
                    {
                        string text = ArrayToString(parse);
                        Console.WriteLine("$DANI: " + text);
                        found = true;
                        using (SpeechSynthesizer synth = new SpeechSynthesizer())
                        {
                            synth.SelectVoiceByHints(VoiceGender.Male);
                            synth.Speak(text);
                        }
                    }

                }
            }
            if (found == false)
            {
                Console.WriteLine("$DANI: I dont know the answer to that question.");
                using (SpeechSynthesizer synth = new SpeechSynthesizer())
                {
                    synth.SelectVoiceByHints(VoiceGender.Male);
                    synth.Speak("I dont know the answer to that question");
                }
            }
        }

        //ArrayToString method: Useful for changing the reply array to a string in the daniSpeak method and making DANI speak more naturally.
        static string ArrayToString(string[] array)
        {
            string result = string.Join(" ", array);
            return result;
        }//end method ArrayToString


        //Main Method
        static void Main(string[] args)
        {
            bool found = false;
            wordList = new List<Words>();
            saveList = new List<string>();

            //StartUp Information
        Main:
            Console.Clear();
            Console.WriteLine("----------------------------------------------------");
            Console.WriteLine("Welcome To DANI: Dynamic Artificial Non-Intelligence");
            Console.WriteLine("----------------------------------------------------");
            Console.WriteLine("(*) To Talk to Dani, first choose an option below and");
            Console.WriteLine("press enter.");
            Console.WriteLine();
            Console.WriteLine("-------");
            Console.WriteLine("Options");
            Console.WriteLine("-------");
            Console.WriteLine("(Option 1) To populate the word lists with books type \"loadbook\"");
            Console.WriteLine("and then press enter. *** NOTE: Type \"info\" for book information. ***");
            Console.WriteLine();
            Console.WriteLine("(Option 2) To populate the word lists with a previous conversation");
            Console.WriteLine("type \"loadconvo\" and then press enter.");
            Console.WriteLine();
            Console.WriteLine("(Option 3) To populate the word lists with both books and a previous");
            Console.WriteLine("conversation type \"loadall\" and then press enter.");
            Console.WriteLine();
            Console.WriteLine("(Option 4) Type \"start\" and then press enter to start Dani with no");
            Console.WriteLine("knowledge of language. *** WARNING: This option will delete the");
            Console.WriteLine("word lists from a previous conversation. You will have to teach ");
            Console.WriteLine("DANI how to speak all over again from scratch! ***");
            Console.WriteLine();
            

            bool option = false;

            while (option == false)
            {
                //Choose an option
                Console.Write("$Enter Option: ");
                string load = Console.ReadLine().ToLower();
                if (load == "loadbook")
                {
                    loadfile(ulysses);
                    loadfile(htapos);
                    loadfile(grimm);
                    option = true;

                    Console.Clear();
                    Console.WriteLine("Books were successfully loaded!");
                    Console.WriteLine("You can now begin your convorsation with DANI.");
                    Console.WriteLine();
                    Console.WriteLine("To talk to DANI type what you want to say and press enter.");
                    Console.WriteLine("If you hire up the volume on your computer you will hear her");
                    Console.WriteLine("speak!");
                    Console.WriteLine();
                    Console.WriteLine("If you would like DANI to learn something specific, start your");
                    Console.WriteLine("sentence with \"learn\" and DANI will remember whatever you told");
                    Console.WriteLine("it to learn the next time you ask it!");
                    Console.WriteLine();
                    Console.WriteLine("To exit the program, type \":q\" and then press enter.");
                }//end if
                if (load == "loadall")
                {
                    loadfile(ulysses);
                    loadfile(htapos);
                    loadfile(grimm);
                    loadfile(convo);
                    option = true;

                    Console.Clear();
                    Console.WriteLine("Books and previous conversation were successfully loaded!");
                    Console.WriteLine("You can now begin your convorsation with DANI.");
                    Console.WriteLine();
                    Console.WriteLine("To talk to DANI type what you want to say and press enter.");
                    Console.WriteLine("If you hire up the volume on your computer you will hear her");
                    Console.WriteLine("speak!");
                    Console.WriteLine();
                    Console.WriteLine("If you would like DANI to learn something specific, start your");
                    Console.WriteLine("sentence with \"learn\" and DANI will remember whatever you told");
                    Console.WriteLine("it to learn the next time you ask it!");
                    Console.WriteLine();
                    Console.WriteLine();
                    Console.WriteLine("To exit the program, type \":q\" and then press enter.");
                }
                else if (load == "loadconvo")
                {
                    loadfile(convo);
                    Console.Clear();
                    Console.WriteLine("Previous Convorsation was successfully loaded!");
                    Console.WriteLine("You can now begin your convorsation with DANI.");
                    Console.WriteLine();
                    Console.WriteLine("To talk to DANI type what you want to say and press enter.");
                    Console.WriteLine("If you hire up the volume on your computer you will hear her");
                    Console.WriteLine("speak!");
                    Console.WriteLine();
                    Console.WriteLine("If you would like DANI to learn something specific, start your");
                    Console.WriteLine("sentence with \"learn\" and DANI will remember whatever you told");
                    Console.WriteLine("it to learn the next time you ask it!");
                    Console.WriteLine();
                    Console.WriteLine();
                    Console.WriteLine("To exit the program, type \":q\" and then press enter.");
                    option = true;
                }//end else if
                else if (load == "start")
                {
                    List<string> empty = new List<string>();
                    File.WriteAllLines("knowledge.txt", empty);
                    Console.Clear();
                    Console.WriteLine("DANI started without any knowledge. Its up to you to teach him!");
                    Console.WriteLine("You can now begin your convorsation with DANI.");
                    Console.WriteLine();
                    Console.WriteLine("To talk to DANI type what you want to say and press enter.");
                    Console.WriteLine("If you hire up the volume on your computer you will hear her");
                    Console.WriteLine("speak!");
                    Console.WriteLine();
                    Console.WriteLine("If you would like DANI to learn something specific, start your");
                    Console.WriteLine("sentence with \"learn\" and DANI will remember whatever you told");
                    Console.WriteLine("it to learn the next time you ask it!");
                    Console.WriteLine();
                    Console.WriteLine("To exit the program, type \":q\" and then press enter.");
                    option = true;
                }//end else if
                else if (load == "info")
                {
                    bool main = false;

                    while (main == false)
                    {
                        Console.Clear();
                        Console.WriteLine("----------------");
                        Console.WriteLine("Book Information");
                        Console.WriteLine("----------------");
                        Console.WriteLine();
                        Console.WriteLine("Three books will be loaded into the system. They are:");
                        Console.WriteLine();
                        Console.WriteLine("(1) \"Ulysses\" by James Joyce.");
                        Console.WriteLine("(2) \"How to Analyze People on Sight\" by Elsie Lincoln Benedict and Ralph Paine Benedic.");
                        Console.WriteLine("(3) \"Grimms' Fairy Tales\" by Jacob Grimm and Wilhelm Grimm.");
                        Console.WriteLine();
                        Console.WriteLine("Type \"return\" and then press Enter to return to main menu...");
                        Console.WriteLine();
                        Console.Write("$Enter Option: ");
                        string op = Console.ReadLine().ToLower();

                        if (op == "return")
                        {
                            goto Main;
                        }
                        else
                        {
                            Console.WriteLine("Incorrect option. Please try again.");
                        }
                    }//end while

                }
                else
                {
                    Console.WriteLine("Incorrect option. Try again.");
                    Console.WriteLine();
                }//end else
            }

            //Conversation part of the program/
        beginning:
            while (true)
            {
                Console.WriteLine();
                Console.Write("$User: ");
                string chat = Console.ReadLine();
                bool go = preDefinedQuestions(chat);
                if (go == true)
                {
                    goto beginning;
                }
                //Saving all input text to a saveList makes it easy to save the conversations when we are finished with the program.
                saveList.Add(chat);

                if (chat == ":q")
                {
                    break;
                }//end if
                else
                {
                    string[] parsedChat = chat.Split(' ');

                    //Two if statements. The first checks to see if the first word in the sentence is "learn" and if it is DANI will learn what ever is typed
                    //next.
                    if (parsedChat[0] == "learn") //<------------------------here
                    {
                        learn(parsedChat);
                        goto beginning;
                    }
                    //The 2nd if statement check if the input sentence is a question, and if it is will check to see if it learned the sentence through
                    //the remember method.
                    if (parsedChat[0] == "what" || parsedChat[0] == "who" || parsedChat[0] == "where" || parsedChat[0] == "why" || parsedChat[0] == "when" || parsedChat[0] == "do" || parsedChat[0] == "how")
                    {
                        int last = parsedChat.Length - 1;
                        string l = parsedChat[last];
                        remember(parsedChat, l);
                        goto beginning;
                    }

                    //Searching the Wordlist for the input word.
                    for (int i = 0; i < parsedChat.Length - 1; i++)
                    {
                        for (int j = 0; j < wordList.Count; j++)
                        {
                            if (parsedChat[i] == wordList[j].Word)
                            {
                                found = true;
                            }//end if
                        }//end for

                        //If the word is not in the list we add it to the wordList
                        if (found == false)
                        {
                            addWord(parsedChat[i], parsedChat[i + 1]);
                        }//end if
                        //If it is in the list we call the addFollowUpWord method (see method above for a description)
                        else
                        {
                            addFollowUpWord(parsedChat[i], parsedChat[i + 1]);
                            //reset found to false
                            found = false;
                        }
                    }

                    //Call daniTalk Method
                    daniTalk(parsedChat);
                }
            }

            Console.Clear();
            //Saving the convorsation
            Console.WriteLine("(*) Press \"Y\" to save the new word lists Dani learned");
            Console.WriteLine("in the previous convorsation or press \"N\" to exit.");
            Console.WriteLine();

            bool l_option = false;
            while (l_option == false)
            {
                Console.Write("$Enter option: ");
                string ans = Console.ReadLine().ToLower();
                if (ans == "y")
                {
                    Console.WriteLine("\nPlease wait while DANI Writes all his knowledge to File.");
                    Console.WriteLine("If you exit without the process ending you will lose data.");
                    Console.WriteLine();
                    Console.WriteLine("Processing......");
                    Console.WriteLine();

                    File.AppendAllLines("knowledge.txt", saveList);
                    Console.WriteLine("Process Complete. Bye Bye!");
                    l_option = true;
                }
                else if (ans == "n")
                {
                    Console.WriteLine("Bye Bye!");
                    l_option = true;
                }
                else
                {
                    Console.WriteLine("Incorrect option. Try again.");
                    Console.WriteLine();
                }
            }
        }
    }
}
