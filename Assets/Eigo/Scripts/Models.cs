namespace Eigo.Models
{
    using System;

    [Serializable]
    public class QuestionParser
    {
        public Question[] questions;
    }

    [Serializable]
    public class Question
    {
        public string id;
        public string date;
        public Profile profile;
        public string sentence;
    }

    [Serializable]
    public class Profile
    {
        public string name;
        public string image;
    }
}
