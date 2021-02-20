using System;
using UnityEngine;

namespace Eigo.Common
{
    public static class PlayData
    {
        public static int TotalCorrectQuestionNumber = 0;
        public static int TotalaAlreadyQuestionNumber = 0;
        public static int CurrentQuestionNumber = 0;

        public static string GetRate()
        {
            if (TotalaAlreadyQuestionNumber == 0)
            {
                return "Rate: 0.0% (0/0)";
            }
            string rate = ((decimal)TotalCorrectQuestionNumber / TotalaAlreadyQuestionNumber).ToString("P2");
            string correct = String.Format("{0:#,0}", TotalCorrectQuestionNumber);
            string total = String.Format("{0:#,0}", TotalaAlreadyQuestionNumber);
            return $"Rate: {rate} ({correct}/{total})";
        }

        public static void LoadData()
        {
            CurrentQuestionNumber = PlayerPrefs.GetInt("CurrentQuestionNumber", 0);
            TotalCorrectQuestionNumber = PlayerPrefs.GetInt("TotalCorrectQuestionNumber", 0);
            TotalaAlreadyQuestionNumber = PlayerPrefs.GetInt("TotalaAlreadyQuestionNumber", 0);
        }
    }
}
