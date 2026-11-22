namespace FlashCardApp.Utils;

public static class AppConstants
{
    public static class Default_Api_Prefix
    {
        public const string Value = "api";
    }

    public static class App_Options
    {
        public const string JwtOptionsSection = "Jwt";
    }

    public static class ApiRoutes
    {
        public const string FlashCardSets = "flash_card_sets";
        public const string FlashCards = "flash_cards";
        public const string Auth = "auth";
        public const string StudySessions = "study_sessions";
        public const string CardReviews = "card_reviews";
    }

    public static class Tags
    {
        public const string FlashCardApp = "FlashCardApp";
        public const string FlashCardSets = "FlashCardSets";
        public const string FlashCards = "FlashCards";
        public const string Auth = "Auth";
        public const string Users = "Users";
        public const string StudySession = "StudySession";
        public const string CardReviews = "CardReviews";
    }

    public static class Domains
    {
        public static class Core
        {
            public const string Name = "Core";

            public static class Tables
            {
                public const string FlashCardSets = "FlashCardSets";
                public const string FlashCards = "FlashCards";
                public const string Users = "Users";
                public const string StudySessions = "StudySessions";
                public const string CardReviews = "CardReviews";
            }
        }
    }
} 