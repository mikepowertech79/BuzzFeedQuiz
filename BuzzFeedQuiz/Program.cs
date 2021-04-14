using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BuzzFeedQuiz
{
    public class Making
    {
        public string UserName = "";
        public string correctPassword = "";

        //---------------------------------------FUNCTIONS----------------------------------------
        public SqlConnection CreateDBConnection()
        {
            SqlConnection connection = new SqlConnection(@"Data Source=10.1.10.148;Initial Catalog=Buzzfeed04; User ID=academy_admin;Password=12345");
            connection.Open();
            return connection;
        }

        //----

        public int CreateUser(string UserName, string PassWord, SqlConnection connection)
        {
            int userID = 0;
            SqlCommand InsertUserNamePassword = new SqlCommand($"INSERT INTO Users (Username, Userpassword) VALUES ('{UserName}', '{PassWord}'); SELECT @@Identity AS ID", connection);

            SqlDataReader ReadBackPassword = InsertUserNamePassword.ExecuteReader();
            if (ReadBackPassword.HasRows)
            {
                ReadBackPassword.Read();
                userID = Convert.ToInt32(ReadBackPassword["id"]);
                Console.WriteLine($"Cool! your user ID is '{ReadBackPassword["id"]}'");
            }
            ReadBackPassword.Close();
            Console.WriteLine("Great, you are ready to go!!!");

            return userID;
        }

        //----

        public bool CheckUserName(string UserName, SqlConnection connection)
        {
            SqlCommand CheckIfNameTaken = new SqlCommand($"SELECT * FROM [Users] WHERE Username = '{UserName}'", connection);
            SqlDataReader CheckNameReader = CheckIfNameTaken.ExecuteReader();
            if (CheckNameReader.HasRows)
            {
                CheckNameReader.Close();
                return false;
            }
            else
            {
                CheckNameReader.Close();
                return true;
            }
        }

        //------------------------------------END FUNCTIONS-------------------------------------
    }

    class Program
    {
        static void Main(string[] args)
        {
            Making make = new Making();
            SqlConnection connection = make.CreateDBConnection();

            int currentUser = 0;

            bool registering = true;
            while (registering)
            {
                Console.WriteLine("Hi, welcome to 'Make your Own Buzzfeed test!' Would you like to login or register?");
                string LoginOrRegister = Console.ReadLine().ToLower();
                if (LoginOrRegister == "login")
                {
                    Console.WriteLine("Please enter your username to login");
                    string LoginName = Console.ReadLine();
                    SqlCommand CheckIfLoginExists = new SqlCommand($"SELECT * FROM [Users] WHERE Username = '{LoginName}'", connection);
                    SqlDataReader LoginReader = CheckIfLoginExists.ExecuteReader();
                    if (LoginReader.HasRows)
                    {
                        LoginReader.Read();
                        make.correctPassword = Convert.ToString(LoginReader["userpassword"]);
                        make.UserName = Convert.ToString(LoginReader["username"]);
                        currentUser = Convert.ToInt32(LoginReader["userid"]);
                        Console.WriteLine($"Cool!!! Welcome back, {LoginName}! Your UserID is '{currentUser}'. Now please enter your password");
                        string ReturningPassword = Console.ReadLine();
                        if (make.correctPassword == ReturningPassword)
                        {
                            Console.WriteLine("Yay, you entered in the correct password! Let's go!");
                            LoginReader.Close();
                            registering = false;
                        }
                        else
                        {
                            Console.WriteLine("Sorry, that's the wrong password!");
                        }
                        LoginReader.Close();
                    }
                    else
                    {
                        Console.WriteLine("Sorry, that username does not exist...");
                    }
                }
                else if (LoginOrRegister == "register")
                {
                    bool MakingUserName = true;

                    while (MakingUserName)
                    {
                        Console.WriteLine("What do you want your username to be?");
                        make.UserName = Console.ReadLine();
                        if (make.CheckUserName(make.UserName, connection))
                        {
                            Console.WriteLine("Awesome, your name has been entered! Now please choose what you want your password to be");
                            string PassWord = Console.ReadLine();
                            currentUser = make.CreateUser(make.UserName, PassWord, connection);
                            MakingUserName = false;
                            registering = false;
                        }
                        else
                        {
                            Console.WriteLine("Sorry, that name is taken!");
                        }
                    }
                }
                else
                {
                    Console.WriteLine("Please enter either login or register...");
                }
            }

            //-------------------------------MAKE A TEST!!!-------------------------------------------

            Console.WriteLine($"Welcome {make.UserName}!");
            Console.WriteLine("What would you like to do? A) Take a test; B) Create a Test; C) Sign out");
            string choice = Console.ReadLine().ToLower();
            Console.Clear();
            bool testing = true;
            while (testing)
            {
                if (choice == "a")
                {
                    // TAKE A TEST
                }

                else if (choice == "b") //make a test
                {
                    Console.WriteLine("Let's make a test!");
                    Console.WriteLine("What is the title of your test?");
                    string testTitle = Console.ReadLine();
                    SqlCommand insertTestTitle = new SqlCommand($"INSERT INTO Tests (Title, UserId) VALUES ('{testTitle}', {currentUser}); SELECT @@Identity AS ID", connection);
                    SqlDataReader readTestId = insertTestTitle.ExecuteReader();
                    int testID = 0; //will need to plug testID into [Answers] below
                    if (readTestId.HasRows)
                    {
                        readTestId.Read();
                        testID = Convert.ToInt32(readTestId["ID"]);
                    }
                    readTestId.Close();

                    //ask for number of questions and number of answers (correspond to each of two for loops below)

                    bool FalseQuestionNum = true;
                    int testQnum = 0;


                    while (FalseQuestionNum)
                    {
                        Console.WriteLine("How many questions would you like? (1-10)");


                        string stringText = Console.ReadLine();
                        int n;
                 

                        if (int.TryParse(stringText, out n) && n > 0 && n < 11) 
                        {
                            testQnum = n;
                            FalseQuestionNum = false;
                            
                        }

                     

                        else
                        {
                            Console.WriteLine("Please enter a valid number between 1-10");
                        }
                    }

                    bool FalseAnswerNum = true;
                    int testAnum = 0;
                    while (FalseAnswerNum)
                    {
                        Console.WriteLine("How many answers would you like each question to have? (2-4)");
                        string stringAnswer = Console.ReadLine();
                        int n;

                        if (int.TryParse(stringAnswer, out n) && n > 1 && n < 5)
                        {
                            testAnum = n;
                            FalseAnswerNum = false;
                        }
                        else
                        {
                            Console.WriteLine("Please enter a valid number between 2 - 4");
                        }
                    }

                    //loops(i) = number of questions
                    for (int i = 1; i <= testQnum; i++)
                    {
                        Console.WriteLine($"Please enter Question #{i}: ");
                        string testQuestion = Console.ReadLine();

                        //insert (TestId, Question) to [Questions]; return QuestionId to set to int
                        SqlCommand insertQuestion = new SqlCommand($"INSERT INTO Questions (TestId, Question, SortOrder) VALUES ({testID}, '{testQuestion}', {i}); SELECT @@Identity AS ID", connection);
                        SqlDataReader readQuestion = insertQuestion.ExecuteReader();
                        int questionID = 0; //set int for questionID (must be tied to answers)
                        if (readQuestion.HasRows)
                        {
                            readQuestion.Read();
                            questionID = Convert.ToInt32(readQuestion["ID"]);
                        }
                        readQuestion.Close();
                        //loops(j) = number of answers 
                        int answerValue = 0;
                        for (int j = 1; j <= testAnum; j++)
                        {
                            Console.WriteLine($"Please enter Answer #{j}/{testAnum}: ");
                            string questionAnswer = Console.ReadLine();
                            Console.WriteLine($"What value would you like to assign Answer #{j}/{testAnum}");
                            //int answerValue = Convert.ToInt32(Console.ReadLine());
                            string stringAnswer = Console.ReadLine();
                            int n;
                            bool looping = true;

                            while (looping)
                            {
                                Console.WriteLine($"What value would you like to assign Answer #{j}/{testAnum}");
                                stringAnswer = Console.ReadLine();
                                if (int.TryParse(stringAnswer, out n))
                                {
                                    answerValue = n;
                                    looping = false;
                                }

                                else
                                {
                                    Console.WriteLine("Please enter a valid numeric value");
                                }
                            }
                            

                            //insert (QuestionId, Value, Answer) to[Answers]
                            SqlCommand insertAnswer = new SqlCommand($"INSERT INTO Answers (QuestionId, Value, Answer) VALUES ({questionID}, {answerValue}, '{questionAnswer}')", connection);
                            insertAnswer.ExecuteNonQuery();
                        }
                    }
                    Console.WriteLine("Congratulations! Your test has been saved.");
                    Console.ReadLine();
                    testing = false;
                }

                else if (choice == "c")
                {
                    Console.Clear();
                    Console.WriteLine("Bye!");
                    testing = false;
                }
                else
                {
                    Console.WriteLine("Invalid choice. Please select again.");
                    Console.Clear();

                }
            }
            Console.ReadLine();
            connection.Close();
        }
    }
}